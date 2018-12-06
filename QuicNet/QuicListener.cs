using QuicNet.Context;
using QuicNet.Infrastructure;
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

        private Unpacker _unpacker;
        private PacketCreator _packetCreator;
        private Dispatcher _dispatcher;

        private int _port;
        private bool _started;

        public event Action<QuicContext> OnDataReceived;

        public QuicListener(int port)
        {
            _started = false;
            _port = port;
            
            _unpacker = new Unpacker();
            _packetCreator = new PacketCreator();
            _dispatcher = new Dispatcher(_packetCreator);
        }

        public void Start()
        {
            _client = new UdpClient(_port);
        }

        public void Close()
        {
            _client.Close();
        }

        public void Receive()
        {
            while (true)
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, _port);
                byte[] data = _client.Receive(ref endpoint);

                Packet packet = _unpacker.Unpack(data);

                // Discard unknown packets
                if (packet == null)
                    continue;

                // TODO: Validate packet before dispatching
                Packet result = _dispatcher.Dispatch(packet);

                // Send a response packet if the dispatcher has information for the peer. (1-RTT)
                if (result != null)
                {

                }
            }
        }
    }
}
