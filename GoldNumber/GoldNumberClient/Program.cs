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
    class GameClientException : Exception
    {
        public GameClientException(string message) : base(message) {
        }
    }
    public class GoldNumberClient: TcpClient  {
        public GoldNumberClient()
        {
        }
        private StreamReader reader;
        private StreamWriter writer;
        private int MaxbufferSize = 0x40;
        private int MaxKeywordSize = 18;
        private int MaxUsernameSize = 18;
        private string UserName = "";
        private string ver = "1.0";
        // Summary:
        //     Gets or set a value that indicates whether auto receive the request after every command.
        //     The received message will be printed on screen
        public bool AutoHandShake = true;
        public void init() {
            if(this.Active) {
                NetworkStream ns = GetStream();
                reader = new StreamReader(ns, Encoding.ASCII, true);           
                writer = new StreamWriter(ns, Encoding.ASCII, 1024 * 8);
                Receive();
                Send("VRSN " + ver);
                Receive();
            } else {
                throw new GameClientException("Connection did not establish");
            }
        }
        public void Send(string str)
        {
            str += "\r\n";
            //byte[] data = System.Text.Encoding.ASCII.GetBytes(str);
            //this.Client.Send(data);
            if (str.Length > MaxbufferSize)
                throw new GameClientException("The Send Buffer Size is too big");
            writer.Write(str);
            writer.Flush();
        }
        // Summary:
        //     Return a new string message. If Socket has no transfered data, the method will block Thread.
        public string Receive()
        {
            return reader.ReadLine();
        }
        // Summary:
        //     Login the server with given username and password
        public void Login(string name, string password)
        {
            if (name.Length > MaxUsernameSize)
                throw new GameClientException("The size of username is too long");
            if (password.Length > MaxKeywordSize)
                throw new GameClientException("The size of keyword is too long");
            Send("LOGN " + name + " " + password);
            if (AutoHandShake)
                Console.WriteLine(Receive());
        }
        // <Summary>
        //     Register user to server with given username and password
        // </Summary>
        public void Register(string name, string password) {
            if (name.Length > MaxUsernameSize)
                throw new GameClientException("The size of username is too long");
            if (password.Length > MaxKeywordSize)
                throw new GameClientException("The size of keyword is too long");
            Send("REGT " + name + " " + password);
            if (AutoHandShake)
                Console.WriteLine(Receive());
        }
        // Summary:
        //     Commit the an array of real number to game server. Every one cannot commit more than 5 numbers.
        //     But regarding the server config, only finite number will be accepted.
        public void Commit(params double[] val)
        {
            if(val.Length > 5) 
                throw new GameClientException("Commit too many number to server");
            string str = "CMMT";
            foreach (double item in val)
            {
                str += " " + item.ToString("0.00000");
            }
            Send(str);
            if (AutoHandShake)
                Console.WriteLine(Receive());
        }
        
        // Summary:
        //     Log out current user. This method currently will not throw exception.
        public void Logout()
        {
            if (UserName != "")
                Send("LGOT");
            if(AutoHandShake)
                Console.WriteLine(reader.ReadLine());
        }
        public void Exit()
        {
            Send("EXIT");
        }
    }
    static class Program
    {
        public static int port = 2020;
        public static string ip = "127.0.0.1";
        static void Main(string[] args)
        {
            GoldNumberClient client = new GoldNumberClient();
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
                        double cmt2 = ran.NextDouble() * 100;
                        client.Commit(cmt, cmt2);
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
    }
}
