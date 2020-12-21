using QuickNet.Utilities;
using QuicNet.Connections;
using QuicNet.Constants;
using QuicNet.Context;
using QuicNet.Events;
using QuicNet.Exceptions;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
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
        private UInt64 _maximumStreamData;
        private UInt64 _currentTransferRate;
        private UInt64 _sendOffset;

        public StreamState State { get; set; }
        public StreamType Type { get; set; }
        public StreamId StreamId { get; }

        public StreamDataReceivedEvent OnStreamDataReceived { get; set; }

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
            _sendOffset = 0;

            _connection = connection;
        }

        public bool Send(byte[] data)
        {
            if (Type == StreamType.ServerUnidirectional)
                throw new StreamException("Cannot send data on unidirectional stream.");

            _connection.IncrementRate(data.Length);

            int numberOfPackets = (data.Length / QuicSettings.PMTU) + 1;
            int leftoverCarry = data.Length % QuicSettings.PMTU;

            for (int i = 0; i < numberOfPackets; i++)
            {
                bool eos = false;
                int dataSize = QuicSettings.PMTU;
                if (i == numberOfPackets - 1)
                {
                    eos = true;
                    dataSize = leftoverCarry;
                }

                byte[] buffer = new byte[dataSize];
                Buffer.BlockCopy(data, (Int32)_sendOffset, buffer, 0, dataSize);

                ShortHeaderPacket packet = _connection.PacketCreator.CreateDataPacket(this.StreamId.IntegerValue, buffer, _sendOffset, eos);
                if (i == 0 && data.Length >= QuicSettings.MaxStreamData)
                    packet.AttachFrame(new MaxStreamDataFrame(this.StreamId.IntegerValue, (UInt64)(data.Length + 1)));

                if (_connection.MaximumReached())
                    packet.AttachFrame(new StreamDataBlockedFrame(StreamId.IntegerValue, (UInt64)data.Length));

                _sendOffset += (UInt64)buffer.Length;

                _connection.SendData(packet);
            }
            
            return true;
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

        public bool IsOpen()
        {
            if (State == StreamState.DataRecvd || State == StreamState.ResetRecvd)
                return false;

            return true;
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
                ShortHeaderPacket errorPacket = _connection.PacketCreator.CreateConnectionClosePacket(Infrastructure.ErrorCode.FLOW_CONTROL_ERROR, frame.ActualType, ErrorConstants.MaxDataTransfer);
                _connection.SendData(errorPacket);
                _connection.TerminateConnection();

                return;
            }

            if (State == StreamState.SizeKnown && IsStreamFull())
            {
                State = StreamState.DataRecvd;

                OnStreamDataReceived?.Invoke(this, Data);
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

                length += (UInt64)kvp.Value.Length;
            }

            return true;
        }
    }
}
