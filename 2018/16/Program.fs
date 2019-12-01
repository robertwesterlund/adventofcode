// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Collections.Generic
open System.Text

let part1Data =
    File.ReadLines(".\\part1.data") |> Seq.filter (fun line -> not (String.IsNullOrWhiteSpace(line))) |> Seq.toArray 

let part2Data =
    File.ReadLines(".\\part2.data") |> Seq.filter (fun line -> not (String.IsNullOrWhiteSpace(line))) |> Seq.toArray 

type Sample =
    {
        Before : bigint[]
        Instructions : bigint[]
        After : bigint[]
    }  
    
let parseDataArray (numberPart:string) =
    numberPart.Split(' ') |> Seq.map (fun num -> bigint.Parse(num.Trim(','))) |> Seq.toArray

let parseSamples (data:string array) = 
    seq {0 .. 3 .. data.Length - 1} 
        |> Seq.map (fun index ->
            {
                Sample.Before = data.[index + 0].Substring("Before: [".Length, data.[index + 0].Length - 1 - "Before: [".Length) |> parseDataArray
                Sample.Instructions = data.[index + 1] |> parseDataArray
                Sample.After = data.[index + 2].Substring("After:  [".Length, data.[index + 2].Length - 1 - "After:  [".Length) |> parseDataArray
            }
        ) |> Seq.toArray

let getValue (registers:bigint array) paramType paramValue =
    match paramType with
    | 'i' -> paramValue
    | t when t = 'r' || t = 't' || t = 'd' || t = 'l' || t = 'n' -> registers.[paramValue |> int]
    | _ -> raise (Exception("Invalid param type " + paramType.ToString()))

let getOperation op : bigint -> bigint -> bigint =
    match op with
    | "ad" -> (+)
    | "mu" -> (*)
    | "ba" -> (&&&)
    | "bo" -> (|||)
    | "se" -> fun a _ -> a
    | "gt" -> fun a b -> if a > b then 1I else 0I
    | "eq" -> fun a b -> if a = b then 1I else 0I
    | _ -> raise (Exception("Invalid operation name " + op))

let allOperationNames = 
    [
        "addr"
        "addi"
        "mulr"
        "muli"
        "banr"
        "bani"
        "borr"
        "bori"
        "setr"
        "seti"
        "gtir"
        "gtri"
        "gtrr"
        "eqir"
        "eqri"
        "eqrr"
    ]

let execute (registers:bigint array) op firstParamType secondParamType firstParam secondParam thirdParam =
    registers.[thirdParam |> int] <- (getOperation op) (getValue registers firstParamType firstParam) (getValue registers secondParamType secondParam)
    registers

let executeConventionNamed (registers:bigint array) op firstParam secondParam thirdParam =
    let opNameFixedForSeti =
        match op with
        | "seti" -> "seit"
        | "setr" -> "sert"
        | _ -> op
    execute registers (op.Substring(0, 2)) opNameFixedForSeti.[2] opNameFixedForSeti.[3] firstParam secondParam thirdParam

let createRegisters() = 
    [|0I;0I;0I;0I;|]

let getSuccessfulOperations sample =
    allOperationNames 
        |> Seq.map(fun opName ->
            (
                opName,                
                sample,
                executeConventionNamed (sample.Before |> Seq.map (id) |> Seq.toArray) opName sample.Instructions.[1] sample.Instructions.[2] sample.Instructions.[3]
            )
        ) 
        |> Seq.filter (fun (opName, sample, result) -> sample.After = result)

let doesOpMapWorkForSample operationMap sample = 
        let registers = sample.Before |> Seq.toArray
        executeConventionNamed registers (operationMap |> Map.find sample.Instructions.[0]) sample.Instructions.[1] sample.Instructions.[2] sample.Instructions.[3] |> ignore
        if registers = sample.After
        then
            true
        else
            false

let testOperations samples =
    samples
        |> Seq.map getSuccessfulOperations
        |> Seq.filter (fun res -> (res |> Seq.length) > 2)

let getPossibleOpNamesFor opCode =
    let samplesTestingOpCode = part1Data |> parseSamples |> Seq.map getSuccessfulOperations |> Seq.filter (fun g -> g |> Seq.exists (fun (_, sample, _) -> sample.Instructions.[0] = opCode))
    samplesTestingOpCode |> Seq.collect (fun g -> g |> Seq.map (fun (opName, _ ,_) -> opName)) |> Seq.distinct

let opnameByOpCode = 
    let mutable opCodesLeft = seq { 0I .. 15I } |> Set.ofSeq
    let mutable opNamesFound = Set.empty
    let mutable results = Set.empty
    while Set.count opNamesFound <> List.length allOperationNames do
        for opCode in opCodesLeft do
            let possibleNames = 
                match getPossibleOpNamesFor opCode |> Seq.except opNamesFound with
                | names when names |> Seq.length > 0 -> names
                | _ -> allOperationNames |> Seq.ofList
            if Seq.length possibleNames = 1
            then
                let opName = possibleNames |> Seq.head
                opNamesFound <- opNamesFound |> Set.add opName
                results <- results |> Set.add (opCode, opName)
                opCodesLeft <- opCodesLeft |> Set.remove opCode
    results |> Map.ofSeq        

[<EntryPoint>]
let main argv =
    let sampleData = 
        [|
            "Before: [3, 2, 1, 1]"
            "9 2 1 2"
            "After:  [3, 2, 2, 1]"
        |]
    sampleData |> parseSamples |> Seq.collect getSuccessfulOperations |> Seq.iter (fun (opName, _, _) -> printfn "Sample data succeded on %s" opName)
    part1Data |> parseSamples |> Seq.length |> printfn "Number of sample: %i"
    part1Data |> parseSamples |> testOperations |> Seq.length |> printfn "%i samples behaved like more than 2 op codes"
    
    let testExecutor sample = 
        match doesOpMapWorkForSample opnameByOpCode sample with
        | true -> ()
        | false -> raise (Exception "Identification of opCode -> opName is incorrect")

    part1Data |> parseSamples |> Seq.iter testExecutor

    printfn "%i" (part2Data |> Seq.map parseDataArray |> Seq.length)
    let registers = createRegisters()
    let executor (instructions:bigint array) = 
        executeConventionNamed registers (opnameByOpCode |> Map.find instructions.[0]) instructions.[1] instructions.[2] instructions.[3] |> ignore
    part2Data 
        |> Seq.map parseDataArray 
        |> Seq.iter (fun instruction -> executor instruction)
    registers.[0] |> printfn "First register after all operations: %A"
    0 // return an integer exit code