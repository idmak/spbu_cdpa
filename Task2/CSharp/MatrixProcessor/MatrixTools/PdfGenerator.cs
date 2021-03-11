using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace MatrixProcessor
{
    public static class PdfGenerator
    {
        public static void Generate(Matrix<Boolean> origin, Matrix<Boolean> result, String output)
        {
            List<String> lines = new List<String>() { "digraph G {" };
            for (int i = 0; i < result.Height; i++)
                lines.Add(i.ToString() + " ;");
            for (int i = 0; i < result.Height; i++)
                for (int j = 0; j < result.Width; j++)
                    if (result[i, j].Value)
                        lines.Add(i.ToString() + " -> " + j.ToString() + " [" + (origin[i, j].Value ? "" : "color=red") + "];");
            lines.Add("}");
            File.WriteAllLines(output+".dot", lines);

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "dot";
                process.StartInfo.Arguments = "-Tpdf " + output + ".dot" + " -o " + output + ".pdf";
                process.Start();
                process.WaitForExit();
            }
            File.Delete(output + ".dot");

            
        }
    }
}
