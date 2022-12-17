open System

type Context = { RegisterX: int32; CycleCount: int64 }

type Instruction =
    | ADDX of value: int32
    | NOOP

    member this.apply context =
        match this with
        | ADDX value ->
            [ { context with CycleCount = context.CycleCount + 1L }
              { context with
                  CycleCount = context.CycleCount + 2L
                  RegisterX = context.RegisterX + value } ]
        | NOOP -> [ { context with CycleCount = context.CycleCount + 1L } ]

let parse (input: string) =
    input.Replace("\r\n", "\n").Split("\n")
    |> Array.toList
    |> List.map (fun line ->
        let split = line.Split(" ")

        match split.[0] with
        | "addx" -> ADDX(Int32.Parse split.[1])
        | "noop" -> NOOP)

let calculateAllStates input =
    let (contexts, _) =
        input
        |> parse
        |> List.mapFold
            (fun context instruction ->
                let output = instruction.apply context
                (output, output |> List.last))
            { RegisterX = 1; CycleCount = 1L }

    contexts |> List.collect id

let part1 input =
    input
    |> calculateAllStates
    |> List.filter (fun c -> (c.CycleCount - 20L) % 40L = 0L)
    |> List.fold (fun acc c -> acc + (c.CycleCount * (int64) c.RegisterX)) 0L

let part2 input =
    let screen =
        [| 1 .. 6 |]
        |> Array.map (fun _ -> [| 1 .. 40 |] |> Array.map (fun _ -> '.'))

    input
    |> calculateAllStates
    |> List.iter (fun context ->
        let crtScreenPosition = (int) ((context.CycleCount - 1L) % 240L)
        let yPosition = crtScreenPosition / 40
        let xPosition = crtScreenPosition % 40

        if Math.Abs(xPosition - context.RegisterX) <= 1 then
            screen.[yPosition].[xPosition] <- '#')

    let sb = new Text.StringBuilder()

    screen
    |> Array.toList
    |> List.iter (fun line ->
        line
        |> Array.toList
        |> List.iter (fun c -> sb.Append(c) |> ignore)

        sb.Append('\n') |> ignore)

    sb.ToString()

Input.testData
|> part1
|> printfn "Part 1 - test data: %i"

Input.realData
|> part1
|> printfn "Part 1 - real data: %i"

Input.testData
|> part2
|> printfn "Part 2 - test data: \n%s"

Input.realData
|> part2
|> printfn "Part 2 - real data: \n%s"
