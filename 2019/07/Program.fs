// Learn more about F# at http://fsharp.org

open System
open System.Runtime.CompilerServices;

let data = 
    [|3;8;1001;8;10;8;105;1;0;0;21;34;51;64;81;102;183;264;345;426;99999;3;9;102;2;9;9;1001;9;4;9;4;9;99;3;9;101;4;9;9;102;5;9;9;1001;9;2;9;4;9;99;3;9;101;3;9;9;1002;9;5;9;4;9;99;3;9;102;3;9;9;101;3;9;9;1002;9;4;9;4;9;99;3;9;1002;9;3;9;1001;9;5;9;1002;9;5;9;101;3;9;9;4;9;99;3;9;102;2;9;9;4;9;3;9;101;1;9;9;4;9;3;9;1001;9;1;9;4;9;3;9;101;1;9;9;4;9;3;9;101;2;9;9;4;9;3;9;102;2;9;9;4;9;3;9;101;2;9;9;4;9;3;9;102;2;9;9;4;9;3;9;102;2;9;9;4;9;3;9;102;2;9;9;4;9;99;3;9;101;2;9;9;4;9;3;9;1001;9;1;9;4;9;3;9;1002;9;2;9;4;9;3;9;1001;9;1;9;4;9;3;9;1001;9;2;9;4;9;3;9;101;1;9;9;4;9;3;9;1002;9;2;9;4;9;3;9;102;2;9;9;4;9;3;9;1002;9;2;9;4;9;3;9;101;1;9;9;4;9;99;3;9;1002;9;2;9;4;9;3;9;102;2;9;9;4;9;3;9;102;2;9;9;4;9;3;9;101;1;9;9;4;9;3;9;101;2;9;9;4;9;3;9;101;2;9;9;4;9;3;9;1002;9;2;9;4;9;3;9;1001;9;1;9;4;9;3;9;1001;9;2;9;4;9;3;9;1002;9;2;9;4;9;99;3;9;1001;9;1;9;4;9;3;9;102;2;9;9;4;9;3;9;1002;9;2;9;4;9;3;9;101;2;9;9;4;9;3;9;101;2;9;9;4;9;3;9;1002;9;2;9;4;9;3;9;102;2;9;9;4;9;3;9;1002;9;2;9;4;9;3;9;1001;9;1;9;4;9;3;9;1001;9;1;9;4;9;99;3;9;1002;9;2;9;4;9;3;9;102;2;9;9;4;9;3;9;1001;9;2;9;4;9;3;9;101;2;9;9;4;9;3;9;102;2;9;9;4;9;3;9;1001;9;1;9;4;9;3;9;1002;9;2;9;4;9;3;9;1001;9;2;9;4;9;3;9;102;2;9;9;4;9;3;9;101;1;9;9;4;9;99|] |> Array.toList

type Sample = {instructions: int list; expectedAnswer: int}

let part1Sample1 =
    {instructions = [3;15;3;16;1002;16;10;16;1;16;15;15;4;15;99;0;0]; expectedAnswer = 43210}

let part1Sample2 =
    {instructions = [3;23;3;24;1002;24;10;24;1002;23;-1;23;101;5;23;23;1;24;23;23;4;23;99;0;0]; expectedAnswer = 54321}

let part1Sample3 = 
    {instructions = [3;31;3;32;1002;32;10;32;1001;31;-2;31;1007;31;0;33;1002;33;7;33;1;33;31;31;1;32;31;31;4;31;99;0;0;0]; expectedAnswer = 65210}

let part2Sample1 = 
    {instructions = [3;26;1001;26;-4;26;3;27;1002;27;2;27;1;27;26;27;4;27;1001;28;-1;28;1005;28;6;99;0;0;5]; expectedAnswer = 139629729}

let part2Sample2 = 
    {instructions = [3;52;1001;52;-5;52;3;53;1;52;56;54;1007;54;5;55;1005;55;26;1001;54;-5;54;1105;1;12;1;53;54;53;1008;54;0;55;1001;55;1;55;2;53;55;53;4;53;1001;56;-1;56;1005;56;6;99;0;0;0;0;10]; expectedAnswer = 18216}

