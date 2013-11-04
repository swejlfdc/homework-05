using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System.Threading;

namespace GoldNumberServer
{
    class cmp1 : IComparer<Tuple<string, double>>
    {
        double num;
        public cmp1(double GoldNumber) {
            num = GoldNumber;
        }
        public int Compare(Tuple<string, double> a, Tuple<string, double> b)
        {
            double x, y;
            y = Math.Abs(a.Item2 - num);
            x = Math.Abs(b.Item2 - num);
            if (x > y) return 1;
            //if (y < x) return -1;
            return 0;
        }
    }
    class cmp2 : IComparer<Tuple<string, int>>
    {
        public int Compare(Tuple<string, int> a, Tuple<string, int> b)
        {
            if (a.Item2 < b.Item2) return 1;
            if (a.Item2 > b.Item2) return -1;
            return 0;
        }
    }
    class DisplayData {
        public Double GoldNumber;
        public int Round;
        public List<Tuple<string, int>> SortedGrade = new List<Tuple<string,int>>();
        public List<Tuple<string, int>> SortedRoundGrade = new List<Tuple<string,int>>();
        public List<Tuple<string, double>> result = new List<Tuple<string, double>>();
    }
    public partial class PlayServer : AppServer<ComSession>
    {
        public Dictionary<string, int> Grade;
        public Dictionary<string, int> RoundGrade;
        private GameEngine engine = new GameEngine();
        Thread GameThread;
        public bool GameStarted = false;
        public bool CommitPermision = false;
        public int Round = 0;
        public int CommitNumber
        {
            get;
            internal set;
        }
        double CurrentGoldNumber;

        private void GameRun() {
            Round = 0;
            engine.Register(UserManagement.CurrentUserList.ToArray<string>());
            CommitNumber = 1;
            while (true)
            {
                if (!GameStarted) break;
                ++Round;
#if TRACE
                Console.WriteLine(DateTime.Now.ToLongTimeString() + " Round " + Round + " Start");
#endif
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
                Thread.Sleep(200);//wait for 700ms
                // Transfer data to engine
                CommitPermision = false;
                List<Tuple<string, double>> CmmtNumber = new List<Tuple<string, double>>();
                foreach (ComSession session in this.GetAllSessions()) {
                    if (session.UserId != null && session.Commited)
                    {
                        session.Send("INFO Commit end");
                        CmmtNumber.Add(new Tuple<string, double>(session.UserId, session.CommitNumber));
                    }
                }
                engine.Play(CmmtNumber);
                this.CurrentGoldNumber = engine.GoldNumber;
                DistributeGrade();
                DisplayGrade();
            }
        }
        private void DisplayGrade()
        {
            if (DisplayServer == null) return;
            DisplayData data = new DisplayData();
            data.Round = this.Round;
            data.GoldNumber = this.CurrentGoldNumber;
            foreach(KeyValuePair<string ,int> pr in engine.Grade) {
                data.SortedGrade.Add(new Tuple<string,int>(pr.Key, pr.Value));
            }
            foreach(KeyValuePair<string ,int> pr in engine.RoundGrade) {
                data.SortedRoundGrade.Add(new Tuple<string, int>(pr.Key, pr.Value));
            }
            foreach (Tuple<string, double> tp in engine.result)
            {
                data.result.Add(new Tuple<string, double>(tp.Item1, tp.Item2));
            }            
            data.SortedRoundGrade.Sort(new cmp2());
            data.SortedGrade.Sort(new cmp2());
            DisplayServer.SendResult(data);
        }
        private void DistributeGrade() {
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
        public List<Tuple<string, double>>CommitedNumber;
        public Dictionary<string, int> Grade;
        public Dictionary<string, int> RoundGrade;
        public List<Tuple<string, double>> result;
        public Double GoldNumber;
        public delegate void GradeSetter(string name, int Grade);

        public void sort()
        {
            for (int i = 0; i != result.Capacity; ++i)
            {
                int k = i;
                double best = Math.Abs(result[k].Item2 - GoldNumber);
                for (int j = i; j != result.Capacity; ++j)
                {
                    if (best > Math.Abs(result[j].Item2 - GoldNumber))
                    {
                        best = Math.Abs(result[j].Item2 - GoldNumber);
                        k = j;
                    }
                }
                Tuple<string, double> tmp = result[i];
                result[i] = result[k];
                result[k] = tmp;
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
        public void Play(List<Tuple<string, double>> CmmtNumber)
        {
            double tmp = 0;
            CommitedNumber = CmmtNumber;

            foreach (Tuple<string, double> pr in CommitedNumber)
            {
                if (Players.Contains(pr.Item1))
                    tmp += pr.Item2;
            }
            tmp /= Players.Count;
            GoldNumber = tmp * 0.6180339887;
            // sort D-value
            /*
            List<Tuple<string, double>> result = new List<Tuple<string, double>>(CmmtNumber.Count);
            foreach (Tuple<string, double> pr in CmmtNumber)
            {
                result.Add(new Tuple<string, double>(pr.Item1, Math.Abs(pr.Item2 - GoldNumber)));
            }
             * // old code
            */
            result = new List<Tuple<string, double>>(CmmtNumber);
            sort();
            //result.Sort(new cmp1(GoldNumber));
            // Unique result  
            {
                HashSet<string> uni_set = new HashSet<string>();
                List<Tuple<string, double>> uni_list = new List<Tuple<string, double>>((int)CmmtNumber.Count);
                foreach (Tuple<string, double> item in result)
                {
                    if (uni_set.Contains(item.Item1) == false)
                    {
                        uni_list.Add(item);
                        uni_set.Add(item.Item1);
                    }
                }
                result = uni_list;
            }
            //need optimized
            foreach (string key in Players)
            {
                RoundGrade[key] = -10;
            }

            int point = 10;
            foreach (Tuple<string, double> pr in result)
            {
                RoundGrade[pr.Item1] = point;
                if (point > 0) --point;
            }
            if (result.Count > 1)
                RoundGrade[result[result.Count - 1].Item1] = -1;
            foreach (KeyValuePair<string, int> item in RoundGrade)
            {
                Grade[item.Key] += item.Value;
            }
        }

    }
}
