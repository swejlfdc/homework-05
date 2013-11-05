using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldNumberClient;
using System.Net;
using System.Threading;

namespace SocketTest
{
    class Program
    {
        public static int port = 2020;
        public static string ip = "127.0.0.1";
        static void Main(string[] args)
        {
            IPAddress IPAddr = IPAddress.Parse(ip);
            IPEndPoint endpoint = new IPEndPoint(IPAddr, port);
            PlayClient client = new PlayClient();
            client.Connect(endpoint);
            client.init();
            string str = new string('a', 8000);
            while (true) {            
                Thread.Sleep(300);
                client.Send(str);
                Console.WriteLine(client.Receive());
            }
            while (true)
            {
                str = Console.ReadLine();
                Console.WriteLine(str);
            }
        }
    }
}
