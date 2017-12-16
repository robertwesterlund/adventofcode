module Prog

    open System

    type Prog = {Name: string; Weight: int}

    type ProgNode = 
        | Leaf of Prog
        | Node of Prog * Children: Prog list

    let nodes data:string[] = 
        data

    type Prog = {Name: string; Weight: int; Carries: string[]}
