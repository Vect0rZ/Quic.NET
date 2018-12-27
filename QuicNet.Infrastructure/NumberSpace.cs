using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure
{
    public class NumberSpace
    {
        private UInt32 _max = UInt32.MaxValue;
        private UInt32 _n = 0;

        public NumberSpace()
        {
        }

        public NumberSpace(UInt32 max)
        {
            _max = max;
        }

        public bool IsMax()
        {
            return _n == _max;
        }

        public UInt32 Get()
        {
            if (_n >= _max)
                return 0;

            _n++;
            return _n;
        }
    }
}
