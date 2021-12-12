type Octopus = { Energy: int; Row: int; Column: int }

let parse filename =
    System.IO.File.ReadAllLines(filename)
    |> Array.toList
    |> List.indexed
    |> List.map (fun (rowIndex, line) ->
        line.ToCharArray()
        |> Array.toList
        |> List.indexed
        |> List.map (fun (columnIndex, digit) ->
            { Energy = System.Int32.Parse(digit.ToString())
              Row = rowIndex
              Column = columnIndex }))
    |> List.concat

let singleStep octos =
    let mutable state =
        octos
        |> List.map (fun o -> ({ o with Energy = o.Energy + 1 }, false))

    let mutable flashersThisStep = 0L

    let isNewFlasher =
        fun (o, hasFlashed) -> not hasFlashed && o.Energy > 9

    let isAdjacent o1 o2 =
        abs (o1.Row - o2.Row) <= 1
        && abs (o1.Column - o2.Column) <= 1

    while state
          |> List.tryFind isNewFlasher
          |> Option.isSome do
        let newFlashers = state |> List.filter isNewFlasher

        flashersThisStep <-
            flashersThisStep
            + (int64) (newFlashers |> List.length)

        state <-
            state
            |> List.map (fun (o, hasFlashed) ->
                let adjacentFlashers =
                    newFlashers
                    |> List.filter (fun (o2, _) -> isAdjacent o o2)
                    |> List.length

                ({ o with Energy = o.Energy + adjacentFlashers },
                 hasFlashed
                 || (newFlashers
                     |> List.tryFind (fun (o2, _) -> o2.Row = o.Row && o2.Column = o.Column)
                     |> Option.isSome)))

    (state
     |> List.map (fun (o, hasFlashed) -> { o with Energy = if hasFlashed then 0 else o.Energy }),
     flashersThisStep)

let nSteps numberOfSteps data =
    let (_, flashCount) =
        [ 1 .. numberOfSteps ]
        |> List.fold
            (fun (state, flashers) stepNumber ->
                let (s, f) = singleStep state
                //printfn "On step number %i, there were %i flashers" stepNumber f
                (s, flashers + f))
            (data, 0L)

    flashCount

let findStepWithAllFlashers data =
    let rec _inner stepNumber state =
        match state |> singleStep with
        | (_, 100L) -> stepNumber + 1
        | (newState, _) -> newState |> _inner (stepNumber + 1)

    _inner 0 data

let part1 filename =
    filename
    |> parse
    |> nSteps 100
    |> printfn "%s, flashers after 100 days: %i" filename

let part2 filename =
    filename
    |> parse
    |> findStepWithAllFlashers
    |> printfn "%s, first step where all octos flash: %i" filename

"testdata.txt" |> part1
"input.txt" |> part1

"testdata.txt" |> part2
"input.txt" |> part2
