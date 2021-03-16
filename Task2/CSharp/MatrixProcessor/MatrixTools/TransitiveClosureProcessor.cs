using System;

namespace MatrixProcessor.MatrixTools
{
    public static class TransitiveClosureProcessor
    {
        public static Matrix<Boolean> Process(Matrix<Boolean> matrix)
        {
            Matrix<Boolean> result = new Matrix<Boolean>(matrix.GetBody());

            for (int i = 0; i < matrix.Height; i++)
                for (int j = 0; j < matrix.Width; j++)
                    for (int k = 0; k < matrix.Height; k++)
                        result[i, j] = result[i, j].Add(result[i, k].Multiply(result[k, j])) as Boolean;
            return result;
        }
    }
}
