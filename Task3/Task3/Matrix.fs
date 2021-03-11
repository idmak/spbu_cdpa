namespace Task3

open System.Text
open System.IO

type Matrix<'a> = Matrix of 'a[,]

module Matrix = 
    let createRandom<'a> (size: int) (getNextRandom: unit -> 'a) = 
        Array2D.init size size (fun _ _ -> getNextRandom ())
        |> Matrix

    let writeToFile<'a> (fullPathToOutputFile: string) (convertToString: 'a -> string) ((Matrix matrix): Matrix<'a>) = 
        let sb = StringBuilder()

        matrix
        |> Array2D.iteri 
            (fun i j v ->
                let stringValue = convertToString v
                if j = Array2D.length2 matrix - 1 then
                    sb.Append (sprintf "%s\n" stringValue) |> ignore
                else
                    sb.Append (sprintf "%s " stringValue) |> ignore
            )

        File.WriteAllText(fullPathToOutputFile, sb.ToString())