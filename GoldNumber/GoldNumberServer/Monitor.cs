using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace GoldNumberServer
{
    public static class Monitor
    {
        public static StreamWriter GradeLog = new StreamWriter("LastPlay.txt");
        public static StreamWriter CommitLog = new StreamWriter("CommitLog.txt",true, Encoding.ASCII, 4096);        
        public static StreamWriter ServerLog = new StreamWriter("ServerLog.txt", true, Encoding.ASCII, 4096);
        static string lock1 = "lock1", lock2 = "lock2", lock3 = "lock3";
        
        public static string GetNow() {
            return DateTime.Now.ToLongTimeString();
        }
        public static void Print(string msg)
        {
            Console.WriteLine("{0} {1}", GetNow(), msg);
            LogState(msg);
        }
        class LogData
        {           
            public StreamWriter writer;
            public String message;
            public String Lock;
            public LogData(StreamWriter wr, String msg, String lok)
            {
                writer = wr;
                message = msg;
                Lock = lok;
            }
        }
        static void Log(Object data)
        {
            LogData log = data as LogData;
            StreamWriter wr = log.writer;
            String Msg = log.message;
            String Lock = log.Lock;
            lock (Lock)
            {
                wr.WriteLine(Msg);
            }
        }
        public static void LogGrade(string str) {
            new Thread(new ParameterizedThreadStart(Log)).Start(
            new LogData(GradeLog, GetNow() + " " + str, lock1));
            //GradeLog.WriteLine("{0} {1}", GetNow(), str);
        }
        public static void LogCommit(string str) {
            new Thread(new ParameterizedThreadStart(Log)).Start(
            new LogData(CommitLog, GetNow() + " " + str, lock2));
            //CommitLog.WriteLine("{0} {1}", GetNow(),str);
        }
        public static void LogState(string str) {
            new Thread(new ParameterizedThreadStart(Log)).Start(
            new LogData(ServerLog, GetNow() + " " + str, lock3));
            //ServerLog.WriteLine("{0} {1}", GetNow(), str);
        }
        public static void flush()
        {
            lock (lock1)
            {
                lock (lock2)
                {
                    lock (lock3)
                    {
                        GradeLog.Flush();
                        CommitLog.Flush();
                        ServerLog.Flush();
                    }
                }
            }
        }
    }
}
