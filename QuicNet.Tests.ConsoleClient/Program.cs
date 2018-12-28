using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Tests.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            QuicClient client = new QuicClient();
            client.Connect("127.0.0.1", 11000);
        }
    }
}
