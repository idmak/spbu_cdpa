namespace Task4

type HandledType = 
    | Integer 
    | Boolean
    | Real
    | ExtendedReal

[<Struct>]
type IntegerSemiring = IntegerSemiring of int
with 
    interface ISemiring<IntegerSemiring> with
        override this.Add(IntegerSemiring other) = IntegerSemiring (IntegerSemiring.Unwrap this + other)
        override this.Mult(IntegerSemiring other) = IntegerSemiring (IntegerSemiring.Unwrap this * other)
        override this.IsZero() = IntegerSemiring.Unwrap this = IntegerSemiring.Unwrap IntegerSemiring.Zero

    static member Unwrap(IntegerSemiring x) = x
    static member Zero = IntegerSemiring 0

[<Struct>]
type BooleanSemiring = BooleanSemiring of bool
with 
    interface ISemiring<BooleanSemiring> with
        override this.Add(BooleanSemiring other) = BooleanSemiring (BooleanSemiring.Unwrap this || other)
        override this.Mult(BooleanSemiring other) = BooleanSemiring (BooleanSemiring.Unwrap this && other)
        override this.IsZero() = BooleanSemiring.Unwrap this = BooleanSemiring.Unwrap BooleanSemiring.Zero

    static member Unwrap(BooleanSemiring x) = x
    static member Zero = BooleanSemiring false

[<Struct>]
type RealSemiring = RealSemiring of float
with 
    interface ISemiring<RealSemiring> with
        override this.Add(RealSemiring other) = RealSemiring (RealSemiring.Unwrap this + other)
        override this.Mult(RealSemiring other) = RealSemiring (RealSemiring.Unwrap this * other)
        override this.IsZero() = abs (RealSemiring.Unwrap this) < 1e-16

    static member Unwrap(RealSemiring x) = x
    static member Zero = RealSemiring 0.

[<Struct>]
type ExtendedReal = 
    | Real of float
    | Infinity 
    | Indeterminacy

[<Struct>]
type ExtendedRealSemiring = ExtendedRealSemiring of ExtendedReal
with 
    interface ISemiring<ExtendedRealSemiring> with
        override this.Add(ExtendedRealSemiring other) =
            match ExtendedRealSemiring.Unwrap this, other with
            | Real x, Real y -> ExtendedRealSemiring <| Real (x + y)
            | _, Infinity -> ExtendedRealSemiring <| Infinity
            | Infinity, _ -> ExtendedRealSemiring <| Infinity
            | Indeterminacy, _ -> ExtendedRealSemiring <| Indeterminacy
            | _, Indeterminacy -> ExtendedRealSemiring <| Indeterminacy

        override this.Mult(ExtendedRealSemiring other) = 
            match ExtendedRealSemiring.Unwrap this, other with
            | Real x, Real y -> ExtendedRealSemiring <| Real (x + y)
            | Real x, Infinity -> ExtendedRealSemiring <| if abs x < 1e-16 then Indeterminacy else Infinity
            | Infinity, Real y -> ExtendedRealSemiring <| if abs y < 1e-16 then Indeterminacy else Infinity
            | Infinity, Infinity -> ExtendedRealSemiring <| Infinity
            | Indeterminacy, _ -> ExtendedRealSemiring <| Indeterminacy
            | _, Indeterminacy -> ExtendedRealSemiring <| Indeterminacy

        override this.IsZero() = 
            match ExtendedRealSemiring.Unwrap this with
            | Infinity -> false
            | Indeterminacy -> false
            | Real value -> abs value < 1e-16

    static member Unwrap(ExtendedRealSemiring x) = x
    static member Zero = ExtendedRealSemiring <| Real 0.