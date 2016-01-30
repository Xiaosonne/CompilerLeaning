using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXStateMachine
{

    public class StateMachine
    {
        SMStartState startState;
        public HashSet<SMState> States { get; set; }
        public HashSet<SMInputBase> Inputs { get; set; } 
        public void AddConvert(string s, string input, string next)
        {
            var start = addState(s);
            var nex = addState(next);
            var inputs = SMInputBase.GetInput(start, nex, input);
            start.AddAccept(inputs);
            Inputs.Add(inputs);
        }
        public StateMachine(string startState)
        {
            States = new HashSet<SMState>();
            Inputs = new HashSet<SMInputBase>();
            SMStartState start = new SMStartState(startState);
            States.Add(start);
            this.startState = start;
        }
        public void AddEndState(string next)
        {
            SMEndState end = new SMEndState(next);
            if (!States.Any(p => p.Equals(end)))
            {
                States.Add(end);
            }
        }
        private SMState addState(string s)
        {
            SMState state = null;
            if (States.Any(p => p.StateName == s))
            {
                state = States.Single(p => p.StateName == s);
            }
            else
            {
                state = new SMState(s);
                States.Add(state);
            }
            return state;
        }
        public bool AcceptLetters(String[] letters)
        {
            int i = 0;
            string start = letters[0];
            var next = startState.Accept(start);
            while (next != null)
            {
                Console.WriteLine("index {0} name {1} input {2} ", i, next.StateName, letters[i]);
                if (next is SMEndState)
                {
                    return next.Accept(letters[i]) == null;
                }
                i++; 
                next = next.Accept(letters[i]); 
            }
            return false;
        }
        SMState currentState;
        public SMState CurrentState
        {
            get
            {
                return currentState;
            }
        }
    }

    public class SMState : IEquatable<SMState>
    {
        public const string StartStateName = "SMStartState";
        public const string EndStateName = "SMEndState";
        private String stateName;
        private HashSet<SMInputBase> stateAccepts;
        public SMState(String name)
        {
            stateName = name;
            stateAccepts = new HashSet<SMInputBase>();
        }
        public String StateName
        {
            get
            {
                return stateName;
            }
        }
        public virtual void AddAccept(SMInputBase input)
        {
            if (!stateAccepts.Any(p => p.Equals(input)))
            {
                this.stateAccepts.Add(input);
            }
        }
        public virtual bool Equals(SMState other)
        {
            return this.StateName == other.StateName;
        }
        public virtual SMState Accept(String letter)
        {
            if (stateAccepts.Any(p => p.InputContent == letter))
            {
                var input = stateAccepts.SingleOrDefault(p => p.InputContent == letter);
                if (input == null)
                    return null;
                else
                    return input.NextState;
            }
            return null;
        }
    }
    public class SMStartState : SMState
    {
        public SMStartState(String Content)
            : base(Content)
        {

        }
 
        public override bool Equals(SMState obj)
        {
            if (obj is SMStartState)
            {
                return obj.Equals(this);
            }
            return false;
        }
    }
    public class SMEndState : SMState
    {
        public SMEndState(String Content)
            : base(Content)
        {
        } 
        public override bool Equals(SMState other)
        {
            if (other is SMEndState)
            {
                return other.Equals(this);
            }
            return false;
        }
    }
    public abstract class SMInputBase : IEquatable<SMInputBase>
    {
        public const string Epsilon = "ϵ";
        protected SMState ownerState;
        protected SMState nextState;
        public SMState OwnerState
        {
            get
            {
                return ownerState;
            }
            protected set
            {
                ownerState = value;
            }
        }
        public SMState NextState
        {
            get
            {
                return nextState;
            }
            protected set
            {
                nextState = value;
            }
        }
        protected String inputContent;
        public string InputContent { get { return inputContent; } }
        public SMInputBase(SMState s1, SMState s2)
        {
            this.OwnerState = s1;
            this.NextState = s2;
        }
        public bool Equals(SMInputBase other)
        {
            return inputContent.Equals(other.InputContent) && ownerState.Equals(other.OwnerState) && nextState.Equals(other.NextState);
        }
        public static SMInputBase GetInput(SMState s1, SMState s2, string inputs)
        {

            if (inputs != null && inputs.Count() > 0)
            {
                SMInputBase bases = null;
                if (inputs == SMInputBase.Epsilon)
                    bases = new EpsilonInput(s1, s2);
                else
                    bases = new AlphaInput(inputs, s1, s2);
                return bases;
            }
            return null;
        }
    }
    public class EpsilonInput : SMInputBase
    {
        public EpsilonInput(SMState s1, SMState s2)
            : base(s1, s2)
        {
            this.inputContent = SMInputBase.Epsilon;
        }
    }
    public class AlphaInput : SMInputBase
    {
        public AlphaInput(String input, SMState s1, SMState s2)
            : base(s1, s2)
        {
            this.inputContent = input;

        }
    }
}
