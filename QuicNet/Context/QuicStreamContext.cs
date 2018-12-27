using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Packets;
using QuicNet.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Context
{
    /// <summary>
    /// Wrapper to represent the stream.
    /// </summary>
    public class QuicStreamContext
    {
        /// <summary>
        /// The connection's context.
        /// </summary>
        public QuicContext ConnectionContext { get; set; }

        /// <summary>
        /// Data received
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Unique stream identifier
        /// </summary>
        public UInt64 StreamId { get; private set; }

        /// <summary>
        /// Send data to the client.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Send(byte[] data)
        {
            if (Stream.CanSendData() == false)
                return false;

            // Ignore empty packets
            if (data == null || data.Length <= 0)
                return true;

            Packet packet = ConnectionContext.Connection.PacketCreator.CreateDataPacket(StreamId, data);
            byte[] sendData = packet.Encode();

            bool result = ConnectionContext.Send(sendData);

            return result;
        }

        #region Internal

        internal QuicStream Stream { get; set; }

        /// <summary>
        /// Internal constructor to prevent creating the context outside the scope of Quic.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="streamId"></param>
        internal QuicStreamContext(QuicStream stream, QuicContext context, byte[] data)
        {
            Stream = stream;
            StreamId = stream.StreamId;
            ConnectionContext = context;
            Data = data;
        }

        #endregion
    }
}
