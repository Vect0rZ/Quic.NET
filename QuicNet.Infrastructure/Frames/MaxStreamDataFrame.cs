using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class MaxStreamDataFrame : Frame
    {
        public override byte Type => 0x05;

        public override byte[] Build()
        {
            throw new NotImplementedException();
        }
    }
}
