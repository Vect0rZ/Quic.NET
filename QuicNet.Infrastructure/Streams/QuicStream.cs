using QuickNet.Utilities;
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

        public QuicStream(StreamId streamId)
        {
            StreamId = streamId;
        }
    }
}
