using QuickNet.Utilities;
using QuicNet.Infrastructure.Connections;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Streams
{
    /// <summary>
    /// Virtual multiplexing channel.
    /// </summary>
    public class QuicStream
    {
        public StreamState State { get; set; }
        public StreamId StreamId { get; }

        private QuicConnection _connection;

        public QuicStream(QuicConnection connection, StreamId streamId)
        {
            StreamId = streamId;
            _connection = connection;
        }
    }
}
