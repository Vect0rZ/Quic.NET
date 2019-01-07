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
        public VariableInteger DataLimit { get; set; }

        public DataBlockedFrame(UInt64 dataLimit)
        {
            DataLimit = dataLimit;
        }

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();
            DataLimit = array.ReadVariableInteger();
        }

        public override byte[] Encode()
        {
            List<byte> result = new List<byte>();
            result.Add(Type);
            byte[] dataLimit = DataLimit;

            result.AddRange(dataLimit);

            return result.ToArray();
        }
    }
}
