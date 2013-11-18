using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using GoldNumberClient;

namespace AttackClient
{
    static class Program
    {
        public static int port = 2020;
        public static string ip = "127.0.0.1";
        static void Main(string[] args)
        {
            PlayClient client = new PlayClient();
            IPAddress IPAddr = IPAddress.Parse(ip);
            IPEndPoint endpoint = new IPEndPoint(IPAddr, port);
            //client.ConnectCompleted += Handshake;
            //client.ReceiveCompleted += Play;  
            try
            {
                client.Connect(endpoint);
            }
            catch
            {
                Console.WriteLine("Server did not run!");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                return;
            }
            client.init();

            string name, password;
            if (Environment.GetCommandLineArgs().Length >= 3)
            {
                name = Environment.GetCommandLineArgs()[1];
                password = Environment.GetCommandLineArgs()[2];
            }
            else
            {
                name = Console.ReadLine().Trim();
                password = Console.ReadLine().Trim();   // keyword not allow space char 
            }
            client.Login(name, password);

            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            while (true)
            {
                try
                {
                    string str = client.Receive();
                    Console.WriteLine(str);
                    string[] param = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (param[0] == "BEGN")
                    {
                        double cmt = ran.NextDouble() * 100;
                        double cmt2 = ran.NextDouble() * 100;
                        client.Commit(cmt, cmt2);
                        Console.WriteLine("Commit number " + cmt.ToString("0.000") + " " + cmt2.ToString("0.000") + "  to server");
                        string rslt = client.Receive();
                        Console.WriteLine(rslt);
                    }
                }
                catch
                {
                    break;
                }
            }
            client.Close();
        }
    }
}
