using System;
using System.IO;
using System.Net.Http.Headers;

namespace MatrixProcessor
{
    public static class MatrixReader<T> 
        where T: class, ISerializable, ISemiRing, new()
    {
        public static Matrix<T> Read(string Filename)
        {
                string[] lines = File.ReadAllLines(Filename);
                string[] line;
                Matrix<T> result = new Matrix<T>(lines.Length, lines[0].Split().Length);
                for (int i = 0; i < lines.Length; i++)
                {
                    line = lines[i].Split();
                    for (int j = 0; j < line.Length; j++)
                        result[i, j].FromWord(line[j]);
                }
                if (result != null)
                    return result;
                else throw new Exception("No data");
        }
    }
}