type Context = 
    {
        InputStream: int list
        OutputStream: int list
        Data : int list
        InstructionPointer : int
        HasHalted : bool
        IsAwaitingInput : bool 
    }
    member this.WithDataEntryReplaced index newValue =
        {this with Data = this.Data |> List.mapi (fun i d -> if i = index then newValue else d)}    

    static member Create data inputStream =
        {Context.Data = data; InputStream = inputStream; OutputStream = []; InstructionPointer = 0; HasHalted = false; IsAwaitingInput = false}

type Parameter = 
    | Immediate of value: int
    | Position of index: int
    
    member this.GetValue (context:Context) =
        match this with
        | Immediate value -> value
        | Position index -> context.Data.[index]    

type OpCode =
    | Halt     
    | Add of left: Parameter * right: Parameter * registryIndex: int
    | Multiply of left: Parameter * right: Parameter * registryIndex: int
    | Input of registryIndex: int
    | Output of param: Parameter
    | JumpIfTrue of compare: Parameter * jumpTo: Parameter
    | JumpIfFalse of compare: Parameter * jumpTo: Parameter
    | LessThan of left: Parameter * right: Parameter * registryIndex: int
    | Equals of left: Parameter * right: Parameter * registryIndex: int

    member this.CodeLength = 
        match this with 
        | Halt -> 1
        | Add _ -> 4
        | Multiply _ -> 4
        | Input _ -> 2
        | Output _ -> 2
        | JumpIfTrue _ -> 3
        | JumpIfFalse _ -> 3
        | LessThan _ -> 4
        | Equals _ -> 4

    member this.Apply (context: Context) = 
        match this with
        | Halt -> {context with HasHalted = true}
        | Add(l,r,o) -> context.WithDataEntryReplaced o (l.GetValue(context) + r.GetValue(context))
        | Multiply(l,r,o) -> context.WithDataEntryReplaced o (l.GetValue(context) * r.GetValue(context))
        | Input index ->
            match context.InputStream with
            | head::tail -> {context.WithDataEntryReplaced index head with InputStream = tail; IsAwaitingInput = false}
            | [] -> {context with IsAwaitingInput = true}
        | Output param -> {context with OutputStream = param.GetValue(context) :: context.OutputStream}
        | JumpIfTrue(p1, p2) -> 
            match p1.GetValue(context) with
            | p when p <> 0 -> {context with InstructionPointer = p2.GetValue(context)}
            | _ -> context
        | JumpIfFalse(p1, p2) -> 
            match p1.GetValue(context) with
            | p when p = 0 -> {context with InstructionPointer = p2.GetValue(context)}
            | _ -> context
        | LessThan(l, r, o) -> context.WithDataEntryReplaced o (if l.GetValue(context) < r.GetValue(context) then 1 else 0)
        | Equals(l, r, o) -> context.WithDataEntryReplaced o (if l.GetValue(context) = r.GetValue(context) then 1 else 0)

let getOperationAt (data: int list) index =
    let instruction = data.[index]
    let opCode = instruction % 100
    let getParameterMode paramIndex = 
        let comparer = pown 10 (paramIndex + 2)
        (instruction / comparer) % 10
    let getParameter paramIndex : Parameter =
        let mode = getParameterMode paramIndex
        let value = data.[index + paramIndex + 1]
        match mode with 
        | 0 -> Position value
        | 1 -> Immediate value 
        | unknown -> failwith ("Unknown parameter mode " + unknown.ToString() + " at index " + index.ToString() + ". Instruction was: " + instruction.ToString())        
    let getOutputParameter paramIndex = data.[index + paramIndex + 1]
        
    match opCode with
    | 99 -> Halt
    | 1 -> Add(getParameter(0), getParameter(1), getOutputParameter(2))
    | 2 -> Multiply(getParameter(0), getParameter(1), getOutputParameter(2))
    | 3 -> Input(getOutputParameter(0))
    | 4 -> Output(getParameter(0))
    | 5 -> JumpIfTrue(getParameter(0), getParameter(1))
    | 6 -> JumpIfFalse(getParameter(0), getParameter(1))
    | 7 -> LessThan(getParameter(0), getParameter(1), getOutputParameter(2))
    | 8 -> Equals(getParameter(0), getParameter(1), getOutputParameter(2))
    | unknown -> failwith ("Unknown operation requested: " + unknown.ToString() + ", at index " + index.ToString() + ". Instruction was: " + instruction.ToString())

