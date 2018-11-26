using QuicNet.Infrastructure.Frames;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    /// <summary>
    /// Base data transfer unit of QUIC Transport.
    /// </summary>
    public abstract class Packet
    {
        protected List<Frame> _frames;
        public abstract byte Type { get; }

        public abstract byte[] Encode();
        public abstract void Decode(byte[] packet);
    }
}
