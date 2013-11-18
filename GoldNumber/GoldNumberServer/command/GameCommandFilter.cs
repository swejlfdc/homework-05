using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;

namespace GoldNumberServer
{
    public class GameCommandFilter : CommandFilterAttribute
    {
        
        public override void OnCommandExecuting(CommandExecutingContext commandContext)
        {
            DateTime now = DateTime.Now;
            ComSession session = commandContext.Session as ComSession;
            if (now.Subtract(session.lastTime).TotalSeconds > 1)
            {
                session.lastTime = now;
                session.LastRecieveDataLength = 0;
            }
            else
            {
                session.LastRecieveDataLength++;
                if (session.LastRecieveDataLength > 5)
                {
                    Monitor.Print(session.RemoteEndPoint.Address.ToString() + 
                        " send too many data to server in a second");
                    (session.AppServer as PlayServer).IPFilter.add(session.RemoteEndPoint);
                    session.Close();
                }
            }
        }

        public override void OnCommandExecuted(CommandExecutingContext commandContext)
        {
        }
    }
}
