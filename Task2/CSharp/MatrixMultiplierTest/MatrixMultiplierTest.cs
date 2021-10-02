using Microsoft.VisualStudio.TestTools.UnitTesting;
using MatrixProcessor;
using System.Globalization;

namespace MatrixMultiplierTest
{
    [TestClass]
    public class MatrixMultiplierTest
    {
        [TestMethod]
        public void NaturalMatrixMultiplicationTest()
        {
            uint[,] table1 = new uint[,] { { 1, 2 },
                                          { 3, 4 },
                                          { 0, 1 } };

            uint[,] table2 = new uint[,] { { 2, 4, 5 },
                                         { 0, 1, 0 } };

            uint[,] table3 = new uint[,] { { 2, 6, 5 },
                                         { 6, 16, 15 },
                                         { 0, 1, 0 } };

            Matrix<NaturalNumber> factor1 = new Matrix<NaturalNumber>(3,2);
            Matrix<NaturalNumber> factor2 = new Matrix<NaturalNumber>(2,3);
            Matrix<NaturalNumber> factor3 = new Matrix<NaturalNumber>(3,3);

            for (int i = 0; i < factor1.Height; i++)
                for (int j = 0; j < factor1.Width; j++)
                    factor1[i, j] = new NaturalNumber(table1[i, j]);

            for(int i = 0; i < factor2.Height; i++)
                for (int j = 0; j < factor2.Width; j++)
                factor2[i, j] = new NaturalNumber(table2[i, j]);

            for(int i = 0; i < factor3.Height; i++)
                for (int j = 0; j < factor3.Width; j++)
                factor3[i, j] = new NaturalNumber(table3[i, j]);

            Assert.AreEqual(factor3, factor1 * factor2);
        }

        [TestMethod]
        public void BooleanMatrixMultiplicationTest()
        {
            bool[,] table1 = new bool[,] { { true, false },
                                           { false, true },
                                           { true, true} };

            bool[,] table2 = new bool[,] { { false, false, true },
                                           { true, true, true } };

            bool[,] table3 = new bool[,] { { false, false, true },
                                           { true, true, true },
                                           { true, true, true } };

            Matrix<Boolean> factor1 = new Matrix<Boolean>(3, 2);
            Matrix<Boolean> factor2 = new Matrix<Boolean>(2, 3);
            Matrix<Boolean> factor3 = new Matrix<Boolean>(3, 3);

            for (int i = 0; i < factor1.Height; i++)
                for (int j = 0; j < factor1.Width; j++)
                    factor1[i, j] = new Boolean(table1[i, j]);

            for (int i = 0; i < factor2.Height; i++)
                for (int j = 0; j < factor2.Width; j++)
                    factor2[i, j] = new Boolean(table2[i, j]);

            for (int i = 0; i < factor3.Height; i++)
                for (int j = 0; j < factor3.Width; j++)
                    factor3[i, j] = new Boolean(table3[i, j]);

            Assert.AreEqual(factor3, factor1 * factor2);
        }
    }
}
