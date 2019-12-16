// Learn more about F# at http://fsharp.org

open System
open System.Runtime.CompilerServices;

let convertToMap arr = 
    new Map<int32, int64> (arr |> Array.mapi (fun index value -> (index, value)))

let data = [|1102L; 34463338L; 34463338L; 63L; 1007L; 63L; 34463338L; 63L; 1005L; 63L; 53L; 1102L; 3L; 1L; 1000L; 109L; 988L; 209L; 12L; 9L; 1000L; 209L; 6L; 209L; 3L; 203L; 0L; 1008L; 1000L; 1L; 63L; 1005L; 63L; 65L; 1008L; 1000L; 2L; 63L; 1005L; 63L; 904L; 1008L; 1000L; 0L; 63L; 1005L; 63L; 58L; 4L; 25L; 104L; 0L; 99L; 4L; 0L; 104L; 0L; 99L; 4L; 17L; 104L; 0L; 99L; 0L; 0L; 1102L; 1L; 30L; 1010L; 1102L; 1L; 38L; 1008L; 1102L; 1L; 0L; 1020L; 1102L; 22L; 1L; 1007L; 1102L; 26L; 1L; 1015L; 1102L; 31L; 1L; 1013L; 1102L; 1L; 27L; 1014L; 1101L; 0L; 23L; 1012L; 1101L; 0L; 37L; 1006L; 1102L; 735L; 1L; 1028L; 1102L; 1L; 24L; 1009L; 1102L; 1L; 28L; 1019L; 1102L; 20L; 1L; 1017L; 1101L; 34L; 0L; 1001L; 1101L; 259L; 0L; 1026L; 1101L; 0L; 33L; 1018L; 1102L; 1L; 901L; 1024L; 1101L; 21L; 0L; 1016L; 1101L; 36L; 0L; 1011L; 1102L; 730L; 1L; 1029L; 1101L; 1L; 0L; 1021L; 1102L; 1L; 509L; 1022L; 1102L; 39L; 1L; 1005L; 1101L; 35L; 0L; 1000L; 1102L; 1L; 506L; 1023L; 1101L; 0L; 892L; 1025L; 1101L; 256L; 0L; 1027L; 1101L; 25L; 0L; 1002L; 1102L; 1L; 29L; 1004L; 1102L; 32L; 1L; 1003L; 109L; 9L; 1202L; -3L; 1L; 63L; 1008L; 63L; 39L; 63L; 1005L; 63L; 205L; 1001L; 64L; 1L; 64L; 1106L; 0L; 207L; 4L; 187L; 1002L; 64L; 2L; 64L; 109L; -2L; 1208L; -4L; 35L; 63L; 1005L; 63L; 227L; 1001L; 64L; 1L; 64L; 1105L; 1L; 229L; 4L; 213L; 1002L; 64L; 2L; 64L; 109L; 5L; 1206L; 8L; 243L; 4L; 235L; 1106L; 0L; 247L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; 14L; 2106L; 0L; 1L; 1105L; 1L; 265L; 4L; 253L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; -25L; 1201L; 4L; 0L; 63L; 1008L; 63L; 40L; 63L; 1005L; 63L; 285L; 1106L; 0L; 291L; 4L; 271L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; 14L; 2107L; 37L; -7L; 63L; 1005L; 63L; 313L; 4L; 297L; 1001L; 64L; 1L; 64L; 1106L; 0L; 313L; 1002L; 64L; 2L; 64L; 109L; -7L; 21101L; 40L; 0L; 5L; 1008L; 1013L; 37L; 63L; 1005L; 63L; 333L; 1105L; 1L; 339L; 4L; 319L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; -7L; 1207L; 0L; 33L; 63L; 1005L; 63L; 355L; 1106L; 0L; 361L; 4L; 345L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; 7L; 21102L; 41L; 1L; 9L; 1008L; 1017L; 41L; 63L; 1005L; 63L; 387L; 4L; 367L; 1001L; 64L; 1L; 64L; 1106L; 0L; 387L; 1002L; 64L; 2L; 64L; 109L; -1L; 21102L; 42L; 1L; 10L; 1008L; 1017L; 43L; 63L; 1005L; 63L; 411L; 1001L; 64L; 1L; 64L; 1106L; 0L; 413L; 4L; 393L; 1002L; 64L; 2L; 64L; 109L; -5L; 21101L; 43L; 0L; 8L; 1008L; 1010L; 43L; 63L; 1005L; 63L; 435L; 4L; 419L; 1106L; 0L; 439L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; 16L; 1206L; 3L; 455L; 1001L; 64L; 1L; 64L; 1106L; 0L; 457L; 4L; 445L; 1002L; 64L; 2L; 64L; 109L; -8L; 21107L; 44L; 45L; 7L; 1005L; 1017L; 479L; 4L; 463L; 1001L; 64L; 1L; 64L; 1106L; 0L; 479L; 1002L; 64L; 2L; 64L; 109L; 6L; 1205L; 5L; 497L; 4L; 485L; 1001L; 64L; 1L; 64L; 1106L; 0L; 497L; 1002L; 64L; 2L; 64L; 109L; 1L; 2105L; 1L; 6L; 1105L; 1L; 515L; 4L; 503L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; -10L; 2108L; 36L; -1L; 63L; 1005L; 63L; 535L; 1001L; 64L; 1L; 64L; 1105L; 1L; 537L; 4L; 521L; 1002L; 64L; 2L; 64L; 109L; -12L; 2101L; 0L; 6L; 63L; 1008L; 63L; 32L; 63L; 1005L; 63L; 561L; 1001L; 64L; 1L; 64L; 1105L; 1L; 563L; 4L; 543L; 1002L; 64L; 2L; 64L; 109L; 25L; 21108L; 45L; 46L; -2L; 1005L; 1018L; 583L; 1001L; 64L; 1L; 64L; 1105L; 1L; 585L; 4L; 569L; 1002L; 64L; 2L; 64L; 109L; -23L; 2108L; 34L; 4L; 63L; 1005L; 63L; 607L; 4L; 591L; 1001L; 64L; 1L; 64L; 1106L; 0L; 607L; 1002L; 64L; 2L; 64L; 109L; 3L; 1202L; 7L; 1L; 63L; 1008L; 63L; 22L; 63L; 1005L; 63L; 633L; 4L; 613L; 1001L; 64L; 1L; 64L; 1106L; 0L; 633L; 1002L; 64L; 2L; 64L; 109L; 12L; 21108L; 46L; 46L; 3L; 1005L; 1015L; 651L; 4L; 639L; 1106L; 0L; 655L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; -5L; 2102L; 1L; -1L; 63L; 1008L; 63L; 35L; 63L; 1005L; 63L; 679L; 1001L; 64L; 1L; 64L; 1105L; 1L; 681L; 4L; 661L; 1002L; 64L; 2L; 64L; 109L; 13L; 21107L; 47L; 46L; -7L; 1005L; 1013L; 701L; 1001L; 64L; 1L; 64L; 1105L; 1L; 703L; 4L; 687L; 1002L; 64L; 2L; 64L; 109L; -2L; 1205L; 2L; 715L; 1106L; 0L; 721L; 4L; 709L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; 17L; 2106L; 0L; -7L; 4L; 727L; 1105L; 1L; 739L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; -23L; 2107L; 38L; -6L; 63L; 1005L; 63L; 759L; 1001L; 64L; 1L; 64L; 1106L; 0L; 761L; 4L; 745L; 1002L; 64L; 2L; 64L; 109L; -3L; 1207L; -4L; 40L; 63L; 1005L; 63L; 779L; 4L; 767L; 1105L; 1L; 783L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; -8L; 2101L; 0L; -1L; 63L; 1008L; 63L; 35L; 63L; 1005L; 63L; 809L; 4L; 789L; 1001L; 64L; 1L; 64L; 1105L; 1L; 809L; 1002L; 64L; 2L; 64L; 109L; -6L; 2102L; 1L; 8L; 63L; 1008L; 63L; 32L; 63L; 1005L; 63L; 835L; 4L; 815L; 1001L; 64L; 1L; 64L; 1106L; 0L; 835L; 1002L; 64L; 2L; 64L; 109L; 6L; 1201L; 5L; 0L; 63L; 1008L; 63L; 37L; 63L; 1005L; 63L; 857L; 4L; 841L; 1106L; 0L; 861L; 1001L; 64L; 1L; 64L; 1002L; 64L; 2L; 64L; 109L; 2L; 1208L; 0L; 32L; 63L; 1005L; 63L; 883L; 4L; 867L; 1001L; 64L; 1L; 64L; 1106L; 0L; 883L; 1002L; 64L; 2L; 64L; 109L; 23L; 2105L; 1L; -2L; 4L; 889L; 1001L; 64L; 1L; 64L; 1106L; 0L; 901L; 4L; 64L; 99L; 21102L; 27L; 1L; 1L; 21101L; 0L; 915L; 0L; 1106L; 0L; 922L; 21201L; 1L; 55337L; 1L; 204L; 1L; 99L; 109L; 3L; 1207L; -2L; 3L; 63L; 1005L; 63L; 964L; 21201L; -2L; -1L; 1L; 21101L; 0L; 942L; 0L; 1105L; 1L; 922L; 21202L; 1L; 1L; -1L; 21201L; -2L; -3L; 1L; 21102L; 957L; 1L; 0L; 1105L; 1L; 922L; 22201L; 1L; -1L; -2L; 1106L; 0L; 968L; 21201L; -2L; 0L; -2L; 109L; -3L; 2105L; 1L; 0L|] |> convertToMap

