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
        public StreamId StreamId { get; set; }

        public MaxStreamsFrame()
        {

        }

        public MaxStreamsFrame(UInt64 maximumStreamId, StreamType appliesTo)
        {
            StreamId = new StreamId(maximumStreamId, appliesTo);
        }

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();
            StreamId = array.ReadStreamId();
        }

        public override byte[] Encode()
        {
            List<byte> result = new List<byte>();
            result.Add(Type);

            byte[] streamId = StreamId;
            result.AddRange(streamId);

            return result.ToArray();
        }
    }
}
