using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketEngine;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography;
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
                    PlayServer Server = (bootstrap.GetServerByName("PlayServer") as PlayServer);
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
                if (str == "q") break;
                if (str == "end")
                {
                    PlayServer Server = (bootstrap.GetServerByName("PlayServer") as PlayServer);
                    Server.GameEnd();
                }
            }
            Console.WriteLine();

            //Stop the appServer
            bootstrap.Stop();

            Console.WriteLine("The server was stopped!");
        }
    }
}
