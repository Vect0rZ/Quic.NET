using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class MaxStreamDataFrame : Frame
    {
        public override byte Type => 0x11;
        public VariableInteger StreamId { get; set; }
        public VariableInteger MaximumStreamData { get; set; }

        public StreamId ConvertedStreamId { get; set; }

        public MaxStreamDataFrame()
        {

        }

        public MaxStreamDataFrame(UInt64 streamId, UInt64 maximumStreamData)
        {
            StreamId = streamId;
            MaximumStreamData = maximumStreamData;
        }

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();
            StreamId = array.ReadVariableInteger();
            MaximumStreamData = array.ReadVariableInteger();
        }

        public override byte[] Encode()
        {
            List<byte> result = new List<byte>();

            result.Add(Type);
            result.AddRange(StreamId.ToByteArray());
            result.AddRange(MaximumStreamData.ToByteArray());

            return result.ToArray();
        }
    }
}
