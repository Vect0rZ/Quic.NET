using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.InternalInfrastructure
{
    internal class ConnectionData
    {
        public PacketWireTransfer PWT { get; set; }
        public GranularInteger ConnectionId { get; set; }
        public GranularInteger PeerConnectionId { get; set; }

        public ConnectionData(PacketWireTransfer pwt, GranularInteger connectionId, GranularInteger peerConnnectionId)
        {
            PWT = pwt;
            ConnectionId = connectionId;
            PeerConnectionId = peerConnnectionId;
        }
    }
}
