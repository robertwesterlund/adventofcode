let testdata =
    "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"

let testdatapart2 =
    "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))"

let realdata =
    System.IO.File.ReadAllText("realdata.txt")

let regex =
    new System.Text.RegularExpressions.Regex(
        @"(?<command>mul|do|don't)\((?:(?<=mul\()(?<first>\d{1,3}),(?<second>\d{1,3})|(?<=don't\()|(?<=do\())\)",
        options = System.Text.RegularExpressions.RegexOptions.None,
        matchTimeout = System.TimeSpan.FromSeconds(1)
    )

let part1 data =
    regex.Matches(data)
    |> Seq.filter (fun m -> m.Groups.["command"].Value = "mul")
    |> Seq.map (fun m ->
        int64 m.Groups.["first"].Value
        * int64 m.Groups.["second"].Value)
    |> Seq.sum

let part2 data =
    regex.Matches(data)
    |> Seq.fold
        (fun (acc, shouldDo) m ->
            match m.Groups.["command"].Value with
            | "mul" ->
                if shouldDo then
                    (acc
                     + int64 m.Groups.["first"].Value
                       * int64 m.Groups.["second"].Value,
                     true)
                else
                    (acc, false)
            | "do" -> (acc, true)
            | "don't" -> (acc, false)
            | _ ->
                System.NotImplementedException("Unknown command: " + m.Groups.["command"].Value)
                |> raise)
        (0L, true)
    |> fst

testdata
|> part1
|> printfn "part 1 - test data - %i"

realdata
|> part1
|> printfn "part 1 - real data - %i"

testdatapart2
|> part2
|> printfn "part 2 - test data - %i"

realdata
|> part2
|> printfn "part 2 - real data - %i"
