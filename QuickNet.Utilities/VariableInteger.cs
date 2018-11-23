using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNet.Utilities
{
    public class VariableInteger
    {
        private int _size;
        private byte[] _bytes;
        public Int64 Integer { get; set; }

        public UInt64 Convert(UInt64 integer)
        {
            // Test
            UInt64 res = 0;
            if (integer < 64)
            {
                res = (integer >> 2);
            }
            else if (integer < 16383)
            {
                res = (1 << 14) | (integer >> 2);
            }
            else if (integer < 1073741823)
            {
                res = (2UL << 30) | (integer >> 2);
            }

            return res;
        }
    }
}
