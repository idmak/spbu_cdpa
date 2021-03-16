namespace Task21

open System.IO
open System.Text
open System.Threading.Tasks

type Matrix<'a when 'a :> ISemiring<'a>> = Matrix of 'a[,]

module Matrix = 
    let readFromFile<'a when 'a :> ISemiring<'a>> (fullPathToMatrix: string) (convertFromString: string -> 'a) = 
        fullPathToMatrix
        |> File.ReadLines
        |> Seq.map (fun line -> line.Trim().Split " ")
        |> array2D
        |> Array2D.map convertFromString
        |> Matrix

    let writeToFile<'a when 'a :> ISemiring<'a>> (fullPathToOutputFile: string) ((Matrix matrix): Matrix<'a>) = 
        let sb = StringBuilder()
        matrix
        |> Array2D.iteri (fun i j v ->
                let stringValue = v.ToString()
                if j = Array2D.length2 matrix - 1 then
                    sb.Append (sprintf "%s\n" stringValue) |> ignore
                else
                    sb.Append (sprintf "%s " stringValue) |> ignore
            )

        File.WriteAllText(fullPathToOutputFile, sb.ToString())

    let multiplyInParallel<'a when 'a :> ISemiring<'a>> ((Matrix leftMatrix): Matrix<'a>) ((Matrix rightMatrix): Matrix<'a>) = 
        if Array2D.length2 leftMatrix <> Array2D.length1 rightMatrix then 
            failwith "Invalid matrix size"

        let resultRowCount = Array2D.length1 leftMatrix
        let resultColCount = Array2D.length2 rightMatrix
        let resultMatrix = Array2D.zeroCreate<'a> resultRowCount resultColCount
        
        let parallelTask idx =
            let i = idx / resultColCount
            let j = idx % resultColCount
            let leftRow = leftMatrix.[i, *]
            let rightCol = rightMatrix.[*, j]

            resultMatrix.[i, j] <- 
                leftRow
                |> Array.Parallel.mapi (fun i v -> v.Mult rightCol.[i])
                |> Array.reduce (fun x y -> x.Add y)

        Parallel.For(0, resultRowCount * resultColCount, parallelTask) |> ignore
        Matrix resultMatrix

    let findTransitiveClosure<'a when 'a :> ISemiring<'a>> ((Matrix matrix): Matrix<'a>) = 
        let rowCount = Array2D.length1 matrix
        let pattern = matrix |> Array2D.map (fun x -> not <| x.IsZero())
        let matrix' = 
            pattern 
            |> Array2D.mapi (fun i j v -> (i = j) || v)
            |> Array2D.map BooleanSemiring
            |> Matrix

        seq { 1 .. rowCount }
        |> Seq.fold (fun matrixOfDegreeK _ -> multiplyInParallel matrixOfDegreeK matrix') matrix'

    let findDistanceMatrix<'a when 'a :> ISemiring<'a>> (matrix: Matrix<'a>) = 
        let (Matrix inner) = matrix
        let rowCount = Array2D.length1 inner

        seq { 1 .. rowCount }
        |> Seq.fold (fun matrixOfDegreeK _ -> multiplyInParallel matrixOfDegreeK matrix) matrix