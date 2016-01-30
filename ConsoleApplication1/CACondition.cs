using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXLanguage
{
    public class CACondition : Symbol
    {
        public CACondition(string ch)
            : base(ch)
        {
            Actions = new List<CAAction>();
        }
        public List<CAAction> Actions { get; set; }
        public override List<string> FirstSet
        {
            get
            {
                if (firstSet == null)
                {
                    firstSet = new List<string>();
                    foreach (var s in Actions)
                    {
                        var f = s.First();
                        firstSet.AddRange(f.FirstSet);
                    }
                }
                return firstSet;
            }
        }
        bool EpsilonSymbolProc = false;
        bool isEpsilonSymbol;
        public override bool IsEpsilonSymbol
        {
            get
            {
                if (EpsilonSymbolProc)
                    return isEpsilonSymbol;
                EpsilonSymbolProc = true;
                isEpsilonSymbol = false;
                foreach (var s in this.Actions)
                {
                    bool ise2 = false;
                    foreach (var ss in s)
                    {
                        if (ss.IsEpsilonSymbol)
                        {
                            ise2 = true;
                            break;
                        }
                    }
                    if (ise2 == true)
                    {
                        isEpsilonSymbol = true;
                        break;
                    }
                }
                return isEpsilonSymbol;
            }
        }

        public List<Symbol> SymbolAtNext(Symbol ch)
        {
            List<Symbol> lis = new List<Symbol>();
            foreach (var s in Actions)
            {
                var ss = s.SymbolAtNext(ch);
                if (ss != null)
                    lis.Add(ss);
            }
            return lis;
        }
    }
}
