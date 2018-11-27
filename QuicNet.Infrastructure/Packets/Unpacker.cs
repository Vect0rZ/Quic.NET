using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Packets
{
    public class Unpacker
    {
        private Dictionary<byte, PacketType> _typeMap = new Dictionary<byte, PacketType>()
        {
            { 0xFF, PacketType.InitialPacket },
            { 0xFE, PacketType.RetryPacket },
            { 0x8E, PacketType.ShortHeaderPacket },
            { 0x77, PacketType.LongHeaderPacket },
            { 0x80, PacketType.VersionNegotiationPacket },
            { 0x7D, PacketType.HandshakePacket }
        };

        public Unpacker()
        {
            
        }

        public Packet Unpack(byte[] data)
        {
            Packet result = null;

            PacketType type = GetPacketType(data);
            switch(type)
            {
                case PacketType.InitialPacket:
                    result = new InitialPacket();
                    break;
            }

            result.Decode(data);

            return result;
        }

        public PacketType GetPacketType(byte[] data)
        {
            if (data == null || data.Length <= 0)
                return PacketType.BrokenPacket;
            byte type = data[0];

            if (_typeMap.ContainsKey(type) == false)
                return PacketType.BrokenPacket;

            return _typeMap[type];            
        }
    }
}
