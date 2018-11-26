using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Connections
{
    /// <summary>
    /// Since UDP is a stateless protocol, the ConnectionPool is used as a Conenction Manager to 
    /// route packets to the right "Connection".
    /// </summary>
    public class ConnectionPool
    {
        /// <summary>
        /// Starting point for connection identifiers.
        /// ConnectionId's are incremented sequentially by 1.
        /// </summary>
        private static UInt32 _connectionIdIterator = 0;

        private Dictionary<UInt32, QuicConnection> _pool = new Dictionary<UInt32, QuicConnection>();

        public ConnectionPool()
        {

        }
    }
}
