namespace Task23

open System

[<Struct>]
type IntegerMinPlusSemiring = 
    | Integer of int
    | PositiveInfinity
with 
    override this.ToString() = 
        match this with
        | PositiveInfinity -> "inf"
        | Integer v -> sprintf "%i" v

    interface Task21.ISemiring<IntegerMinPlusSemiring> with
        override this.Add(other: IntegerMinPlusSemiring) = 
            match (this, other) with
            | Integer x, Integer y -> Integer <| Math.Min(x, y)
            | Integer x, PositiveInfinity -> Integer x
            | PositiveInfinity, Integer y -> Integer y
            | PositiveInfinity, PositiveInfinity -> PositiveInfinity

        override this.Mult(other: IntegerMinPlusSemiring) = 
            match (this, other) with
            | Integer x, Integer y -> Integer <| x + y
            | Integer x, PositiveInfinity -> PositiveInfinity
            | PositiveInfinity, Integer y -> PositiveInfinity
            | PositiveInfinity, PositiveInfinity -> PositiveInfinity

        override this.IsZero() = this = IntegerMinPlusSemiring.Zero

    static member Zero = PositiveInfinity
    static member FromString(str: string) = 
        match str with 
        | "inf" -> PositiveInfinity
        | v -> Integer (int v)

[<Struct>]
type FloatMinPlusSemiring = 
    | Float of float
    | PositiveInfinity
with 
    override this.ToString() = 
        match this with
        | PositiveInfinity -> "inf"
        | Float v -> sprintf "%f" v

    interface Task21.ISemiring<FloatMinPlusSemiring> with
        override this.Add(other: FloatMinPlusSemiring) = 
            match (this, other) with
            | Float x, Float y -> Float <| Math.Min(x, y)
            | Float x, PositiveInfinity -> Float x
            | PositiveInfinity, Float y -> Float y
            | PositiveInfinity, PositiveInfinity -> PositiveInfinity

        override this.Mult(other: FloatMinPlusSemiring) = 
            match (this, other) with
            | Float x, Float y -> Float <| x + y
            | Float x, PositiveInfinity -> PositiveInfinity
            | PositiveInfinity, Float y -> PositiveInfinity
            | PositiveInfinity, PositiveInfinity -> PositiveInfinity

        override this.IsZero() = this = FloatMinPlusSemiring.Zero

    static member Zero = PositiveInfinity
    static member FromString(str: string) = 
        match str with
        | "inf" -> PositiveInfinity
        | _ -> Float (float str)