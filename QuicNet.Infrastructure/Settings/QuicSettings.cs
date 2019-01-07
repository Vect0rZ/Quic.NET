using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Settings
{
    public class QuicSettings
    {

        /// <summary>
        /// Path Maximum Transmission Unit. Indicates the mandatory initial packet capacity, and the maximum UDP packet capacity.
        /// </summary>
        public const int PMTU = 1200;

        /// <summary>
        /// Does the server want the first connected client to decide it's initial connection id?
        /// </summary>
        public const bool CanAcceptInitialClientConnectionId = false;

        /// <summary>
        /// TBD. quic-transport 5.1.
        /// </summary>
        public const int MaximumConnectionIds = 8;

        /// <summary>
        /// Maximum number of streams that connection can handle.
        /// </summary>
        public const int MaximumStreamId = 128;

        /// <summary>
        /// Maximum packets that can be transferred before any data transfer (loss of packets, packet resent, infinite ack loop)
        /// </summary>
        public const int MaximumInitialPacketNumber = 100;

        /// <summary>
        /// Should the server buffer packets that came before the initial packet?
        /// </summary>
        public const bool ShouldBufferPacketsBeforeConnection = false;

        /// <summary>
        /// Limit the maximum number of frames a packet can carry.
        /// </summary>
        public const int MaximumFramesPerPacket = 10;

        /// <summary>
        /// Maximum data that can be transferred for a Connection.
        /// Currently 10MB.
        /// </summary>
        public const int MaxData = 10 * 1000 * 1000;

        /// <summary>
        /// Maximum data that can be transferred for a Stream.
        /// Currently 0.078125 MB, which is MaxData / MaximumStreamId
        /// </summary>
        public const int MaxStreamData = 78125;
    }
}
