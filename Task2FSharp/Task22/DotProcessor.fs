namespace Task22

open System.IO
open System.Diagnostics

module DotProcessor = 
    let exportToPdf (pathToDot: string) (pathToOutputDir: string) = 
        let filename = Path.GetFileNameWithoutExtension pathToDot
        let outputFile = Path.GetFullPath <| Path.Combine [| pathToOutputDir; filename + ".pdf" |]
        
        let pInfo = new ProcessStartInfo()
        pInfo.FileName <- "dot"
        pInfo.Arguments  <- sprintf "-Tpdf -o %s %s" outputFile pathToDot
        use p = Process.Start(pInfo)
        p.WaitForExit(0) |> ignore
        outputFile
