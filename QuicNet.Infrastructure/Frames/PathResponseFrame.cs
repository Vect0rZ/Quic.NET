using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class PathResponseFrame : Frame
    {
        public override byte Type => 0x0f;

        public override byte[] Build()
        {
            throw new NotImplementedException();
        }
    }
}
