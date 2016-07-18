using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZX.SVM
{
    public class Xn<T>
    {
        public T[] X { get; set; }
    }
    public class TkDataSet<T> : List<Xn<T>>
    {
        public static TkDataSet<T> BuildTkDataSet(T[][] arr)
        {
            TkDataSet<T> set = new TkDataSet<T>();
            for (int i = 0; i <= arr.GetUpperBound(0); i++)
            {
                Xn<T> xn = new Xn<T>();
                xn.X = new T[arr[i].GetUpperBound(0) + 1];
                Array.Copy(arr[i], xn.X, arr[i].Length);
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
    }

    public class KdTree
    {
        public int Demension { get; set; }

        public TreeNode RootNode { get; set; }

        public static TreeNode GetTreeNode<T>(TkDataSet<T> allItem, int deep)
        {
            //6 3 7 3
            var orderBy_i_demension = allItem.OrderBy(p => p.X[deep]).ToList();
            int count = allItem.Count;
            int index = (count - (count % 2)) / 2;
            Xn<T> midNumber = orderBy_i_demension[index];
            TreeNode tn = new TreeNode();
            tn.NodeValue = midNumber;
            TkDataSet<T> itemsLeft = new TkDataSet<T>();
            TkDataSet<T> itemsRight = new TkDataSet<T>();
            for (int j = 0; j < index; j++)
            {
                itemsLeft.Add(orderBy_i_demension[j]);
            }
            for (int j = index; j < orderBy_i_demension.Count; j++)
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
