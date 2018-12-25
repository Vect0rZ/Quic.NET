using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class ResetStreamFrame : Frame
    {
        public override byte Type => 0x04;
        public VariableInteger StreamId { get; set; }
        public UInt16 ApplicationErrorCode { get; set; }
        public VariableInteger FinalOffset { get; set; }

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();
            StreamId = array.ReadVariableInteger();
            ApplicationErrorCode = array.ReadUInt16();
            FinalOffset = array.ReadVariableInteger();
        }

        public override byte[] Encode()
        {
            List<byte> result = new List<byte>();

            result.Add(Type);
            byte[] streamId = StreamId;
            result.AddRange(streamId);

            result.AddRange(ByteUtilities.GetBytes(ApplicationErrorCode));

            byte[] offset = FinalOffset;
            result.AddRange(offset);

            return result.ToArray();
        }
    }
}
