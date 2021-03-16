using System;

namespace MatrixProcessor
{
    public static class FloydWarshallProcessor<T>
        where T: class, ISemiRing, IOrder, ISerializable, new()
    {
        public static Matrix<T> Process(Matrix<T> Matrix)
        {
            Matrix<T> result = new Matrix<T>(Matrix.GetBody());

            for (int i = 0; i < Matrix.Height; i++)
                for (int j = 0; j < Matrix.Width; j++)
                    for (int k = 0; k < Matrix.Height; k++)
                    {
                        T item = Matrix[i, k].Add(Matrix[k, j]) as T;
                        result[i, j] = result[i, j].LessOrEqual(item) ? result[i, j] : item;
                    }
            return result;
        }
    }
}
