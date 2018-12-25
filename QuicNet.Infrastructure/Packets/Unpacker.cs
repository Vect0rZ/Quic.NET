using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Packets
{
    public class Unpacker
    {
        public Packet Unpack(byte[] data)
        {
            Packet result = null;

            QuicPacketType type = GetPacketType(data);
            switch(type)
            {
                case QuicPacketType.Initial: result = new InitialPacket(); break;
                case QuicPacketType.ShortHeader: result = new ShortHeaderPacket(); break;
            }

            if (result == null)
                return null;

            result.Decode(data);

            return result;
        }

        public QuicPacketType GetPacketType(byte[] data)
        {
            if (data == null || data.Length <= 0)
                return QuicPacketType.Broken;

            byte type = data[0];

            // TODO: Understand how to differentiate between Initial packet and Long Header Packet,
            // and if there is any need to do so, or the LHP is just used for deriving the Initial packet.

            if ((type & 0xC0) == 0xC0)
                return QuicPacketType.Initial;
            if ((type & 0x40) == 0x40)
                return QuicPacketType.ShortHeader;
            if ((type & 0x80) == 0x80)
                return QuicPacketType.VersionNegotiation;
            
            return QuicPacketType.Broken;            
        }
    }
}
