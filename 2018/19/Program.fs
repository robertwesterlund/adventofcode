// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Threading
open System.Threading
open System.Threading

let sampleData = File.ReadAllLines(".\\sample.data")
let inputData = File.ReadAllLines(".\\input.data")

type Instruction =
    {
        Operation : string
        Parameters : bigint[]
    }

type ExecutionContext = 
    {
        InstructionPointerIndex : int
        Registers : bigint[]
        Instructions : Instruction[]
    }

let parseInstructions (data:seq<string>) = 
    data 
        |> Seq.map (fun line ->
            let split = line.Split(' ')
            {
                Instruction.Operation = split.[0]
                Instruction.Parameters = [| bigint.Parse(split.[1]); bigint.Parse(split.[2]); bigint.Parse(split.[3]) |]
            }
        )

let parseData valueAtRegisterZero (data:string[]) =
    let instructionPointerIndex = Int32.Parse(data.[0].Substring("#ip ".Length))
    let instructions = parseInstructions (data |> Seq.skip 1) |> Seq.toArray
    {
        ExecutionContext.InstructionPointerIndex = instructionPointerIndex
        ExecutionContext.Registers = [| valueAtRegisterZero;0I;0I;0I;0I;0I |]
        ExecutionContext.Instructions = instructions
    }


let parseDataArray (numberPart:string) =
    numberPart.Split(' ') |> Seq.map (fun num -> bigint.Parse(num.Trim(','))) |> Seq.toArray

let getValue (registers:bigint array) paramType paramValue =
    match paramType with
    | 'i' -> paramValue
    | t when t = 'r' || t = 't' || t = 'd' || t = 'l' || t = 'n' -> if (paramValue |> int > registers.Length - 1) then -99999999999999999999999999999I else registers.[paramValue |> int]
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

let executeOperation (registers:bigint array) op firstParamType secondParamType firstParam secondParam thirdParam =
    registers.[thirdParam |> int] <- (getOperation op) (getValue registers firstParamType firstParam) (getValue registers secondParamType secondParam)
    registers

let executeConventionNamed (registers:bigint array) (instruction:Instruction) =
    let opNameFixedForSeti =
        match instruction.Operation with
        | "seti" -> "seit"
        | "setr" -> "sert"
        | _ -> instruction.Operation
    executeOperation registers (opNameFixedForSeti.Substring(0, 2)) opNameFixedForSeti.[2] opNameFixedForSeti.[3] instruction.Parameters.[0] instruction.Parameters.[1] instruction.Parameters.[2]

let getNextInstructionIndex context =
    let index = context.Registers.[context.InstructionPointerIndex]
    if index >= 0I && index < (context.Instructions.Length |> bigint)
    then Some(index |> int)
    else None

let executeOnce context = 
    let currentInstruction = context.Instructions.[context.Registers.[context.InstructionPointerIndex] |> int]
    executeConventionNamed context.Registers currentInstruction

let incrementInstructionPointerRegister context = 
    context.Registers.[context.InstructionPointerIndex] <- context.Registers.[context.InstructionPointerIndex] + 1I

let execute context = 
    let rec execute_rec context =
        match getNextInstructionIndex context with
        | None -> context
        | Some(index) -> 
            let currentInstruction = context.Instructions.[index]
            executeConventionNamed context.Registers currentInstruction |> ignore       
            incrementInstructionPointerRegister context
            execute_rec context
    execute_rec context  

let getValueAtRegisterZero context = 
    context.Registers.[0]


let doCalculation (_5:bigint) =
    let highestValueWeNeedToCheck = _5 / 2I 
    let rec doCalculation_rec currentSum currentValue =
        match currentValue <= highestValueWeNeedToCheck with
        | true when _5 % currentValue = 0I -> doCalculation_rec (currentSum + currentValue) (currentValue + 1I)
        | true -> doCalculation_rec currentSum (currentValue + 1I)
        | false -> currentSum
    _5 + (doCalculation_rec 0I 1I)
  
[<EntryPoint>]
let main argv =
    sampleData |> parseData 0I |> execute |> getValueAtRegisterZero |> printfn "Result for Sample Data was: %A"
    inputData |> parseData 0I |> execute |> getValueAtRegisterZero |> printfn "Result for Input Data was: %A"
    //inputData |> parseData 1I |> execute |> getValueAtRegisterZero |> printfn "Result for Input Data starting with 0I on Register Zero was: %A"
    doCalculation 876I |> printfn "Result for doing the calculation with target 876 was: %A"
    doCalculation 10551276I |> printfn "Result for doing the calculation with target 876 was: %A"

    printfn "Hello World from F#!"
    0 // return an integer exit code
