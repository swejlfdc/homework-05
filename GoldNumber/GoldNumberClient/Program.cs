using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Wodsoft.Net.Sockets;

namespace GoldNumberClient
{
    public class MyClient : TcpClient  {
        public MyClient() {
        }
        private StreamReader reader;
        private StreamWriter writer;
            
        public void init() {
            if(this.Active) {
                NetworkStream ns = GetStream();
                reader = new StreamReader(ns, Encoding.ASCII, true);           
                writer = new StreamWriter(ns, Encoding.ASCII, 1024 * 8);
                Console.WriteLine(reader.ReadLine());
                Send("nimas");
                Receive();
            }
        }
        public void Send(string str)
        {
            str += "\r\n";
            //byte[] data = System.Text.Encoding.ASCII.GetBytes(str);
            //this.Client.Send(data);
            writer.Write(str);
            writer.Flush();
        }
        public string Receive()
        {
            return reader.ReadLine();
        }
        public void Login(string name, string password)
        {
            Send("LOGN " + name + " " + password);
            Console.WriteLine(this.Receive());
        }
        public void Register(string name, string password) {
            Send("REGT " + name + " " + password);
            Console.WriteLine(this.Receive());
        }
        public void Commit(params double[] val)
        {
            string str = "CMMT";
            foreach (double item in val)
            {
                str += " " + item.ToString("0.0000");
            }
            Send(str);
            Console.WriteLine(this.Receive());
        }

    }
    class Program
    {
        public static int port = 2020;
        public static string ip = "127.0.0.1";
        static void Main(string[] args)
        {
            MyClient client = new MyClient();
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
            Random ran = new Random((int)(tick & 0xffffffffL) | (int) (tick >> 32));
            
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
                        client.Commit(cmt);
                        Console.WriteLine("Commit number " + cmt.ToString("0.000") + "  to server");
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
        //public static void Handshake(Object sender, TCPClientEventArgs e)
        //{
        //    MyClient client = sender as MyClient;
        //    client.Send("Hello World");
        //    Console.WriteLine(DateTime.Now.ToShortTimeString() + "Connect Server successfully");
        //}
        //public static void Play(Object sender, TCPClientEventArgs e)
        //{
        //    MyClient client = sender as MyClient;
        //    string str = System.Text.Encoding.ASCII.GetString(e.Data);
        //    Console.WriteLine(str);
        //}
    }
}