let rec runProgram context =
    let opCode = getOperationAt context.Data context.InstructionPointer
    match opCode with
    | Halt -> 
        context |> opCode.Apply
    | _ -> 
        let newContext = opCode.Apply context
        if newContext.IsAwaitingInput 
        then newContext 
        else 
            match newContext.InstructionPointer with
            | p when p <> context.InstructionPointer -> 
                newContext |> runProgram
            | p ->
                {newContext with InstructionPointer = newContext.InstructionPointer + opCode.CodeLength} |> runProgram

let part1RunAmplifier instructions phaseSetting inputInstruction  =
    Context.Create instructions [phaseSetting; inputInstruction] |> runProgram

let part1RunAllAmplifiers instructions phaseSequence =
    let rec inner phasesLeft input =
        match phasesLeft with 
        | head::tail -> 
            let result = part1RunAmplifier instructions head input
            inner tail (result.OutputStream |> List.head)
        | [] -> input
    inner phaseSequence 0

let part1PossiblePhaseSequences : int list list = 
    let rec inner (phasesSelected:int list) : int list list=
        match phasesSelected |> List.length with
        | 5 -> [phasesSelected]
        | _ -> [0..4] |> List.filter (fun p -> not (phasesSelected |> List.contains p)) |> List.collect (fun p -> inner (p::phasesSelected))
    inner []

let part1RunAllCombinations instructions =
    part1PossiblePhaseSequences |> List.map (fun sequence ->
        let result = sequence |> part1RunAllAmplifiers instructions
        (result, sequence)
    ) |> List.sortByDescending (fun (result, _) -> result)

let part2PossiblePhaseSequences : int list list = 
    let rec inner (phasesSelected:int list) : int list list=
        match phasesSelected |> List.length with
        | 5 -> [phasesSelected]
        | _ -> [5..9] |> List.filter (fun p -> not (phasesSelected |> List.contains p)) |> List.collect (fun p -> inner (p::phasesSelected))
    inner []

let part2RunAllAmplifiers instructions phaseSequence =
    let mutable amplifierContexts = phaseSequence |> List.map (fun phase -> Context.Create instructions [phase]) |> List.toArray
    let mutable previousOutput = [0]
    while amplifierContexts |> Seq.filter (fun c -> not c.HasHalted) |> Seq.length > 0 do
        if previousOutput.Length = 0 && (amplifierContexts |> Seq.filter (fun c -> c.IsAwaitingInput && c.InputStream.Length = 0 && c.OutputStream.Length = 0)) |> Seq.length = amplifierContexts.Length then failwith "All are waiting for input, but none has input or output"
        for i = 0 to amplifierContexts.Length - 1 do
            amplifierContexts.[i] <- {amplifierContexts.[i] with InputStream = List.append amplifierContexts.[i].InputStream previousOutput}
            amplifierContexts.[i] <- amplifierContexts.[i] |> runProgram
            previousOutput <- amplifierContexts.[i].OutputStream
            amplifierContexts.[i] <- {amplifierContexts.[i] with OutputStream = []}
    previousOutput

let part2RunAllCombinations instructions =
    part2PossiblePhaseSequences |> List.map (fun sequence ->
        let result = sequence |> part2RunAllAmplifiers instructions
        (result |> List.head, sequence)
    ) |> List.sortByDescending (fun (result, _) -> result)

[<EntryPoint>]
let main argv =
    let runSample partExecutor sample =
        sample.instructions |> partExecutor |> List.head |> fun (result, sequence) -> printfn "Successful: %b, value: %i, sequence: %A" (result = sample.expectedAnswer) result sequence

    let part1TestSample =
        runSample part1RunAllCombinations

    part1Sample1 |> part1TestSample
    part1Sample2 |> part1TestSample
    part1Sample3 |> part1TestSample    

    data |> part1RunAllCombinations |> List.head |> fun (result, sequence) -> printfn "Value: %i, sequence: %A" result sequence

    let part2TestSample =
        runSample part2RunAllCombinations

    part2Sample1 |> part2TestSample
    part2Sample2 |> part2TestSample

    data |> part2RunAllCombinations |> List.head |> fun (result, sequence) -> printfn "Value: %i, sequence: %A" result sequence

    0 // return an integer exit code
