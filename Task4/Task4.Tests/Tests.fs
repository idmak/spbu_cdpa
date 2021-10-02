module Tests

open Expecto
open System
open System.IO
open Task4
open Expecto.Logging
open Expecto.Logging.Message

let creationTest = testCase "create -> write should make non-empty file" <| fun () ->
    let filename = Path.Combine [| __SOURCE_DIRECTORY__; "random.txt" |]
    try
        let commands = 
            [
                Create {| VarName = "A"; MatrixType = Integer; Size = 5 |}
                Write {| VarName = "A"; FullPathToOutputFile = filename |}
            ]
        
        let dispatcher = Actors.dispatcher ()
        commands |> List.iter (fun cmd -> dispatcher.Post <| UserRequest cmd)
        dispatcher.PostAndReply (fun rc -> UserRequest <| WaitAllAndExit rc) |> ignore

        Expect.isTrue (File.Exists filename) "File should exists"
        Expect.isNotEmpty (File.ReadAllText filename) "File shouldn`t be empty"

    finally
        if File.Exists filename then File.Delete filename

let deathTest = testCase "create -> exit -> write shouldn`t create file" <| fun () ->
    let filename = Path.Combine [| __SOURCE_DIRECTORY__; "death.txt" |]
    try
        let createCmd = Create {| VarName = "A"; MatrixType = Integer; Size = 5 |}
        let writeCmd = Write {| VarName = "A"; FullPathToOutputFile = filename |}
        
        let dispatcher = Actors.dispatcher ()
        dispatcher.Post <| UserRequest createCmd
        dispatcher.PostAndReply (fun rc -> UserRequest <| WaitAllAndExit rc) |> ignore
        dispatcher.Post <| UserRequest writeCmd

        Expect.isTrue (not <| File.Exists filename) "File shouldn`t exists"

    finally
        if File.Exists filename then File.Delete filename

let logger = Log.create "simple"

let multiplicationTest = testCase "read x2 -> multiply -> write should make correct matrix" <| fun () ->
    let matrixA = Path.Combine [| __SOURCE_DIRECTORY__; "matrixA.txt" |]
    let matrixB = Path.Combine [| __SOURCE_DIRECTORY__; "matrixB.txt" |]
    let multExpected = Path.Combine [| __SOURCE_DIRECTORY__; "multResult.txt" |]
    let filename = Path.Combine [| __SOURCE_DIRECTORY__; "mult.txt" |]
    try
        let commands = 
            [
                Read {| VarName = "A"; MatrixType = Integer; FullPathToMatrix = matrixA |}
                Read {| VarName = "B"; MatrixType = Integer; FullPathToMatrix = matrixB |}
                Multiply {| LeftVarName = "A"; RightVarName = "B"; ResultVarName = "C" |}
                Write {| VarName = "C"; FullPathToOutputFile = filename |}
            ]
        
        let dispatcher = Actors.dispatcher ()
        commands |> List.iter (fun cmd -> dispatcher.Post <| UserRequest cmd)
        dispatcher.PostAndReply (fun rc -> UserRequest <| WaitAllAndExit rc) |> ignore

        Expect.isTrue (File.Exists filename) "File should exists"
        let (Matrix matrixActual) = Matrix.readFromFile filename (int >> IntegerSemiring)
        let (Matrix matrixExpected) = Matrix.readFromFile multExpected (int >> IntegerSemiring)

        "Actual and expected matrix should be equal"
        |> Expect.sequenceEqual
            (matrixActual |> Array2D.map IntegerSemiring.Unwrap |> Seq.cast<int>)
            (matrixExpected |> Array2D.map IntegerSemiring.Unwrap |> Seq.cast<int>)

    finally
        if File.Exists filename then File.Delete filename

let trcTest = testCase "read -> trc -> write should make correct matrix" <| fun () ->
    let matrixA = Path.Combine [| __SOURCE_DIRECTORY__; "matrixA.txt" |]
    let trcExpected = Path.Combine [| __SOURCE_DIRECTORY__; "trcResult.txt" |]
    let filename = Path.Combine [| __SOURCE_DIRECTORY__; "trc.txt" |]
    try
        let commands = 
            [
                Read {| VarName = "A"; MatrixType = Integer; FullPathToMatrix = matrixA |}
                FindTrc {| VarName = "A"; ResultVarName = "B" |}
                Write {| VarName = "B"; FullPathToOutputFile = filename |}
            ]
        
        let dispatcher = Actors.dispatcher ()
        commands |> List.iter (fun cmd -> dispatcher.Post <| UserRequest cmd)
        dispatcher.PostAndReply (fun rc -> UserRequest <| WaitAllAndExit rc) |> ignore

        Expect.isTrue (File.Exists filename) "File should exists"
        let (Matrix matrixActual) = Matrix.readFromFile filename ((fun (str: string) -> System.Boolean.Parse str) >> BooleanSemiring)
        let (Matrix matrixExpected) = Matrix.readFromFile trcExpected ((fun (str: string) -> System.Boolean.Parse str) >> BooleanSemiring)

        "Actual and expected matrix should be equal"
        |> Expect.sequenceEqual
            (matrixActual |> Array2D.map BooleanSemiring.Unwrap |> Seq.cast<bool>)
            (matrixExpected |> Array2D.map BooleanSemiring.Unwrap |> Seq.cast<bool>)

    finally
        if File.Exists filename then File.Delete filename

