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
            VariableInteger vi = new VariableInteger();
            vi.Convert(494878333);

            var a = 2 << 62;
            string binary = Convert.ToString(3L << 30, 2);
            var res = (2U << 30) | (2642361981U >> 2);
        }
    }
}
