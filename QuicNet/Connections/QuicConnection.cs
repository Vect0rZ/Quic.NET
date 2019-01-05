using QuicNet.Context;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.PacketProcessing;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
using QuicNet.Streams;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace QuicNet.Connections
{
    public class QuicConnection
    {
        public UInt32 ConnectionId { get; private set; }
        public UInt32 PeerConnectionId { get; private set; }
        public QuicContext Context { get; private set; }
        public PacketCreator PacketCreator { get; private set; }
        public UInt64 MaxData { get; private set; }
        public UInt64 MaxStreams { get; private set; }

        private UInt64 _currentTransferRate;
        private ConnectionState _state;
        private Dictionary<UInt64, QuicStream> _streams;
        
        public QuicConnection(UInt32 id, UInt32 peerConnectionId)
        {
            _currentTransferRate = 0;
            _state = ConnectionState.Open;
            _streams = new Dictionary<UInt64, QuicStream>();

            ConnectionId = id;
            PeerConnectionId = peerConnectionId;
            // Also creates a new number space
            PacketCreator = new PacketCreator(ConnectionId, PeerConnectionId);
            MaxData = QuicSettings.MaxData;
            MaxStreams = QuicSettings.MaximumStreamId;
        }

        public void AttachContext(QuicContext context)
        {
            Context = context;
            Context.Connection = this;
        }

        public void ProcessFrames(List<Frame> frames)
        {
            foreach (Frame frame in frames)
            {
                if (frame.Type == 0x01)
                    OnRstStreamFrame(frame);
                if (frame.Type == 0x02)
                    OnConnectionCloseFrame(frame);
                if (frame.Type == 0x04)
                    OnRstStreamFrame(frame);
                if (frame.Type >= 0x08 && frame.Type <= 0x0f)
                    OnStreamFrame(frame);
                if (frame.Type == 0x10)
                    OnMaxDataFrame(frame);
                if (frame.Type >= 0x12 && frame.Type <= 0x13)
                    OnMaxStreamFrame(frame);
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
            _state = ConnectionState.Draining;
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
            if (_streams.ContainsKey(sf.StreamId.Value) == false)
            {
                QuicStream stream = new QuicStream(this, sf.ConvertedStreamId);
                stream.ProcessData(sf);

                if ((UInt64)_streams.Count < MaxStreams)
                    _streams.Add(sf.StreamId.Value, stream);
                else
                    SendMaximumStreamReachedError();
            }
            else
            {
                QuicStream stream = _streams[sf.StreamId];
                stream.ProcessData(sf);
            }
        }

        private void OnMaxDataFrame(Frame frame)
        {
            MaxDataFrame sf = (MaxDataFrame)frame;
            if (sf.MaximumData.Value > MaxData)
                MaxData = sf.MaximumData.Value;
        }

        private void OnMaxStreamFrame(Frame frame)
        {
            MaxStreamsFrame msf = (MaxStreamsFrame)frame;
            if (msf.MaximumStreams > MaxStreams)
                MaxStreams = msf.MaximumStreams.Value;
        }

        internal void TerminateConnection()
        {
            _state = ConnectionState.Draining;
        }

        internal void SendMaximumStreamReachedError()
        {
            ShortHeaderPacket packet = PacketCreator.CreateConnectionClosePacket(Infrastructure.ErrorCode.STREAM_LIMIT_ERROR, "Maximum number of streams reached.");
            Context.Send(packet);
        }
    }
}
