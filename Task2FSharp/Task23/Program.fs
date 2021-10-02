namespace Task23

open System
open Argu
open FsToolkit.ErrorHandling
open System.IO
open Task21

module Program = 
    type HandledType = 
        | Integer 
        | Float

    type CliArguments =
        | [<Mandatory; Unique; AltCommandLine("-t")>] Type of HandledType
        | [<Mandatory; Unique; AltCommandLine("-p")>] Input of string
        | [<Mandatory; Unique; AltCommandLine("-o")>] Output of string
        interface IArgParserTemplate with
            member this.Usage =
                match this with
                | Type _ -> "specify type of input matrix"
                | Input _ -> "specify path to input matrix"
                | Output _ -> "specify path to output matrix"

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

        let matrixType = args.GetResult Type
        let input = args.GetResult Input
        let output = args.GetResult Output

        let validationResult = validation {
            return! validateInputPath input
        } 

        match validationResult with
        | Ok _ -> 
            try
                match matrixType with
                | Integer -> 
                    let matrix = Matrix.readFromFile input IntegerMinPlusSemiring.FromString
                    let distances = Matrix.findDistanceMatrix matrix
                    distances |> Matrix.writeToFile output
                
                | Float -> 
                    let matrix = Matrix.readFromFile input FloatMinPlusSemiring.FromString
                    let distances = Matrix.findDistanceMatrix matrix
                    distances |> Matrix.writeToFile output

            with exn -> printfn "%s" exn.Message
        | Error e -> printf "%A" e

        0