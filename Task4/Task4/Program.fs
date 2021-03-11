open System
open FsToolkit.ErrorHandling
open Task4
open System.IO

let validateType matrixType = 
    match matrixType with
    | "integer" -> Ok HandledType.Integer
    | "boolean" -> Ok HandledType.Boolean
    | "real" -> Ok HandledType.Real
    | "extended" -> Ok HandledType.ExtendedReal
    | _ -> Error ["Type error. Type should be one of the following: integer, boolean, real, extended"]

let validateSize (size: string) = 
    let mutable number = 0
    if Int32.TryParse(size, &number) && number > 0 then
        Ok number
    else
        Error ["Size error. Size should be number > 0"]

let validatePath filename = 
    try
        let path = Path.Combine [| __SOURCE_DIRECTORY__; filename |]
        Ok <| Path.GetFullPath path
    with
    | :? ArgumentException -> Error ["Path error"]

let userCommandsHelper = 
    """
    1) create <varname> <type> <size> -- create random square matrix of type [integer|boolean|real|extended]
    2) read <varname> <type> <pathToFile> -- read matrix of defined type from file
    3) write <varname> <pathToFile> -- write matrix to file
    4) multiply <result> <left> <right> -- multiply 2 matrix in parallel in bind the result 
    5) trc <result> <matrix> -- find transitive closure of matix and bind the result
    6) exit -- wait all tasks and exit
    """

[<EntryPoint>]
let main argv =
    printfn "Start ..."
    printfn "%s" userCommandsHelper

    let cts = new Threading.CancellationTokenSource()
    let dispatcher = Actors.dispatcher ()

    let rec mainloop () = async {
        let input = Console.ReadLine().Trim()
        match input.Split " " with
        | [| "create"; varName; matrixType; size |] -> 
            let validatedCmd = validation {
                let! matrixType = matrixType |> validateType
                let! size = size |> validateSize
                return {| VarName = varName; MatrixType = matrixType; Size = size |}
            }
            
            match validatedCmd with
            | Ok cmd -> dispatcher.Post <| UserRequest (Create cmd)
            | Error e -> printfn "%s -> %A" input e

        | [| "read"; varName; matrixType; filename |] -> 
            let validatedCmd = validation {
                let! matrixType = matrixType |> validateType
                let! fullPathToMatrix = filename |> validatePath
                return {| VarName = varName; MatrixType = matrixType; FullPathToMatrix = fullPathToMatrix |}
            }

            match validatedCmd with
            | Ok cmd -> dispatcher.Post <| UserRequest (Read cmd)
            | Error e -> printfn "%s -> %A" input e

        | [| "write"; varName; filename |] ->
            let validatedCmd = validation {
                let! fullPathToOutputFile = filename |> validatePath
                return {| VarName = varName; FullPathToOutputFile = fullPathToOutputFile |}
            }

            match validatedCmd with
            | Ok cmd -> dispatcher.Post <| UserRequest (Write cmd)
            | Error e -> printfn "%s -> %A" input e

        | [| "multiply"; result; left; right |] -> 
            let cmd = {| LeftVarName = left; RightVarName = right; ResultVarName = result |}
            dispatcher.Post <| UserRequest (Multiply cmd)

        | [| "trc"; result; matrix |] -> 
            let cmd = {| VarName = matrix; ResultVarName = result |}
            dispatcher.Post <| UserRequest (FindTrc cmd)
        
        | [| "exit" |] -> 
            dispatcher.PostAndReply(fun replyChannel -> UserRequest <| WaitAllAndExit replyChannel)
            |> List.iter (printfn "%s")

            cts.Cancel()

        | _ -> printfn "%s -> %s" input "Invalid command"
            
        return! mainloop ()
    }

    Async.StartImmediate(mainloop (), cts.Token) 
    0
