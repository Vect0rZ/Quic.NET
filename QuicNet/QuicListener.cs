using QuicNet.Infrastructure;
using QuicNet.Infrastructure.Connections;
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
    public class QuicListener
    {
        private UdpClient _client;
        private IPEndPoint _endPoint;

        private Unpacker _unpacker;
        private PacketCreator _packetCreator;
        private Dispatcher _dispatcher;

        public QuicListener()
        {
            _endPoint = new IPEndPoint(IPAddress.Any, 11000);
            _client = new UdpClient(11000);
            _unpacker = new Unpacker();
            _packetCreator = new PacketCreator();
            _dispatcher = new Dispatcher(_packetCreator);
        }

        public void Start()
        {
            Receive();
        }

        public void Receive()
        {
            byte[] data = _client.Receive(ref _endPoint);

            Packet packet = _unpacker.Unpack(data);

            // Disgard unknown packets
            if (packet == null)
                return;

            // TODO: Validate packet before dispatching
            Packet result = _dispatcher.Dispatch(packet);

            // Send a response packet if the dispatcher has information for the peer.
            if (result != null)
            {
                
            }
        }
    }
}
