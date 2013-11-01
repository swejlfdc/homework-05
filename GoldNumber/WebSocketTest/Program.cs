using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperWebSocket;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using SuperSocket.Common;
using System.Configuration;
using System.Runtime.Serialization.Json;
using System.IO;

namespace WebSocketTest
{
    static class JSON
    {
        public static string stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
    static class Program
    {
        
        public static Dictionary<string, int> Grade =  new Dictionary<string,int>();
        public static List<Tuple<string, int>> gr = new List<Tuple<string, int>>();
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start the server!");

            Console.ReadKey();
            Console.WriteLine();
            var bootstrap = BootstrapFactory.CreateBootstrap();
            Grade["nimas"] = 123;
            Grade["nimei"] = 321;
            gr.Add(new Tuple<string, int>("nimas", 123));
            gr.Add(new Tuple<string, int>("nimas", 123));
            if (!bootstrap.Initialize())
            {
                Console.WriteLine("Failed to initialize!");
                Console.ReadKey();
                return;
            }

            var result = bootstrap.Start();

            Console.WriteLine("Start result: {0}!", result);

            var socketServer = bootstrap.AppServers.FirstOrDefault(s => s.Name.Equals("WSServer")) as WSServer; 


            //socketServer.NewMessageReceived += new SessionHandler<WebSocketSession, StringRequestInfo>(socketServer_NewMessageReceived);//添加新消息到达事件  
            socketServer.NewSessionConnected += socketServer_NewSessionConnected;//添加新客户端接入事件  
            //socketServer.SessionClosed += socketServer_SessionClosed;//添加连接关闭事件  

            Console.WriteLine();

            Console.WriteLine("The server started successfully, press key 'q' to stop it!");

            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine();
                continue;
            }

            Console.WriteLine();
            Console.WriteLine("The server was stopped!");
        }

        static void socketServer_NewSessionConnected(JsonWebSocketSession session)
        {
            session.Send("Welcome to SuperSocket Telnet Server");
            //SendResponse("{\"employees\": [{ \"firstName\":\"Bill\" , \"lastName\":\"Gates\" },{ \"firstName\":\"George\" , \"lastName\":\"Bush\" },{ \"firstName\":\"Thomas\" , \"lastName\":\"Carter\" }]}");
            //JSON.stringify(Grade);
            //session.SendJsonMessage("test", Grade);
            session.SendJsonMessage("", Grade);

        }
    }
}
