using System;
using QuickGraph;
using MatrixMultiplier;

namespace PathFinder
{
    public static class GraphCreator
    {
        public static (AdjacencyGraph<int, Edge<int>>, Func<Edge<int>, double>) Create(Matrix MyMatrix)
        {
            AdjacencyGraph<int, Edge<int>> graph = new AdjacencyGraph<int, Edge<int>>();

            for (int i = 0; i < MyMatrix.Height; i++)
                for (int j = 0; j < MyMatrix.Width; j++)
                    if (MyMatrix[i, j] > 0)
                        graph.AddVerticesAndEdge(new Edge<int>(i, j));

            Func<Edge<int>, double> costFun = edge => MyMatrix[edge.Source, edge.Target];

            return (graph, costFun);
        }
    }
}
