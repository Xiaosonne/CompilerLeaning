using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZX.SVM
{
    public interface IDistance<T>
    {
        double ComputeDistance(T t1, T t2);
    }

    public class XnDistance<T> : IDistance<T>
    {
        private Func<T, T, double> computeFunc;
        public XnDistance(Func<T, T, double> fun)
        {
            computeFunc = fun;
        }

        public double ComputeDistance(T t1, T t2)
        {
            return this.computeFunc(t1, t2);
        }
    }

    public class XnComparer<T> : IComparer<T>
    {
        private Func<T, T, int> compareFunc;
        public XnComparer(Func<T, T, int> comFunc)
        {
            compareFunc = comFunc;
        }

        public int Compare(T x, T y)
        {
            return compareFunc(x, y);
        }

        
    }
    public static class XnExtends
    {
        public static IComparer<T> GetIComparer<T>(this T t, Func<T, T, int> fun)
        {
            return new XnComparer<T>(fun);
        }
        public static IDistance<T> GetIDistance<T>(this T t, Func<T, T, double> fun)
        {
            return new XnDistance<T>(fun);
        }
    }
}
