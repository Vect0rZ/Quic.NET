using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class MaxStreamsFrame : Frame
    {
        public override byte Type => 0x12;
        public VariableInteger MaximumStreams { get; set; }

        public MaxStreamsFrame()
        {

        }

        public MaxStreamsFrame(UInt64 maximumStreamId, StreamType appliesTo)
        {
            MaximumStreams = new VariableInteger(maximumStreamId);
        }

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();
            MaximumStreams = array.ReadVariableInteger();
        }

        public override byte[] Encode()
        {
            List<byte> result = new List<byte>();

            result.Add(Type);
            result.AddRange(MaximumStreams.ToByteArray());

            return result.ToArray();
        }
    }
}
