using QuicNet.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Connections
{
    /// <summary>
    /// Since UDP is a stateless protocol, the ConnectionPool is used as a Conenction Manager to 
    /// route packets to the right "Connection".
    /// </summary>
    public static class ConnectionPool
    {
        /// <summary>
        /// Starting point for connection identifiers.
        /// ConnectionId's are incremented sequentially by 1.
        /// </summary>
        private static UInt32 _connectionIdIterator = 0;

        private static Dictionary<UInt32, QuicConnection> _pool = new Dictionary<UInt32, QuicConnection>();

        /// <summary>
        /// Adds a connection to the connection pool.
        /// For now assume that the client connection id is valid, and just send it back.
        /// Later this should change in a way that the server validates, and regenerates a connection Id.
        /// </summary>
        /// <param name="id">Connection Id</param>
        /// <returns></returns>
        public static bool AddConnection(UInt32 id)
        {
            if (_pool.ContainsKey(id))
                return false;

            if (_pool.Count > QuicSettings.MaximumConnectionIds)
                return false;

            _pool.Add(id, new QuicConnection(id));

            return true;
        }

        public static QuicConnection Find(UInt32 id)
        {
            if (_pool.ContainsKey(id) == false)
                return null;

            return _pool[id];
        }
    }
}
