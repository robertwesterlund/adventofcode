let testdata =
    System.IO.File.ReadAllLines("testdata.txt")

let realdata =
    System.IO.File.ReadAllLines("realdata.txt")

type Report =
    { Levels: int64 list }
    member this.diffs =
        let (diffs, _) =
            this.Levels
            |> List.tail
            |> List.fold (fun (acc, prev) x -> (acc @ [ x - prev ], x)) (List.empty, this.Levels |> List.head)

        diffs

    member this.isAllIncreasing =
        this.diffs
        |> List.tryFind (fun x -> x < 0L)
        |> Option.isNone

    member this.isAllDecreasing =
        this.diffs
        |> List.tryFind (fun x -> x > 0L)
        |> Option.isNone

    member this.isAllLevelDiffsWithinThreshold =
        this.diffs
        |> List.tryFind (fun x -> abs x < 1L || abs x > 3L)
        |> Option.isNone

    member this.isSafePart1 =
        (this.isAllIncreasing || this.isAllDecreasing)
        && this.isAllLevelDiffsWithinThreshold

    member this.isSafePart2 =
        // The multiplier makes us only have to consider increasing sequences
        [ 1L; -1L ]
        |> List.tryFind (fun multiplier ->
            this.Levels
            |> List.mapi (fun i x -> this.Levels |> List.removeAt i)
            |> List.tryFind (fun list ->
                list
                |> List.map (fun x -> x * multiplier)
                |> List.pairwise
                |> List.tryFind (fun (a, b) -> a - b < 1L || a - b > 3L)
                |> Option.isNone)
            |> Option.isSome)
        |> Option.isSome

let parse (data: string array) =
    data
    |> Array.map (fun line ->
        { Levels =
            line.Split([| ' ' |], System.StringSplitOptions.RemoveEmptyEntries)
            |> Array.toList
            |> List.map (fun x -> int64 x) })
    |> Array.toList

testdata
|> parse
|> List.filter (fun l -> l.isSafePart1)
|> List.length
|> printfn "part 1 - test data: %i"

realdata
|> parse
|> List.filter (fun l -> l.isSafePart1)
|> List.length
|> printfn "part 1 - real data: %i"

testdata
|> parse
|> List.filter (fun l -> l.isSafePart2)
|> List.length
|> printfn "part 2 - test data: %i"

realdata
|> parse
|> List.filter (fun l -> l.isSafePart2)
|> List.length
|> printfn "part 2 - real data: %i"
