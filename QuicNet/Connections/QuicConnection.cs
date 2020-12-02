using QuickNet.Utilities;
using QuicNet.Context;
using QuicNet.Exceptions;
using QuicNet.Infrastructure;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.PacketProcessing;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
using QuicNet.InternalInfrastructure;
using QuicNet.Streams;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace QuicNet.Connections
{
    public class QuicConnection
    {
        private UInt64 _currentTransferRate;
        private ConnectionState _state;
        private string _lastError;
        private Dictionary<UInt64, QuicStream> _streams;
        private NumberSpace _ns = new NumberSpace();

        private PacketWireTransfer _pwt;

        public UInt32 ConnectionId { get; private set; }
        public UInt32 PeerConnectionId { get; private set; }

        public PacketCreator PacketCreator { get; private set; }
        public UInt64 MaxData { get; private set; }
        public UInt64 MaxStreams { get; private set; }

        public event Action<QuicStream> OnDataReceived;

        /// <summary>
        /// Creates a new stream for sending/receiving data.
        /// </summary>
        /// <param name="type">Type of the stream (Uni-Bidirectional)</param>
        /// <returns>A new stream instance or Null if the connection is terminated.</returns>
        public QuicStream CreateStream(StreamType type)
        {
            UInt32 streamId = _ns.Get();
            if (_state != ConnectionState.Open)
                return null;

            QuicStream stream = new QuicStream(this, new QuickNet.Utilities.StreamId(streamId, type));
            _streams.Add(streamId, stream);

            return stream;
        }

        public void ProcessFrames(List<Frame> frames)
        {
            foreach (Frame frame in frames)
            {
                if (frame.Type == 0x01)
                    OnRstStreamFrame(frame);
                if (frame.Type == 0x04)
                    OnRstStreamFrame(frame);
                if (frame.Type >= 0x08 && frame.Type <= 0x0f)
                    OnStreamFrame(frame);
                if (frame.Type == 0x10)
                    OnMaxDataFrame(frame);
                if (frame.Type == 0x11)
                    OnMaxStreamDataFrame(frame);
                if (frame.Type >= 0x12 && frame.Type <= 0x13)
                    OnMaxStreamFrame(frame);
                if (frame.Type == 0x14)
                    OnDataBlockedFrame(frame);
                if (frame.Type == 0x1c)
                    OnConnectionCloseFrame(frame);
            }
        }

        public async Task ProcessFramesAsync(List<Frame> frames)
        {
            foreach (Frame frame in frames)
            {
                switch (frame.Type)
                {
                    case 0x01:
                    case 0x04:
                        OnRstStreamFrame(frame);
                        break;
                    case byte type when type >= 0x08 && frame.Type <= 0x0f:
                        await OnStreamFrameAsync(frame);
                        break;
                    case 0x10:
                        OnMaxDataFrame(frame);
                        break;
                    case 0x11:
                        OnMaxStreamDataFrame(frame);
                        break;
                    case 0x12:
                    case 0x13:
                        OnMaxStreamFrame(frame);
                        break;
                    case 0x14:
                        OnDataBlockedFrame(frame);
                        break;
                    case 0x1c:
                        OnConnectionCloseFrame(frame);
                        break;
                    default:
                        break;
                }
            }
        }

        public void IncrementRate(int length)
        {
            _currentTransferRate += (UInt32)length;
        }

        public bool MaximumReached()
        {
            if (_currentTransferRate >= MaxData)
                return true;

            return false;
        }

        private void OnConnectionCloseFrame(Frame frame)
        {
            ConnectionCloseFrame ccf = (ConnectionCloseFrame)frame;
            _state = ConnectionState.Draining;
            _lastError = ccf.ReasonPhrase;
        }

        private void OnRstStreamFrame(Frame frame)
        {
            ResetStreamFrame rsf = (ResetStreamFrame)frame;
            if (_streams.ContainsKey(rsf.StreamId))
            {
                // Find and reset the stream
                QuicStream stream = _streams[rsf.StreamId];
                stream.ResetStream(rsf);

                // Remove the stream from the connection
                _streams.Remove(rsf.StreamId);
            }
        }

        private void OnStreamFrame(Frame frame)
        {
            StreamFrame sf = (StreamFrame)frame;
            if (_streams.ContainsKey(sf.ConvertedStreamId.Id) == false)
            {
                QuicStream stream = new QuicStream(this, sf.ConvertedStreamId);
                stream.ProcessData(sf);

                if ((UInt64)_streams.Count < MaxStreams)
                    _streams.Add(sf.ConvertedStreamId.Id, stream);
                else
                    SendMaximumStreamReachedError();
            }
            else
            {
                QuicStream stream = _streams[sf.ConvertedStreamId.Id];
                stream.ProcessData(sf);
            }
        }

        private async Task OnStreamFrameAsync(Frame frame)
        {
            StreamFrame sf = (StreamFrame)frame;
            if (_streams.ContainsKey(sf.ConvertedStreamId.Id) == false)
            {
                QuicStream stream = new QuicStream(this, sf.ConvertedStreamId);
                await stream.ProcessDataAsync(sf);

                if ((UInt64)_streams.Count < MaxStreams)
                    _streams.Add(sf.ConvertedStreamId.Id, stream);
                else
                    SendMaximumStreamReachedError();
            }
            else
            {
                QuicStream stream = _streams[sf.ConvertedStreamId.Id];
                await stream.ProcessDataAsync(sf);
            }
        }

        private void OnMaxDataFrame(Frame frame)
        {
            MaxDataFrame sf = (MaxDataFrame)frame;
            if (sf.MaximumData.Value > MaxData)
                MaxData = sf.MaximumData.Value;
        }

        private void OnMaxStreamDataFrame(Frame frame)
        {
            MaxStreamDataFrame msdf = (MaxStreamDataFrame)frame;
            if (_streams.ContainsKey(msdf.StreamId))
            {
                // Find and set the new maximum stream data on the stream
                QuicStream stream = _streams[msdf.ConvertedStreamId.Id];
                stream.SetMaximumStreamData(msdf.MaximumStreamData.Value);
            }
        }

        private void OnMaxStreamFrame(Frame frame)
        {
            MaxStreamsFrame msf = (MaxStreamsFrame)frame;
            if (msf.MaximumStreams > MaxStreams)
                MaxStreams = msf.MaximumStreams.Value;
        }

        private void OnDataBlockedFrame(Frame frame)
        {
            // TODO: Tuning of data transfer.
            // Since no stream id is present on this frame, we should be
            // stopping the connection.

            TerminateConnection();
        }

        private void OnStreamDataBlockedFrame(Frame frame)
        {
            StreamDataBlockedFrame sdbf = (StreamDataBlockedFrame)frame;

            if (_streams.ContainsKey(sdbf.ConvertedStreamId.Id) == false)
                return;
            QuicStream stream = _streams[sdbf.ConvertedStreamId.Id];

            stream.ProcessStreamDataBlocked(sdbf);

            // Remove the stream from the connection
            _streams.Remove(sdbf.ConvertedStreamId.Id);
        }

        #region Internal

        internal QuicConnection(ConnectionData connection)
        {
            _currentTransferRate = 0;
            _state = ConnectionState.Open;
            _lastError = string.Empty;
            _streams = new Dictionary<UInt64, QuicStream>();
            _pwt = connection.PWT;

            ConnectionId = connection.ConnectionId;
            PeerConnectionId = connection.PeerConnectionId;
            // Also creates a new number space
            PacketCreator = new PacketCreator(ConnectionId, PeerConnectionId);
            MaxData = QuicSettings.MaxData;
            MaxStreams = QuicSettings.MaximumStreamId;
        }

        /// <summary>
        /// Client only!
        /// </summary>
        /// <returns></returns>
        internal void ReceivePacket()
        {
            Packet packet = _pwt.ReadPacket();

            if (packet is ShortHeaderPacket)
            {
                ShortHeaderPacket shp = (ShortHeaderPacket)packet;
                ProcessFrames(shp.GetFrames());
            }

            // If the connection has been closed
            if (_state == ConnectionState.Draining)
            {
                if (string.IsNullOrWhiteSpace(_lastError))
                    _lastError = "Protocol error";

                TerminateConnection();

                throw new QuicConnectivityException(_lastError);
            }

        }

        /// <summary>
        /// Client async only!
        /// </summary>
        /// <returns></returns>
        internal async Task ReceivePacketAsync()
        {
            Packet packet = await _pwt.ReadPacketAsync();

            if (packet is ShortHeaderPacket)
            {
                ShortHeaderPacket shp = (ShortHeaderPacket)packet;
                await ProcessFramesAsync(shp.GetFrames());
            }

            // If the connection has been closed
            if (_state == ConnectionState.Draining)
            {
                if (string.IsNullOrWhiteSpace(_lastError))
                    _lastError = "Protocol error";

                TerminateConnection();

                throw new QuicConnectivityException(_lastError);
            }

        }

        internal bool SendData(Packet packet)
        {
            return _pwt.SendPacket(packet);
        }

        internal async Task<bool> SendDataAsync(Packet packet)
        {
            return await _pwt.SendPacketAsync(packet);
        }

        internal void DataReceived(QuicStream context)
        {
            OnDataReceived?.Invoke(context);
        }

        internal void TerminateConnection()
        {
            _state = ConnectionState.Draining;
            _streams.Clear();

            ConnectionPool.RemoveConnection(this.ConnectionId);
        }

        internal void SendMaximumStreamReachedError()
        {
            ShortHeaderPacket packet = PacketCreator.CreateConnectionClosePacket(Infrastructure.ErrorCode.STREAM_LIMIT_ERROR, "Maximum number of streams reached.");
            Send(packet);
        }

        /// <summary>
        /// Used to send protocol packets to the peer.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        internal bool Send(Packet packet)
        {
            // Encode the packet
            byte[] data = packet.Encode();

            // Increment the connection transfer rate
            IncrementRate(data.Length);

            // If the maximum transfer rate is reached, send FLOW_CONTROL_ERROR
            if (MaximumReached())
            {
                packet = PacketCreator.CreateConnectionClosePacket(Infrastructure.ErrorCode.FLOW_CONTROL_ERROR, "Maximum data transfer reached.");

                TerminateConnection();
            }

            // Ignore empty packets
            if (data == null || data.Length <= 0)
                return true;

            bool result = _pwt.SendPacket(packet);

            return result;
        }

        #endregion
    }
}
