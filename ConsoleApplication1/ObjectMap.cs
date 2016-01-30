using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXLanguage
{
    public class ObjectMap<T1, T2, T3>
    {
        Dictionary<T1, Dictionary<T2, T3>> dict = new Dictionary<T1, Dictionary<T2, T3>>();
        public T3 this[T1 t1, T2 t2]
        {
            get
            {
                if (dict.ContainsKey(t1))
                {
                    var t11 = dict[t1];
                    if (t11 == null)
                    {
                        dict[t1] = new Dictionary<T2, T3>();
                        dict[t1].Add(t2, default(T3)); ;
                        return default(T3);
                    }
                    else
                    {
                        if (dict[t1].ContainsKey(t2))
                        {
                            return dict[t1][t2];
                        }
                        else
                        {
                            dict[t1].Add(t2, default(T3));
                            return default(T3);
                        }
                    }
                }
                else
                {
                    var t2t3 = new Dictionary<T2, T3>();
                    t2t3.Add(t2, default(T3));
                    dict.Add(t1, t2t3);
                    return default(T3);
                }
            }
            set
            {
                if (dict.ContainsKey(t1))
                {
                    var t11 = dict[t1];
                    if (t11 == null)
                    {
                        dict[t1] = new Dictionary<T2, T3>();
                        dict[t1].Add(t2, value); ;
                    }
                    else
                    {
                        if (dict[t1].ContainsKey(t2))
                        {
                            dict[t1][t2] = value;
                        }
                        else
                        {
                            dict[t1].Add(t2, value);

                        }
                    }
                }
                else
                {
                    var t2t3 = new Dictionary<T2, T3>();
                    t2t3.Add(t2, value);
                    dict.Add(t1, t2t3);
                }
            }
        }

        public bool Has(T1 t1, T2 t2)
        {
            if (this.dict.ContainsKey(t1) && dict[t1].ContainsKey(t2) && dict[t1][t2] != null)
            {
                return true;
            }
            else
                return false;
        }
    }
    public class CAMap : ObjectMap<Symbol, Symbol, CAAction>
    {

    }
}
