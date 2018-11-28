using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class MaxDataFrame : Frame
    {
        public override byte Type => 0x04;

        public override byte[] Build()
        {
            throw new NotImplementedException();
        }
    }
}
