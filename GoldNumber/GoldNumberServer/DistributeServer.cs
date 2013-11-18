using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.Common;
using SuperSocket.SocketEngine;
using System.Threading;
using SuperSocket.SocketBase.Protocol;

namespace GoldNumberServer
{
    public class DistributeSession : AppSession<DistributeSession>
    {

        void CloseSession(Object session)
        {
            Thread.Sleep(100);
            (session as DistributeSession).Close();
        }
        protected override void OnSessionStarted()
        {
            DistributeServer Server = this.AppServer as DistributeServer;
            this.Send("" + Server.ports[Server.pos]);
            Server.pos = (Server.pos + 1) % Server.ports.Length;
            new Thread(new ParameterizedThreadStart(CloseSession)).Start(this);
        }

        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            this.Close();
        }

        protected override void HandleException(Exception e)
        {
            this.Close();                
        }


        protected override void OnSessionClosed(CloseReason reason)
        {
        }
    }
    public class DistributeServer : AppServer<DistributeSession>
    {
        public int[] ports = new int[] { 7020, 7021, 7022, 7023, 7024, 7025, 7026, 7027, 7028, 7029, 7031, 7032, 7033, 7034, 7035 };
        public int pos = 0;
        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
            
        }
        protected override void OnNewSessionConnected(DistributeSession session)
        {
            base.OnNewSessionConnected(session);
        }

        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }
    }
}
