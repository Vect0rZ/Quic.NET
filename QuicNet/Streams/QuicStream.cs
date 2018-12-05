using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Infrastructure.Frames;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Streams
{
    /// <summary>
    /// Virtual multiplexing channel.
    /// </summary>
    public class QuicStream
    {
        public StreamState State { get; set; }
        public StreamType Type { get; set; }
        public StreamId StreamId { get; }

        private QuicConnection _connection;

        public QuicStream(QuicConnection connection, StreamId streamId)
        {
            StreamId = streamId;
            Type = streamId.Type;
            _connection = connection;
        }

        public void ProcessData(StreamFrame frame)
        {

        }
    }
}
