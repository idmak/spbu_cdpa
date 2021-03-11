using System;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace PathFinder
{
    public static class PdfGenerator    {
        public static void Generate(string dot, string FileName)
        {
            String tmp = FileName + ".dot";

            using (StreamWriter writer = File.CreateText(tmp)) 
                writer.Write(dot);
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = "dot";
                process.StartInfo.Arguments = "-Tpdf " + tmp + " -o " + FileName + ".pdf";
                process.Start();
                while (!process.HasExited)
                    process.Refresh();
            }
            File.Delete(tmp);
        }
    }
}