let sample2 = [|1102L; 34915192L; 34915192L; 7L; 4L; 7L; 99L; 0L |] |> convertToMap
let sample3 = [|104L; 1125899906842624L; 99L|] |> convertToMap

type Context = 
    {
        InputStream: int64 list
        OutputStream: int64 list
        Data : Map<int32, int64>
        InstructionPointer : int
        HasHalted : bool
        IsAwaitingInput : bool
        RelativeBaseOffset : int
    }

type Parameter = 
    | Immediate of value: int64
    | Position of index: int
    | Relative of offset: int
    
    member this.GetValue (context:Context) : int64 =
        match this with
        | Immediate value -> value
        | Position index -> if context.Data |> Map.containsKey index then context.Data.[index] else 0L
        | Relative offset -> 
            let index = context.RelativeBaseOffset + offset
            if context.Data |> Map.containsKey index then context.Data.[index] else 0L

type Context with
    member this.WithDataEntryReplaced (indexParameter:Parameter) newValue =
        let index = 
            match indexParameter with
            | Position index -> index
            | Relative offset -> this.RelativeBaseOffset + offset
            | Immediate _ -> failwith "Shouldn't end up with an immediate parameter for location of where to store data"
        {
            this with 
                Data = 
                    if this.Data |> Map.containsKey index
                        then this.Data |> Map.remove index |> Map.add index newValue
                        else this.Data |> Map.add index newValue
        }

    static member CreateWithInputStream inputStream data =
        {   
            Context.Data = data
            InputStream = inputStream
            OutputStream = []
            InstructionPointer = 0
            HasHalted = false
            IsAwaitingInput = false
            RelativeBaseOffset = 0
        }

    static member Create = Context.CreateWithInputStream []

