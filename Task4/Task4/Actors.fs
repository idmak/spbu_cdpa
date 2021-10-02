namespace Task4

open System
open System.Collections.Generic
open FSharp.Core

type UserRequest = 
    | Create of {| VarName: string; MatrixType: HandledType; Size: int |}
    | Read of {| VarName: string; MatrixType: HandledType; FullPathToMatrix: string |}
    | Write of {| VarName: string; FullPathToOutputFile: string |}
    | Multiply of {| LeftVarName: string; RightVarName: string; ResultVarName: string |}
    | FindTrc of {| VarName: string; ResultVarName: string |}
    | WaitAllAndExit of AsyncReplyChannel<string list>

type CommonWorkerCommand = 
    | WaitAllAndDie of AsyncReplyChannel<string>

type CreateCommand = 
    | CreateArgs of {| Key: int; MatrixType: HandledType; Size: int |}
    | CommonWorkerCommand of CommonWorkerCommand

type ReadCommand = 
    | ReadArgs of {| Key: int; MatrixType: HandledType; FullPathToMatrix: string |}
    | CommonWorkerCommand of CommonWorkerCommand

type WriteCommand = 
    | WriteArgs of {| Key: int; MatrixType: HandledType; FullPathToOutputFile: string; Matrix: obj |}
    | CommonWorkerCommand of CommonWorkerCommand

type MultiplyCommand = 
    | MultiplyArgs of {| Key: int; MatrixType: HandledType; LeftMatrix: obj; RightMatrix: obj |}
    | CommonWorkerCommand of CommonWorkerCommand

type FindTrcCommand = 
    | FindTrcArgs of {| Key: int; MatrixType: HandledType; Matrix: obj |}
    | CommonWorkerCommand of CommonWorkerCommand

type WorkerCommand = 
    | CreateCommand of CreateCommand
    | ReadCommand of ReadCommand
    | WriteCommand of WriteCommand
    | MultiplyCommand of MultiplyCommand
    | FindTrcCommand of FindTrcCommand

type Workers = {
    Creator: MailboxProcessor<CreateCommand>
    Reader: MailboxProcessor<ReadCommand>
    Writer: MailboxProcessor<WriteCommand>
    Multiplier: MailboxProcessor<MultiplyCommand>
    TrcFinder: MailboxProcessor<FindTrcCommand>
}

type WorkerResponse = {
    ReplyingMessage: WorkerCommand
    AnswerMessage: Result<obj, string list>
}

type DispatcherCommand = 
    | UserRequest of UserRequest
    | WorkerResponse of WorkerResponse

type Dispatcher = MailboxProcessor<DispatcherCommand>

module DispatcherExceptionMessages = 
    let variableIsAlreadyInContext variable = sprintf "Variable %s is already in context" variable
    let variableIsAlreadyProcessing variable = sprintf "Variable %s is already processing" variable
    let variableIsNotInContext variable = sprintf "Variable %s is not in context" variable
    let operandsTypesMismatch a b = sprintf "Variables %s and %s have different types" a b

