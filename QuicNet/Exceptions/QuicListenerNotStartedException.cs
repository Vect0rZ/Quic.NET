using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Exceptions
{
    public class QuicListenerNotStartedException : Exception
    {
        public QuicListenerNotStartedException() { }

        public QuicListenerNotStartedException(string message) : base(message)
        {
        }
    }
}
