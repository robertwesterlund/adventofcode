type Rucksack =
    { C1: char array
      C2: char array
      AllContent: string }

let parseInput (input: string) =
    input.Replace("\r\n", "\n").Split("\n")
    |> Array.map (fun r ->
        let middle = r.Length / 2

        { C1 = r.Substring(0, middle).ToCharArray()
          C2 = r.Substring(middle).ToCharArray()
          AllContent = r })
    |> Array.toList

let getValue (character: char) =
    match System.Char.IsUpper(character) with
    | true -> ((int64) character) - 64L + 26L
    | false -> ((int64) character) - 96L

let part1 input =
    input
    |> parseInput
    |> List.map (fun r ->
        r.C1
        |> Array.find (fun c -> r.C2 |> Array.contains c))
    |> List.map getValue
    |> List.sum

let part2 input =
    input
    |> parseInput
    |> List.chunkBySize 3
    |> List.map (fun [ a; b; c ] ->
        a.AllContent.ToCharArray()
        |> Array.find (fun item ->
            b.AllContent.Contains(item)
            && c.AllContent.Contains(item)))
    |> List.map getValue
    |> List.sum

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
