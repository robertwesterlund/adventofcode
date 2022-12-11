type Instruction =
    { Count: int32
      Source: int32
      Destination: int32 }

let parse (input: string) =
    let [| initState; instructions |] =
        input.Replace("\r\n", "\n").Split("\n\n")

    let initStateLines = initState.Split("\n")

    let stackCountLine =
        initStateLines.[initStateLines.Length - 1]

    let numberOfStacks =
        System.Int32.Parse(
            stackCountLine.[stackCountLine.Length - 1]
                .ToString()
        )

    (initStateLines
     |> Array.fold
         (fun acc curr ->
             match curr.Trim().[0] with
             | '[' ->
                 acc
                 |> List.mapi (fun index stack ->
                     if curr.Length > index * 4 then
                         match curr.[(index * 4) + 1] with
                         | ' ' -> stack
                         | x -> stack @ [ x ]
                     else
                         stack)
             | _ -> acc)
         ([ 1 .. numberOfStacks ] |> List.map (fun _ -> [])),
     (instructions.Split("\n")
      |> Array.toList
      |> List.map (fun (inst: string) ->
          let split = inst.Split(" ")

          { Count = System.Int32.Parse(split.[1])
            Source = System.Int32.Parse(split.[3])
            Destination = System.Int32.Parse(split.[5]) })))

let move groupGrabConverter input =
    let (state, instructions) = input |> parse

    instructions
    |> List.fold
        (fun acc curr ->
            acc
            |> List.mapi (fun index stack ->
                match index + 1 with
                | x when x = curr.Source -> stack |> List.skip curr.Count
                | x when x = curr.Destination ->
                    List.append
                        (acc.[curr.Source - 1]
                         |> List.take curr.Count
                         |> groupGrabConverter)
                        stack
                | _ -> stack))
        state
    |> List.fold (fun acc stack -> acc @ [ stack |> List.head ]) []
    |> List.fold (fun acc curr -> acc + curr.ToString()) ""

let part1 = move List.rev
let part2 = move (fun s -> s)

Input.testData
|> part1
|> printfn "Part 1 - test data: %A"

Input.realData
|> part1
|> printfn "Part 1 - real data: %A"

Input.testData
|> part2
|> printfn "Part 2 - test data: %A"

Input.realData
|> part2
|> printfn "Part 2 - real data: %A"
