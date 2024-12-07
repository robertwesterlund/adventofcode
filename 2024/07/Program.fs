let testdata =
    System.IO.File.ReadAllLines("testdata.txt")

let realdata =
    System.IO.File.ReadAllLines("realdata.txt")

type Operator =
    | Add
    | Multiply
    | Concatenate
    member this.Apply(a, b) =
        match this with
        | Add -> a + b
        | Multiply -> a * b
        | Concatenate -> int64 (sprintf "%i%i" a b)

type Line =
    { TestValue: int64
      Numbers: int64 list }

let parse (data: string array) =
    data
    |> Seq.map (fun line ->
        let parts = line.Split(':')

        { TestValue = int64 parts.[0]
          Numbers =
            parts.[1]
                .Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
            |> Seq.map int64
            |> List.ofSeq })
    |> Seq.toList

let solve (allowedOperators: Operator list) data =
    let getAllOptions length =
        let rec _getOptions prev leftToAdd =
            match leftToAdd with
            | 0 -> [ prev ]
            | _ ->
                allowedOperators
                |> List.collect (fun op -> _getOptions (op :: prev) (leftToAdd - 1))

        _getOptions [] length

    data
    |> List.filter (fun line ->
        let options = getAllOptions (line.Numbers.Length - 1)

        options
        |> List.exists (fun opList ->
            let res =
                opList
                |> Seq.mapi (fun i op -> i, op)
                |> Seq.fold (fun acc (i, op) -> op.Apply(acc, line.Numbers.[i + 1])) line.Numbers.[0]

            res = line.TestValue))
    |> List.sumBy (fun line -> line.TestValue)

let part1 = solve [ Add; Multiply ]
let part2 = solve [ Add; Multiply; Concatenate ]

testdata
|> parse
|> part1
|> printfn "part 1 - testdata - %i"

realdata
|> parse
|> part1
|> printfn "part 1 - realdata - %i"

testdata
|> parse
|> part2
|> printfn "part 2 - testdata - %i"

realdata
|> parse
|> part2
|> printfn "part 2 - realdata - %i"
