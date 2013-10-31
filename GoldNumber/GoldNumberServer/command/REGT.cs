using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace GoldNumberServer
{
    public class REGT : CommandBase<ComSession, StringRequestInfo>
    {
        public override void ExecuteCommand(ComSession session, StringRequestInfo requestInfo)
        {
            PlayServer Server = session.AppServer as PlayServer;
            if (Server == null) session.Send("ERRO System Error");
            if (requestInfo.Parameters.Length < 2)
                session.Send("ERRO Parameter imcomplete");
            string name, password;
            name = requestInfo[0];
            password = requestInfo[1];
            if (name.Length > 32)
            {
                session.Send("ERRO Too long name");
                return;
            }
            if (!Server.Register(name, password))
                session.Send("ERRO Register fail");
            else
                session.Send("INFO Register successfully");
        }
    }
}
