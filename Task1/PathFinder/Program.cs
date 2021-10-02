using System;
using System.Collections.Generic;
using QuickGraph;
using QuickGraph.Algorithms;
using MatrixMultiplier;

namespace PathFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                String path = args[0];
                (IVertexAndEdgeListGraph<int, Edge<int>> graph,
                    Func<Edge<int>, double> edgeCost) = GraphCreator.Create(MatrixReader.Read(path));
                TryFunc<int, IEnumerable<Edge<int>>> tryGetPath = graph.ShortestPathsDijkstra(edgeCost, 0);
                PdfGenerator.Generate(DotGenerator<int,Edge<int>>.GetDotCode(graph, tryGetPath), args[1]);
            }
            catch (IndexOutOfRangeException)
            {
                Console.Error.WriteLine("Path to graph matrix must be given");
            }
        }
    }
}
