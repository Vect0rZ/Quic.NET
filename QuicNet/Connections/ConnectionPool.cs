using QuicNet.Context;
using QuicNet.Infrastructure;
using QuicNet.Infrastructure.Settings;
using QuicNet.InternalInfrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Connections
{
    /// <summary>
    /// Since UDP is a stateless protocol, the ConnectionPool is used as a Conenction Manager to 
    /// route packets to the right "Connection".
    /// </summary>
    internal static class ConnectionPool
    {
        /// <summary>
        /// Starting point for connection identifiers.
        /// ConnectionId's are incremented sequentially by 1.
        /// </summary>
        private static NumberSpace _ns = new NumberSpace(QuicSettings.MaximumConnectionIds);

        private static Dictionary<UInt64, QuicConnection> _pool = new Dictionary<UInt64, QuicConnection>();

        private static List<QuicConnection> _draining = new List<QuicConnection>();

        /// <summary>
        /// Adds a connection to the connection pool.
        /// For now assume that the client connection id is valid, and just send it back.
        /// Later this should change in a way that the server validates, and regenerates a connection Id.
        /// </summary>
        /// <param name="id">Connection Id</param>
        /// <returns></returns>
        public static bool AddConnection(ConnectionData connection, out UInt64 availableConnectionId)
        {
            availableConnectionId = 0;

            if (_pool.ContainsKey(connection.ConnectionId.Value))
                return false;

            if (_pool.Count > QuicSettings.MaximumConnectionIds)
                return false;

            availableConnectionId = _ns.Get();

            connection.PeerConnectionId = connection.ConnectionId;
            _pool.Add(availableConnectionId, new QuicConnection(connection));

            return true;
        }

        public static void RemoveConnection(UInt64 id)
        {
            if (_pool.ContainsKey(id))
                _pool.Remove(id);
        }

        public static QuicConnection Find(UInt64 id)
        {
            if (_pool.ContainsKey(id) == false)
                return null;

            return _pool[id];
        }
    }
}
