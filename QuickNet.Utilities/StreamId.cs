using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNet.Utilities
{
    public enum StreamType
    {
        ClientBidirectional = 0x0,
        ServerBidirectional = 0x1,
        ClientUnidirectional = 0x2,
        ServerUnidirectional = 0x3
    }

    public class StreamId
    {
        private UInt64 _id;
        private StreamType _type;

        public UInt64 Value { get { return _id; } }
        public StreamType Type { get { return _type; } }

        public StreamId(UInt64 id, StreamType type)
        {
            _id = id;
            _type = type;
        }

        public static byte[] Encode(UInt64 id, StreamType type)
        {
            UInt64 identifier = id << 2 | (UInt64)type;

            byte[] result = ByteUtilities.GetBytes(identifier);

            return result;
        }

        public static StreamId Decode(byte[] data)
        {
            StreamId result;
            UInt64 id = ByteUtilities.ToUInt64(data);
            UInt64 identifier = id >> 2;
            UInt64 type = (UInt64)(0x03 & id);
            StreamType streamType = (StreamType)type;

            result = new StreamId(identifier, streamType);

            return result;
        }
    }
}
