using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXLanguage
{
    public class Language
    {
        HashSet<CAAction> AllActions = new HashSet<CAAction>();
        HashSet<Symbol> AllSymbol = new HashSet<Symbol>();
        HashSet<CACondition> NonterminalSymbol = new HashSet<CACondition>();
        HashSet<Symbol> TerminalSymbol = new HashSet<Symbol>();
        Dictionary<Symbol, HashSet<Symbol>> FollowSet = new Dictionary<Symbol, HashSet<Symbol>>();
        CACondition CurrentCondition;
        CACondition StartCondition;
        Symbol EndSymbol;
        public HashSet<Symbol> Symbols { get { return AllSymbol; } }
        public Language DefineQueryTerm(String ch)
        {
            var sin = NonterminalSymbol.SingleOrDefault(p => p.SymbolChar == ch);
            if (sin == null)
            {
                sin = new CACondition(ch);
                sin.IsNonTerminalSymbol = true;
                NonterminalSymbol.Add(sin);
            }
            CurrentCondition = sin;
            return this;
        }
        public CACondition GetCondition(string chs)
        {
            var s = NonterminalSymbol.SingleOrDefault(p => p.SymbolChar == chs);
            if (s == null)
                return new CACondition(chs);
            return s;
        }
        public CAAction GetAction(string geneStr)
        {
            var ca = AllActions.SingleOrDefault(p => p.ToString() == geneStr);
            return ca;
        }
        public void AddQueryItem(string genStr)
        {
            var arr = genStr.Replace("->", "γ").Split('γ');
            this.DefineQueryTerm(arr[0]);
            var arr2 = arr[1].Split('|');
            foreach (var s in arr2)
            {
                this.AddTerms(s);
            }
        }
        bool endAddTerms = false;
        public void EndAddTerms(string startsymbol)
        {
            endAddTerms = true;
            foreach (var s in NonterminalSymbol)
                AllSymbol.Add(s);
            foreach (var s in TerminalSymbol)
                AllSymbol.Add(s);
            StartCondition = (CACondition)AllSymbol.SingleOrDefault(p => p.SymbolChar == startsymbol);
            var ss = new Symbol("$");
            ss.IsNonTerminalSymbol = false;
            EndSymbol = ss;
            AllSymbol.Add(ss);
        }
        public Language AddTermsBySplitChar(char split, string expression)
        {
            var arrays = expression.Split(split);
            foreach (var s in arrays)
            {
                AddTerms(s.Trim());
            }
            return this;
        }
        public Language AddTerms(string chs)
        {
            if (endAddTerms)
                return this;
            CAAction action = new CAAction();
            var charray = chs.ToCharArray();
            int i = 0;
            for (; i < charray.Count(); i++)
            {
                string actstr = "";
                char f = charray[i];
                Symbol s = null;
                actstr = new string(new char[] { f });
                if (f >= 'a' && f <= 'z')
                {
                    char next;
                    actstr = new string(new char[] { f });
                    if ((i + 1) < charray.Count())
                    {
                        next = charray[i + 1];
                        if (next >= 'a' && next <= 'z')
                        {
                            int j = i + 1;
                            while (j < charray.Count())
                            {
                                next = charray[j];
                                if (next >= 'a' && next <= 'z')
                                {
                                    j++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            actstr = new string(charray.Skip(i).Take(j - i).ToArray());
                            i = j;
                        }
                    }
                }
                if (f >= 'A' && f <= 'Z')
                {
                    actstr = new string(new char[] { f });
                    if ((i + 1) < charray.Count())
                    {
                        char next = charray[i + 1];
                        if (next == '\'')
                        {
                            actstr = new string(new char[] { charray[i], '\'' });
                            i++;
                        }
                    }
                }
                char ff = actstr.First();
                if (ff >= 'A' && ff <= 'Z')
                {
                    s = NonterminalSymbol.SingleOrDefault(p => p.SymbolChar == actstr);
                    if (s == null)
                    {
                        s = new CACondition(actstr);
                        s.IsNonTerminalSymbol = true;
                        NonterminalSymbol.Add((CACondition)s);
                    }
                }
                else
                {
                    s = TerminalSymbol.SingleOrDefault(p => p.SymbolChar == actstr);
                    if (s == null)
                    {
                        s = new Symbol(actstr);
                        s.IsNonTerminalSymbol = false;
                        TerminalSymbol.Add(s);
                    }
                }
                action.AddLast(s);
            }
            if (CurrentCondition != null)
            {
                action.Condition = CurrentCondition;
                CurrentCondition.Actions.Add(action);
                AllActions.Add(action);
                CurrentCondition = null;
            }
            return this;
        }
        public void GetFollow()
        {
            foreach (var s in this.NonterminalSymbol)
            {
                FollowSet.Add(s, new HashSet<Symbol>());
            }
            FollowSet[StartCondition].Add(AllSymbol.SingleOrDefault(p => p.SymbolChar == "$"));
            while (true)
            {
                int change = 0;
                foreach (var nont in this.NonterminalSymbol)
                {
                    //遍历所有的产生式
                    foreach (var act in AllActions)
                    {
                        //如果当前符号在右边
                        //Console.WriteLine(nont + " :  " + act);
                        if (act.IsRightTerms(nont.SymbolChar))
                        {
                            var snext = act.SymbolAtNext(nont);
                            if (snext != null)
                            {
                                List<Symbol> lisright = new List<Symbol>();
                                var temp = snext;
                                while (temp != null)
                                {
                                    lisright.Add(temp);
                                    temp = act.SymbolAtNext(temp);
                                }
                                //Console.WriteLine(nont + " right " + string.Join(" ", lisright));
                                foreach (var sss in snext.FirstSet)
                                {
                                    if (sss != "ϵ")
                                    {
                                        var sing = AllSymbol.SingleOrDefault(p => p.SymbolChar == sss);
                                        if (sing != null)
                                        {
                                            if (FollowSet[nont].Add(sing))
                                                change++;
                                        }

                                    }
                                }
                                if (snext.IsEpsilonSymbol)
                                {
                                    foreach (var s in FollowSet[act.Condition])
                                    {
                                        if (FollowSet[nont].Add(s))
                                            change++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var s in FollowSet[act.Condition])
                                {
                                    if (FollowSet[nont].Add(s))
                                        change++;
                                }
                            }
                        }
                    }
                }
                if (change == 0)
                    break;
            }
        }

        CAMap M = new CAMap();
        public void PrintFollow()
        {
            foreach (var s in this.FollowSet)
            {
                foreach (var ss in s.Value)
                {
                    Console.WriteLine(s.Key + "-----" + ss);
                }
            }
            foreach (var s in this.FollowSet)
            {
                Console.WriteLine(s.Key + " FOLLOW " + string.Join(",", s.Value));
            }
        }
        public void BuildSelectSet()
        {
            foreach (var A in AllActions)//A->alpha
            {
                var first = A.First();
                foreach (var fir in first.FirstSet)
                {
                    var a = AllSymbol.Single(p => p.SymbolChar == fir);
                    if (!a.IsNonTerminalSymbol)
                    {
                        if (!a.IsEpsilonSymbol)
                            M[A.Condition, a] = A;//A->a=> A->alpha
                        if (fir == "ϵ")
                        {
                            foreach (var b in FollowSet[A.Condition])
                            {
                                if (!b.IsNonTerminalSymbol && !b.IsEpsilonSymbol)
                                {
                                    M[A.Condition, b] = A;
                                    Console.WriteLine("[" + A.Condition + "," + b + "] : " + A.Condition + "->" + string.Join("", A));
                                }
                            }
                        }
                    }
                    Console.WriteLine("[" + A.Condition + "," + a + "] : " + A.Condition + "->" + string.Join("", A));
                }
            }
        }

        public bool Accept(string s)
        {
            Stack<Symbol> sStack = new Stack<Symbol>();
            sStack.Push(EndSymbol);
            sStack.Push(StartCondition);
            var sm = sStack.Peek();
            char[] arr = s.ToCharArray();
            int i = 0;
            int j = 0;
            while (sm.SymbolChar != "$")
            {

                i = j;
                string str = "";
                NextSymbol(arr, ref i, ref str);
                if (!sm.IsNonTerminalSymbol)
                {
                    if (sm.SymbolChar == str)
                    {
                        sStack.Pop();
                        sm = sStack.Peek();
                        j = i;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    var sin = AllSymbol.SingleOrDefault(p => p.SymbolChar == str);
                    if (M.Has(sm, sin))
                    {
                        var act = M[sm, sin];
                        sStack.Pop();
                        var k = act.Last;
                        while (k != null)
                        {
                            if (!k.Value.IsEpsilonSymbol)
                                sStack.Push(k.Value);
                            k = k.Previous;
                        }
                        sm = sStack.Peek();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        List<ItemSet> itemSet = null;
        public List<ItemSet> BuildItemSetFamily(string startAction)
        {
            if (itemSet == null)
            {
                var at = this.GetAction(startAction);
                ItemSet iss = new ItemSet();
                iss.Add(new Item() { Action = at, Position = 0 });
                LinkedList<ItemSet> hs = new LinkedList<ItemSet>();
                var start = iss.Closure();
                start.SetIndex(0);
                hs.AddFirst(start);
                var fi = hs.First;
                int i = 1;
                while (true)
                {
                    if (fi != null)
                    {
                        foreach (var ss in this.Symbols)
                        {
                            var its = fi.Value.Goto(ss.SymbolChar);
                            if (its.Count <= 0)
                                continue;
                            var k = hs.SingleOrDefault(p => p.Equals(its));
                            if (k == null)
                            {
                                its.SetIndex(i++);
                                // Console.WriteLine(ss.SymbolChar + " ::: " + its);
                                hs.AddLast(its);
                            }
                        }
                        fi = fi.Next;
                    }
                    else
                    {
                        break;
                    }
                }
                itemSet = hs.ToList();
            }
            return itemSet;
        }
        public HashSet<string> GenStateDFA()
        {
            HashSet<String> lis = new HashSet<string>();
            foreach (var s in itemSet) {
                foreach (var ss in s) {
                    var sim = ss.GetDotRight();
                    if (sim == null)
                        continue;
                    var t = s.Goto(sim.SymbolChar);
                    var sing = itemSet.SingleOrDefault(p => p.Equals(t));
                    if (sing != null)
                    {
                        lis.Add(string.Format("{0} {1} {2}", s.Name, sim.SymbolChar, sing.Name));
                    }
                }
            }
            return lis;
        }
        public HashSet<string> GenSLRAction()
        {
            GetFollow();
            HashSet<String> lis = new HashSet<string>();
            foreach (var i in itemSet)
            {
                foreach (var ss in i)
                {
                    var sim = ss.GetDotRight();
                    if (sim == null) {
                        foreach (var fw in FollowSet[ss.Action.Condition]) {
                            lis.Add(string.Format("{0} {1} 规约 {2}", i.Name, fw.SymbolChar,ss.Action)); 
                        }
                        continue;
                    }
                        
                    var t = i.Goto(sim.SymbolChar);
                    var sing = itemSet.SingleOrDefault(p => p.Equals(t));
                    if (!sim.IsNonTerminalSymbol) {
                        if (sing != null)
                        {
                            lis.Add(string.Format("{0} {1} 入栈 {2}", i.Name, sim.SymbolChar, sing.Name));
                        }
                    } 
                }
            }
            return lis;
        }
        private static void NextSymbol(char[] arr, ref int i, ref string str)
        {
            char ch = arr[i];
            if (ch >= 'a' && ch <= 'z')
            {
                int j = i + 1;
                while (j < arr.Length)
                {
                    if (arr[j] <= 'z' && arr[j] >= 'a')
                    {
                        j++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (j > (i + 1))
                {
                    str = new string(arr.Skip(i).Take(j - i).ToArray());
                    i = j;
                }
                else
                {
                    str = new string(new char[] { ch });
                }
            }
            else
            {
                str = new string(new char[] { ch });
            }
        }
    }
}
