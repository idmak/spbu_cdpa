namespace Task3

open System

[<Struct>]
type ExtendedReal = 
    | Real of float
    | Infinity 
    | Indeterminacy

module RandomGenerators =
    let internal rand = Random()

    let getNextInteger () = rand.Next(Int32.MinValue, Int32.MaxValue)

    let getNextBoolean () = rand.Next() % 2 = 0

    let getNextReal () = 
        if getNextBoolean () then rand.NextDouble() * Double.MaxValue
        else (-1.) * rand.NextDouble() * Double.MaxValue

    let getNextExtendedReal () = 
        let cases = ["real"; "inf"; "nan"]
        match cases.[rand.Next cases.Length] with
        | "real" -> getNextReal () |> Real
        | "inf" -> Infinity
        | "nan" -> Indeterminacy
        | _ -> failwith "fail to generate next extended real"