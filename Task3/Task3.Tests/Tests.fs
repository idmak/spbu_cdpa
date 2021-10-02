module Tests

open Expecto
open System.IO

let simpleCorrectnessTest = testCase "Simple correctness test" <| fun () ->
    let path = "test"
    let matrixType = "Integer"
    let size = "4"
    let count = 10
    let pathToOutputDirectory = Path.Combine [| path; matrixType; string size |]

    try
        [| "-t"; matrixType; "-n"; size; "-p"; path; "-k"; string count|]
        |> Program.main
        |> ignore

        for i = 1 to count do
            let path = Path.Combine [| pathToOutputDirectory; sprintf "%i.txt" i |]
            sprintf "File %s should exist" path
            |> Expect.isTrue (File.Exists path) 

    finally
        if Directory.Exists path then 
            Directory.Delete(path, true)


[<Tests>]
let tests =
    testList "Task3 tests" [
        simpleCorrectnessTest
    ]
