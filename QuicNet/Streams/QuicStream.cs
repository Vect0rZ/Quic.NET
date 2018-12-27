using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Context;
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
        private SortedList<UInt64, byte[]> _data = new SortedList<ulong, byte[]>();
        private QuicConnection _connection;
        private QuicStreamContext _context;

        public StreamState State { get; set; }
        public StreamType Type { get; set; }
        public StreamId StreamId { get; }

        public QuicStream(QuicConnection connection, StreamId streamId)
        {
            StreamId = streamId;
            Type = streamId.Type;

            _connection = connection;
            _context = null;
        }

        public void ResetStream(ResetStreamFrame frame)
        {
            State = StreamState.ResetRecvd;
        }

        public bool CanSendData()
        {
            if (Type == StreamType.ServerUnidirectional)
                return false;

            if (State == StreamState.Recv || State == StreamState.SizeKnown)
                return true;

            return false;
        }

        public void ProcessData(StreamFrame frame)
        {
            // Do not accept data if the stream is reset.
            if (State == StreamState.ResetRecvd)
                return;

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

            // Either this frame marks the end of the stream,
            // or fin frame came before the data frames
            if (frame.EndOfStream)
                State = StreamState.SizeKnown;

            if (State == StreamState.SizeKnown && IsStreamFull())
            {
                byte[] aggregatedData = _data.SelectMany(v => v.Value).ToArray();
                BuildContext(aggregatedData);

                _connection.Context.DataReceived(_context);

                State = StreamState.DataRecvd;
            }
                
        }

        private bool IsStreamFull()
        {
            UInt64 length = 0;

            foreach (var kvp in _data)
            {
                if (kvp.Key > 0 && kvp.Key != length)
                    return false;

                length = (UInt64)kvp.Value.Length;
            }

            return true;
        }

        private void BuildContext(byte[] data)
        {
            _context = new QuicStreamContext(this, _connection.Context, data);
        }
    }
}
