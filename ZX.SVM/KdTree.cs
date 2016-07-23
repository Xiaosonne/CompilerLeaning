using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZX.SVM
{

    public class Xn<T>
    {
        public T[] X { get; set; }

        public override string ToString()
        {
            return String.Join(",", X);
        }
    }

    public class XnWithClassLable : Xn<Object>
    {
        public object Label { get; set; }

        public override string ToString()
        {
            return base.ToString() + "->" + Label;
        }

        public override bool Equals(object obj)
        {
            XnWithClassLable xn = obj as XnWithClassLable;
            return base.Equals(obj);
        }
    }

    public class SimpleSpace : HashSet<Object>
    {
        public T GetElement<T>(T i)
        {
            if (this.Add(i))
            {
                return i;
            }
            else
            {
                return (T)this.Single(p => p.Equals(i));
            }
        }
    }

    public class ProbablyMapDataSet : List<XnWithClassLable>
    {
        public ProbablyCalculator<XyProbably> GetXY()
        {
            return xyProbablySet;
        }
        SimpleSpace simpleSpaceX = new SimpleSpace();
        SimpleSpace simpleSpaceY = new SimpleSpace();
        public T GetElementX<T>(T i)
        {
            return simpleSpaceX.GetElement(i);
        }
        public T GetElementY<T>(T i)
        {
            return simpleSpaceY.GetElement(i);
        }

        ProbablyCalculator<XyProbably> xyProbablySet = new ProbablyCalculator<XyProbably>();
        public void BuiltProbabyMap()
        {
            xyProbablySet = new ProbablyCalculator<XyProbably>();
            foreach (var xnWithClassLable in this)
            {
                var key = new XyProbably()
                {
                    X1 = GetElementX(xnWithClassLable.X[0]),
                    X2 = GetElementX(xnWithClassLable.X[1]),
                    Y = GetElementY(xnWithClassLable.Label)
                };
                xyProbablySet.Add(key);
            }
        } 
    }

    public class TkDataSet<T> : List<Xn<T>>
    {
        public static TkDataSet<T> BuildTkDataSet(T[,] arr)
        {
            TkDataSet<T> set = new TkDataSet<T>();
            for (int i = 0; i <= arr.GetUpperBound(0); i++)
            {
                Xn<T> xn = new Xn<T>();
                xn.X = new T[arr.GetUpperBound(1) + 1];
                for (int j = 0; j <= arr.GetUpperBound(1); j++)
                {
                    xn.X[j] = arr[i, j];
                }
                set.Add(xn);
            }
            return set;
        }

        public static ProbablyMapDataSet BuildBayerSet(Object[,] dataWithLable)
        {
            ProbablyMapDataSet dt = new ProbablyMapDataSet();

            for (int i = 0; i <= dataWithLable.GetUpperBound(0); i++)
            {
                XnWithClassLable label = new XnWithClassLable();
                int d1 = dataWithLable.GetUpperBound(1);
                label.X = Enumerable.Range(0, d1).Select(p => dataWithLable[i, p]).ToArray();
                label.Label = dataWithLable[i, d1];
                dt.Add(label);
            }
            return dt;
        }
    }


    public class TreeNode
    {
        public int NodeDeep { get; set; }
        public object NodeValue { get; set; }
        //左侧节点小于当前维度结点
        public TreeNode LeftNode { get; set; }
        public TreeNode RightNode { get; set; }
        public TreeNode ParentNode { get; set; }
        public override string ToString()
        {
            return "node (" + NodeValue.ToString() + ")";
        }
    }



    public class KdTree<T>
    {
        public IComparer<T> GetValueComparer { get; set; }
        public IDistance<Xn<T>> GetValueDistance { get; set; }

        public int Demension { get; set; }

        public TreeNode RootNode { get; set; }

        public static TreeNode GetTreeNode<T>(TkDataSet<T> allItem, int deep)
        {
            TreeNode tn = new TreeNode();
            if (allItem.Count == 1)
            {
                tn.NodeValue = allItem[0];
                return tn;
            }
            //6 3 7 3
            var orderBy_i_demension = allItem.OrderBy(p => p.X[deep]).ToList();
            int count = allItem.Count;
            int index = (count - (count % 2)) / 2;
            Xn<T> midNumber = orderBy_i_demension[index];
            tn.NodeValue = midNumber;
            tn.NodeDeep = deep;
            TkDataSet<T> itemsLeft = new TkDataSet<T>();
            TkDataSet<T> itemsRight = new TkDataSet<T>();
            for (int j = 0; j < index; j++)
            {
                itemsLeft.Add(orderBy_i_demension[j]);
            }
            for (int j = index + 1; j < orderBy_i_demension.Count; j++)
            {
                itemsRight.Add(orderBy_i_demension[j]);
            }
            if (itemsLeft.Count > 0)
            {
                var node = GetTreeNode<T>(itemsLeft, deep + 1);
                tn.LeftNode = node;
                node.ParentNode = tn;

            }
            if (itemsRight.Count > 0)
            {
                var node = GetTreeNode<T>(itemsRight, deep + 1);
                tn.RightNode = node;
                node.ParentNode = tn;
            }
            return tn;
        }

        public static TreeNode GetLatesTreeNode<T>(KdTree<T> tree, Xn<T> x)
        {
            int demension = x.X.GetUpperBound(0);
            TreeNode nodeCur = tree.RootNode;
            for (int i = 0; i <= demension; i++)
            {
                Xn<T> xi = nodeCur.NodeValue as Xn<T>;
                if (tree.GetValueComparer.Compare(x.X[i], xi.X[i]) < 0)
                {
                    if (nodeCur.LeftNode != null)
                    {
                        nodeCur = nodeCur.LeftNode;
                    }
                }
                else
                {
                    if (nodeCur.RightNode != null)
                    {
                        nodeCur = nodeCur.RightNode;

                    }
                }
            }

            var compute = tree.GetValueDistance;
            Xn<T> xn = nodeCur.NodeValue as Xn<T>;
            TreeNode nodeNerest = nodeCur;
            double minDistance = compute.ComputeDistance(xn, x);
            while (true)
            {
                TreeNode nextNode = null;
                if (nodeCur.ParentNode != null)
                {
                    nextNode = nodeCur.ParentNode;
                    Xn<T> xni = nextNode.NodeValue as Xn<T>;
                    double d = compute.ComputeDistance(xni, x);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        nodeNerest = nextNode;
                        nextNode = object.ReferenceEquals(nextNode.LeftNode, nodeCur) ? nextNode.RightNode : nextNode.LeftNode;
                        if (nextNode == null)
                        {
                            nextNode = nodeCur.ParentNode;
                        }
                    }
                    nodeCur = nextNode;
                }
                else
                {
                    return nodeNerest;
                }
            }
            return nodeNerest;
        }
    }
}
