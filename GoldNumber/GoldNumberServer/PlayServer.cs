using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System.Net;
using WebSocketTest;

namespace GoldNumberServer
{
    public class BlackBoard : IConnectionFilter
    {
        private HashSet<string> IPSec = new HashSet<string>();
        public string Name { get; private set; }
       public bool Initialize(string name, IAppServer appServer)
       {
           this.Name = name;
           return true;
       }

       public void add(IPEndPoint Address)
       {
           IPSec.Add(Address.Address.ToString());
       }

       public void Clear()
       {
           IPSec.Clear();
       }
       public bool AllowConnect(IPEndPoint remoteAddress)
       {
           if(IPSec.Contains(remoteAddress.Address.ToString()))
               return false;
           return true;
       }
    }
    public partial class PlayServer : AppServer<ComSession>
    {
        protected UserModule UserManagement;
        public bool LoginPermission = true;
        public bool ConnectPermission = true;
        public WSServer DisplayServer;
        public BlackBoard IPFilter;
        public DistributeServer Distribute;
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            bool result = base.Setup(rootConfig, config);
            foreach (IConnectionFilter IF in this.ConnectionFilters)
            {
                IPFilter = IF as BlackBoard;
                break;
            }
            UserManagement = new UserModule("UserList.txt");
            return result;
        }

        protected override void OnNewSessionConnected(ComSession session)
        {
            if (ConnectPermission == false)
                session.Close();
            else
                base.OnNewSessionConnected(session);
        }

        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override void OnStopped()
        {
            this.GameEnd();
            base.OnStopped();
            UserManagement.Close(); // Write Back the user data
        }
        public bool Register(string name, string password)
        {
            return UserManagement.AddUser(name, password);
        }
        public bool Login(string name, string password)
        {
            return UserManagement.Login(name, password);
        }
        public void PrintPlayers()
        {
            foreach (var session in this.GetAllSessions())
            {
                if (session.UserId != null)
                {
                    Console.WriteLine("{0} ip:{1} port:{2}", session.UserId, session.RemoteEndPoint.Address.ToString(), session.LocalEndPoint.Port);
                }
            }
        }
        public void Logout(string name)
        {
            UserManagement.Logout(name);
#if TRACE
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + name + " log out");
            Monitor.LogCommit(name + "log out");
#endif
        }
    }
}
