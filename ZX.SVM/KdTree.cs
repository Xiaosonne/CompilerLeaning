using System;
using System.Collections.Generic;
using System.Linq;
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
    }

    public class TreeNode
    {
        public object NodeValue { get; set; }
        public TreeNode LeftNode { get; set; }
        public TreeNode RightNode { get; set; }
        public TreeNode ParentNode { get; set; }
        public override string ToString()
        {
            return "node (" + NodeValue.ToString() + ")";
        }
    }

    public class KdTree
    {
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
    }
}
