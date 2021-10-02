using System;

namespace MatrixMultiplier
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string path1 = args[0];
                string path2 = args[1];
                Matrix factor1 = MatrixReader.Read(path1);
                Matrix factor2 = MatrixReader.Read(path2);
                Console.WriteLine(factor1 * factor2);
            }
            catch (IndexOutOfRangeException)
            {
                Console.Error.WriteLine("paths to factors must be given");
            }
        }
    }
}
