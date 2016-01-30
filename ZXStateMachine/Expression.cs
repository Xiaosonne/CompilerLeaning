using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZXStateMachine2;

namespace ZXStateMachine
{

    public class DFABuilder
    {
        Node rootNode;
        List<LeafNode> allLeafNodes = new List<LeafNode>();
        List<Node> allMTNodes = new List<Node>();
        public DFABuilder(Node rootnode)
        {
            this.rootNode = rootnode;
        }
        StateMachineGen2 stateMachine;
        public StateMachineGen2 StateMachine
        {
            get
            {
                return stateMachine;
            }

        }
        public void BuildStateMachine()
        {
            stateMachine = new StateMachineGen2();
            string e = ZXStateMachine2.Accept.Epsilon.A;
            foreach (var i in rootNode.GetFirstPos())
            {
                stateMachine.AddStartState(i);
            }
            foreach (var s in allLeafNodes)
            {
                var follo = s.GetFollowpos();
                if (follo.Count == 0)
                {
                    stateMachine.AddEndState(s.Index);
                }
                else
                {
                    foreach (var ss in follo)
                    {
                        stateMachine.AddConvertState(s.Index, s.Token.ToString(), ss);
                    }
                }
            }
        }
        public void BuildFollowpos()
        {
            print(new Node[] { rootNode }.ToList());
            getfollow();
            foreach (var s in allLeafNodes)
            {
                Console.WriteLine(s.Index + " " + string.Join(",", s.GetFollowpos()));
            }
        }

        void getfollow()
        {
            foreach (var s in allMTNodes)
            {
                if (s.Token.ToString() == "&")
                {
                    var mt = s as MTNode;
                    foreach (var fi in mt.LeftChild.GetLastPos())
                    {
                        var snode = allLeafNodes.SingleOrDefault(p => p.Index == fi);
                        foreach (var id in mt.RightChild.GetFirstPos())
                        {
                            snode.AddFollowpos(id);
                        }
                    }
                }
                if (s.Token.ToString() == "*")
                {
                    foreach (var i in s.GetLastPos())
                    {
                        var sing = allLeafNodes.SingleOrDefault(p => p.Index == i);
                        foreach (var ii in s.GetFirstPos())
                        {
                            sing.AddFollowpos(ii);
                        }
                    }
                }
            }
        }
        void print(List<Node> nodes)
        {
            if (nodes.Count == 0)
                return;
            List<Node> lis = new List<Node>();
            foreach (var node in nodes)
            {
                if (node is STNode)
                {
                    lis.Add((node as STNode).Child);
                }
                if (node is MTNode)
                {
                    var mt = node as MTNode;
                    lis.Add(mt.LeftChild);
                    lis.Add(mt.RightChild);
                }
            }
            foreach (var node in nodes)
            {
                Console.Write(" " + node.Token);
                if (node.Index == 0)
                {
                    allMTNodes.Add(node);
                    node.GetFirstPos();
                    node.GetLastPos();
                }
                else
                    allLeafNodes.Add((LeafNode)node);
            }
            Console.WriteLine();
            print(lis);

        }
    }
    public class Expression
    {
        public static char[] OpChars = { '+', '-', '*', '/', '(', ')', '|', };
        public static bool isOperator(char ch)
        {
            return OpChars.Contains(ch);
        }
        static int index = 1;
        //"(a|b)|(m*cdefg)"
        public static Node RegTrans(string regstr)
        {
            char[] allOps = regstr.ToCharArray();
            int i = 0;
            int length = allOps.Length;
            char ch = allOps[i];
            Node tn = null;

            for (; i < length; i++)
            {
                ch = allOps[i];
                if (isOperator(ch))
                {
                    //如果是左括号 
                    if (ch == '(')
                    {
                        int l = 1;
                        int j = i + 1;
                        int k = 0;
                        char[] sub = new char[256];
                        //取出括号内所有的字符串
                        while (l != 0)
                        {
                            char chnext = allOps[j];
                            if (chnext == '(')
                                l = l + 1;
                            if (chnext == ')')
                                l = l - 1;
                            if (l != 0)
                                sub[k++] = (chnext);
                            j++;
                        }
                        //获取子节点
                        var node = RegTrans(new string(sub.Take(k).ToArray()));
                        i += (k + 1);
                        //取下一个符号 若是* 则产生 star节点
                        if ((i + 1) < allOps.Count())
                        {
                            char next = allOps[i + 1];
                            switch (next)
                            {
                                case '*':
                                    STNode st = new STNode()
                                    {
                                        Token = new Token('*'),
                                        Child = node
                                    };
                                    GenSubNode(ref tn, st);
                                    break;
                                default:
                                    GenSubNode(ref tn, node);
                                    break;
                            }

                        }
                        else//否则 直接产生下一个节点
                        {
                            GenSubNode(ref tn, node);
                        }
                        continue;
                    }
                    else
                    {
                        GenSubNodeWithOpChar(ch, ref tn);
                    }

                }
                else//非符号节点
                {
                    LeafNode tntemp = new LeafNode()
                    {
                        Index = index++,
                        Token = new Token(ch)
                    };

                    if ((i + 1) < allOps.Count())
                    {
                        char next = allOps[i + 1];
                        switch (next)
                        {
                            case '*':
                                //若下一个符号是star节点 生成start节点 xxxA*
                                STNode st = new STNode()
                                {
                                    Token = new Token('*'),
                                    Child = tntemp
                                };
                                GenSubNode(ref tn, st);
                                break;
                            default:
                                //否则直接生成节点
                                GenSubNode(ref tn, tntemp);
                                break;
                        }

                    }
                    else//已经完了
                    {
                        GenSubNode(ref tn, tntemp);
                    }

                }
            }
            return tn;
        }

