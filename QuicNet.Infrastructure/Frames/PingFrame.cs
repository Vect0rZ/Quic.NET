using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class PingFrame : Frame
    {
        public override byte Type => 0x01;

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();
        }

        public override byte[] Encode()
        {
            List<byte> data = new List<byte>();
            data.Add(Type);

            return data.ToArray();
        }
    }
}
