using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Context;
using QuicNet.Exceptions;
using QuicNet.Infrastructure;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.PacketProcessing;
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
        private InitialPacketCreator _packetCreator;

        private int _port;
        private bool _started;

        public QuicListener(int port)
        {
            _started = false;
            _port = port;
            
            _unpacker = new Unpacker();
            _packetCreator = new InitialPacketCreator();
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

        public QuicConnection AcceptQuicClient()
        {
            if (!_started)
                throw new QuicListenerNotStartedException("Please call the Start() method before receving data.");

            /*
             * Wait until there is initial packet incomming.
             * Otherwise we still need to orchestrate any other protocol or data pakcets.
             * */
            while (true)
            {
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, _port);
                byte[] data = _client.Receive(ref endpoint);

                Packet packet = _unpacker.Unpack(data);
                if (packet is InitialPacket)
                {
                    QuicConnection connection = ProcessInitialPacket(packet, endpoint);
                    return connection;
                }

                OrchestratePacket(packet);
            }
        }

        private void Receive()
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
                OrchestratePacket(packet);
            }
        }

        private void OrchestratePacket(Packet packet)
        {
            if (packet is ShortHeaderPacket)
            {
                ProcessShortHeaderPacket(packet);
            }
        }

        private QuicConnection ProcessInitialPacket(Packet packet, IPEndPoint endPoint)
        {
            QuicConnection result = null;
            UInt32 availableConnectionId;
            byte[] data;
            // Unsupported version. Version negotiation packet is sent only on initial connection. All other packets are dropped. (5.2.2 / 16th draft)
            if (packet.Version != QuicVersion.CurrentVersion || !QuicVersion.SupportedVersions.Contains(packet.Version))
            {
                VersionNegotiationPacket vnp = _packetCreator.CreateVersionNegotiationPacket();
                data = vnp.Encode();

                _client.Send(data, data.Length, endPoint);
                return null;
            }

            InitialPacket cast = packet as InitialPacket;
            InitialPacket ip = _packetCreator.CreateInitialPacket(0, cast.SourceConnectionId);

            // Protocol violation if the initial packet is smaller than the PMTU. (pt. 14 / 16th draft)
            if (cast.Encode().Length < QuicSettings.PMTU)
            {
                ip.AttachFrame(new ConnectionCloseFrame(ErrorCode.PROTOCOL_VIOLATION, "PMTU have not been reached."));
            }
            else if (ConnectionPool.AddConnection(cast.SourceConnectionId, out availableConnectionId) == true)
            {
                // Tell the peer the available connection id
                ip.SourceConnectionId = (byte)availableConnectionId;

                // We're including the maximum possible stream id during the connection handshake. (4.5 / 16th draft)
                ip.AttachFrame(new MaxStreamsFrame(QuicSettings.MaximumStreamId, StreamType.ServerBidirectional));

                // Set the return result
                result = ConnectionPool.Find(availableConnectionId);
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
                return result;

            return null;
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
