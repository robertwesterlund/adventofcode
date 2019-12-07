// Learn more about F# at http://fsharp.org

open System

let getDay2Input () = 
    [|1;0;0;3;1;1;2;3;1;3;4;3;1;5;0;3;2;1;10;19;2;9;19;23;1;9;23;27;2;27;9;31;1;31;5;35;2;35;9;39;1;39;10;43;2;43;13;47;1;47;6;51;2;51;10;55;1;9;55;59;2;6;59;63;1;63;6;67;1;67;10;71;1;71;10;75;2;9;75;79;1;5;79;83;2;9;83;87;1;87;9;91;2;91;13;95;1;95;9;99;1;99;6;103;2;103;6;107;1;107;5;111;1;13;111;115;2;115;6;119;1;119;5;123;1;2;123;127;1;6;127;0;99;2;14;0;0|]

let sample () = 
    [|2;4;4;5;99;0|]
    //[|1;9;10;3;2;3;11;0;99;30;40;50|]
    //[|1;1;1;4;99;5;6;0;99|]

type OpCode =
    | Halt     
    | Add of leftIndex: int * rightIndex: int * outputIndex: int
    | Multiply of leftIndex: int * rightIndex: int * outputIndex: int

    member this.CodeLength = 
        match this with 
        | Halt -> 1
        | Add _ -> 4
        | Multiply _ -> 4

    member this.Apply (data:int[]) = 
        match this with
        | Halt -> failwith "Should never apply a halt operation"
        | Add(l,r,o) -> data.[o] <- data.[l] + data.[r]
        | Multiply(l,r,o) -> data.[o] <- data.[l] * data.[r]

let getOperationAt (data: int[]) index =
    match data.[index] with
    | 99 -> Halt
    | 1 -> Add(data.[index + 1], data.[index + 2], data.[index + 3])
    | 2 -> Multiply(data.[index + 1], data.[index + 2], data.[index + 3])
    | unknown -> failwith (String.Concat("Unknown operation requested: ",unknown))

let runProgram noun verb data =
    let getOp = getOperationAt data
    let rec runProgramFromIndex index =
        let opCode = getOp index
        match opCode with
        | Halt -> data
        | _ -> 
            opCode.Apply data
            runProgramFromIndex (index + opCode.CodeLength)
    data.[1] <- noun
    data.[2] <- verb    
    runProgramFromIndex 0

let getNounAndVerbToGetCorrectResult expectedResult dataRetriever =
    let pairs = seq { 1 .. 99 } |> Seq.map(fun noun -> 
        seq { 1 .. 99 } |> Seq.map(fun verb ->
            match runProgram noun verb (dataRetriever()) |> Seq.head with
            | result when result = expectedResult -> Some (noun, verb)
            | _ -> None
        ) |> Seq.tryFind Option.isSome |> Option.flatten
    )
    match pairs |> Seq.find Option.isSome with
    | Some (noun, verb) -> (noun, verb)
    | None -> failwith "Could not find any pair that received the requested value"    

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let input = getDay2Input()
    input |> runProgram 12 2 |> Seq.head |> printfn "Result Part 1: %i"
    let (noun, verb) = getDay2Input |> getNounAndVerbToGetCorrectResult 19690720
    printfn "Result Part 2: %s%s" (noun.ToString("00")) (verb.ToString("00"))

    0 // return an integer exit code