type OpCode =
    | Halt     
    | Add of left: Parameter * right: Parameter * registryIndex: Parameter
    | Multiply of left: Parameter * right: Parameter * registryIndex: Parameter
    | Input of registryIndex: Parameter
    | Output of param: Parameter
    | JumpIfTrue of compare: Parameter * jumpTo: Parameter
    | JumpIfFalse of compare: Parameter * jumpTo: Parameter
    | LessThan of left: Parameter * right: Parameter * registryIndex: Parameter
    | Equals of left: Parameter * right: Parameter * registryIndex: Parameter
    | AdjustRelativeBase of offset : Parameter

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
        | AdjustRelativeBase _ -> 2

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
            | p when p <> 0L -> {context with InstructionPointer = int32(p2.GetValue(context))}
            | _ -> context
        | JumpIfFalse(p1, p2) -> 
            match p1.GetValue(context) with
            | p when p = 0L -> {context with InstructionPointer = int32(p2.GetValue(context))}
            | _ -> context
        | LessThan(l, r, o) -> context.WithDataEntryReplaced o (if l.GetValue(context) < r.GetValue(context) then 1L else 0L)
        | Equals(l, r, o) -> context.WithDataEntryReplaced o (if l.GetValue(context) = r.GetValue(context) then 1L else 0L)
        | AdjustRelativeBase offset -> {context with RelativeBaseOffset = context.RelativeBaseOffset + int32(offset.GetValue(context))}

let getCurrentOperation context =
    let instruction = context.Data.[context.InstructionPointer]
    let opCode = instruction % 100L
    let getParameterMode paramIndex = 
        let comparer = pown 10L (paramIndex + 2)
        (instruction / comparer) % 10L
    let getParameter paramIndex : Parameter =
        let mode = getParameterMode paramIndex
        let value = context.Data.[context.InstructionPointer + paramIndex + 1]
        match mode with 
        | 0L -> Position (int32(value))
        | 1L -> Immediate value
        | 2L -> Relative (int32(value))
        | unknown -> failwith ("Unknown parameter mode " + unknown.ToString() + " at index " + context.InstructionPointer.ToString() + ". Instruction was: " + instruction.ToString())        
    let getOutputParameter paramIndex = // int32(data.[index + paramIndex + 1])
        let p = getParameter paramIndex
        match p with
        | Immediate _ -> failwith ("Instruction " + instruction.ToString() + " seems to be missbehaving... Shouldn't have immediate mode on an output parameter")
        | _ -> p
        
    match opCode with
    | 99L -> Halt
    | 1L -> Add(getParameter(0), getParameter(1), getOutputParameter(2))
    | 2L -> Multiply(getParameter(0), getParameter(1), getOutputParameter(2))
    | 3L -> Input(getOutputParameter(0))
    | 4L -> Output(getParameter(0))
    | 5L -> JumpIfTrue(getParameter(0), getParameter(1))
    | 6L -> JumpIfFalse(getParameter(0), getParameter(1))
    | 7L -> LessThan(getParameter(0), getParameter(1), getOutputParameter(2))
    | 8L -> Equals(getParameter(0), getParameter(1), getOutputParameter(2))
    | 9L -> AdjustRelativeBase(getParameter(0))
    | unknown -> failwith ("Unknown operation requested: " + unknown.ToString() + ", at index " + context.InstructionPointer.ToString() + ". Instruction was: " + instruction.ToString())

let rec runProgram context =
    let opCode = context |> getCurrentOperation
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

[<EntryPoint>]
let main argv =

    sample2 |> Context.Create |> runProgram |> printfn "Sample 2: \n%A\n"
    sample3 |> Context.Create |> runProgram |> printfn "Sample 3: \n%A\n"
    data |> Context.CreateWithInputStream [1L] |> runProgram |> printfn "Part 1 Solution: \n%A\n"
    data |> Context.CreateWithInputStream [2L] |> runProgram |> printfn "Part 2 Solution: \n%A\n"
    0 // return an integer exit code
