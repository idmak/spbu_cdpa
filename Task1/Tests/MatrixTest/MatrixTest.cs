using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MatrixMultiplier;


namespace MatrixTest
{
    [TestClass]
    public class MatrixTest
    {
        static int[,] table1 = new int[,] { { 1, 2 },
                                            { 3, 4 },
                                            { 0, 1 } };

        static int[,] table2 = new int[,] { { 2, 4, 5 },
                                            { 0, 1, 0 } };

        static int[,] table3 = new int[,] { { 2, 6, 5 },
                                            { 6, 16, 15 },
                                            { 0, 1, 0 } };

        static Matrix matrix1 = new Matrix(table1); // 3 x 2 matrix
        static Matrix matrix2 = new Matrix(table2); // 2 x 3 matrix
        static Matrix matrix3 = new Matrix(table3); // 3 x 3 result matrix

        [TestMethod]
        public void GetBodyTest()
        {
            Assert.AreEqual(table1, matrix1.GetBody());
        }

        [TestMethod]
        public void MultiplicationTest()
        {
            Assert.AreEqual(matrix1 * matrix2, matrix3);
        }

        [TestMethod]
        public void InvaldMatrixMultiplicationTest()
        {
            Assert.ThrowsException<ArgumentException>(delegate { Matrix result = matrix1 * matrix3; });
        }
    }
}
