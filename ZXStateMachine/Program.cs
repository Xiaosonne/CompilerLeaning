using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZXStateMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var tn = Expression.RegTrans("a(c|d)*bd*#"); 

            DFABuilder buid = new DFABuilder(tn);
            buid.BuildFollowpos();
            buid.BuildStateMachine(); 

            var state = buid.StateMachine; 

            state.PrintState();

            if (state.AcceptInputs("aaaaaaabb"))
            {
                Console.WriteLine("aaaaaaabb");
            }
            if (state.AcceptInputs("aaaaaaab"))
            {
                Console.WriteLine("aaaaaaab");
            }
            if (state.AcceptInputs("aaaaaacb"))
            {
                Console.WriteLine("aaaaaacb");
            }
            if (state.AcceptInputs("aaaaaacbd"))
            {
                Console.WriteLine("aaaaaacbd");
            }
            if (state.AcceptInputs("abb"))
            {
                Console.WriteLine("abb");
            }
            if (state.AcceptInputs("aaaaaaabbbb"))
            {
                Console.WriteLine("aaaaaaab");
            }
            Console.Read();
        }

        private static void a3()
        {
            ZXStateMachine2.StateMachineGen2 s2 = new ZXStateMachine2.StateMachineGen2();
            string e = ZXStateMachine2.Accept.Epsilon.A;


            string a = "a";
            string b = "b";
            //s2.AddConvertState(0, e, 1);
            //s2.AddConvertState(0, e, 7);
            //s2.AddConvertState(1, e, 2);
            //s2.AddConvertState(1, e, 4);
            //s2.AddConvertState(2, a, 3);
            //s2.AddConvertState(3, e, 6);
            //s2.AddConvertState(4, b, 5);
            //s2.AddConvertState(5, e, 6);
            //s2.AddConvertState(6, e, 1);
            //s2.AddConvertState(6, e, 7);
            //s2.AddConvertState(7, a, 8);
            //s2.AddConvertState(8, b, 9);
            //s2.AddConvertState(9, b, 10);
            //s2.AddStartState(0);
            //s2.AddEndState(10);
            //if (s2.AcceptInputs("aaaaaaaaaaaaaaaaaaaaaaaaaaabb")) {
            //    Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaaaaaabb");
            //}
            s2.AddConvertState(0, a, 1);
            s2.AddConvertState(1, a, 2);
            s2.AddConvertState(2, b, 3);
            s2.AddStartState(0);
            s2.AddEndState(3);
            if (s2.AcceptInputs("aab"))
            {
                Console.WriteLine("aab");
            }
        }

        private static void a2(ZXStateMachine2.StateMachineGen2 s2)
        {
            StringBuilder sb = new StringBuilder();
            using (FileStream fs = new FileStream("data.txt", FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    char[] buffer = new char[256];
                    int read = sr.Read(buffer, 0, 256);
                    while (read > 0)
                    {
                        sb.Append(buffer, 0, read);
                        read = sr.Read(buffer, 0, 256);
                    }
                }
            }
            HashSet<char> allchars = new HashSet<char>(sb.ToString().ToArray());
            var charset = allchars.ToList();
            int i = 1;
            var chs = sb.ToString().ToCharArray();
            for (; i < chs.Length - 1; i++)
            {
                var c1 = chs[i - 1];
                var c2 = chs[i];
                s2.AddConvertState(charset.IndexOf(c1), c1, charset.IndexOf(c2));
            }
            string s = Console.ReadLine();
            while (!string.IsNullOrEmpty(s))
            {
                s2.GetNextState(s);
                Console.WriteLine("");
                s = Console.ReadLine();
            }
        }

        private static void a1()
        {
            StateMachine sm = new StateMachine("1");
            sm.AddEndState("7");
            sm.AddConvert("1", "我", "2");
            sm.AddConvert("2", "爱", "3");
            sm.AddConvert("3", "你", "4");
            sm.AddConvert("4", "贾", "5");
            sm.AddConvert("5", "玉", "6");
            sm.AddConvert("6", "兰", "7");

            sm.AddConvert("3", "贾", "5");
            sm.AddConvert("3", "玉", "6");

            if (sm.AcceptLetters(new String[] { "我", "爱", "你", "贾", "玉", "兰" }))
            {
                Console.WriteLine("我爱你贾玉兰");
            }
            if (sm.AcceptLetters(new String[] { "我", "不", "爱", "贾", "玉", "兰" }))
            {
                Console.WriteLine("我不爱贾玉兰");
            }
            if (sm.AcceptLetters(new String[] { "我", "爱", "贾", "玉", "兰" }))
            {
                Console.WriteLine("我爱贾玉兰");
            }
            if (sm.AcceptLetters(new String[] { "我", "爱", "玉", "兰" }))
            {
                Console.WriteLine("我爱玉兰");
            }
        }
    }
}