        private static void GenSubNodeWithOpChar(char ch, ref Node tn)
        {
            switch (ch)
            {
                case '|':
                case '*':
                    Node tn1 = null;
                    if (ch == '|')
                    {
                        tn1 = new MTNode()
                        {
                            Token = new Token(ch),
                            LeftChild = tn
                        };
                    }
                    if (ch == '*')
                    {
                        tn1 = new STNode()
                        {
                            Token = new Token(ch),
                            Child = tn
                        };
                    }
                    tn = tn1;
                    break;
                default:
                    break;
            }
        }

        private static void GenSubNode(ref Node tn, Node tntemp)
        {
            if (tn is MTNode)
            {
                var tnn = tn as MTNode;
                if (tnn.LeftChild == null)
                {
                    tnn.LeftChild = tntemp;
                }
                else if (tnn.RightChild == null)
                {
                    tnn.RightChild = tntemp;
                }
                else
                {
                    MTNode tnnew = new MTNode()
                    {
                        Token = new Token('&'),
                        LeftChild = tn,
                        RightChild = tntemp
                    };
                    tn = tnnew;
                }
            }
            else
            {
                if (tn == null)
                {
                    tn = tntemp;
                }
                else
                {
                    MTNode tnnew = new MTNode()
                    {
                        Token = new Token('&'),
                        LeftChild = tn,
                        RightChild = tntemp
                    };
                    tn = tnnew;
                }
            }
        }

        private static MTNode AppendTreeNode(char ch, MTNode tn)
        {

            return tn;
        }
        public static int NextBracket(int start, char[] chs)
        {
            char ch = chs[start];
            if (ch == '(')
            {
                start = NextBracket(start + 1, chs);
            }
            while (start < chs.Length)
            {
                ch = chs[start];
                if (ch == ')')
                    return start;
                start++;
            }
            return start;
        }
    }
    public enum eTokenType
    {

    }
    public class Token
    {

