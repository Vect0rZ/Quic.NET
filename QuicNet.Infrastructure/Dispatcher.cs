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
            // Initial connection
            if (packet is InitialPacket)
            {
                // Unsupported version. Version negotiation packet is sent only on initial connection. All other packets are dropped. (5.2.2)
                if (packet.Version != QuicVersion.CurrentVersion || !QuicVersion.SupportedVersions.Contains(packet.Version))
                {
                    VersionNegotiationPacket vnp = _packetCreator.CreateVersionNegotiationPacket();
                    return vnp;
                }

                InitialPacket cast = packet as InitialPacket;
                if (!ConnectionPool.AddConnection(cast.SourceConnectionId))
                {
                    // Not accepting connections. Send initial packet with CONNECTION_CLOSE frame.
                    InitialPacket error = _packetCreator.CreateServerBusyPacket();
                }

            }

            // TODO: Buffering. The server might buffer incomming 0-RTT packets in anticipation of late delivery InitialPacket.
            // Maximum buffer size should be set in QuicSettings.

            return null;
        }
    }
}
