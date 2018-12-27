using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Infrastructure.Packets;
using QuicNet.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Context
{
    /// <summary>
    /// Wrapper of the UdpClient to represent the Connection.
    /// </summary>
    public class QuicContext
    {
        private UdpClient _client;

        public IPEndPoint Endpoint { get; }
        public bool IsConnectionClosed { get; private set; }
        public event Action<QuicStreamContext> OnDataReceived;

        #region Internal

        internal QuicConnection Connection { get; set; }

        /// <summary>
        /// Internal constructor to prevent creating the context outside the scope of Quic.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endpoint"></param>
        internal QuicContext(UdpClient client, IPEndPoint endpoint)
        {
            _client = client;
            Endpoint = endpoint;
        }

        internal void DataReceived(QuicStreamContext context)
        {
            OnDataReceived?.Invoke(context);
        }

        /// <summary>
        /// Used to send protocol packets to the peer.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        internal bool Send(Packet packet)
        {
            // Encode the packet
            byte[] data = packet.Encode();

            // Increment the connection transfer rate
            Connection.IncrementRate(data.Length);

            // If the maximum transfer rate is reached, send FLOW_CONTROL_ERROR
            if (Connection.MaximumReached())
            {
                packet = Connection.PacketCreator.CreateConnectionClosePacket(Infrastructure.ErrorCode.FLOW_CONTROL_ERROR, "Maximum data transfer reached.");
                data = packet.Encode();

                Connection.TerminateConnection();

                return false;
            }

            // Ignore empty packets
            if (data == null || data.Length <= 0)
                return true;

            int result = _client.Send(data, data.Length, Endpoint);
            if (result <= 0)
                return false;

            return true;
        }

        #endregion
    }
}
