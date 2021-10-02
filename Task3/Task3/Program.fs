open System
open Argu
open FsToolkit.ErrorHandling
open System.IO
open Task3

type HandledType = 
    | Integer
    | Boolean
    | Real
    | ExtendedReal

type CliArguments =
    | [<Mandatory; Unique; AltCommandLine("-t")>] Type of HandledType
    | [<Mandatory; Unique; AltCommandLine("-n")>] Size of int
    | [<Mandatory; Unique; AltCommandLine("-p")>] Path of string
    | [<Mandatory; Unique; AltCommandLine("-k")>] Count of int
    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Type _ -> "specify type of matrix elements"
            | Size _ -> "specify a matrix size"
            | Path _ -> "specify path to target output directory"
            | Count _ -> "specify amount of matrices to create"

let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some ConsoleColor.Red)
let parser = ArgumentParser.Create<CliArguments>(programName = "Task3", errorHandler = errorHandler)

let validateSize (size: int) = 
    if size > 0 then
        Ok size
    else
        Error ["Size error. Size should be > 0"]

let validateCount (count: int) = 
    if count > 0 then
        Ok count
    else
        Error ["Count error. Count should be > 0"]

[<EntryPoint>]
let main argv =
    let args = parser.ParseCommandLine argv

    let matrixType = args.GetResult Type
    let size = args.GetResult Size
    let path = args.GetResult Path
    let count = args.GetResult Count

    let validationResult = validation {
        let! _ = validateSize size
        let! _ = validateCount count
        return ()
    }    

    match validationResult with
    | Ok () -> 
        try 
            let pathToOutputDirectory = Path.GetFullPath <| Path.Combine [| path; sprintf "%A" matrixType; sprintf "%i" size |]
            pathToOutputDirectory |> Directory.CreateDirectory |> ignore

            seq { 1 .. count }
            |> Seq.iter (fun i ->
                let fullPathToOutputFile = Path.Combine [| pathToOutputDirectory; sprintf "%i.txt" i |]

                match matrixType with
                | Integer -> 
                    let matrix = Matrix.createRandom size RandomGenerators.getNextInteger
                    matrix |> Matrix.writeToFile fullPathToOutputFile (sprintf "%i")
                    
                | Boolean -> 
                    let matrix = Matrix.createRandom size RandomGenerators.getNextBoolean
                    matrix |> Matrix.writeToFile fullPathToOutputFile (sprintf "%b")

                | HandledType.Real -> 
                    let matrix = Matrix.createRandom size RandomGenerators.getNextReal
                    matrix |> Matrix.writeToFile fullPathToOutputFile (sprintf "%f")

                | ExtendedReal -> 
                    let matrix = Matrix.createRandom size RandomGenerators.getNextExtendedReal
                    let convertToString value = 
                        match value with
                        | Indeterminacy -> "nan"
                        | Infinity -> "inf"
                        | ExtendedReal.Real v -> sprintf "%f" v

                    matrix |> Matrix.writeToFile fullPathToOutputFile convertToString
            )
            printfn "Matrices generated at %s" pathToOutputDirectory

        with exn -> printfn "%s" exn.Message
    | Error e -> printf "%A" e

    0