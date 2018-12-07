using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Context;
using QuicNet.Exceptions;
using QuicNet.Infrastructure;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
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

        private int _port;
        private bool _started;

        public event Action<QuicContext> OnClientConnected;

        public QuicListener(int port)
        {
            _started = false;
            _port = port;
            
            _unpacker = new Unpacker();
            _packetCreator = new PacketCreator();
        }

        public void Start()
        {
            _client = new UdpClient(_port);
            _started = true;
        }

        public void Close()
        {
            _client.Close();
        }

        public void Receive()
        {
            if (!_started)
                throw new QuicListenerNotStartedException("Please call the Start() method before receving data.");

            while (true)
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, _port);
                byte[] data = _client.Receive(ref endpoint);

                Packet packet = _unpacker.Unpack(data);

                // Discard unknown packets
                if (packet == null)
                    continue;

                // TODO: Validate packet before dispatching
                ProcessPacket(packet, endpoint);
            }
        }

        private void ProcessPacket(Packet packet, IPEndPoint endPoint)
        {
            if (packet is InitialPacket)
            {
                ProcessInitialPacket(packet, endPoint);
            }
            else if (packet is ShortHeaderPacket)
            {
                ProcessShortHeaderPacket(packet);
            }
        }

        private void ProcessInitialPacket(Packet packet, IPEndPoint endPoint)
        {
            byte[] data;
            // Unsupported version. Version negotiation packet is sent only on initial connection. All other packets are dropped. (5.2.2 / 16th draft)
            if (packet.Version != QuicVersion.CurrentVersion || !QuicVersion.SupportedVersions.Contains(packet.Version))
            {
                VersionNegotiationPacket vnp = _packetCreator.CreateVersionNegotiationPacket();
                data = vnp.Encode();

                _client.Send(data, data.Length, endPoint);
                return;
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

            data = ip.Encode();
            int dataSent = _client.Send(data, data.Length, endPoint);
            if (dataSent > 0)
            {
                // Create a QuicContext to represent the connected client.
                QuicContext context = new QuicContext(_client, endPoint);
                ConnectionPool.AttachContext(cast.SourceConnectionId, context);

                OnClientConnected(context);
            }
        }

        private void ProcessShortHeaderPacket(Packet packet)
        {
            ShortHeaderPacket shp = (ShortHeaderPacket)packet;

            QuicConnection connection = ConnectionPool.Find(shp.DestinationConnectionId);
            if (connection == null)
                return; // TODO: Figure out if the packet should be discarded in that case?

            connection.ProcessFrames(shp.GetFrames());
        }
    }
}
