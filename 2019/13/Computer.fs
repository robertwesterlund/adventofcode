module Computer

open System
open System.Runtime.CompilerServices

type Context =
    { InputStream: int64 list
      OutputStream: int64 list
      Data: Map<int32, int64>
      InstructionPointer: int
      HasHalted: bool
      IsAwaitingInput: bool
      RelativeBaseOffset: int }

type Parameter =
    | Immediate of value: int64
    | Position of index: int
    | Relative of offset: int

    member this.GetValue(context: Context): int64 =
        match this with
        | Immediate value -> value
        | Position index -> if context.Data |> Map.containsKey index then context.Data.[index] else 0L
        | Relative offset ->
            let index = context.RelativeBaseOffset + offset
            if context.Data |> Map.containsKey index then context.Data.[index] else 0L

type Context with

    member this.WithDataEntryReplaced (indexParameter: Parameter) newValue =
        let index =
            match indexParameter with
            | Position index -> index
            | Relative offset -> this.RelativeBaseOffset + offset
            | Immediate _ -> failwith "Shouldn't end up with an immediate parameter for location of where to store data"

        { this with
              Data =
                  if this.Data |> Map.containsKey index then
                      this.Data
                      |> Map.remove index
                      |> Map.add index newValue
                  else
                      this.Data |> Map.add index newValue }

    static member CreateWithInputStream inputStream data =
        { Context.Data = data
          InputStream = inputStream
          OutputStream = []
          InstructionPointer = 0
          HasHalted = false
          IsAwaitingInput = false
          RelativeBaseOffset = 0 }

    static member Create = Context.CreateWithInputStream []

    static member CreateFromDataArray data =
        let convertToMap arr =
            new Map<int32, int64>(arr
                                  |> Array.mapi (fun index value -> (index, value)))

        data |> convertToMap |> Context.Create

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
    | AdjustRelativeBase of offset: Parameter

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

    member this.Apply(context: Context) =
        match this with
        | Halt -> { context with HasHalted = true }
        | Add (l, r, o) -> context.WithDataEntryReplaced o (l.GetValue(context) + r.GetValue(context))
        | Multiply (l, r, o) -> context.WithDataEntryReplaced o (l.GetValue(context) * r.GetValue(context))
        | Input index ->
            match context.InputStream with
            | head :: tail ->
                { context.WithDataEntryReplaced index head with
                      InputStream = tail
                      IsAwaitingInput = false }
            | [] -> { context with IsAwaitingInput = true }
        | Output param ->
            { context with
                  OutputStream = param.GetValue(context) :: context.OutputStream }
        | JumpIfTrue (p1, p2) ->
            match p1.GetValue(context) with
            | p when p <> 0L ->
                { context with
                      InstructionPointer = int32 (p2.GetValue(context)) }
            | _ -> context
        | JumpIfFalse (p1, p2) ->
            match p1.GetValue(context) with
            | p when p = 0L ->
                { context with
                      InstructionPointer = int32 (p2.GetValue(context)) }
            | _ -> context
        | LessThan (l, r, o) ->
            context.WithDataEntryReplaced
                o
                (if l.GetValue(context) < r.GetValue(context)
                 then 1L
                 else 0L)
        | Equals (l, r, o) ->
            context.WithDataEntryReplaced
                o
                (if l.GetValue(context) = r.GetValue(context)
                 then 1L
                 else 0L)
        | AdjustRelativeBase offset ->
            { context with
                  RelativeBaseOffset =
                      context.RelativeBaseOffset
                      + int32 (offset.GetValue(context)) }

let getCurrentOperation context =
    let instruction =
        context.Data.[context.InstructionPointer]

    let opCode = instruction % 100L

    let getParameterMode paramIndex =
        let comparer = pown 10L (paramIndex + 2)
        (instruction / comparer) % 10L

    let getParameter paramIndex: Parameter =
        let mode = getParameterMode paramIndex

        let value =
            context.Data.[context.InstructionPointer + paramIndex + 1]

        match mode with
        | 0L -> Position(int32 (value))
        | 1L -> Immediate value
        | 2L -> Relative(int32 (value))
        | unknown ->
            failwith
                ("Unknown parameter mode "
                 + unknown.ToString()
                 + " at index "
                 + context.InstructionPointer.ToString()
                 + ". Instruction was: "
                 + instruction.ToString())

    let getOutputParameter paramIndex = // int32(data.[index + paramIndex + 1])
        let p = getParameter paramIndex
        match p with
        | Immediate _ ->
            failwith
                ("Instruction "
                 + instruction.ToString()
                 + " seems to be missbehaving... Shouldn't have immediate mode on an output parameter")
        | _ -> p

    match opCode with
    | 99L -> Halt
    | 1L -> Add(getParameter (0), getParameter (1), getOutputParameter (2))
    | 2L -> Multiply(getParameter (0), getParameter (1), getOutputParameter (2))
    | 3L -> Input(getOutputParameter (0))
    | 4L -> Output(getParameter (0))
    | 5L -> JumpIfTrue(getParameter (0), getParameter (1))
    | 6L -> JumpIfFalse(getParameter (0), getParameter (1))
    | 7L -> LessThan(getParameter (0), getParameter (1), getOutputParameter (2))
    | 8L -> Equals(getParameter (0), getParameter (1), getOutputParameter (2))
    | 9L -> AdjustRelativeBase(getParameter (0))
    | unknown ->
        failwith
            ("Unknown operation requested: "
             + unknown.ToString()
             + ", at index "
             + context.InstructionPointer.ToString()
             + ". Instruction was: "
             + instruction.ToString())

let rec runProgram context =
    let opCode = context |> getCurrentOperation
    match opCode with
    | Halt -> context |> opCode.Apply
    | _ ->
        let newContext = opCode.Apply context
        if newContext.IsAwaitingInput then
            newContext
        else
            match newContext.InstructionPointer with
            | p when p <> context.InstructionPointer -> newContext |> runProgram
            | p ->
                { newContext with
                      InstructionPointer = newContext.InstructionPointer + opCode.CodeLength }
                |> runProgram
