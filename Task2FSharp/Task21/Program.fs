namespace Task21

open System
open Argu
open FsToolkit.ErrorHandling
open System.IO
open Task21

module Program = 
    type HandledType = 
        | Integer 
        | Boolean
        | Real
        | ExtendedReal

    type CliArguments =
        | [<Mandatory; Unique; AltCommandLine("-t")>] Type of HandledType
        | [<Mandatory; Unique; AltCommandLine("-p")>] Paths of string list
        | [<Mandatory; Unique; AltCommandLine("-o")>] Output of string
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Type _ -> "specify type of matrices"
                | Paths _ -> "specify paths to input matrices"
                | Output _ -> "specify path to output matrix"

    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
    let parser = ArgumentParser.Create<CliArguments>(programName = "Task21", errorHandler = errorHandler)

    let validatePathsList (paths: string list) = 
        if paths.Length = 2 then
            if File.Exists paths.[0] && File.Exists paths.[0] then
                Ok (paths.[0], paths.[1])
            else 
                Error ["Paths error. Both files should exist"]
        else
            Error ["Paths error. Paths should be a list of length 2"]

    [<EntryPoint>]
    let main argv =
        let args = parser.ParseCommandLine argv

        let matrixType = args.GetResult Type
        let paths = args.GetResult Paths
        let output = args.GetResult Output

        let validationResult = validation {
            return! validatePathsList paths
        } 

        match validationResult with
        | Ok (leftMatrixPath, rightMatrixPath) -> 
            try
                match matrixType with
                | Integer -> 
                    let matrixA = Matrix.readFromFile leftMatrixPath IntegerSemiring.FromString
                    let matrixB = Matrix.readFromFile rightMatrixPath IntegerSemiring.FromString
                    let result = Matrix.multiplyInParallel matrixA matrixB
                    result |> Matrix.writeToFile output
                    
                | Boolean -> 
                    let matrixA = Matrix.readFromFile leftMatrixPath BooleanSemiring.FromString
                    let matrixB = Matrix.readFromFile rightMatrixPath BooleanSemiring.FromString
                    let result = Matrix.multiplyInParallel matrixA matrixB
                    result |> Matrix.writeToFile output

                | HandledType.Real -> 
                    let matrixA = Matrix.readFromFile leftMatrixPath RealSemiring.FromString
                    let matrixB = Matrix.readFromFile rightMatrixPath RealSemiring.FromString
                    let result = Matrix.multiplyInParallel matrixA matrixB
                    result |> Matrix.writeToFile output

                | ExtendedReal -> 
                    let matrixA = Matrix.readFromFile leftMatrixPath ExtendedRealSemiring.FromString
                    let matrixB = Matrix.readFromFile rightMatrixPath ExtendedRealSemiring.FromString
                    let result = Matrix.multiplyInParallel matrixA matrixB
                    result |> Matrix.writeToFile output

            with exn -> printfn "%s" exn.Message
        | Error e -> printf "%A" e

        0