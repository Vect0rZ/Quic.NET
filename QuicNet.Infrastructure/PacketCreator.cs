using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
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

        public InitialPacket CreateInitialPacket(byte sourceConnectionId, byte destinationConnectionId)
        {
            InitialPacket packet = new InitialPacket();
            packet.SourceConnectionId = sourceConnectionId;
            packet.DestinationConnectionId = destinationConnectionId;
            packet.Version = QuicVersion.CurrentVersion;

            int length = packet.Encode().Length;
            int padding = QuicSettings.PMTU - length;

            for (int i = 0; i < padding; i++)
                packet.AttachFrame(new PaddingFrame());

            return packet;
        }

        public VersionNegotiationPacket CreateVersionNegotiationPacket()
        {
            return new VersionNegotiationPacket();
        }

        public InitialPacket CreateServerBusyPacket()
        {
            return new InitialPacket();
        }

        public ShortHeaderPacket CreateShortHeaderPacket()
        {
            return new ShortHeaderPacket();
        }
    }
}
