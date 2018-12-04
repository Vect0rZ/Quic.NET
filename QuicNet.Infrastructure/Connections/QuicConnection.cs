using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Streams;
using System;
using System.Collections.Generic;

namespace QuicNet.Infrastructure.Connections
{
    public class QuicConnection
    {
        private Dictionary<int, QuicStream> _streams;
        public QuicConnection(UInt32 id)
        {
            _streams = new Dictionary<int, QuicStream>();
        }

        public Packet ProcessFrames(List<Frame> frames)
        {
            return null;
        }
    }
}
