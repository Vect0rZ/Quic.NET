using QuicNet.Infrastructure.Connections;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure
{
    public class Dispatcher
    {
        private PacketCreator _packetCreator;
        public Dispatcher(PacketCreator packetCreator)
        {
            _packetCreator = packetCreator;
        }

        public Packet Dispatch(Packet packet)
        {
            // Unsupported version
            if (packet.Version != QuicVersion.CurrentVersion || !QuicVersion.SupportedVersions.Contains(packet.Version))
            {
                VersionNegotiationPacket vnp = _packetCreator.CreateVersionNegotiationPacket();
                return vnp;
            }

            // Initial connection
            if (packet is InitialPacket)
            {
                InitialPacket cast = packet as InitialPacket;

                if (!ConnectionPool.AddConnection(cast.SourceConnectionId))
                {

                }
            }

            return null;
        }
    }
}
