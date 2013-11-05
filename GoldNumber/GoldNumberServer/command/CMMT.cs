using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace GoldNumberServer
{
    public class CMMT : CommandBase<ComSession, StringRequestInfo>
    {
        public override void ExecuteCommand(ComSession session, StringRequestInfo requestInfo)
        {
            PlayServer Server = session.AppServer as PlayServer;
            if (Server == null)
            {
                session.Send("ERRO System Error");
                return;
            }
            if (Server.GameStarted == false)
            {
                session.Send("ERRO Game have not start");
                return;
            }
            if (session.Commited == true)
            {
                session.Send("ERRO Over Time");
                return;
            }
            else if (requestInfo.Parameters.Length < 1)
            {
                session.Send("ERRO No Number");
                return;
            }
            try
            {
                int ValidNumber = Math.Min(Server.CommitNumber, requestInfo.Parameters.Length);
                double tmp = Convert.ToDouble(requestInfo.Parameters[0]);
                session.CommitNumber = tmp;
                session.Commited = true;
            }
            catch
            {
                session.Send("ERRO Number too long or Wrong Format");
            }
        }
    }
}
