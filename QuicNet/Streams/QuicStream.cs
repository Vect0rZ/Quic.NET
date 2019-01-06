using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Context;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Packets;
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

        public StreamState State { get; set; }
        public StreamType Type { get; set; }
        public StreamId StreamId { get; }

        public byte[] Data
        {
            get
            {
                return _data.SelectMany(v => v.Value).ToArray();
            }
        }

        public QuicStream(QuicConnection connection, StreamId streamId)
        {
            StreamId = streamId;
            Type = streamId.Type;

            _connection = connection;
        }

        public bool Send(byte[] data)
        {
            ShortHeaderPacket packet = _connection.PacketCreator.CreateDataPacket(this.StreamId.Value, data);

            return _connection.SendData(packet);
        }

        public byte[] Receive()
        {
            while (!IsStreamFull() || State == StreamState.Recv)
            {
                _connection.ReceivePacket();
            }

            return Data;
        }

        public void ResetStream(ResetStreamFrame frame)
        {
            // Reset the state
            State = StreamState.ResetRecvd;
            // Clear data
            _data.Clear();
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
                _connection.DataReceived(this);

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
    }
}
