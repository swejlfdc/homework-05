using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperWebSocket;
using SuperSocket.SocketBase.Config;
using System.Configuration;

namespace WebSocketTest
{
    public class WSServer : WebSocketServer<JsonWebSocketSession>
    {
        public WSServer()
        {
        }
        public void SendResult(Object obj)
        {
            foreach(JsonWebSocketSession session in this.GetAllSessions()) {
                session.SendJsonMessage("", obj);
            }
        }
    }
}
