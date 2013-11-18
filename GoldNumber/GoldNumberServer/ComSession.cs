using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace GoldNumberServer
{
    public partial class ComSession : AppSession<ComSession>
    {
        public string UserId;
        public bool Joined;
        public bool Commited;
        public double[] CommitNumber;
        public DateTime lastTime = DateTime.Now;
        public UInt32 LastRecieveDataLength = 0;

        public void ResetPlayInfo()
        {
            PlayServer server = this.AppServer as PlayServer;
            Commited = false;
            //CommitNumber = new double[server.CommitAmount];
        }
        protected override void OnSessionStarted()
        {
            this.Send("Welcome to GoldNumber game~");
#if TRACE
            Monitor.Print(this.RemoteEndPoint.Address.ToString() + " try Connect");
#endif
        }

        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            this.Send("Unknow request");
        }

        protected override void HandleException(Exception e)
        {
            this.Send("Application error: {0}", e.Message);
        }


        protected override void OnSessionClosed(CloseReason reason)
        {
            if (UserId != null) (AppServer as PlayServer).Logout(UserId);
            base.OnSessionClosed(reason);
#if TRACE
            Monitor.Print(this.RemoteEndPoint.Address.ToString() + " Disconnected");
#endif
        }
    }
}
