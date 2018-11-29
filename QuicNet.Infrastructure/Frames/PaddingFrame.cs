using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class PaddingFrame : Frame
    {
        public override byte Type => 0x00;

        public override byte[] Build()
        {
            throw new NotImplementedException();
        }
    }
}
