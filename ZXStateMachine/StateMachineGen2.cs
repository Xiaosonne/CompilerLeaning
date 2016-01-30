using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXStateMachine2
{

    public class State
    {
        HashSet<SAPair> StateNextSet = new HashSet<SAPair>();
        HashSet<SAPair> StatePreSet = new HashSet<SAPair>();
        public string S { get; set; }
        public State(String s)
        {
            this.S = s;
        }
        public HashSet<State> StateAcceptInput(string s)
        {
            HashSet<State> ret = new HashSet<State>();
            StateNextSet.ToList().ForEach(p =>
            {
                if (p.Accept.A.Equals(s))
                {
                    ret.Add(p.State);
                }
            });
            return ret;
        }
        public void AddNext(State s, Accept a)
        {
            StateNextSet.Add(new SAPair() { Accept = a, State = s });
        }
        public void AddPre(State s, Accept a)
        {
            StatePreSet.Add(new SAPair() { Accept = a, State = s });
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is State)
            {
                var s = obj as State;
                return this.S.Equals(s.S);
            }
            else
                return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class Accept
    {
        public static Accept Epsilon = new Accept("ϵ");
        public string A { get; set; }
        public Accept(string a)
        {
            this.A = a;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is Accept)
            {
                var s = obj as Accept;
                return this.A.Equals(s.A);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class SAPair
    {
        public Accept Accept { get; set; }
        public State State { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is SAPair)
            {
                SAPair s = obj as SAPair;
                return s.State.Equals(State) && s.Accept.Equals(Accept);
            }
            else
                return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class StateMap
    {
        public State State { get; set; }
        public Accept Accept { get; set; }
        public State NextState { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is StateMap)
            {
                StateMap s = obj as StateMap;
                return s.State.Equals(State) && s.NextState.Equals(NextState) && s.Accept.Equals(Accept);
            }
            else
                return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class StateMachineGen2
    {
        HashSet<State> States = new HashSet<State>();
        HashSet<Accept> Accepts = new HashSet<Accept>();
        HashSet<StateMap> StateMaps = new HashSet<StateMap>();
        HashSet<State> StartState=new HashSet<State>();
        HashSet<State> EndStates = new HashSet<State>();
        //状态集合S经过a转换后能到达的集合
        public HashSet<State> Move(HashSet<State> s, string a)
        {
            HashSet<State> ret = new HashSet<State>();

            foreach (var ss in s)
            {
                foreach (var sss in ss.StateAcceptInput(a)) {
                    ret.Add(sss);
                }
                //StateMaps
                //    .Where(p => p.State.Equals(ss) && p.Accept.A.Equals(a))
                //    .ToList()
                //    .ForEach(p =>
                //    {
                //        ret.Add(p.NextState);
                //    });
            }
            return ret;
        }
        //状态s经过a能到达的状态集合
        public HashSet<State> Move(State s, string a)
        {
            HashSet<State> ret = new HashSet<State>();
            StateMaps
                   .Where(p => p.State.Equals(s) && p.Accept.A.Equals(a))
                   .ToList()
                   .ForEach(p =>
                   {
                       ret.Add(p.NextState);
                   });
            return ret;
        }
        //所有集合经过epsilon能到达的集合
        public HashSet<State> EClosure(IEnumerable<State> states)
        {
            HashSet<State> ret = new HashSet<State>(states);
            Stack<State> stack = new Stack<State>(states);
            while (stack.Count != 0)
            {
                var s = stack.Pop();
                foreach (var ss in Move(s, Accept.Epsilon.A))
                {
                    if (!ret.Contains(ss))
                    {
                        ret.Add(ss);
                        stack.Push(ss);
                    }
                }
            }
            return ret;
        }
        public void GetNextState(string input)
        {
            int i = 0;
            HashSet<string> ins = new HashSet<string>(new string[] { input });
            var lis = this.StateMaps.Where(p => ins.Contains(p.Accept.A)).ToList();
            while (i < 3)
            {
                ins = new HashSet<string>();
                if (lis != null && lis.Count > 0)
                {
                    HashSet<StateMap> maps = new HashSet<StateMap>();
                    foreach (var s in lis)
                    {
                        this.StateMaps.Where(p => s.NextState.Equals(p.State))
                            .ToList()
                            .ForEach(p =>
                            {
                                maps.Add(p);
                            });
                    }
                }
                i++;
            }
        }
        public void AddConvertState(object s1, object input, object s2)
        {
            var ss = GetState(s1.ToString());
            var ss2 = GetState(s2.ToString());
            var aa = GetAccept(input.ToString());
            ss.AddNext(ss2, aa);
            ss2.AddPre(ss, aa);
            GetStateMap(ss, aa, ss2);
        }
        public bool AcceptInputs(string inputs)
        {
            var S = EClosure(StartState.ToArray());
            int i = 0;
            while (i < inputs.Length)
            {
                string input = inputs.Substring(i, 1);
                S = EClosure(Move(S, input));
                i++;
            }
            return S.Overlaps(EndStates);
        }
        public void PrintState() {
            foreach (var s in this.StateMaps) {
                Console.WriteLine("{0}-{1}-{2}", s.State.S, s.Accept.A, s.NextState.S);
            }
        }
        public void AddStartState(object s)
        {
            var ss = GetState(s.ToString());
            StartState.Add(ss);
        }
        public void AddEndState(object s)
        {
            EndStates.Add(GetState(s.ToString()));
        }
        private void GetStateMap(State s1, Accept a, State s2)
        {
            StateMap sm = new StateMap();
            sm.Accept = a;
            sm.State = s1;
            sm.NextState = s2;
            var sm2 = StateMaps.SingleOrDefault(p => p.Equals(sm));
            if (sm2 == null)
            {
                this.StateMaps.Add(sm);
            }
        }
        private State GetState(string s)
        {
            var ss = States.SingleOrDefault(p => p.S.Equals(s));
            if (ss == null)
            {
                ss = new State(s);
                this.States.Add(ss);
                return ss;
            }
            return ss;
        }
        private Accept GetAccept(string s)
        {
            var ss = Accepts.SingleOrDefault(p => p.A.Equals(s));
            if (ss == null)
            {
                ss = new Accept(s);
                this.Accepts.Add(ss);
            }
            return ss;
        }
    }
}
