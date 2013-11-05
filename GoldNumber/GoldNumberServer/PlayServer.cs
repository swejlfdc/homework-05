﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using WebSocketTest;

namespace GoldNumberServer
{
    public partial class PlayServer : AppServer<ComSession>
    {
        protected UserModule UserManagement;
        public bool LoginPermission = true;
        public WSServer DisplayServer;
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
           
            UserManagement = new UserModule("UserList.txt");
            return base.Setup(rootConfig, config);
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
        public void Logout(string name)
        {
            UserManagement.Logout(name);
#if TRACE
            Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + name + "log out");
#endif
        }
    }
}
