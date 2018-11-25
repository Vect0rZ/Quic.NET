using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    public class RetryPacket : Packet
    {
        public override Packet Decode(byte[] packet)
        {
            throw new NotImplementedException();
        }

        public override byte[] Encode()
        {
            throw new NotImplementedException();
        }
    }
}
