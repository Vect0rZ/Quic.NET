using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Constants;
using QuicNet.Context;
using QuicNet.Events;
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
using System.Text;
using System.Threading.Tasks;

namespace QuicNet
{
    /// <summary>
    /// Quic Listener - a Quic server that processes incomming connections and if possible sends back data on it's peers.
    /// </summary>
    public class QuicListener : QuicTransport
    {
        private readonly Unpacker _unpacker;
        private readonly InitialPacketCreator _packetCreator;

        private PacketWireTransfer _pwt;

        private UdpClient _client;

        private int _port;
        private bool _started;

        public event ClientConnectedEvent OnClientConnected;

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

            while (true)
            {
                Packet packet = _pwt.ReadPacket();
                if (packet is InitialPacket)
                {
                    QuicConnection connection = ProcessInitialPacket(packet, _pwt.LastTransferEndpoint());

                    OnClientConnected?.Invoke(connection);
                }

                if (packet is ShortHeaderPacket)
                {
                    ProcessShortHeaderPacket(packet);
                }
            }
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public void Close()
        {
            if (_started)
                _client.Close();
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
            UInt64 availableConnectionId;
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
                ip.AttachFrame(new ConnectionCloseFrame(ErrorCode.PROTOCOL_VIOLATION, 0x00, ErrorConstants.PMTUNotReached));
            }
            else if (ConnectionPool.AddConnection(new ConnectionData(new PacketWireTransfer(_client, endPoint), cast.SourceConnectionId, 0), out availableConnectionId) == true)
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
                ip.AttachFrame(new ConnectionCloseFrame(ErrorCode.CONNECTION_REFUSED, 0x00, ErrorConstants.ServerTooBusy));
            }

            data = ip.Encode();
            int dataSent = _client.Send(data, data.Length, endPoint);
            if (dataSent > 0)
                return result;

            return null;
        }
    }
}
