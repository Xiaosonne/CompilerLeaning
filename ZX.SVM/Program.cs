using System;

namespace ZX.SVM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int[,] arr = new int[6, 2]
                {
                    {2,3}, 
                    {5,4},
                    {9,6},
                    {4,7},
                    {8,1},
                    {7,2} 
                };
            var dt = TkDataSet<int>.BuildTkDataSet(arr);
            var node = KdTree.GetTreeNode(dt, 0);
        }
    }
}
