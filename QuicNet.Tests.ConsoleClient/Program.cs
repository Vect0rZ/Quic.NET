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

            Console.ReadKey();
        }

        static string GenerateData(int kb)
        {
            string res = "";
            for (int i = 0; i < kb; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    res += "!!!!!!!!!!";
                }
            }

            return res;
        }

        static string GenerateBytes(int bytes)
        {
            string result = string.Empty;
            for (int i = 0; i < bytes; i++)
            {
                result += "!";
            }

            return result;
        }
    }
}
