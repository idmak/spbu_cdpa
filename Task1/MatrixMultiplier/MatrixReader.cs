using System;
using System.IO;

namespace MatrixMultiplier
{
    public static class MatrixReader
    {

        public static Matrix Read(string Filename)
        {
                string[] lines = File.ReadAllLines(Filename);
                string[] line;
                int[,] array = new int[lines.Length, lines[0].Split().Length];
                for (int i = 0; i < lines[0].Split().Length; i++)
                {
                    line = lines[i].Split();
                    for (int j = 0; j < line.Length; j++)
                        array[i, j] = int.Parse(line[j]);
                }
                if (array != null)
                    return new Matrix(array);
                else throw new Exception("No data");
        }
    }
}
