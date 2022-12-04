type Elf = { calorieList: List<int64> }

let parseInput (data: string) =
    data.Replace("\r\n", "\n").Split("\n\n")
    |> Array.map (fun elfData ->
        { calorieList =
            elfData.Split("\n")
            |> Array.map (fun c -> System.Int64.Parse(c))
            |> Array.toList })
    |> Array.toList

let part1 input =
    input
    |> parseInput
    |> List.map (fun e -> e.calorieList |> List.sum)
    |> List.sortDescending
    |> List.head

let part2 input =
    input
    |> parseInput
    |> List.map (fun e -> e.calorieList |> List.sum)
    |> List.sortDescending
    |> List.take 3
    |> List.sum

Input.testData
|> part1
|> printfn "Part 1 - testdata: %A"

Input.realData
|> part1
|> printfn "Part 1 - realdata: %A"

Input.testData
|> part2
|> printfn "Part 2 - testdata: %A"

Input.realData
|> part2
|> printfn "Part 2 - realdata: %A"
