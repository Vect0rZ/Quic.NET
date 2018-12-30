using QuicNet.Context;
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
            QuicContext context = client.Connect("127.0.0.1", 11000);   // Connect to peer (Server)
            QuicStreamContext sc = client.CreateStream();               // Create a data stream
            sc.Send(Encoding.UTF8.GetBytes("Hello from Client!"));      // Send Data

            sc.Close();                                                 // Close the stream after processing
        }
    }
}
