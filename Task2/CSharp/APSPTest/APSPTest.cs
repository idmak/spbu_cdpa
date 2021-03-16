using Microsoft.VisualStudio.TestTools.UnitTesting;
using MatrixProcessor.MatrixTools;
using MatrixProcessor;
using System;
using Boolean = MatrixProcessor.Boolean;

namespace APSPTest
{
    [TestClass]
    public class APSPTest
    {
        [TestMethod]
        public void FloydWarschallTest()
        {
            uint[,] array0 = new uint[,] { { 0, 1, 4 },
                                        { 1, 0, 2 }, 
                                        { 4, 2, 0 } };

            uint[,] array1 = new uint[,] { { 0, 1, 3 },
                                        { 1, 0, 2 },
                                        { 3, 2, 0 } };

            Matrix<NaturalNumber> origin = new Matrix<NaturalNumber>(3,3);
            Matrix<NaturalNumber> expected = new Matrix<NaturalNumber>(3, 3);

            for (int i = 0; i <= array0.GetUpperBound(0); i++)
                for (int j = 0; j < array0.GetUpperBound(1); j++)
                {
                    origin[i, j] = new NaturalNumber(array0[i, j]);
                    expected[i, j] = new NaturalNumber(array1[i, j]);
                }

            Matrix<NaturalNumber> result = FloydWarshallProcessor<NaturalNumber>.Process(origin);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TransitiveClousureTest()
        {
            bool[,] array0 = new bool[,] { { true, true, false, false },
                                           { false, true, true, false },
                                           { true, false, true, false },
                                           {false, false, true, true } };

            bool[,] array1 = new bool[,] { { true, true, true, false },
                                           { true, true, true, false },
                                           { true, true, true, false },
                                           { true, true, true, true } };

            Matrix<Boolean> origin = new Matrix<Boolean>(4, 4);
            Matrix<Boolean> expected = new Matrix<Boolean>(4, 4);

            for (int i = 0; i < array0.GetLength(0); i++)
                for (int j = 0; j < array0.GetLength(1); j++)
                {
                    origin[i, j] = new Boolean(array0[i, j]);
                    expected[i, j] = new Boolean(array1[i, j]);
                }

            Matrix<Boolean> result = TransitiveClosureProcessor.Process(origin);

            Assert.AreEqual(expected, result);
        }
    }
}
