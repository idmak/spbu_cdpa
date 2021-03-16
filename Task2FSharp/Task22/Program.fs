namespace Task22

open System
open Argu
open FsToolkit.ErrorHandling
open System.IO
open Task21

module Program = 
    type CliArguments =
        | [<Mandatory; Unique; AltCommandLine("-p")>] Input of string
        | [<Mandatory; Unique; AltCommandLine("-o")>] Output of string
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Input _ -> "specify path to input matrix"
                | Output _ -> "specify path to output directory"
    
    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
    let parser = ArgumentParser.Create<CliArguments>(programName = "Task23", errorHandler = errorHandler)

    let validateInputPath (path: string) = 
        if File.Exists path then
            Ok path
        else 
            Error ["Path error. Input file should exist"]

    [<EntryPoint>]
    let main argv =
        let args = parser.ParseCommandLine argv

        let inputMatrix = args.GetResult Input
        let outputDir = args.GetResult Output

        let validationResult = validation {
            return! validateInputPath inputMatrix
        } 

        match validationResult with
        | Ok _ -> 
            try
                let (Matrix matrix) = Matrix.readFromFile inputMatrix BooleanSemiring.FromString
                let (Matrix trc) = Matrix matrix |> Matrix.findTransitiveClosure

                let outputDotFile = Path.Combine [| outputDir; Path.GetFileNameWithoutExtension inputMatrix + ".dot" |]
                use writer = new StreamWriter(outputDotFile)

                fprintfn writer "digraph G {"
                Array2D.iteri (fun i j v ->
                    if BooleanSemiring.Unwrap v then
                        fprintfn writer "\t%i -> %i;" i j
                    elif BooleanSemiring.Unwrap trc.[i, j] then
                        fprintfn writer "\t%i -> %i  [color=red];" i j
                ) matrix
                fprintfn writer "}"

                let resultPath = DotProcessor.exportToPdf outputDotFile outputDir
                printfn "PDF exported to %s" resultPath

            with exn -> printfn "%s" exn.Message
        | Error e -> printf "%A" e

        0