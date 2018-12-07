using QuicNet.Infrastructure.Packets;
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
    /// Wrapper of the UdpClient to be referenced in the Connections/Streams.
    /// </summary>
    public class UdpSession
    {
        private UdpClient _client;
        private IPEndPoint _endpoint;

        public UdpSession(UdpClient client, IPEndPoint endpoint)
        {
            _client = client;
            _endpoint = endpoint;
        }

        public bool Send(Packet packet)
        {
            byte[] data = packet.Encode();
            // Ignore empty packets
            if (data == null || data.Length <= 0)
                return true;

            int result = _client.Send(data, data.Length, _endpoint);
            if (result <= 0)
                return false;

            return true;
        }
    }
}
