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
        /// Used to send stream data back to it's peer.
        /// This method assumes that the data is an actual encoded packet.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal bool Send(byte[] data)
        {
            int result = _client.Send(data, data.Length, Endpoint);
            if (result <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Used to send protocol packets to the peer.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        internal bool Send(Packet packet)
        {
            byte[] data = packet.Encode();

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
