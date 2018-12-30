using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Context;
using QuicNet.Exceptions;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.PacketProcessing;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
using QuicNet.Streams;
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

        private QuicConnection _connection;
        private QuicStream _stream;

        private Unpacker _unpacker;
        private InitialPacketCreator _packetCreator;

        private UInt64 _maximumStreams = QuicSettings.MaximumStreamId;

        public QuicClient()
        {
            _client = new UdpClient();
            _unpacker = new Unpacker();
            _packetCreator = new InitialPacketCreator();
        }

        public QuicContext Connect(string ip, int port)
        {
            // Establish socket connection
            _peerIp = new IPEndPoint(IPAddress.Parse(ip), port);

            // Start initial protocol process
            InitialPacket connectionPacket = _packetCreator.CreateInitialPacket(0, 0);
            byte[] data = connectionPacket.Encode();

            // Send the initial packet
            _client.Send(data, data.Length, _peerIp);

            // Await response for sucessfull connection creation by the server
            byte[] peerData = _client.Receive(ref _peerIp);
            if (peerData == null)
                throw new QuicConnectivityException("Server did not respond properly.");

            Packet packet = _unpacker.Unpack(peerData);
            if ((packet is InitialPacket) == false)
                throw new QuicConnectivityException("Server did not respond properly.");

            InitialPacket ini = (InitialPacket)packet;

            HandleInitialFrames(packet);
            EstablishConnection(ini.SourceConnectionId, ini.SourceConnectionId);

            // Create the QuicContext
            QuicContext context = new QuicContext(_client, _peerIp);

            // Cross reference with Connection
            _connection.AttachContext(context);

            return context;
        }

        public QuicStreamContext CreateStream()
        {
            QuicStream stream = new QuicStream(_connection, new StreamId(1, StreamType.ClientBidirectional));
            
            return stream.Context;
        }

        private void HandleInitialFrames(Packet packet)
        {
            List<Frame> frames = packet.GetFrames();
            for (int i = frames.Count - 1; i > 0; i--)
            {
                Frame frame = frames[i];
                if (frame is ConnectionCloseFrame)
                {
                    ConnectionCloseFrame ccf = (ConnectionCloseFrame)frame;

                    throw new QuicConnectivityException(ccf.ReasonPhrase);
                }

                if (frame is MaxStreamsFrame)
                {
                    MaxStreamsFrame msf = (MaxStreamsFrame)frame;
                    _maximumStreams = msf.MaximumStreams.Value;
                }

                // Break out if the first Padding Frame has been reached
                if (frame is PaddingFrame)
                    break;
            }
        }

        private void EstablishConnection(UInt32 connectionId, UInt32 peerConnectionId)
        {
            _connection = new QuicConnection(connectionId, peerConnectionId);
        }
    }
}
