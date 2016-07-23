using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZX.SVM
{


    public class ProbablyCalculator<T> : HashSet<T> where T : BaseProbably
    {
        public ProbablyCalculator(IEnumerable<T> enu)
            : base(enu)
        {

        }

        public ProbablyCalculator()
            : base()
        {
        }
        public virtual object MaxProbablyWhen(Func<T, bool>[] xValues, SimpleSpace xx, SimpleSpace y)
        {
            return null;
        }
        public virtual ProbablyWhenCalculator<T> When(Func<T, bool> when)
        {
            ProbablyWhenCalculator<T> ret = new ProbablyWhenCalculator<T>(this);

            if (_probablyWhile != null)
                ret.Probably(_probablyWhile);
            if (when != null)
                return ret.When(when);
            else
                return ret;
        }

        protected Func<T, bool> _probablyWhile;
        public virtual ProbablyCalculator<T> Probably(Func<T, bool> expr)
        {
            if (_probablyWhile == null)
            {
                _probablyWhile = expr;
                if (_probablyWhile == null)
                {
                    throw new Exception("Not Binary Expression");
                }
            }
            else
            {
                _probablyWhile = t => expr(t) && _probablyWhile(t);
            }
            return this;
        }

        public virtual double CalcProbably()
        {

            int whileCount = this.Where(p => _probablyWhile(p)).ToList().Count;
            _probablyWhile = null;
            return (double)whileCount / (double)this.Count;
        }
    }
    public class ProbablyWhenCalculator<T> : ProbablyCalculator<T> where T : BaseProbably
    {
        public ProbablyWhenCalculator(IEnumerable<T> enu)
            : base(enu)
        {

        }

        public ProbablyWhenCalculator()
            : base()
        {
        }
        protected Func<T, bool> _probablyWhen;
        public override ProbablyWhenCalculator<T> When(Func<T, bool> expr)
        {
            if (_probablyWhen == null)
            {
                _probablyWhen = expr;
                if (_probablyWhen == null)
                {
                    throw new Exception("Not Binary Expression");
                }
            }
            else
            {
                _probablyWhen = t => expr(t) && _probablyWhen(t);
            }
            return this;
        }

        public override double CalcProbably()
        {
            int whenCount = this.Count;
            int whileCount = 0;
            if (_probablyWhen != null && _probablyWhile != null)
            {

                var when = this.Where(p => _probablyWhen(p)).ToList();
                whenCount = when.Count;
                whileCount = when.Where(p => _probablyWhile(p)).ToList().Count;
                _probablyWhen = null;
                _probablyWhile = null;
                return (double)whileCount / whenCount;
            }
            else
            {
                _probablyWhen = null;
                _probablyWhile = null;

                return 0;
            }
        }
    }

    public class BaseProbably
    {
    }
    public class XyProbably : BaseProbably
    {
        public object X1 { get; set; }
        public object X2 { get; set; }
        public object Y { get; set; }


        public override string ToString()
        {
            return string.Format("(X={0},Y={1})", X1, X2);
        }
    }

}
