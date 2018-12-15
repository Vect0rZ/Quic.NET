using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Infrastructure.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuicNet.Streams
{
    /// <summary>
    /// Virtual multiplexing channel.
    /// </summary>
    public class QuicStream
    {
        private SortedDictionary<UInt64, byte[]> _data = new SortedDictionary<ulong, byte[]>();
        private QuicConnection _connection;

        public StreamState State { get; set; }
        public StreamType Type { get; set; }
        public StreamId StreamId { get; }

        public QuicStream(QuicConnection connection, StreamId streamId)
        {
            StreamId = streamId;
            Type = streamId.Type;
            _connection = connection;
        }

        public void ProcessData(StreamFrame frame)
        { 
            byte[] data = frame.StreamData;
            if (frame.Offset != null)
            {
                _data.Add(frame.Offset.Value, frame.StreamData);
            }
            else
            {
                // TODO: Careful with duplicate 0 offset packets on the same stream. Probably PROTOCOL_VIOLATION?
                _data.Add(0, frame.StreamData);
            }
            
            if (frame.EndOfStream)
                _connection.Context.DataReceived(ReorderData(), StreamId);
        }

        private byte[] ReorderData()
        {
            byte[] data = _data.Values.SelectMany(v => v).ToArray();

            return data;
        }
    }
}
