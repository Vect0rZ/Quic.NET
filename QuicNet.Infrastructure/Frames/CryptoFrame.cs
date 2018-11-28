using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class CryptoFrame : Frame
    {
        public override byte Type => 0x18;

        public override byte[] Build()
        {
            throw new NotImplementedException();
        }
    }
}
