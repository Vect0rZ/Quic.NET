using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Context;
using QuicNet.Exceptions;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Streams
{
    /// <summary>
    /// Virtual multiplexing channel.
    /// </summary>
    public class QuicStream
    {
        private SortedList<UInt64, byte[]> _data = new SortedList<ulong, byte[]>();
        private QuicConnection _connection;
        private UInt64 _maximumStreamData;
        private UInt64 _currentTransferRate;

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

            _maximumStreamData = QuicSettings.MaxStreamData;
            _currentTransferRate = 0;

            _connection = connection;
        }

        public bool Send(byte[] data)
        {
            if (Type == StreamType.ServerUnidirectional)
                throw new StreamException("Cannot send data on unidirectional stream.");

            _connection.IncrementRate(data.Length);

            ShortHeaderPacket packet = _connection.PacketCreator.CreateDataPacket(this.StreamId.IntegerValue, data);
            if (_connection.MaximumReached())
                packet.AttachFrame(new StreamDataBlockedFrame(StreamId.IntegerValue, (UInt64)data.Length));

            return _connection.SendData(packet);
        }

        public async Task<bool> SendAsync(byte[] data)
        {
            if (Type == StreamType.ServerUnidirectional)
                throw new StreamException("Cannot send data on unidirectional stream.");

            _connection.IncrementRate(data.Length);

            ShortHeaderPacket packet = _connection.PacketCreator.CreateDataPacket(this.StreamId.IntegerValue, data);
            if (_connection.MaximumReached())
                packet.AttachFrame(new StreamDataBlockedFrame(StreamId.IntegerValue, (UInt64)data.Length));

            return await _connection.SendDataAsync(packet);
        }

        /// <summary>
        /// Client only!
        /// </summary>
        /// <returns></returns>
        public byte[] Receive()
        {
            if (Type == StreamType.ClientUnidirectional)
                throw new StreamException("Cannot receive data on unidirectional stream.");

            while (!IsStreamFull() || State == StreamState.Recv)
            {
                _connection.ReceivePacket();
            }

            return Data;
        }

        /// <summary>
        /// Client async only!
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> ReceiveAsync()
        {
            if (Type == StreamType.ClientUnidirectional)
                throw new StreamException("Cannot receive data on unidirectional stream.");

            while (!IsStreamFull() || State == StreamState.Recv)
            {
                await _connection.ReceivePacketAsync();
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

        public void SetMaximumStreamData(UInt64 maximumData)
        {
            _maximumStreamData = maximumData;
        }

        public bool CanSendData()
        {
            if (Type == StreamType.ServerUnidirectional || Type == StreamType.ClientUnidirectional)
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

            _currentTransferRate += (UInt64)data.Length;

            // Terminate connection if maximum stream data is reached
            if (_currentTransferRate >= _maximumStreamData)
            {
                ShortHeaderPacket errorPacket = _connection.PacketCreator.CreateConnectionClosePacket(Infrastructure.ErrorCode.FLOW_CONTROL_ERROR, "Maximum stream data transfer reached.");
                _connection.SendData(errorPacket);
                _connection.TerminateConnection();

                return;
            }

            if (State == StreamState.SizeKnown && IsStreamFull())
            {
                _connection.DataReceived(this);

                State = StreamState.DataRecvd;
            }
        }

        public async Task ProcessDataAsync(StreamFrame frame)
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

            _currentTransferRate += (UInt64)data.Length;

            // Terminate connection if maximum stream data is reached
            if (_currentTransferRate >= _maximumStreamData)
            {
                ShortHeaderPacket errorPacket = _connection.PacketCreator.CreateConnectionClosePacket(Infrastructure.ErrorCode.FLOW_CONTROL_ERROR, "Maximum stream data transfer reached.");
                await _connection.SendDataAsync(errorPacket);
                _connection.TerminateConnection();

                return;
            }

            if (State == StreamState.SizeKnown && IsStreamFull())
            {
                _connection.DataReceived(this);

                State = StreamState.DataRecvd;
            }
        }

        public void ProcessStreamDataBlocked(StreamDataBlockedFrame frame)
        {
            State = StreamState.DataRecvd;
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
