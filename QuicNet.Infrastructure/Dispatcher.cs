using QuickNet.Utilities;
using QuicNet.Infrastructure.Connections;
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
    public class Dispatcher
    {
        private PacketCreator _packetCreator;
        public Dispatcher(PacketCreator packetCreator)
        {
            _packetCreator = packetCreator;
        }

        public Packet Dispatch(Packet packet)
        {
            Packet result = null;
            // Initial connection
            if (packet is InitialPacket)
                result = ProcessInitialPacket(packet);

            return result;
        }

        private Packet ProcessInitialPacket(Packet packet)
        {
            // Unsupported version. Version negotiation packet is sent only on initial connection. All other packets are dropped. (5.2.2 / 16th draft)
            if (packet.Version != QuicVersion.CurrentVersion || !QuicVersion.SupportedVersions.Contains(packet.Version))
            {
                VersionNegotiationPacket vnp = _packetCreator.CreateVersionNegotiationPacket();
                return vnp;
            }

            InitialPacket ip = new InitialPacket();
            ip.DestinationConnectionId = ip.SourceConnectionId;

            InitialPacket cast = packet as InitialPacket;
            if (ConnectionPool.AddConnection(cast.SourceConnectionId) == true)
            {
                // We're including the maximum possible stream id during the connection handshake. (4.5 / 16th draft)
                ip.AttachFrame(new MaxStreamIdFrame(QuicSettings.MaximumStreamId, StreamType.ServerBidirectional));
            }
            else
            {
                // Not accepting connections. Send initial packet with CONNECTION_CLOSE frame.
                // TODO: Buffering. The server might buffer incomming 0-RTT packets in anticipation of late delivery InitialPacket.
                // Maximum buffer size should be set in QuicSettings.
                ip.AttachFrame(new ConnectionCloseFrame(ErrorCode.SERVER_BUSY, "The server is too busy to process your request."));
            }

            return ip;
        }

        private Packet ProcessPacketFrames(Packet packet)
        {
            Packet result = null;

            List<Frame> frames = packet.GetFrames();
            if (frames == null)
                return result;

            foreach (Frame frame in frames)
            {

            }

            return result;
        }

        private Packet ProcessFrame(Frame frame)
        {
            Packet result = null;

            switch (frame.Type)
            {
                case 0x00: break;
                case 0x01: break;
            }

            return result;
        }
    }
}
