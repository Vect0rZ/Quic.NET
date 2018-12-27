using QuickNet.Utilities;
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
        NumberSpace _ns;
        public PacketCreator()
        {
            _ns = new NumberSpace();
        }

        public InitialPacket CreateInitialPacket(byte sourceConnectionId, byte destinationConnectionId)
        {
            InitialPacket packet = new InitialPacket();
            packet.PacketNumber = _ns.Get();
            packet.SourceConnectionId = sourceConnectionId;
            packet.DestinationConnectionId = destinationConnectionId;
            packet.Version = QuicVersion.CurrentVersion;

            int length = packet.Encode().Length;
            int padding = QuicSettings.PMTU - length;

            for (int i = 0; i < padding; i++)
                packet.AttachFrame(new PaddingFrame());

            return packet;
        }

        public ShortHeaderPacket CreateConnectionClosePacket(ErrorCode code, string reason, byte destinationConnectionId)
        {
            ShortHeaderPacket packet = new ShortHeaderPacket();
            packet.PacketNumber = _ns.Get();
            packet.DestinationConnectionId = destinationConnectionId;
            packet.AttachFrame(new ConnectionCloseFrame() { ErrorCode = (UInt16)code, ReasonPhrase = reason, ReasonPhraseLength = new VariableInteger((UInt64)reason.Length) });

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
