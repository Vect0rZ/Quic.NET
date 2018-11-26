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

        private Unpacker _packetDispatcher;

        public event Action<QuicConnection> OnClientConnected;

        public QuicListener()
        {
            _endPoint = new IPEndPoint(IPAddress.Any, 11000);
            _client = new UdpClient(11000);
        }

        public void Start()
        {
            byte[] data = _client.Receive(ref _endPoint);
        }
    }
}
