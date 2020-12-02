using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Context;
using QuicNet.Exceptions;
using QuicNet.Infrastructure;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.PacketProcessing;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
using QuicNet.InternalInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet
{
    /// <summary>
    /// Quic Listener - a Quic server that processes incomming connections and if possible sends back data on it's peers.
    /// </summary>
    public class QuicListener
    {
        private UdpClient _client;

        private Unpacker _unpacker;
        private InitialPacketCreator _packetCreator;

        private PacketWireTransfer _pwt;

        private int _port;
        private bool _started;

        /// <summary>
        /// Create a new instance of QuicListener.
        /// </summary>
        /// <param name="port">The port that the server will listen on.</param>
        public QuicListener(int port)
        {
            _started = false;
            _port = port;
            
            _unpacker = new Unpacker();
            _packetCreator = new InitialPacketCreator();
        }

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public void Start()
        {
            _client = new UdpClient(_port);
            _started = true;
            _pwt = new PacketWireTransfer(_client, null);
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public void Close()
        {
            _client.Close();
        }

        /// <summary>
        /// Blocks and waits for incomming connection. Does NOT block additional incomming packets.
        /// </summary>
        /// <returns>Returns an instance of QuicConnection.</returns>
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
                Packet packet = _pwt.ReadPacket();
                if (packet is InitialPacket)
                {
                    QuicConnection connection = ProcessInitialPacket(packet, _pwt.LastTransferEndpoint());
                    return connection;
                }

                OrchestratePacket(packet);
            }
        }

        /// <summary>
        /// Blocks and waits for incomming connection. Does NOT block additional incomming packets.
        /// </summary>
        /// <returns>Returns an instance of QuicConnection.</returns>
        public async Task<QuicConnection> AcceptQuicClientAsync()
        {
            if (!_started)
                throw new QuicListenerNotStartedException("Please call the Start() method before receving data.");

            /*
             * Wait until there is initial packet incomming.
             * Otherwise we still need to orchestrate any other protocol or data pakcets.
             * */
            while (true)
            {
                Packet packet = await _pwt.ReadPacketAsync();
                if (packet is InitialPacket)
                {
                    QuicConnection connection = await ProcessInitialPacketAsync(packet, _pwt.LastTransferEndpoint());
                    return connection;
                }

                OrchestratePacket(packet);
            }
        }

        /// <summary>
        /// Starts receiving data from clients.
        /// </summary>
        private void Receive()
        {
            while (true)
            {
                Packet packet = _pwt.ReadPacket();

                // Discard unknown packets
                if (packet == null)
                    continue;

                // TODO: Validate packet before dispatching
                OrchestratePacket(packet);
            }
        }

        /// <summary>
        /// Starts receiving data from clients.
        /// </summary>
        private async Task ReceiveAsync()
        {
            while (true)
            {
                Packet packet = await _pwt.ReadPacketAsync();

                // Discard unknown packets
                if (packet == null)
                    continue;

                // TODO: Validate packet before dispatching
                OrchestratePacket(packet);
            }
        }

        /// <summary>
        /// Orchestrates packets to connections, depending on the packet type.
        /// </summary>
        /// <param name="packet"></param>
        private void OrchestratePacket(Packet packet)
        {
            if (packet is ShortHeaderPacket)
            {
                ProcessShortHeaderPacket(packet);
            }
        }

        /// <summary>
        /// Processes incomming initial packet and creates or halts a connection.
        /// </summary>
        /// <param name="packet">Initial Packet</param>
        /// <param name="endPoint">Peer's endpoint</param>
        /// <returns></returns>
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
            else if (ConnectionPool.AddConnection(new ConnectionData(_pwt, cast.SourceConnectionId, 0), out availableConnectionId) == true)
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

        /// <summary>
        /// Processes incomming initial packet and creates or halts a connection.
        /// </summary>
        /// <param name="packet">Initial Packet</param>
        /// <param name="endPoint">Peer's endpoint</param>
        /// <returns></returns>
        private async Task<QuicConnection> ProcessInitialPacketAsync(Packet packet, IPEndPoint endPoint)
        {
            QuicConnection result = null;
            UInt32 availableConnectionId;
            byte[] data;
            // Unsupported version. Version negotiation packet is sent only on initial connection. All other packets are dropped. (5.2.2 / 16th draft)
            if (packet.Version != QuicVersion.CurrentVersion || !QuicVersion.SupportedVersions.Contains(packet.Version))
            {
                VersionNegotiationPacket vnp = _packetCreator.CreateVersionNegotiationPacket();
                data = vnp.Encode();

                await _client.SendAsync(data, data.Length, endPoint);
                return null;
            }

            InitialPacket cast = packet as InitialPacket;
            InitialPacket ip = _packetCreator.CreateInitialPacket(0, cast.SourceConnectionId);

            // Protocol violation if the initial packet is smaller than the PMTU. (pt. 14 / 16th draft)
            if (cast.Encode().Length < QuicSettings.PMTU)
            {
                ip.AttachFrame(new ConnectionCloseFrame(ErrorCode.PROTOCOL_VIOLATION, "PMTU have not been reached."));
            }
            else if (ConnectionPool.AddConnection(new ConnectionData(_pwt, cast.SourceConnectionId, 0), out availableConnectionId) == true)
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
            int dataSent = await _client.SendAsync(data, data.Length, endPoint);
            if (dataSent > 0)
                return result;

            return null;
        }

        /// <summary>
        /// Processes short header packet, by distributing the frames towards connections.
        /// </summary>
        /// <param name="packet"></param>
        private void ProcessShortHeaderPacket(Packet packet)
        {
            ShortHeaderPacket shp = (ShortHeaderPacket)packet;

            QuicConnection connection = ConnectionPool.Find(shp.DestinationConnectionId);

            // No suitable connection found. Discard the packet.
            if (connection == null)
                return;

            connection.ProcessFrames(shp.GetFrames());
        }
    }
}
