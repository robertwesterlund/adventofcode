// Learn more about F# at http://fsharp.org

open System

let getData () = 
    [|3;225;1;225;6;6;1100;1;238;225;104;0;1101;81;30;225;1102;9;63;225;1001;92;45;224;101;-83;224;224;4;224;102;8;223;223;101;2;224;224;1;224;223;223;1102;41;38;225;1002;165;73;224;101;-2920;224;224;4;224;102;8;223;223;101;4;224;224;1;223;224;223;1101;18;14;224;1001;224;-32;224;4;224;1002;223;8;223;101;3;224;224;1;224;223;223;1101;67;38;225;1102;54;62;224;1001;224;-3348;224;4;224;1002;223;8;223;1001;224;1;224;1;224;223;223;1;161;169;224;101;-62;224;224;4;224;1002;223;8;223;101;1;224;224;1;223;224;223;2;14;18;224;1001;224;-1890;224;4;224;1002;223;8;223;101;3;224;224;1;223;224;223;1101;20;25;225;1102;40;11;225;1102;42;58;225;101;76;217;224;101;-153;224;224;4;224;102;8;223;223;1001;224;5;224;1;224;223;223;102;11;43;224;1001;224;-451;224;4;224;1002;223;8;223;101;6;224;224;1;223;224;223;1102;77;23;225;4;223;99;0;0;0;677;0;0;0;0;0;0;0;0;0;0;0;1105;0;99999;1105;227;247;1105;1;99999;1005;227;99999;1005;0;256;1105;1;99999;1106;227;99999;1106;0;265;1105;1;99999;1006;0;99999;1006;227;274;1105;1;99999;1105;1;280;1105;1;99999;1;225;225;225;1101;294;0;0;105;1;0;1105;1;99999;1106;0;300;1105;1;99999;1;225;225;225;1101;314;0;0;106;0;0;1105;1;99999;8;226;677;224;1002;223;2;223;1006;224;329;1001;223;1;223;7;226;226;224;102;2;223;223;1006;224;344;101;1;223;223;108;677;677;224;1002;223;2;223;1006;224;359;101;1;223;223;1107;226;677;224;1002;223;2;223;1005;224;374;101;1;223;223;1008;677;226;224;1002;223;2;223;1005;224;389;101;1;223;223;1007;677;226;224;1002;223;2;223;1005;224;404;1001;223;1;223;1107;677;226;224;1002;223;2;223;1005;224;419;1001;223;1;223;108;677;226;224;102;2;223;223;1006;224;434;1001;223;1;223;7;226;677;224;102;2;223;223;1005;224;449;1001;223;1;223;107;226;226;224;102;2;223;223;1006;224;464;101;1;223;223;107;677;226;224;102;2;223;223;1006;224;479;101;1;223;223;1007;677;677;224;1002;223;2;223;1006;224;494;1001;223;1;223;1008;226;226;224;1002;223;2;223;1006;224;509;101;1;223;223;7;677;226;224;1002;223;2;223;1006;224;524;1001;223;1;223;1007;226;226;224;102;2;223;223;1006;224;539;101;1;223;223;8;677;226;224;1002;223;2;223;1006;224;554;101;1;223;223;1008;677;677;224;102;2;223;223;1006;224;569;101;1;223;223;1108;677;226;224;102;2;223;223;1005;224;584;101;1;223;223;107;677;677;224;102;2;223;223;1006;224;599;1001;223;1;223;1108;677;677;224;1002;223;2;223;1006;224;614;1001;223;1;223;1107;677;677;224;1002;223;2;223;1005;224;629;1001;223;1;223;108;226;226;224;1002;223;2;223;1005;224;644;101;1;223;223;8;226;226;224;1002;223;2;223;1005;224;659;101;1;223;223;1108;226;677;224;1002;223;2;223;1006;224;674;101;1;223;223;4;223;99;226|]

let getSample() =
    [|3;21;1008;21;8;20;1005;20;22;107;8;21;20;1006;20;31;1106;0;36;98;0;0;1002;21;125;20;4;20;1105;1;46;104;999;1105;1;46;1101;1000;1;20;4;20;1105;1;46;98;99|]

let inputStreamPart1 = 
    System.Collections.Generic.Queue([|1|])
let inputStreamPart2 = 
    System.Collections.Generic.Queue([|5|])

//Rewrite this to pass an immutable context instead of mutating global state...
let mutable currentInputStream = inputStreamPart1
let mutable instructionPointer = 0

let outputStream = 
    System.Collections.Generic.List<int>()

type Parameter = 
    | Immediate of value: int
    | Position of index: int
    
    member this.GetValue (data:int[]) =
        match this with
        | Immediate value -> value
        | Position index -> data.[index]

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

    member this.Apply (data:int[]) = 
        match this with
        | Halt -> failwith "Should never apply a halt operation"
        | Add(l,r,o) -> data.[o] <- l.GetValue(data) +  r.GetValue(data)
        | Multiply(l,r,o) -> data.[o] <- l.GetValue(data) *  r.GetValue(data)
        | Input index -> data.[index] <- currentInputStream.Dequeue()
        | Output param -> outputStream.Add(param.GetValue(data))
        | JumpIfTrue(p1, p2) -> 
            match p1.GetValue(data) with
            | p when p <> 0 -> instructionPointer <- p2.GetValue(data)
            | _ -> ()
        | JumpIfFalse(p1, p2) -> 
            match p1.GetValue(data) with
            | p when p = 0 -> instructionPointer <- p2.GetValue(data)
            | _ -> ()
        | LessThan(l, r, o) -> data.[o] <- if l.GetValue(data) < r.GetValue(data) then 1 else 0
        | Equals(l, r, o) -> data.[o] <- if l.GetValue(data) = r.GetValue(data) then 1 else 0

let getOperationAt (data: int[]) index =
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

let runProgram data =
    instructionPointer <- 0
    let getOp = getOperationAt data
    let rec continueProgram () =
        let opCode = getOp instructionPointer
        match opCode with
        | Halt -> data
        | _ -> 
            let pointerBefore = instructionPointer
            opCode.Apply data
            match instructionPointer with
            | p when p <> pointerBefore -> 
                continueProgram()
            | p ->
                instructionPointer <- instructionPointer + opCode.CodeLength
                continueProgram()
    continueProgram ()

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    getData() |> runProgram |> printfn "Result Part 1: %A"
    outputStream.ToArray() |> printfn "Output stream contains: %A"

    currentInputStream <- inputStreamPart2
    outputStream.Clear()
    getData() |> runProgram |> printfn "Result Part 2: %A"
    outputStream.ToArray() |> printfn "Output stream contains: %A"

    0 // return an integer exit code
