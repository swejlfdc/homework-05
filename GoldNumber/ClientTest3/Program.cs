using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ClientTest3
{
    class Program
    {
        static void Main()
        {
            ClientAPI.GNClient.SetIpAddr("127.0.0.1");//设置服务器IP地址，如果是本地就设置为127.0.0.1，否则设置为服务器真实地址，仅供测试使用
            ClientAPI.GNClient client = new ClientAPI.GNClient();
            //client.SetUserInfo("11061099", "601657");//可以使用a1到a80之间的所有用户名登入，密码均为a
            client.SetUserInfo("shadowj", "j");//可以使用a1到a80之间的所有用户名登入，密码均为a
            if (client.Login()) Console.WriteLine("Welcome to the game");
            else Console.WriteLine("Failed to log in");
            double[] goldNumber = new double[1000];
            int times = 0;
            double shNum1 = 10, shNum2 = 10;
            while (true)
            {
                string str = client.Receive();//获得服务器返回信息，如果是开始信息，那么提交数字
                string[] param = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (param[0] == "BEGN" && param[1] == "1")//是开始信息，并且游戏模式是提交一个数字
                {
                    client.Commit(61.8);//提交数字
                    Console.WriteLine("Commit number {0} to server", 61.8);
                    Console.WriteLine(client.Receive());//获得提交信息，成功或者失败
                }
                if (param[0] == "BEGN" && param[1] == "2")
                {
                    if (times <= 1)
                    {
                        if (shNum1 > shNum2)
                            client.Commit(shNum1, 99.9);
                        else
                        {
                            client.Commit(shNum1, (shNum1 / 0.618 < 99.999) ? shNum1 / 0.618 : 99.9);
                        }
                        Console.WriteLine("Commit number{0} and {1} to server", shNum1, "ni cai");
                    }
                    Console.WriteLine(client.Receive());
                }
                do
                    str = client.Receive();//获得以RSLT为开头的字串，表示本轮信息
                while (!str.Contains("RSLT"));
                string[] info = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine(str);
                shNum1 = shNum2;
                shNum2 = double.Parse(info[3]);//info的内容为：0 RSLT, 1，本轮得分。 2，当前总得分。 3，本局黄金数
                if (shNum2 <= 1) shNum2 = 1.0001;
                if (shNum2 >= 100) shNum2 = 99.9;
            }
        }
    }
}
