using QuickNet.Utilities;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Packets;
using QuicNet.Streams;
using System;
using System.Collections.Generic;

namespace QuicNet.Connections
{
    public class QuicConnection
    {
        private ConnectionState _state;
        private Dictionary<UInt64, QuicStream> _streams;
        public QuicConnection(UInt32 id)
        {
            _state = ConnectionState.Open;
            _streams = new Dictionary<UInt64, QuicStream>();
        }

        public Packet ProcessFrames(List<Frame> frames)
        {
            foreach (Frame frame in frames)
            {
                if (frame.Type == 0x02)
                    OnConnectionCloseFrame(frame);
                if (frame.Type >= 0x10 && frame.Type <= 0x17)
                    OnStreamFrame(frame);
            }

            return null;
        }

        private void OnConnectionCloseFrame(Frame frame)
        {
            _state = ConnectionState.Draining;
        }

        private void OnStreamFrame(Frame frame)
        {
            StreamFrame sf = (StreamFrame)frame;
            if (_streams.ContainsKey(sf.StreamId.Value) == false)
            {
                QuicStream stream = new QuicStream(this, sf.ConvertedStreamId);
                stream.ProcessData(sf);

                _streams.Add(sf.StreamId.Value, stream);
            }
            else
            {
                QuicStream stream = _streams[sf.StreamId];
                stream.ProcessData(sf);
            }
        }
    }
}
