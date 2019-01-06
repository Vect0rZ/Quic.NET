using QuicNet.Exceptions;
using QuicNet.Infrastructure.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.InternalInfrastructure
{
    internal class PacketWireTransfer
    {
        private UdpClient _client;
        private IPEndPoint _peerEndpoint;

        private Unpacker _unpacker;

        public PacketWireTransfer(UdpClient client, IPEndPoint peerEndpoint)
        {
            _client = client;
            _peerEndpoint = peerEndpoint;

            _unpacker = new Unpacker();
        }

        public Packet ReadPacket()
        {
            // Await response for sucessfull connection creation by the server
            byte[] peerData = _client.Receive(ref _peerEndpoint);
            if (peerData == null)
                throw new QuicConnectivityException("Server did not respond properly.");

            Packet packet = _unpacker.Unpack(peerData);

            return packet;
        }

        public bool SendPacket(Packet packet)
        {
            byte[] data = packet.Encode();

            int sent = _client.Send(data, data.Length, _peerEndpoint);

            return sent > 0;
        }

        public IPEndPoint LastTransferEndpoint()
        {
            return _peerEndpoint;
        }
    }
}