module Actors = 
    open DispatcherExceptionMessages

    let error = printfn "%s"

    let creator (dispatcher: Dispatcher) = MailboxProcessor<CreateCommand>.Start <| fun inbox ->
        let cts = new Threading.CancellationTokenSource()
        let rand = Random()

        let rec messageLoop () = async {
            let! cmd = inbox.Receive()

            match cmd with
            | CreateArgs args -> 
                try
                    let resultMatrix = 
                        match args.MatrixType with
                        | HandledType.Integer -> 
                            (fun () -> rand.Next() % 100 |> IntegerSemiring)
                            |> Matrix.createRandom args.Size  
                            |> box

                        | HandledType.Boolean -> 
                            (fun () -> rand.Next() % 2 = 0 |> BooleanSemiring)
                            |> Matrix.createRandom args.Size  
                            |> box

                        | HandledType.Real -> 
                            (fun () -> rand.NextDouble() |> RealSemiring)
                            |> Matrix.createRandom args.Size 
                            |> box

                        | HandledType.ExtendedReal -> 
                            (fun () -> 
                                let cases = ["real"; "zero"; "inf"; "nan"]
                                match cases.[rand.Next cases.Length] with
                                | "real" -> rand.NextDouble() |> Real |> ExtendedRealSemiring
                                | "zero" -> ExtendedRealSemiring.Zero
                                | "inf" -> ExtendedRealSemiring Infinity
                                | "nan" -> ExtendedRealSemiring Indeterminacy
                            )
                            |> Matrix.createRandom args.Size  
                            |> box

                    {
                        ReplyingMessage = CreateCommand cmd
                        AnswerMessage = Ok resultMatrix
                    }
                    |> WorkerResponse
                    |> dispatcher.Post

                with
                | exn -> 
                    {
                        ReplyingMessage = CreateCommand cmd
                        AnswerMessage = Error [exn.Message]
                    }
                    |> WorkerResponse
                    |> dispatcher.Post
            
            | CreateCommand.CommonWorkerCommand common -> 
                match common with
                | WaitAllAndDie replyChannel -> 
                    if inbox.CurrentQueueLength = 0 then
                        cts.Cancel()
                        replyChannel.Reply "Creator is dead"
                    else
                        inbox.Post <| CreateCommand.CommonWorkerCommand (WaitAllAndDie replyChannel)

            return! messageLoop ()
        }

        async { return Async.Start(messageLoop (), cts.Token) }

    let reader (dispatcher: Dispatcher) = MailboxProcessor<ReadCommand>.Start <| fun inbox ->
        let cts = new Threading.CancellationTokenSource()

        let rec messageLoop () = async {
            let! cmd = inbox.Receive()

            match cmd with
            | ReadArgs args ->
                try 
                    let resultMatrix = 
                        match args.MatrixType with
                        | HandledType.Integer -> 
                            int >> IntegerSemiring
                            |> Matrix.readFromFile args.FullPathToMatrix
                            |> box

                        | HandledType.Boolean ->
                            (fun (str: string) -> System.Boolean.Parse str) >> BooleanSemiring
                            |> Matrix.readFromFile args.FullPathToMatrix
                            |> box

                        | HandledType.Real -> 
                            float >> RealSemiring
                            |> Matrix.readFromFile args.FullPathToMatrix
                            |> box

                        | HandledType.ExtendedReal ->   
                            let convertToExtendedReal str = 
                                match str with
                                | "nan" -> Indeterminacy
                                | "inf" -> Infinity
                                | _ -> Real <| float str

                            convertToExtendedReal >> ExtendedRealSemiring
                            |> Matrix.readFromFile args.FullPathToMatrix
                            |> box

                    {
                        ReplyingMessage = ReadCommand cmd
                        AnswerMessage = Ok resultMatrix
                    }
                    |> WorkerResponse
                    |> dispatcher.Post

                with
                | exn -> 
                    {
                        ReplyingMessage = ReadCommand cmd
                        AnswerMessage = Error [exn.Message]
                    }
                    |> WorkerResponse
                    |> dispatcher.Post

            | ReadCommand.CommonWorkerCommand commonCmd -> 
                match commonCmd with
                | WaitAllAndDie replyChannel -> 
                    if inbox.CurrentQueueLength = 0 then
                        cts.Cancel()
                        replyChannel.Reply "Reader is dead"
                    else
                        inbox.Post <| ReadCommand.CommonWorkerCommand (WaitAllAndDie replyChannel)

            return! messageLoop ()
        }
        
        async { return Async.Start(messageLoop (), cts.Token) }

    let writer (dispatcher: Dispatcher) = MailboxProcessor<WriteCommand>.Start <| fun inbox ->
        let cts = new Threading.CancellationTokenSource()

        let rec messageLoop () = async {
            let! cmd = inbox.Receive()
            match cmd with
            | WriteArgs args ->
                try
                    match args.MatrixType with
                    | HandledType.Integer ->
                        args.Matrix :?> Matrix<IntegerSemiring>
                        |> Matrix.writeToFile args.FullPathToOutputFile (sprintf "%i" << IntegerSemiring.Unwrap)

                    | HandledType.Boolean ->
                        args.Matrix :?> Matrix<BooleanSemiring>
                        |> Matrix.writeToFile args.FullPathToOutputFile (sprintf "%b" << BooleanSemiring.Unwrap)

                    | HandledType.Real -> 
                        args.Matrix :?> Matrix<RealSemiring>
                        |> Matrix.writeToFile args.FullPathToOutputFile (sprintf "%f" << RealSemiring.Unwrap)

                    | HandledType.ExtendedReal ->   
                        let convertToString value = 
                            match value with
                            | Indeterminacy -> "nan"
                            | Infinity -> "inf"
                            | Real v -> sprintf "%f" v

                        args.Matrix :?> Matrix<ExtendedRealSemiring>
                        |> Matrix.writeToFile args.FullPathToOutputFile (convertToString << ExtendedRealSemiring.Unwrap)

                    {
                        ReplyingMessage = WriteCommand cmd
                        AnswerMessage = Ok <| box None
                    }
                    |> WorkerResponse
                    |> dispatcher.Post
                
                with
                | exn -> 
                    {
                        ReplyingMessage = WriteCommand cmd
                        AnswerMessage = Error [exn.Message]
                    }
                    |> WorkerResponse
                    |> dispatcher.Post

            | WriteCommand.CommonWorkerCommand commonCmd -> 
                match commonCmd with
                | WaitAllAndDie replyChannel -> 
                    if inbox.CurrentQueueLength = 0 then
                        cts.Cancel()
                        replyChannel.Reply "Writer is dead"
                    else
                        inbox.Post <| WriteCommand.CommonWorkerCommand (WaitAllAndDie replyChannel)

            return! messageLoop ()
        }
        
        async { return Async.Start(messageLoop (), cts.Token) }

    let multiplier (dispatcher: Dispatcher) = MailboxProcessor<MultiplyCommand>.Start <| fun inbox ->
        let cts = new Threading.CancellationTokenSource()

        let rec messageLoop () = async {
            let! cmd = inbox.Receive()

            match cmd with
            | MultiplyArgs args ->
                try
                    let resultMatrix = 
                        match args.MatrixType with
                        | HandledType.Integer -> 
                            let left = args.LeftMatrix :?> Matrix<IntegerSemiring>
                            let right = args.RightMatrix :?> Matrix<IntegerSemiring>
                            Matrix.multiplyInParallel left right |> box

                        | HandledType.Boolean ->
                            let left = args.LeftMatrix :?> Matrix<BooleanSemiring>
                            let right = args.RightMatrix :?> Matrix<BooleanSemiring>
                            Matrix.multiplyInParallel left right |> box

                        | HandledType.Real -> 
                            let left = args.LeftMatrix :?> Matrix<RealSemiring>
                            let right = args.RightMatrix :?> Matrix<RealSemiring>
                            Matrix.multiplyInParallel left right |> box

                        | HandledType.ExtendedReal ->   
                            let left = args.LeftMatrix :?> Matrix<ExtendedRealSemiring>
                            let right = args.RightMatrix :?> Matrix<ExtendedRealSemiring>
                            Matrix.multiplyInParallel left right |> box

                    {
                        ReplyingMessage = MultiplyCommand cmd
                        AnswerMessage = Ok resultMatrix
                    }
                    |> WorkerResponse
                    |> dispatcher.Post

                with
                | exn -> 
                    {
                        ReplyingMessage = MultiplyCommand cmd
                        AnswerMessage = Error [exn.Message]
                    }
                    |> WorkerResponse
                    |> dispatcher.Post

            | MultiplyCommand.CommonWorkerCommand commonCmd -> 
                match commonCmd with
                | WaitAllAndDie replyChannel -> 
                    if inbox.CurrentQueueLength = 0 then
                        cts.Cancel()
                        replyChannel.Reply "Multiplier is dead"
                    else
                        inbox.Post <| MultiplyCommand.CommonWorkerCommand (WaitAllAndDie replyChannel)

            return! messageLoop ()
        }
        
        async { return Async.Start(messageLoop (), cts.Token) }

    let trcFinder (dispatcher: Dispatcher) = MailboxProcessor<FindTrcCommand>.Start <| fun inbox ->
        let cts = new Threading.CancellationTokenSource()

        let rec messageLoop () = async {
            let! cmd = inbox.Receive()

            match cmd with
            | FindTrcArgs args ->
                try
                    let resultMatrix = 
                        match args.MatrixType with
                        | HandledType.Integer -> 
                            args.Matrix :?> Matrix<IntegerSemiring>
                            |> Matrix.findTransitiveClosure 
                            |> box

                        | HandledType.Boolean ->
                            args.Matrix :?> Matrix<BooleanSemiring>
                            |> Matrix.findTransitiveClosure 
                            |> box

                        | HandledType.Real -> 
                            args.Matrix :?> Matrix<RealSemiring>
                            |> Matrix.findTransitiveClosure 
                            |> box

                        | HandledType.ExtendedReal ->   
                            args.Matrix :?> Matrix<ExtendedRealSemiring>
                            |> Matrix.findTransitiveClosure 
                            |> box

                    {
                        ReplyingMessage = FindTrcCommand cmd
                        AnswerMessage = Ok resultMatrix
                    }
                    |> WorkerResponse
                    |> dispatcher.Post

                with
                | exn -> 
                    {
                        ReplyingMessage = FindTrcCommand cmd
                        AnswerMessage = Error [exn.Message]
                    }
                    |> WorkerResponse
                    |> dispatcher.Post
          
            | FindTrcCommand.CommonWorkerCommand commonCmd -> 
                match commonCmd with
                | WaitAllAndDie replyChannel -> 
                    if inbox.CurrentQueueLength = 0 then
                        cts.Cancel()
                        replyChannel.Reply "TrcFinder is dead"
                    else
                        inbox.Post <| FindTrcCommand.CommonWorkerCommand (WaitAllAndDie replyChannel)

            return! messageLoop ()
        }
        
        async { return Async.Start(messageLoop (), cts.Token) }

    let dispatcher () = MailboxProcessor<DispatcherCommand>.Start <| fun inbox ->
        let cts = new Threading.CancellationTokenSource()
        let rand = Random()

        let workers = {
            Creator = creator inbox
            Reader = reader inbox
            Writer = writer inbox
            Multiplier = multiplier inbox
            TrcFinder = trcFinder inbox
        }

        let context = Dictionary<string, obj * HandledType>()

        // список задач, возвращающих результат, которые приняты в работу воркерами
        let processingCommands = Dictionary<WorkerCommand, string>()

        // список задач, которые принимают переменные контекста в качестве аргумента и возвращают значения, 
        // но для которых аргументы еще вычисляются
        let pendingCommands = Dictionary<DispatcherCommand, string>()

        let rec messageLoop () = async {
            let! msg = inbox.Receive()

            match msg with
            | UserRequest request -> 
                match request with 
                | Create args -> 
                    if args.VarName |> context.ContainsKey then
                        error (variableIsAlreadyInContext args.VarName)
                    elif args.VarName |> processingCommands.ContainsValue || args.VarName |> pendingCommands.ContainsValue then
                        error (variableIsAlreadyProcessing args.VarName)
                    else    
                        let commandArgs = CreateArgs {| Key = rand.Next(); MatrixType = args.MatrixType; Size = args.Size |}
                        workers.Creator.Post commandArgs
                        processingCommands.Add(CreateCommand commandArgs, args.VarName) |> ignore

                | Read args -> 
                    if args.VarName |> context.ContainsKey then
                        error (variableIsAlreadyInContext args.VarName)
                    elif args.VarName |> processingCommands.ContainsValue || args.VarName |> pendingCommands.ContainsValue then
                        error (variableIsAlreadyProcessing args.VarName)
                    else 
                        let commandArgs = ReadArgs {| Key = rand.Next(); MatrixType = args.MatrixType; FullPathToMatrix = args.FullPathToMatrix |}
                        workers.Reader.Post commandArgs
                        processingCommands.Add(ReadCommand commandArgs, args.VarName) |> ignore

                | Write args ->
                    if args.VarName |> context.ContainsKey then
                        let (matrix, matrixType) = context.[args.VarName]
                        let commandArgs = WriteArgs {| Key = rand.Next(); MatrixType = matrixType; FullPathToOutputFile = args.FullPathToOutputFile; Matrix = matrix |}
                        workers.Writer.Post commandArgs
                    elif args.VarName |> processingCommands.ContainsValue || args.VarName |> pendingCommands.ContainsValue then
                        inbox.Post <| UserRequest request
                    else 
                        error (variableIsNotInContext args.VarName)

                | Multiply args ->
                    if args.ResultVarName |> context.ContainsKey then
                        error (variableIsAlreadyInContext args.ResultVarName)
                    elif args.ResultVarName |> processingCommands.ContainsValue then 
                        error (variableIsAlreadyProcessing args.ResultVarName)
                    elif args.LeftVarName |> context.ContainsKey && args.RightVarName |> context.ContainsKey then
                        let (left, leftType) = context.[args.LeftVarName]
                        let (right, rightType) = context.[args.RightVarName]
                        if leftType = rightType then
                            let commandArgs = MultiplyArgs {| Key = rand.Next(); MatrixType = leftType; LeftMatrix = left; RightMatrix = right |}
                            workers.Multiplier.Post commandArgs
                            pendingCommands.Remove(UserRequest request) |> ignore
                            processingCommands.Add(MultiplyCommand commandArgs, args.ResultVarName) |> ignore
                        else
                            error (operandsTypesMismatch args.LeftVarName args.RightVarName)
                    elif args.LeftVarName |> processingCommands.ContainsValue || 
                        args.RightVarName |> processingCommands.ContainsValue ||
                        args.LeftVarName |> pendingCommands.ContainsValue || 
                        args.RightVarName |> pendingCommands.ContainsValue then
                        
                        inbox.Post <| UserRequest request
                        pendingCommands.TryAdd(UserRequest request, args.ResultVarName) |> ignore
                    else
                        error (variableIsNotInContext <| sprintf "(%s or %s)" args.LeftVarName args.RightVarName)

                | FindTrc args ->
                    if args.ResultVarName |> context.ContainsKey then
                        error (variableIsAlreadyInContext args.ResultVarName)
                    elif args.ResultVarName |> processingCommands.ContainsValue then 
                        error (variableIsAlreadyProcessing args.ResultVarName)
                    elif args.VarName |> context.ContainsKey then
                        let (matrix, matrixType) = context.[args.VarName]
                        let commandArgs = FindTrcArgs {| Key = rand.Next(); MatrixType = matrixType; Matrix = matrix |}
                        workers.TrcFinder.Post commandArgs
                        pendingCommands.Remove(UserRequest request) |> ignore
                        processingCommands.Add(FindTrcCommand commandArgs, args.ResultVarName) |> ignore
                    elif args.VarName |> processingCommands.ContainsValue || 
                        args.VarName |> pendingCommands.ContainsValue then
                        
                        inbox.Post <| UserRequest request
                        pendingCommands.TryAdd(UserRequest request, args.ResultVarName) |> ignore
                    else
                        error (variableIsNotInContext args.VarName)
                
                | WaitAllAndExit replyChannel -> 
                    if processingCommands.Count = 0 && pendingCommands.Count = 0 && inbox.CurrentQueueLength = 0 then
                        let replies = 
                            [
                                workers.Creator.PostAndAsyncReply(fun replyChannel -> CreateCommand.CommonWorkerCommand <| WaitAllAndDie replyChannel)
                                workers.Reader.PostAndAsyncReply(fun replyChannel -> ReadCommand.CommonWorkerCommand <| WaitAllAndDie replyChannel)
                                workers.Writer.PostAndAsyncReply(fun replyChannel -> WriteCommand.CommonWorkerCommand <| WaitAllAndDie replyChannel)
                                workers.Multiplier.PostAndAsyncReply(fun replyChannel -> MultiplyCommand.CommonWorkerCommand <| WaitAllAndDie replyChannel)
                                workers.TrcFinder.PostAndAsyncReply(fun replyChannel -> FindTrcCommand.CommonWorkerCommand <| WaitAllAndDie replyChannel)
                            ]
                            |> Async.Parallel
                            |> Async.RunSynchronously
                            |> List.ofArray

                        cts.Cancel()
                        replyChannel.Reply <| replies @ [ "Dispatcher is dead" ]
                    else
                        inbox.Post <| UserRequest (WaitAllAndExit replyChannel)

            | WorkerResponse response -> 
                let requestedCommand = response.ReplyingMessage
                let bindResult matrixType = 
                    match response.AnswerMessage with
                    | Ok result -> 
                        let mutable variable = ""
                        if processingCommands.TryGetValue(requestedCommand, &variable) then
                            processingCommands.Remove requestedCommand |> ignore
                            context.Add(variable, (result, matrixType))
                        else 
                            error "Something went wrong :c"
                    | Error e -> 
                        error (sprintf "%A" e)

                match requestedCommand with
                | CreateCommand (CreateArgs args) -> bindResult args.MatrixType
                | ReadCommand (ReadArgs args) -> bindResult args.MatrixType
                | WriteCommand (WriteArgs _) ->
                    match response.AnswerMessage with
                    | Ok _ -> ()
                    | Error e -> error (sprintf "%A" e)
                | MultiplyCommand (MultiplyArgs args) -> bindResult args.MatrixType
                | FindTrcCommand (FindTrcArgs _) -> bindResult HandledType.Boolean
            
            return! messageLoop ()
        }
        
        async { return Async.Start(messageLoop (), cts.Token) }
