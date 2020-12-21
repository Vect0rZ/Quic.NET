using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class MaxDataFrame : Frame
    {
        public override byte Type => 0x10;
        public VariableInteger MaximumData { get; set; }

        public override void Decode(ByteArray array)
        {
            array.ReadByte();
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
