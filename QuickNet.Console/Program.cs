using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNet.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] bytes = new VariableInteger(12345);
            VariableInteger integer = bytes;
            UInt64 uinteger = integer;
            int size = VariableInteger.Size(bytes[0]);

            ByteUtilities.ToUInt64(ByteUtilities.GetBytes(12345));
        }
    }
}
