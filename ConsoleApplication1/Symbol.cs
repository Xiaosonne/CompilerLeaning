using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXLanguage
{
    public class Symbol
    {
        String symbol;
        protected long index = 0;
        protected List<string> firstSet = null;
        /// <summary>
        /// 是否是 非终结符
        /// </summary>
        public bool IsNonTerminalSymbol { get; set; }
        public virtual bool IsEpsilonSymbol
        {
            get
            {
                return symbol == "ϵ";
            }
        }
        public virtual List<string> FirstSet
        {
            get
            {
                if (firstSet == null)
                    firstSet = new List<string>(new string[] { symbol });
                return firstSet;
            }
        }
        protected List<string> followSet = null;

        public Symbol(String ch)
        {
            symbol = ch;
            followSet = new List<string>();
        }
        public override bool Equals(object obj)
        {
            if (this.GetType().Equals(obj.GetType()))
            {
                var objs = obj as Symbol;
                return this.symbol == objs.SymbolChar;
            }
            return false;
        }

        public String SymbolChar
        {
            get
            {
                return symbol;
            }
        }

        public override string ToString()
        {
            return symbol;
        }
    }
}
