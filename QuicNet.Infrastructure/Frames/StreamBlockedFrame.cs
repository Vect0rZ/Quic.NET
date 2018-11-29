using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class StreamBlockedFrame : Frame
    {
        public override byte Type => 0x09;

        public override byte[] Build()
        {
            throw new NotImplementedException();
        }
    }
}
