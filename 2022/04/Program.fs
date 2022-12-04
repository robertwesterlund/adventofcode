type Assignment =
    { lower: int64
      upper: int64
      set: Set<int64> }

let parse (input: string) =
    input.Replace("\r\n", "\n").Split("\n")
    |> Array.map (fun line ->
        line.Split(",")
        |> Array.map (fun assignment ->
            let [| lower; upper |] =
                assignment.Split("-")
                |> Array.map System.Int64.Parse

            { lower = lower
              upper = upper
              set = [ lower .. upper ] |> Set.ofList }))

let part1 input =
    input
    |> parse
    |> Array.filter (fun [| first; second |] ->
        first.set |> Set.isSubset second.set
        || second.set |> Set.isSubset first.set)
    |> Array.length

let part2 input =
    input
    |> parse
    |> Array.filter (fun [| first; second |] -> first.set |> Set.intersect second.set |> Set.count > 0)
    |> Array.length

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
