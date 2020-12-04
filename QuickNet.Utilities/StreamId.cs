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
        public UInt64 Id { get; }
        public UInt64 IntegerValue { get; }
        public StreamType Type { get; private set; }

        public StreamId(UInt64 id, StreamType type)
        {
            Id = id;
            Type = type;
            IntegerValue = id << 2 | (UInt64)type;
        }

        public static implicit operator byte[](StreamId id)
        {
            return Encode(id.Id, id.Type);
        }

        public static implicit operator StreamId(byte[] data)
        {
            return Decode(data);
        }

        public static implicit operator UInt64(StreamId streamId)
        {
            return streamId.Id;
        }

        public static implicit operator StreamId(VariableInteger integer)
        {
            return Decode(ByteUtilities.GetBytes(integer.Value));
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
