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

        public QuicListener()
        {
            _endPoint = new IPEndPoint(IPAddress.Any, 11000);
            _client = new UdpClient(11000);
            _unpacker = new Unpacker();
        }

        public void Start()
        {
            
        }

        public byte[] Receive()
        {
            byte[] data = _client.Receive(ref _endPoint);

            PacketType type = _unpacker.GetPacketType(data);
            switch(type)
            {
                case PacketType.InitialPacket:
                    {
                        InitialPacket packet = new InitialPacket();
                        packet.Decode(data);

                        ConnectionPool.AddConnection(packet.SourceConnectionId);
                        break;
                    }
                case PacketType.BrokenPacket:
                    {
                        // Discard packet
                        break;
                    }
            }

            return data;
        }
    }
}
