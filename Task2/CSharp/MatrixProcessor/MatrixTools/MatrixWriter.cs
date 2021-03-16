using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MatrixProcessor.MatrixTools
{
    public static class MatrixWriter<T> 
        where T: class, ISerializable, ISemiRing, new()
    {
        public static void Write(Matrix<T> matrix, String output)
        {
            List<String> lines = new List<String>();
            List<String> line = new List<String>();
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < matrix.Height; i++)
            {
                line.Clear();
                builder.Clear();
                for (int j = 0; j < matrix.Width; j++)
                    line.Add(matrix[i,j].ToWord());
                builder.AppendJoin(' ', line);
                lines.Add(builder.ToString());
            }
            File.WriteAllLines(output+".txt", lines);
        }
    }
}
