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
            this.NewDataReceived +=WSServer_NewDataReceived;
        }
        void WSServer_NewDataReceived(JsonWebSocketSession session, byte[] data)
        {
            session.Close();
        }
        public void SendResult(string obj)
        {
            foreach(JsonWebSocketSession session in this.GetAllSessions()) {
                try
                {
                    session.Send(obj);
                } catch
                {
                    session.Close();
                }
            }
        }
    }
}
