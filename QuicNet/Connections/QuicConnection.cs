using QuickNet.Utilities;
using QuicNet.Constants;
using QuicNet.Context;
using QuicNet.Events;
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

namespace QuicNet.Connections
{
    public class QuicConnection
    {
        private readonly NumberSpace _ns = new NumberSpace();
        private readonly PacketWireTransfer _pwt;

        private UInt64 _currentTransferRate;
        private ConnectionState _state;
        private string _lastError;
        private Dictionary<UInt64, QuicStream> _streams;

        public GranularInteger ConnectionId { get; private set; }
        public GranularInteger PeerConnectionId { get; private set; }

        public PacketCreator PacketCreator { get; private set; }
        public UInt64 MaxData { get; private set; }
        public UInt64 MaxStreams { get; private set; }

        public StreamOpenedEvent OnStreamOpened { get; set; }
        public ConnectionClosedEvent OnConnectionClosed { get; set; }

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

        public QuicStream ProcessFrames(List<Frame> frames)
        {
            QuicStream stream = null;

            foreach (Frame frame in frames)
            {
                if (frame.Type == 0x01)
                    OnRstStreamFrame(frame);
                if (frame.Type == 0x04)
                    OnRstStreamFrame(frame);
                if (frame.Type >= 0x08 && frame.Type <= 0x0f)
                    stream = OnStreamFrame(frame);
                if (frame.Type == 0x10)
                    OnMaxDataFrame(frame);
                if (frame.Type == 0x11)
                    OnMaxStreamDataFrame(frame);
                if (frame.Type >= 0x12 && frame.Type <= 0x13)
                    OnMaxStreamFrame(frame);
                if (frame.Type == 0x14)
                    OnDataBlockedFrame(frame);
                if (frame.Type >= 0x1c && frame.Type <= 0x1d)
                    OnConnectionCloseFrame(frame);
            }

            return stream;
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

            OnConnectionClosed?.Invoke(this);
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

        private QuicStream OnStreamFrame(Frame frame)
        {
            QuicStream stream;

            StreamFrame sf = (StreamFrame)frame;
            StreamId streamId = sf.StreamId;

            if (_streams.ContainsKey(streamId.Id) == false)
            {
                stream = new QuicStream(this, streamId);

                if ((UInt64)_streams.Count < MaxStreams)
                    _streams.Add(streamId.Id, stream);
                else
                    SendMaximumStreamReachedError();

                OnStreamOpened?.Invoke(stream);
            }
            else
            {
                stream = _streams[streamId.Id];
            }

            stream.ProcessData(sf);

            return stream;
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
            StreamId streamId = msdf.StreamId;
            if (_streams.ContainsKey(streamId.Id))
            {
                // Find and set the new maximum stream data on the stream
                QuicStream stream = _streams[streamId.Id];
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
            StreamId streamId = sdbf.StreamId;

            if (_streams.ContainsKey(streamId.Id) == false)
                return;
            QuicStream stream = _streams[streamId.Id];

            stream.ProcessStreamDataBlocked(sdbf);

            // Remove the stream from the connection
            _streams.Remove(streamId.Id);
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

        public QuicStream OpenStream()
        {
            QuicStream stream = null;

            while (stream == null /* TODO: Or Timeout? */)
            {
                Packet packet = _pwt.ReadPacket();
                if (packet is ShortHeaderPacket shp)
                {
                    stream = ProcessFrames(shp.GetFrames());
                }
            }

            return stream;
        }

        /// <summary>
        /// Client only!
        /// </summary>
        /// <returns></returns>
        internal void ReceivePacket()
        {
            Packet packet = _pwt.ReadPacket();

            if (packet is ShortHeaderPacket shp)
            {
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

        internal bool SendData(Packet packet)
        {
            return _pwt.SendPacket(packet);
        }

        internal void TerminateConnection()
        {
            _state = ConnectionState.Draining;
            _streams.Clear();

            ConnectionPool.RemoveConnection(this.ConnectionId);
        }

        internal void SendMaximumStreamReachedError()
        {
            ShortHeaderPacket packet = PacketCreator.CreateConnectionClosePacket(Infrastructure.ErrorCode.STREAM_LIMIT_ERROR, 0x00, ErrorConstants.MaxNumberOfStreams);
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
                packet = PacketCreator.CreateConnectionClosePacket(Infrastructure.ErrorCode.FLOW_CONTROL_ERROR, 0x00, ErrorConstants.MaxDataTransfer);

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
