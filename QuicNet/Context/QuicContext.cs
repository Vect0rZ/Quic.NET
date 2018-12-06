using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Context
{
    public class QuicContext
    {
        public IPEndPoint EndPoint { get; set; }
        public byte[] Data { get; set; }

        public QuicContext()
        {

        }
    }
}