let tryingBindEqualVars = testCase "read A -> read A -> write should write correct matrix" <| fun () ->
    let matrixA = Path.Combine [| __SOURCE_DIRECTORY__; "matrixA.txt" |]
    let matrixB = Path.Combine [| __SOURCE_DIRECTORY__; "matrixB.txt" |]
    let filename = Path.Combine [| __SOURCE_DIRECTORY__; "bind.txt" |]
    try
        let commands = 
            [
                Read {| VarName = "A"; MatrixType = Integer; FullPathToMatrix = matrixA |}
                Read {| VarName = "A"; MatrixType = Integer; FullPathToMatrix = matrixB |}
                Write {| VarName = "A"; FullPathToOutputFile = filename |}
            ]
        
        let dispatcher = Actors.dispatcher ()
        commands |> List.iter (fun cmd -> dispatcher.Post <| UserRequest cmd)
        dispatcher.PostAndReply (fun rc -> UserRequest <| WaitAllAndExit rc) |> ignore

        Expect.isTrue (File.Exists filename) "File should exists"
        let (Matrix matrixActual) = Matrix.readFromFile filename (int >> IntegerSemiring)
        let (Matrix matrixExpected) = Matrix.readFromFile matrixA (int >> IntegerSemiring)

        "Actual and expected matrix should be equal"
        |> Expect.sequenceEqual
            (matrixActual |> Array2D.map IntegerSemiring.Unwrap |> Seq.cast<int>)
            (matrixExpected |> Array2D.map IntegerSemiring.Unwrap |> Seq.cast<int>)

    finally
        if File.Exists filename then File.Delete filename

let tryingReadNonBindedVar = testCase "write A should not write matrix, cause A not exists" <| fun () ->
    let filename = Path.Combine [| __SOURCE_DIRECTORY__; "write.txt" |]
    try
        let commands = 
            [
                Write {| VarName = "A"; FullPathToOutputFile = filename |}
            ]
        
        let dispatcher = Actors.dispatcher ()
        commands |> List.iter (fun cmd -> dispatcher.Post <| UserRequest cmd)
        dispatcher.PostAndReply (fun rc -> UserRequest <| WaitAllAndExit rc) |> ignore

        Expect.isTrue (not <| File.Exists filename) "File should not exists"

    finally
        if File.Exists filename then File.Delete filename

let stressTest = testCase "stressTest" <| fun () ->
    let matrixA = Path.Combine [| __SOURCE_DIRECTORY__; "matrixA.txt" |]
    let matrixB = Path.Combine [| __SOURCE_DIRECTORY__; "matrixB.txt" |]
    let multExpected = Path.Combine [| __SOURCE_DIRECTORY__; "multResult.txt" |]
    let filename = Path.Combine [| __SOURCE_DIRECTORY__; "stress.txt" |]
    try
        let commands = seq {
            for i = 1 to 100 do
                yield Create {| VarName = sprintf "A%i" i; MatrixType = Integer; Size = 5 |}
                yield Create {| VarName = sprintf "B%i" i; MatrixType = Integer; Size = 5 |}
                yield Multiply {| LeftVarName = sprintf "A%i" i; RightVarName = sprintf "B%i" i; ResultVarName = sprintf "%i" i |}
            yield Write {| VarName = "100"; FullPathToOutputFile = filename |}
        }
        
        let dispatcher = Actors.dispatcher ()
        commands |> Seq.iter (fun cmd -> dispatcher.Post <| UserRequest cmd)
        dispatcher.PostAndReply (fun rc -> UserRequest <| WaitAllAndExit rc) |> ignore

        Expect.isTrue (File.Exists filename) "File should exists"
        Expect.isNotEmpty (File.ReadAllText filename) "File shouldn`t be empty"

    finally
        if File.Exists filename then File.Delete filename

[<Tests>]
let tests =
    testList "Task4, actors tests" [
        creationTest
        deathTest
        multiplicationTest
        trcTest
        tryingBindEqualVars
        tryingReadNonBindedVar
        stressTest
    ]