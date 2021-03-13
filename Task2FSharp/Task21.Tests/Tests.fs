module Tests

open Expecto
open System.IO
open Task21

[<Tests>]
let integerCorrectnessTest = testCase "Multiplication correctness test int" <| fun () ->
    let matrixA = "intA.txt"
    let matrixB = "intB.txt"
    let actualPath = "intActual.txt"
    let expectedPath = "intExpected.txt"
    let matrixType = "Integer"

    try
        [| "-t"; matrixType; "-p"; matrixA; matrixB; "-o"; actualPath |]
        |> Program.main
        |> ignore

        let (Matrix actual) = Matrix.readFromFile actualPath IntegerSemiring.FromString
        let (Matrix expected) = Matrix.readFromFile expectedPath IntegerSemiring.FromString

        "Actual and expected matrix should be equal"
        |> Expect.sequenceEqual
            (actual |> Array2D.map IntegerSemiring.Unwrap |> Seq.cast<int>)
            (expected |> Array2D.map IntegerSemiring.Unwrap |> Seq.cast<int>)

    finally
        if File.Exists actualPath then File.Delete actualPath

[<Tests>]
let booleanCorrectnessTest = testCase "Multiplication correctness test bool" <| fun () ->
    let matrixA = "boolA.txt"
    let matrixB = "boolB.txt"
    let actualPath = "boolActual.txt"
    let expectedPath = "boolExpected.txt"
    let matrixType = "Boolean"

    try
        [| "-t"; matrixType; "-p"; matrixA; matrixB; "-o"; actualPath |]
        |> Program.main
        |> ignore

        let (Matrix actual) = Matrix.readFromFile actualPath BooleanSemiring.FromString
        let (Matrix expected) = Matrix.readFromFile expectedPath BooleanSemiring.FromString

        "Actual and expected matrix should be equal"
        |> Expect.sequenceEqual
            (actual |> Array2D.map BooleanSemiring.Unwrap |> Seq.cast<bool>)
            (expected |> Array2D.map BooleanSemiring.Unwrap |> Seq.cast<bool>)

    finally
        if File.Exists actualPath then File.Delete actualPath