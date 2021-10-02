using System;
using System.Collections.Generic;
using System.Text;
using QuickGraph;

namespace PathFinder
{
    public static class DotGenerator<TVertex, TEdge>
        where TEdge: IEdge<TVertex>
    {
        public static String GetDotCode(IVertexAndEdgeListGraph<TVertex, TEdge> graph, TryFunc<TVertex, IEnumerable<TEdge>> tryGetPath)
        {
            HashSet<TEdge> shortestPath = new HashSet<TEdge>();
            IEnumerable<TEdge> edges;

            foreach (TVertex vertex in graph.Vertices)
                if (tryGetPath(vertex, out edges))
                    shortestPath.UnionWith(edges);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("digraph G {");

            foreach (TVertex vertex in graph.Vertices)
            {
                builder.Append(vertex.ToString());
                builder.AppendLine(";");
            }

            foreach (TEdge edge in graph.Edges)
            {
                builder.Append(edge.ToString());
                if (shortestPath.Contains(edge))
                    builder.AppendLine(" [color = red];");
                else
                    builder.AppendLine(" [];");
            }
            builder.AppendLine("}");

            return builder.ToString();
        }
    }
}
