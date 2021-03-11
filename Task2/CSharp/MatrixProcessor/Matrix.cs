using System;
using System.Collections.Generic;
using System.Text;

namespace MatrixProcessor
{
    public class Matrix<T>
        where T: class, ISemiRing, new()
    {
        private T[,] array;
        public int Height => array.GetLength(0);
        public int Width => array.GetLength(1);
        
        public T this[int index1, int index2] { get { return array[index1, index2]; } set { array[index1, index2] = value; } }

        public Matrix(int Height, int Width)
        {
            array = new T[Height, Width];
            for (int i = 0; i <= array.GetUpperBound(0); i++)
                for (int j = 0; j <= array.GetUpperBound(1); j++)
                    array[i, j] = new T();
        }

        public Matrix(T[,] Array)
        {
            this.array = Array;
        }

        public static Matrix<T> operator *(Matrix<T> operand1, Matrix<T> operand2)
        {
            if (operand1.Width != operand2.Height)
                throw new ArgumentException("Impossible to multiply");
            else
            {
                Matrix<T> result = new Matrix<T>(operand1.Height, operand2.Width);
                for (int i = 0; i < operand1.Height; i++)
                    for (int j = 0; j < operand2.Width; j++)
                        for (int t = 0; t < operand1.Width; t++)
                            result[i,j] = result[i, j].Add(operand1[i, t].Multiply(operand2[t, j])) as T;
                return result;
            }
        }
        public T[,] GetBody() => array;

        public override int GetHashCode() => array.GetHashCode();

        public override bool Equals(object obj)
        {
            Matrix<T> objToCompare = obj as Matrix<T>;

            if (objToCompare.Width != this.Width || objToCompare.Height != this.Height)
                return false;
            for (int i = 0; i < objToCompare.Height; i++)
                for (int j = 0; j < objToCompare.Width; j++)
                    if (!this.array[i, j].Equals(objToCompare[i, j]))
                        return false;
            return true;
        }
    }
}
