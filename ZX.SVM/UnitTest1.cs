using System;
using System.Linq;
using System.Linq.Expressions;

//using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZX.SVM
{
    // [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        public void 测试中位数()
        {
            int[] arr = new[] { 11, 2, 20, 22, 100, 1 };
            int[] arr1 = new[] { 11, 2, 21, 22, 100, 1, 200 };
            //Assert.AreEqual(20, GetMiddleNumber(arr));
            //Assert.AreEqual(21, GetMiddleNumber(arr1));
        }

        // [TestMethod]
        public void 测试KdTree()
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
            var node = KdTree<int>.GetTreeNode(dt, 0);
            KdTree<int> tree = new KdTree<int>();
            tree.RootNode = node;
            tree.GetValueComparer = 1.GetIComparer((p, q) => p - q);

            tree.GetValueDistance = (new Xn<int>()).GetIDistance((p, q) =>
            {
                double a = 0;
                a = Enumerable.Range(0, p.X.Length)
                    .AsParallel()
                    .Aggregate(0.0, (m, i) => m + Math.Pow(p.X[i] - q.X[i], 2));
                return Math.Sqrt(a);
            });
            Xn<int> x = new Xn<int>();
            x.X = new int[] { 9, 3 };
            var node2 = KdTree<int>.GetLatesTreeNode(tree, x);

        }

        public void 测试Bayers()
        {
            object[,] arr = new object[,]
            {
               {1,"S",-1},
               {1,"M",-1},
               {1,"M", 1},
               {1,"S", 1},
               {1,"S",-1},
               {2,"S",-1},
               {2,"M",-1},
               {2,"M", 1},
               {2,"L", 1},
               {2,"L", 1},
               {3,"L", 1},
               {3,"M", 1},
               {3,"M", 1},
               {3,"L", 1},
               {3,"L",-1}
            };
            var arr1 = TkDataSet<Object>.BuildBayerSet(arr);
            arr1.BuiltProbabyMap();
            var xy = arr1.GetXY();
            var a = xy.Probably(p => p.X1.Equals(1)).CalcProbably();
            double y1 = xy.Probably(p => p.Y.Equals(1)).CalcProbably();
            double y_1 = xy.Probably(p => p.Y.Equals(-1)).CalcProbably();
            double y1xm = xy.When(p => p.Y.Equals(1)).Probably(p => p.X1.Equals("M")).CalcProbably();
        }

        public int GetMiddleNumber(int[] arr)
        {
            int count = arr.Length;
            int index = (count - (count % 2)) / 2;
            return arr[index];
        }
    }
}
