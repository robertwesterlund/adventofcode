let parse filename =
    System.IO.File.ReadAllText(filename).Split(',')
    |> Array.toList
    |> List.map (fun v -> System.Convert.ToInt64(v))

type Result = { position: int64; cost: int64 }

let fuelUsagePart1 position target = target - position |> abs

let fuelUsagePart2 position target =
    [ 0L .. ((max position target) - (min position target)) ]
    |> List.sum

let rec testMoves fuelUsageFunc positionsLeft data =
    match positionsLeft with
    | [] -> []
    | h :: t ->
        { position = h
          cost =
            data
            |> List.fold (fun acc curr -> acc + (fuelUsageFunc curr h)) 0L }
        :: (testMoves fuelUsageFunc t data)

let findPlan fuelUsageFunc datafilename =
    let data = datafilename |> parse

    let possiblePositions =
        [ (data |> List.min) .. (data |> List.max) ]

    let bestOption =
        testMoves fuelUsageFunc possiblePositions data
        |> List.minBy (fun c -> c.cost)

    datafilename |> printfn "File: %s"
    bestOption |> printfn "Result: %O"

"testdata.txt" |> findPlan fuelUsagePart1
"input.txt" |> findPlan fuelUsagePart1
"testdata.txt" |> findPlan fuelUsagePart2
"input.txt" |> findPlan fuelUsagePart2
