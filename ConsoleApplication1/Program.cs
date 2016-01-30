using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXLanguage
{
    class Program
    {
        static void Main(string[] args)
        {
            Language lan = new Language();
            //lan.DefineQueryTerm("E'").AddTerms("E");
            //lan.DefineQueryTerm("E").AddTerms("E+T");
            //lan.DefineQueryTerm("E").AddTerms("T");
            //lan.DefineQueryTerm("T").AddTerms("T*F");
            //lan.DefineQueryTerm("T").AddTerms("F");
            //lan.DefineQueryTerm("F").AddTerms("(E)");
            //lan.DefineQueryTerm("F").AddTerms("id");
            lan.AddQueryItem("E'->E");
            lan.AddQueryItem("E->E+T");
            lan.AddQueryItem("E->T");
            lan.AddQueryItem("T->T*F");
            lan.AddQueryItem("T->F");
            lan.AddQueryItem("F->(E)");
            lan.AddQueryItem("F->id");
            lan.EndAddTerms("E'"); 
            var all = lan.BuildItemSetFamily("E'->E");
            if (all.Any(p => p.Equals(all[0].Goto("T"))))
            {
                Console.WriteLine(all[0].Goto("T"));
            }
            foreach (var ss in all)
            {
                Console.WriteLine("->\t"+ss.Name+"\t\t"+ss.ToString());
            }

            var lis = lan.GenStateDFA();
            foreach (var s in lis) {
                Console.WriteLine(s);
            }
            lis = lan.GenSLRAction();
            foreach (var s in lis)
            {
                Console.WriteLine(s);
            }
            Console.Read();
        }

        private static void NewMethod2()
        {
            Language lan = new Language();
            lan.DefineQueryTerm("E'").AddTerms("E");
            lan.DefineQueryTerm("E").AddTerms("E+T");
            lan.DefineQueryTerm("E").AddTerms("T");
            lan.DefineQueryTerm("T").AddTerms("T*F");
            lan.DefineQueryTerm("T").AddTerms("F");
            lan.DefineQueryTerm("F").AddTerms("(E)");
            lan.DefineQueryTerm("F").AddTerms("id");
            lan.EndAddTerms("E'");
            var ca = lan.GetCondition("E'");
            var at = lan.GetAction("E'->E");
            var at2 = lan.GetAction("E->E+T");
            //ItemSet iss = new ItemSet();
            //iss.Add(new Item() { Action = ca.Actions[0] });
            //var k = iss.Closure();
            //foreach (var s in k)
            //{
            //    Console.WriteLine(s.Action.Condition + "->" + string.Join(" ", s.Action));
            //} 
            ItemSet iss = new ItemSet();
            iss.Add(new Item() { Action = at, Position = 1 });
            iss.Add(new Item() { Action = at2, Position = 1 });
            var it = iss.Goto("+");
            foreach (var ss in it)
            {
                Console.WriteLine(ss.Position + " " + ss.Action);
            }
        }

        private static void NewMethod(Language lan)
        {
            //lan.DefineQueryTerm("A").AddTerms("bC'dedGG");
            //lan.DefineQueryTerm("A").AddTerms("bbbb");
            lan.DefineQueryTerm("E").AddTerms("TE'");
            lan.DefineQueryTerm("E'").AddTerms("+TE'");
            lan.DefineQueryTerm("E'").AddTerms("ϵ");
            lan.DefineQueryTerm("T").AddTerms("FT'");
            lan.DefineQueryTerm("T'").AddTerms("*FT'");
            lan.DefineQueryTerm("T'").AddTerms("ϵ");
            lan.DefineQueryTerm("F").AddTerms("(E)");
            lan.DefineQueryTerm("F").AddTerms("id");
            lan.EndAddTerms("E");
            Console.WriteLine(string.Join("  ", lan.GetCondition("E").FirstSet));
            Console.WriteLine(string.Join("  ", lan.GetCondition("E'").FirstSet));
            Console.WriteLine(string.Join("  ", lan.GetCondition("T'").FirstSet));
            lan.GetFollow();
            lan.BuildSelectSet();
            if (lan.Accept("id+id*id+id+id"))
            {
                Console.WriteLine("id+id*id+id+id");
            }
            if (lan.Accept("id+id"))
            {
                Console.WriteLine("id+id");
            }
        }
    }


    public class SymbolIndex
    {
        long currentIndex = 0;
        static SymbolIndex index;
        public static SymbolIndex CurrentIndex
        {
            get
            {
                if (index == null)
                    index = new SymbolIndex();
                return index;
            }
        }
        public void Increase()
        {
            currentIndex++;
        }
        public long Currrent()
        {
            return currentIndex;
        }
    }
}
