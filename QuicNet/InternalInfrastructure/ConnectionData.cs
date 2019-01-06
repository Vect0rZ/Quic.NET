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
        public UInt32 ConnectionId { get; set; }
        public UInt32 PeerConnectionId { get; set; }

        public ConnectionData(PacketWireTransfer pwt, UInt32 connectionId, UInt32 peerConnnectionId)
        {
            PWT = pwt;
            ConnectionId = connectionId;
            PeerConnectionId = peerConnnectionId;
        }
    }
}
