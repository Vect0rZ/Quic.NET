using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Exceptions
{
    public class QuicConnectivityException : Exception
    {
        public QuicConnectivityException(string message) : base(message)
        {
        }
    }
}
