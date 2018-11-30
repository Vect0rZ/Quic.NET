using QuicNet.Infrastructure.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure
{
    public class PacketCreator
    {
        public PacketCreator()
        {

        }

        public VersionNegotiationPacket CreateVersionNegotiationPacket()
        {
            return new VersionNegotiationPacket();
        }

        public InitialPacket CreateServerBusyPacket()
        {
            return new InitialPacket();
        }
    }
}
