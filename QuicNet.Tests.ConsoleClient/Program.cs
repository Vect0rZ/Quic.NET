using System;
using System.Text;
using QuicNet.Connections;
using QuicNet.Streams;

namespace QuicNet.Tests.ConsoleClient
{

    class Program
    {
        static void Main(string[] args)
        {
            QuicClient client = new QuicClient();

            // Connect to peer (Server)
            QuicConnection connection = client.Connect("127.0.0.1", 11000);
            // Create a data stream
            QuicStream stream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional);
            // Send Data
            stream.Send(Encoding.UTF8.GetBytes("Hello from Client!"));   
            // Wait reponse back from the server
            byte[] data = stream.Receive();

            Console.WriteLine(Encoding.UTF8.GetString(data));

            // Create a new data stream
            stream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional);
            // Send Data
            stream.Send(Encoding.UTF8.GetBytes("Hello from Client2!"));
            // Wait reponse back from the server
            data = stream.Receive();

            Console.WriteLine(Encoding.UTF8.GetString(data));

            Console.ReadKey();
        }
    }
}
