using QuicNet.Context;
using QuicNet.Infrastructure.PacketProcessing;
using QuicNet.Infrastructure.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet
{
    public class QuicClient
    {
        private IPEndPoint _peerIp;
        private UdpClient _client;

        private Unpacker _unpacker;
        private InitialPacketCreator _packetCreator;

        public QuicContext Context { get; private set; }

        public QuicClient()
        {

        }

        public bool Connect(string ip, int port)
        {
            // Establish socket connection
            _peerIp = new IPEndPoint(IPAddress.Parse(ip), port);
            _client.Connect(_peerIp);

            // Create quic context
            Context = new QuicContext(_client, _peerIp);

            // Start initial protocol process
            InitialPacket connectionPacket = _packetCreator.CreateInitialPacket(0, 0);
            if (!Context.Send(connectionPacket))
                return false;

            return true;
        }
    }
}
