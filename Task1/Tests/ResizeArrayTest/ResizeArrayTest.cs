using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ResizeArray;

namespace ResizeArrayTest
{
    [TestClass]
    public class ResizeArrayTest
    {
        ResizeArray<int> TestArray;

        [TestMethod]
        public void AppendTest()
        {
            TestArray = new ResizeArray<int>();
            for (int i = 0; i < 1; i++)
            {
                TestArray.Append(i);
                Assert.AreEqual(i, TestArray[i]);
            }
        }

        [TestMethod]
        public void SetItemTest()
        {
            TestArray = new ResizeArray<int>();
            TestArray.Append(2);
            TestArray[0] = 404;
            Assert.AreEqual(404, TestArray[0]);
        }

        [TestMethod]
        public void CountTest()
        {
            TestArray = new ResizeArray<int>();
            Random rnd = new Random(100);
            int N = rnd.Next(0, 100);

            for (int j = 0; j < N; j++)
                TestArray.Append(j);
            Assert.AreEqual(N, TestArray.Count);
        }

    }
}
