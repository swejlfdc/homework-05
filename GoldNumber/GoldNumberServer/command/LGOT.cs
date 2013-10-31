using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace GoldNumberServer
{
    public class LGOT : CommandBase<ComSession, StringRequestInfo>
    {
        public override void ExecuteCommand(ComSession session, StringRequestInfo requestInfo)
        {
            PlayServer Server = session.AppServer as PlayServer;
            if (Server == null) session.Send("ERRO System Error");
            if (session.UserId == null)
            {
                session.Send("ERRO didn't login");
            }
            else
            {
                Server.Logout(session.UserId);
                session.UserId = null;
            }
        }
    }
}
