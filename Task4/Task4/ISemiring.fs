namespace Task4

type ISemiring<'a> = 
    abstract Add: 'a -> 'a
    abstract Mult: 'a -> 'a
    abstract IsZero: unit -> bool