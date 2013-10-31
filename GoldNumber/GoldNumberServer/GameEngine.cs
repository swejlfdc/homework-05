using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System.Threading;

namespace GoldNumberServer
{
    public partial class PlayServer : AppServer<ComSession>
    {
        public Dictionary<string, int> Grade;
        public Dictionary<string, int> RoundGrade;
        private GameEngine engine = new GameEngine();
        Thread GameThread;
        public bool GameStarted = false;
        public bool CommitPermision = false;
        public int Round = 0;
        double CurrentGoldNumber;

        private void GameRun() {
            Round = 0;
            engine.Register(UserManagement.CurrentUserList.ToArray<string>());
            while (true)
            {
                if (!GameStarted) break;
                ++Round;
                CommitPermision = true;
                foreach (ComSession session in this.GetAllSessions())
                {
                    if (session.UserId != null)
                    {
                        session.Commited = false;
                        session.Send("BEGN ROUND " + Round);
                        session.Send("INFO 1s left");
                    }
                }
                Thread.Sleep(700);//wait for 700ms
                // Transfer data to engine
                CommitPermision = false;
                List<Tuple<string, double>> CmmtNumber = new List<Tuple<string, double>>();
                foreach (ComSession session in this.GetAllSessions()) {
                    if (session.UserId != null && session.Commited)
                    {
                        session.Send("INFO Commid end");
                        CmmtNumber.Add(new Tuple<string, double>(session.UserId, session.CommitNumber));
                    }
                }
                engine.Play(CmmtNumber);
                this.CurrentGoldNumber = engine.GoldNumber;
                foreach (ComSession session in this.GetAllSessions())
                {
                    if (session.UserId != null)
                    {
                        int Grd, CurGrd;
                        Grd = engine.Grade[session.UserId];
                        CurGrd = engine.RoundGrade[session.UserId];
                        session.Send("RSLT {0} {1} {2}", CurGrd, Grd, this.CurrentGoldNumber.ToString("0.000"));
                    }
                }
            }
        }
        
        public void GameStart()
        {
            if (GameStarted)
            {
                return;
            }
            else
            {
                GameStarted = true;
            }
            this.LoginPermission = false;   // 游戏开始后不允许再登陆
            Round = 0;
            Grade = new Dictionary<string,int>();
            RoundGrade = new Dictionary<string,int>();
            GameThread = new Thread(GameRun);
            GameThread.Start();
        }
        public void GameEnd()
        {
            GameStarted = false;
            LoginPermission = true;
        }
    }
    class GameEngine
    {
        public HashSet<string> Players;
        public List<Tuple<string, double>>CommitedNumber = new List<Tuple<string,double>>();
        public Dictionary<string, int> Grade;
        public Dictionary<string, int> RoundGrade;
        public List<Tuple<string, int>> result;
        public Double GoldNumber;
        public delegate void GradeSetter(string name, int Grade);
        class cmp : IComparer<Tuple<string, double>>
        {
            public int Compare(Tuple<string, double> a, Tuple<string, double> b)
            {
                if (a.Item2 < b.Item2) return 1;
                if (a.Item2 > b.Item2) return -1;
                return 0;
            }
        }

        public void Clear() {
            CommitedNumber.Clear();
            Players.Clear();
            Grade.Clear();
            RoundGrade.Clear();
        }
        public void Register(IList<string> Ply) {
            Players = new HashSet<string>(Ply);
            Grade = new Dictionary<string, int>();
            RoundGrade = new Dictionary<string, int>();
            foreach (string name in Ply)
            {
                Grade[name] = 0;
                RoundGrade[name] = 0;
            }
        }
        public void Play(List<Tuple<string, double>> CmmtNumber) {
            double tmp = 0;
            CommitedNumber = CmmtNumber;
            foreach (Tuple<string, double> pr in CommitedNumber)
            {
                if(Players.Contains(pr.Item1))
                    tmp += pr.Item2;
            }
            tmp /= Players.Count;
            GoldNumber = tmp * 0.6180339887;
            // sort D-value
            List<Tuple<string, double>> result = new List<Tuple<string, double>>(CmmtNumber.Count);
            foreach (Tuple<string, double> pr in CmmtNumber)
            {
                result.Add(new Tuple<string, double>(pr.Item1, Math.Abs(pr.Item2 - GoldNumber)));
            }
            result.Sort(new cmp());
            //need optimized
            foreach (string key in Players)
            {
                RoundGrade[key] = -10;
            }

            int point = 10;
            foreach (Tuple<string, double> pr in result)
            {
                RoundGrade[pr.Item1] = point;
                if(point > 0) --point;
            }
            if(result.Count > 1)
                RoundGrade[result[result.Count - 1].Item1] = -1;
            foreach (KeyValuePair<string, int> item in RoundGrade)
            {
                Grade[item.Key] += item.Value;
            }
        }

    }
}
