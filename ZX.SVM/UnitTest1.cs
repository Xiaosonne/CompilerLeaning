using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZX.SVM
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void 测试中位数()
        {
            int[] arr = new[] { 11, 2, 20, 22, 100, 1 };
            int[] arr1 = new[] { 11, 2, 21, 22, 100, 1,200 };
            Assert.AreEqual(20, GetMiddleNumber(arr));
            Assert.AreEqual(21, GetMiddleNumber(arr1));
        }

        public int GetMiddleNumber(int[] arr)
        {
            int count = arr.Length;
            int index = (count - (count % 2)) / 2;
            return arr[index];
        }
    }
}
