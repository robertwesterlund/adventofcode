// Learn more about F# at http://fsharp.org

open System
open System.IO
let inputData = File.ReadAllLines(".\\input.data")

type Instruction =
    {
        Operation : string
        Parameters : Int32[]
    }

type ExecutionContext = 
    {
        InstructionPointerIndex : int
        Registers : Int32[]
        Instructions : Instruction[]
    }

let parseInstructions (data:seq<string>) = 
    data 
        |> Seq.map (fun line ->
            let split = line.Split(' ')
            {
                Instruction.Operation = split.[0]
                Instruction.Parameters = [| Int32.Parse(split.[1]); Int32.Parse(split.[2]); Int32.Parse(split.[3]) |]
            }
        )

let parseData valueAtRegisterZero (data:string[]) =
    let instructionPointerIndex = Int32.Parse(data.[0].Substring("#ip ".Length))
    let instructions = parseInstructions (data |> Seq.skip 1) |> Seq.toArray
    {
        ExecutionContext.InstructionPointerIndex = instructionPointerIndex
        ExecutionContext.Registers = [| valueAtRegisterZero;0;0;0;0;0 |]
        ExecutionContext.Instructions = instructions
    }


let parseDataArray (numberPart:string) =
    numberPart.Split(' ') |> Seq.map (fun num -> Int32.Parse(num.Trim(','))) |> Seq.toArray

let getValue (registers:Int32 array) paramType paramValue =
    match paramType with
    | 'i' -> paramValue
    | t when t = 'r' || t = 't' || t = 'd' || t = 'l' || t = 'n' -> if (paramValue |> int > registers.Length - 1) then Int32.MinValue else registers.[paramValue]
    | _ -> raise (Exception("Invalid param type " + paramType.ToString()))

let getOperation op : Int32 -> Int32 -> Int32 =
    match op with
    | "ad" -> (+)
    | "mu" -> (*)
    | "ba" -> (&&&)
    | "bo" -> (|||)
    | "se" -> fun a _ -> a
    | "gt" -> fun a b -> if a > b then 1 else 0
    | "eq" -> fun a b -> if a = b then 1 else 0
    | _ -> raise (Exception("Invalid operation name " + op))

let executeOperation (registers:Int32 array) op firstParamType secondParamType firstParam secondParam thirdParam =
    registers.[thirdParam |> int] <- (getOperation op) (getValue registers firstParamType firstParam) (getValue registers secondParamType secondParam)
    registers

let executeConventionNamed (registers:Int32 array) (instruction:Instruction) =
    let opNameFixedForSeti =
        match instruction.Operation with
        | "seti" -> "seit"
        | "setr" -> "sert"
        | _ -> instruction.Operation
    executeOperation registers (opNameFixedForSeti.Substring(0, 2)) opNameFixedForSeti.[2] opNameFixedForSeti.[3] instruction.Parameters.[0] instruction.Parameters.[1] instruction.Parameters.[2]

let getNextInstructionIndex context =
    let index = context.Registers.[context.InstructionPointerIndex]
    if index >= 0 && index < (context.Instructions.Length)
    then Some(index |> int)
    else None

let executeOnce context = 
    let currentInstruction = context.Instructions.[context.Registers.[context.InstructionPointerIndex] |> int]
    executeConventionNamed context.Registers currentInstruction

let incrementInstructionPointerRegister context = 
    context.Registers.[context.InstructionPointerIndex] <- context.Registers.[context.InstructionPointerIndex] + 1

let execute context = 
    let rec execute_rec context =
        match getNextInstructionIndex context with
        | None -> Some(context)
        | Some(index) -> 
            let currentInstruction = context.Instructions.[index]
            executeConventionNamed context.Registers currentInstruction |> ignore       
            if currentInstruction.Operation = "bani" && context.Registers.[currentInstruction.Parameters.[2] |> int] = 0
            then
                //Since this triggers an infinite loop (according to specification), we will never get a return value
                None
            else            
                incrementInstructionPointerRegister context
                execute_rec context
    execute_rec context  

let getValueOfIndexZeroThatHalts data =
    let rec getValueOfIndexZeroThatHalts_rec currentValueForIndexZero =
        match data |> parseData currentValueForIndexZero |> execute with 
        | None -> getValueOfIndexZeroThatHalts_rec (currentValueForIndexZero + 72)
        | Some (_) -> currentValueForIndexZero
    getValueOfIndexZeroThatHalts_rec 72

[<EntryPoint>]
let main argv =
    inputData |> getValueOfIndexZeroThatHalts |> printfn "Result: %A"

    printfn "Hello World from F#!"
    0 // return an integer exit code
