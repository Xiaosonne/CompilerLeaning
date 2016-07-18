using System;
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
        public void 测试()
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

        public int GetMiddleNumber(int[] arr)
        {
            int count = arr.Length;
            int index = (count - (count % 2)) / 2;
            return arr[index];
        }
    }
}
