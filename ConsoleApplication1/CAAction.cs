using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXLanguage
{
    public class CAAction : LinkedList<Symbol>
    {
        public override bool Equals(object obj)
        {
            if (obj is CAAction)
            {
                var ac = obj as CAAction;
                if (ac.Count != this.Count)
                    return false;
                var f1 = this.First;
                var f2 = ac.First;
                while (f1 != null)
                {
                    if (!f1.Value.Equals(f2.Value))
                    {
                        return false;
                    }
                    f1 = f1.Next;
                    f2 = f2.Next;
                }
                return true;
            }
            return false;
        }
        public override string ToString()
        {
            return Condition + "->" + string.Join("", this);
        }
        public CACondition Condition { get; set; }
        public bool IsRightTerms(string ch)
        {
            return this.Any(p => p.SymbolChar == ch);
        }
        public Symbol SymbolAtPrevious(Symbol ch)
        {
            var node = this.Find(ch);
            if (node == null)
                return null;
            if (node.Previous == null)
                return null;
            return node.Previous.Value;
        }
        public Symbol SymbolAtNext(Symbol ch)
        {
            var node = this.Find(ch);
            if (node == null)
                return null;
            if (node.Next == null)
                return null;
            return node.Next.Value;
        }
        public CAAction()
            : base()
        {

        }
        public CAAction(IEnumerable<Symbol> collection)
            : base(collection)
        {

        }
    }
}
