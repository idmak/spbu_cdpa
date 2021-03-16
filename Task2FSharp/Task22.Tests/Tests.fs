module Tests

open Expecto
open Task22
open System.IO

[<Tests>]
let pdfGenerationTest = testCase "Pdf generation test" <| fun () ->
    let matrix = "matrix.txt"
    let outputDir = "."
    let pdfPath = Path.Combine [| outputDir; Path.GetFileNameWithoutExtension matrix + ".pdf" |]
    let dotPath = Path.Combine [| outputDir; Path.GetFileNameWithoutExtension matrix + ".dot" |]

    try
        [| "-p"; matrix; "-o"; outputDir |]
        |> Program.main
        |> ignore
        Async.Sleep(200) |> Async.RunSynchronously
        Expect.isTrue (File.Exists pdfPath) "Pdf should exists"

    finally
        if File.Exists pdfPath then File.Delete pdfPath
        if File.Exists dotPath then File.Delete dotPath
