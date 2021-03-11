using System;

namespace MatrixMultiplier
{
    public class Matrix
    {
        private int[,] array;
        public int Height => array.GetLength(0);
        public int Width => array.GetLength(1);
        public int this[int index1, int index2] { get { return array[index1, index2]; } set { array[index1, index2] = value; } }

        public Matrix(int Height, int Width)
        {
            array = new int[Height, Width];
        }

        public Matrix(int[,] Array)
        {
            this.array = Array;
        }

        public static Matrix operator *(Matrix operand1, Matrix operand2)
        {
            if (operand1.Width != operand2.Height)
                throw new ArgumentException("Impossible to multiply");
                Matrix result = new Matrix(operand1.Height, operand2.Width);
                for (int i = 0; i < operand1.Height; i++)
                    for (int j = 0; j < operand2.Width; j++)
                        for (int t = 0; t < operand1.Width; t++)
                            result[i, j] += operand1[i, t] * operand2[t, j];
                return result;
        }

        public static Matrix operator +(Matrix operand1, Matrix operand2)
        {
            if (operand1.Width != operand2.Width || operand1.Height != operand2.Width)
                throw new ArgumentException("Impossible to add");
                Matrix result = new Matrix(operand1.Width, operand1.Height);
                for (int i = 0; i < operand1.Height; i++)
                    for (int j = 0; j < operand1.Width; j++)a
                        result[i, j] = operand1[i, j] + operand2[i, j];
                return result;
        }

        public int[,] GetBody() => array;

        public override bool Equals(object obj)
        {
            Matrix objToCompare = obj as Matrix;
            if (objToCompare.Width != this.Width || objToCompare.Height != this.Height)
                return false;
            for (int i = 0; i < objToCompare.Height; i++)
                for (int j = 0; j < objToCompare.Width; j++)
                    if (this.array[i, j] != objToCompare[i, j])
                        return false;
            return true;
        }

        public override int GetHashCode()
        {
            return array.GetHashCode();
        }

        public override string ToString()
        {
            string result = "";
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                    result += array[i, j] + " ";
                result += '\n';
            }
            return result;
        }
    }
}
