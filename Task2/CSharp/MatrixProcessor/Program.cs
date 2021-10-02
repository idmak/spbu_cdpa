using MatrixProcessor.MatrixTools;
using Microsoft.VisualBasic;
using System;

namespace MatrixProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            string input1, input2, output;

            Console.WriteLine("Hello! What would you like to do?\n");
            Console.WriteLine("1. Compute Natural Number Matrix Product");
            Console.WriteLine("2. Get a Transitive Clousure Graph of a Boolean Matrix");
            Console.WriteLine("3. Compute APSP for Natural Matrix");

            switch(Console.ReadLine())
            {
                case "1":
                    Console.WriteLine("Enter paths to the matrixes");
                    input1 = Console.ReadLine();
                    input2 = Console.ReadLine();
                    Console.WriteLine("Enter output filename");
                    output = Console.ReadLine();
                    Matrix<NaturalNumber> m1 = MatrixReader<NaturalNumber>.Read(input1),
                        m2 = MatrixReader<NaturalNumber>.Read(input2);
                    MatrixWriter<NaturalNumber>.Write(m1 * m2, output);
                    break;
                case "2":
                    Console.WriteLine("Enter paths to the matrix");
                    input1 = Console.ReadLine();
                    Console.WriteLine("Enter output filename");
                    output = Console.ReadLine();
                    Matrix<Boolean> boo = MatrixReader<Boolean>.Read(input1);
                    PdfGenerator.Generate(boo,TransitiveClosureProcessor.Process(boo), output);
                    break;
                case "3":
                    Console.WriteLine("Enter paths to the matrix");
                    input1 = Console.ReadLine();
                    Console.WriteLine("Enter output filename");
                    output = Console.ReadLine();
                    Matrix<NaturalNumber> natural = MatrixReader<NaturalNumber>.Read(input1);
                    MatrixWriter<NaturalNumber>.Write(FloydWarshallProcessor<NaturalNumber>.Process(natural), output);
                    break;
            }

            Console.WriteLine(@"output is generated in ..\Task2\CSharp\MatrixProcessor\bin\Debug\netcoreapp3.1");
        }
    }
}
