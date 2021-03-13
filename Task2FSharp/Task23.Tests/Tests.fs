module Tests

open Expecto
open Task23
open Task21
open System.IO

[<Tests>]
let apspCorrectnessTest = testCase "Simple correctness test for apsp" <| fun () ->
    let matrixPath = "matrix.txt"
    let actualPath = "actual.txt"
    let expectedPath = "expected.txt"
    let matrixType = "Integer"

    try
        [| "-t"; matrixType; "-p"; matrixPath; "-o"; actualPath |]
        |> Task23.Program.main
        |> ignore

        let (Matrix actual) = Matrix.readFromFile actualPath IntegerMinPlusSemiring.FromString
        let (Matrix expected) = Matrix.readFromFile expectedPath IntegerMinPlusSemiring.FromString

        "Actual and expected matrix should be equal"
        |> Expect.sequenceEqual
            (actual |> Seq.cast<IntegerMinPlusSemiring>)
            (expected |> Seq.cast<IntegerMinPlusSemiring>)

    finally
        if File.Exists actualPath then File.Delete actualPath
