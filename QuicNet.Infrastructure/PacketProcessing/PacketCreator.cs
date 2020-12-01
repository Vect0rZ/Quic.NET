using QuickNet.Utilities;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.PacketProcessing
{
    public class PacketCreator
    {
        private NumberSpace _ns;
        private UInt32 _connectionId;
        private UInt32 _peerConnectionId;

        public PacketCreator(UInt32 connectionId, UInt32 peerConnectionId)
        {
            _ns = new NumberSpace();

            _connectionId = connectionId;
            _peerConnectionId = peerConnectionId;
        }

        public ShortHeaderPacket CreateConnectionClosePacket(ErrorCode code, string reason)
        {
            ShortHeaderPacket packet = new ShortHeaderPacket();
            packet.PacketNumber = _ns.Get();
            packet.DestinationConnectionId = (byte)_peerConnectionId;
            packet.AttachFrame(new ConnectionCloseFrame(code, reason));

            return packet;
        }

        public ShortHeaderPacket CreateDataPacket(UInt64 streamId, byte[] data, UInt64 offset)
        {
            ShortHeaderPacket packet = new ShortHeaderPacket();
            packet.PacketNumber = _ns.Get();
            packet.DestinationConnectionId = (byte)_peerConnectionId;
            packet.AttachFrame(new StreamFrame(streamId, data, offset, true));

            return packet;
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
