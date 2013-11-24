using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketEngine;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography;
#if TRACE
using System.Diagnostics;
#endif
namespace GoldNumberServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start the server!");
            
            Console.ReadKey();
            Console.WriteLine();
            AppServer a = new AppServer();
            
            var bootstrap = BootstrapFactory.CreateBootstrap();

            if (!bootstrap.Initialize())
            {
                Console.WriteLine("Failed to initialize!");
                Console.ReadKey();
                return;
            }

            var result = bootstrap.Start();
            var ws = bootstrap.GetServerByName("WSServer");
            var ds = bootstrap.GetServerByName("DistributeServer");
            PlayServer Server = (bootstrap.GetServerByName("PlayServer") as PlayServer);
            // connect the server
            Server.DisplayServer = ws as WebSocketTest.WSServer;
            Server.Distribute = ds as DistributeServer;

            Console.WriteLine("Start result: {0}!", result);

            if (result == StartResult.Failed)
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Press key 'q' to stop it!");
            /*
            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }
            */
            while (true)
            {
                string str = Console.ReadLine();
                str = str.Trim();
                if (str == "start")
                {   
                    //PlayServer Server = (bootstrap.GetServerByName("PlayServer") as PlayServer);
                    if (Server.GameStarted)
                    {
                        Console.WriteLine("Game has started");
                    }
                    else
                    {
                        Server.GameStart();
                        Console.WriteLine("Game Start!");
                    }
                }

                switch (str)
                {
                    case "clearIpSec":
                        Server.IPFilter.Clear(); break;
                    case "pause":
                        Server.GamePause(); break;
                    case  "resume":
                        Server.GameResume(); break;
                    case "end" :
                        Server.GameEnd(); break;
                    case "show" :
                        Console.WriteLine("Current Session Number: {0}", Server.SessionCount);
                        Server.PrintPlayers();
                        Console.WriteLine("Current WebSession NUmber: {0}", ws.SessionCount);
                        break;
                    case "clear":
                        foreach(var session in Server.GetAllSessions()) {
                            session.Close();
                        }
                        break;
                }
                if (str == "q") break;
            }
            Monitor.flush();
            Console.WriteLine();

            //Stop the appServer
            bootstrap.Stop();

            Console.WriteLine("The server was stopped!");
        }
    }
}
