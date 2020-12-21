using QuicNet.Connections;
using QuicNet.Context;
using QuicNet.Streams;
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
            Console.WriteLine("Starting client.");
            QuicClient client = new QuicClient();
            
            QuicConnection connection = client.Connect("127.0.0.1", 11000);   // Connect to peer (Server)
            QuicStream stream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional); // Create a data stream
            stream.Send(Encoding.UTF8.GetBytes("Hello from Client!"));        // Send Data
            byte[] data = stream.Receive();

            Console.WriteLine("Waiting for message from the server");
            Console.WriteLine(Encoding.UTF8.GetString(data));

            stream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional); // Create a data stream
            stream.Send(Encoding.UTF8.GetBytes("Hello from Client2!"));
            data = stream.Receive();

            Console.WriteLine("Waiting for message from the server");
            Console.WriteLine(Encoding.UTF8.GetString(data));

            Console.ReadKey();
        }
    }
}
