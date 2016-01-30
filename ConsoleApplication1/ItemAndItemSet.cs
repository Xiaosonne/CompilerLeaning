using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXLanguage
{
    public class ItemSet : HashSet<Item>
    {
        string name = "";
        public void SetIndex(int index)
        {
            name = "I" + index;
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public bool AddWithoutSame(Item it)
        {
            if (this.SingleOrDefault(p => p.Equals(it)) != null)
                return false;
            return this.Add(it);
        }
        public override bool Equals(object obj)
        {
            if (obj is ItemSet)
            {
                var it = obj as ItemSet;
                if (it.Count != this.Count)
                {
                    return false;
                }
                return this.IsSubsetOf(it) && it.IsSubsetOf(this);
            }
            return false;
        }
        public ItemSet Closure()
        {
            ItemSet c = new ItemSet();
            Queue<CACondition> queue = new Queue<CACondition>();
            foreach (var s in this)
            {
                c.Add(s);
                var ss = s.GetDotRight();
                if (ss != null && (ss is CACondition))
                {
                    queue.Enqueue((CACondition)ss);
                }
            }
            while (queue.Count > 0)
            {
                var ss = queue.Dequeue();
                bool cont = false;
                foreach (var action in (ss as CACondition).Actions)
                {
                    Item it = new Item() { Action = action };
                    if (c.AddWithoutSame(it))
                    {
                        var fir = action.First.Value;
                        if (fir is CACondition)
                        {
                            queue.Enqueue((CACondition)fir);
                        }
                    }

                }
                if (queue.Count == 0 && !cont)
                    break;
            }
            return c;
        }
        public ItemSet Goto(string ss)
        {
            ItemSet sss = new ItemSet();
            foreach (var s in this)
            {
                var r = s.GetDotRight();
                if (r != null && r.SymbolChar.Equals(ss))
                {
                    Item it = new Item() { Action = s.Action, Position = s.Position + 1 };
                    sss.AddWithoutSame(it);
                }
            }
            return sss.Closure();
        }
        public override string ToString()
        {
            return string.Join(" ", this);
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
    public class Item
    {
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public Symbol GetDotRight()
        {
            int i = 0;
            var f = Action.First;
            while (i < Position)
            {
                i++;
                f = f.Next;
                if (f == null)
                    return null;
            }
            if (f == null)
                return null;
            return f.Value;
        }
        public CAAction Action { get; set; }
        public int Position { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is Item)
            {
                var it = obj as Item;
                return it.Position == this.Position && this.Action.Equals(it.Action);
            }
            else
            {
                return false;
            }
        }
        public override string ToString()
        {
            var s = Action.ToString();
            return s.Insert(s.IndexOf('>') + 1 + Position, "·");
        }
    }
    public class SLRMapItem {
        public string State { get; set; }
        public string ActionOrGoto { get; set; }
        //s push
        //r specification
        //a accept
        //n goto
        public string SRA { get; set; }
    }
}
