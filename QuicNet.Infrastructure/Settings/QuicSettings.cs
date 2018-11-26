using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Settings
{
    public class QuicSettings
    {
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
    }
}