        char t;
        public Token(char t)
        {
            this.t = t;
        }
        public override string ToString()
        {
            return t.ToString();
        }
    }
    public abstract class Node
    {
        public Node()
        {

        }
        public int Index { get; set; }
        public Token Token { get; set; }
        List<int> firstpos = new List<int>();
        List<int> lastpos = new List<int>();
        bool isnullale = false;
        bool iscalcnullable = false;

        public List<int> GetFirstPos()
        {
            if (firstpos.Count > 0)
                return firstpos.ToList();
            List<int> lis = new List<int>();
            if (this is LeafNode)
            {
                var lf = this as LeafNode;
                lis.Add(lf.Index);
            }
            if (this is STNode)
            {
                var st = this as STNode;
                lis.AddRange(st.Child.GetFirstPos());
            }
            if (this is MTNode)
            {
                var mt = this as MTNode;
                if (this.Token.ToString() == "|")
                {
                    lis.AddRange(mt.LeftChild.GetFirstPos());
                    lis.AddRange(mt.RightChild.GetFirstPos());
                }
                else
                {
                    if (mt.LeftChild.IsNullable())
                    {
                        lis.AddRange(mt.LeftChild.GetFirstPos());
                        lis.AddRange(mt.RightChild.GetFirstPos());
                    }
                    else
                    {
                        lis.AddRange(mt.LeftChild.GetFirstPos());
                    }
                }
            }
            this.firstpos = lis.Distinct().ToList(); ;
            return firstpos;
        }
        public bool IsNullable()
        {
            if (this.iscalcnullable)
                return isnullale;

            if (this is LeafNode)
            {
                iscalcnullable = true;
                return false;
            }
            if (this is STNode)
            {
                var st = this as STNode;
                //isnullale = st.Child.IsNullable();
                isnullale = true;
                iscalcnullable = true;
                return isnullale;
            }
            if (this is MTNode)
            {
                var mt = this as MTNode;
                var left = mt.LeftChild.IsNullable();
                var right = mt.RightChild.IsNullable();
                if (this.Token.ToString() == "|")
                {
                    isnullale = left || right;
                    iscalcnullable = true;
                }
                else
                {
                    isnullale = left && right;
                    iscalcnullable = true;
                }
                return isnullale;
            }
            return false;
        }
        public List<int> GetLastPos()
        {
            if (lastpos.Count > 0)
                return lastpos.ToList();
            List<int> lis = new List<int>();
            if (this is LeafNode)
            {
                var lf = this as LeafNode;
                lis.Add(lf.Index);
            }
            if (this is STNode)
            {
                var st = this as STNode;
                lis.AddRange(st.Child.GetLastPos());
            }
            if (this is MTNode)
            {
                var mt = this as MTNode;
                if (this.Token.ToString() == "|")
                {
                    lis.AddRange(mt.LeftChild.GetLastPos());
                    lis.AddRange(mt.RightChild.GetLastPos());
                }
                else
                {
                    if (mt.RightChild.IsNullable())
                    {
                        lis.AddRange(mt.LeftChild.GetLastPos());
                        lis.AddRange(mt.RightChild.GetLastPos());
                    }
                    else
                    {
                        lis.AddRange(mt.RightChild.GetLastPos());
                    }
                }
            }
            this.lastpos = lis.Distinct().ToList();
            return lastpos;
        }


    }
    public class LeafNode : Node
    {
        public LeafNode()
            : base()
        {
            Followpos = new List<int>();
        }
        public void AddFollowpos(int index)
        {
            Followpos.Add(index);
        }
        public List<int> GetFollowpos()
        {
            return Followpos.ToList();
        }
        List<int> Followpos;
    }
    public class STNode : Node
    {
        public STNode()
            : base()
        {
        }
        public Node Child { get; set; }

    }
    public class MTNode : Node
    {
        public MTNode()
            : base()
        {

        }
        public Node LeftChild { get; set; }
        public Node RightChild { get; set; }
    }
}
