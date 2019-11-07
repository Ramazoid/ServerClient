using System; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using Newtonsoft.Json;
using System.Collections.Generic;

namespace postClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5550);
             
            
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            client.Connect(ipPoint);
            Console.WriteLine("connected.");

            Console.Write("from:");
            string from = Console.ReadLine();
            Console.Write("to:");
            string to = Console.ReadLine();
            Dictionary<string, string> move = new Dictionary<string, string>();
            move["from"] = from;
            move["to"] = to;
            string jstring = JsonConvert.SerializeObject(move);
            byte[] message = Encoding.ASCII.GetBytes(jstring); 
            int byteSent = client.Send(message); 
  
             
            byte[] messageReceived = new byte[1024];  
            int byteRecv = client.Receive(messageReceived); 
            Console.WriteLine("Response from Server -> {0}",  
                Encoding.ASCII.GetString(messageReceived,  
                    0, byteRecv)); 
  
            // Close Socket using  
            // the method Close() 
            client.Shutdown(SocketShutdown.Both); 
            client.Close(); 
        }
    }
}e