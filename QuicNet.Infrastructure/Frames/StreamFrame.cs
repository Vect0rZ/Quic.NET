using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class StreamFrame : Frame
    {
        public override byte Type => 0x10;
        public VariableInteger StreamId { get; set; }
        public VariableInteger Offset { get; set; }
        public VariableInteger Length { get; set; }
        public byte[] StreamData { get; set; }
        public StreamId ConvertedStreamId { get; set; }

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();
            StreamId = array.ReadVariableInteger();
            Offset = array.ReadVariableInteger();
            Length = array.ReadVariableInteger();
            StreamData = array.ReadBytes((int)Length.Value);
            ConvertedStreamId = QuickNet.Utilities.StreamId.Decode(ByteUtilities.GetBytes(Length.Value));
        }

        public override byte[] Encode()
        {
            List<byte> result = new List<byte>();
            result.Add(Type);
            byte[] streamId = StreamId;
            byte[] offset = Offset;
            byte[] length = Length;
            result.AddRange(streamId);
            result.AddRange(offset);
            result.AddRange(length);
            result.AddRange(StreamData);

            return result.ToArray();
        }
    }
}
