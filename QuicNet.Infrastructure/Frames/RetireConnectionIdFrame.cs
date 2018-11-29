using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class RetireConnectionIdFrame : Frame
    {
        public override byte Type => 0x0d;

        public override byte[] Build()
        {
            throw new NotImplementedException();
        }
    }
}
