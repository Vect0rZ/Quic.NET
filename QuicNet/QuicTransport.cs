using QuicNet.Connections;
using QuicNet.Infrastructure.Packets;
using QuicNet.InternalInfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet
{
    public class QuicTransport
    {
        /// <summary>
        /// Processes short header packet, by distributing the frames towards connections.
        /// </summary>
        /// <param name="packet"></param>
        protected void ProcessShortHeaderPacket(Packet packet)
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
