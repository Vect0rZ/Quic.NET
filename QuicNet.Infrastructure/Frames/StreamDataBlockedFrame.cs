using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class StreamDataBlockedFrame : Frame
    {
        public override byte Type => 0x15;
        public VariableInteger StreamId { get; set; }
        public VariableInteger MaximumStreamData { get; set; }

        public StreamDataBlockedFrame()
        {

        }

        public StreamDataBlockedFrame(UInt64 streamId, UInt64 streamDataLimit)
        {
            StreamId = streamId;
            MaximumStreamData = streamDataLimit;
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
