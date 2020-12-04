using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class DataBlockedFrame : Frame
    {
        public override byte Type => 0x14;
        public VariableInteger MaximumData { get; set; }

        public DataBlockedFrame()
        {

        }

        public DataBlockedFrame(UInt64 dataLimit)
        {
            MaximumData = dataLimit;
        }

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();
            MaximumData = array.ReadVariableInteger();
        }

        public override byte[] Encode()
        {
            List<byte> result = new List<byte>();
            result.Add(Type);
            result.AddRange(MaximumData.ToByteArray());

            return result.ToArray();
        }
    }
}
