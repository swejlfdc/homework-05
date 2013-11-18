﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace GoldNumberServer
{
    [GameCommandFilter]
    public class LOGN : CommandBase<ComSession, StringRequestInfo>
    {
        public override void ExecuteCommand(ComSession session, StringRequestInfo requestInfo)
        {
            PlayServer Server = session.AppServer as PlayServer;
            if (Server == null) {
                session.Send("ERRO System Error");
                return;
            }
            if (requestInfo.Parameters.Length < 2) {
                session.Send("ERRO Parameter imcomplete");
                return;
            }
            if (!Server.LoginPermission)
            {
                session.Send("ERRO not permit login");
                return;
            }
            string name, password;
            name = requestInfo[0];
            password = requestInfo[1];
            if (Server.Login(name, password))
            {
                session.UserId = name;
#if TRACE
                Monitor.Print(name + " login System from ip " + session.RemoteEndPoint.Address.ToString());
                Monitor.LogCommit(requestInfo.Key + " " + requestInfo.Body);
#endif
                session.Send("INFO login successfully");
            }
            else
            {
                Monitor.Print("Entiy from ip " + session.RemoteEndPoint.Address + " try to login with name " + name + " Failed");
                session.Send("ERRO login fail");
            }
        }
    }
}
