using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;

namespace GoldNumberServer
{
    [GameCommandFilter]
    public class CMMT : CommandBase<ComSession, StringRequestInfo>
    {
        public override void ExecuteCommand(ComSession session, StringRequestInfo requestInfo)
        {
            PlayServer Server = session.AppServer as PlayServer;
            Monitor.LogCommit("user " + session.UserId + " " +requestInfo.Key + " " + requestInfo.Body);
            if (Server == null)
            {
                session.Send("ERRO System Error");
                return;
            }
            if (session.UserId == null)
            {
                session.Send("ERRO User did not login");
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
                int ValidNumber = Math.Min(Server.CommitAmount, requestInfo.Parameters.Length);
                session.CommitNumber = new double[ValidNumber];
                for(int i = 0; i < ValidNumber; ++i)
                {
                    double tmp = Convert.ToDouble(requestInfo.Parameters[i]);
                    if (tmp > 100 || tmp < 1)
                    {
                        session.CommitNumber = null;
                        session.Send("ERRO Number is beyond the limited range");
                        return;
                    }
                    session.CommitNumber[i] = tmp;
                    
                }
                session.Commited = true;
                session.Send("INFO Commit successfully");                
            }
            catch
            {
                session.Send("ERRO Number too long or Wrong Format");
            }
        }
    }
}
