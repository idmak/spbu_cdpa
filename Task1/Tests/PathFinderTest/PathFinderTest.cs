using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using QuickGraph;
using QuickGraph.Algorithms;
using PathFinder;
using MatrixMultiplier;


namespace PathFinderTest
{
    [TestClass]
    public class PathFinderTest
    {
        public IVertexAndEdgeListGraph<int, Edge<int>> graph;
        public Func<Edge<int>, double> edgeCost;

        private bool graphContainsEdge(IVertexAndEdgeListGraph<int, Edge<int>> graph, Edge<int> edge)
        {
            foreach (Edge<int> e in graph.Edges)
                if (e.Source == edge.Source && e.Target == edge.Target)
                    return true;
            return false;
        }

        [TestMethod]
        public void graphCreationTest()
        {
            Matrix matrix = new Matrix(new int[,] { { 1, 2, 3 },
                                                    { 0, 3, 0},
                                                    {3, 0, 1 } });
            (this.graph, this.edgeCost) = GraphCreator.Create(matrix);
            for (int i = 0; i < matrix.Height; i++)
            {
                for (int j = 0; j < matrix.Width; j++)
                {
                    if (matrix[i, j] > 0)
                    {
                        Edge<int> edge = new Edge<int>(i, j);
                        Assert.IsTrue(graphContainsEdge(this.graph, edge) && this.edgeCost(edge) == matrix[i, j]);
                    }
                    else
                        Assert.IsFalse(graphContainsEdge(this.graph, new Edge<int>(i, j)));                    
                }
            }
        }

        [TestMethod]
        public void PDFCreatorTest()
        {
            Matrix matrix = new Matrix(new int[,] { { 0, 1, 2, 3 },
                                                    { 1, 0, 1, 2 }, 
                                                    { 2, 1, 0, 4 },
                                                    { 3, 2, 4, 0 } } );
            (this.graph, this.edgeCost) = GraphCreator.Create(matrix);
            TryFunc<int, IEnumerable<Edge<int>>> tryGetPath = graph.ShortestPathsDijkstra(this.edgeCost, 0);
            PdfGenerator.Generate(DotGenerator<int, Edge<int>>.GetDotCode(this.graph, tryGetPath), "test");
            Assert.IsTrue(File.Exists("test.pdf"));
            File.Delete("test.pdf");
        }
    }
}
