using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Exceptions
{
    public class StreamException : Exception
    {
        public StreamException() { }

        public StreamException(string message) : base(message)
        {

        }
    }
}
