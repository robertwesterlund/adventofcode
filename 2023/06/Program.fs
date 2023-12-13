open System

let testData =
    """
Time:      7  15   30
Distance:  9  40  200
    """
        .Replace("\r", "")
        .Trim()

let realData =
    """
Time:        35     69     68     87
Distance:   213   1168   1086   1248
    """
        .Replace("\r", "")
        .Trim()

type Race = { Time: decimal; Distance: decimal }

let parsePart1 (input: string) =
    let split = input.Split("\n")

    split.[0]
        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
    |> Seq.zip (
        split.[1]
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
    )
    |> Seq.skip 1
    |> Seq.map (fun (distance, time) ->
        { Distance = Decimal.Parse(distance)
          Time = Decimal.Parse(time) })

let parsePart2 (input: string) =
    let split = input.Split("\n")

    { Time = Decimal.Parse(split.[0].Split(" ", 2).[1].Replace(" ", ""))
      Distance = Decimal.Parse(split.[1].Split(" ", 2).[1].Replace(" ", "")) }

let findFirstWinner race =
    seq { 1m .. race.Time }
    |> Seq.find (fun t -> t * (race.Time - t) > race.Distance)

let findWinningOptions race =
    let firstWinner = race |> findFirstWinner

    (race.Time / 2m - firstWinner) * 2m + 1m

let run parsedInput =
    parsedInput
    |> Seq.map findWinningOptions
    |> Seq.fold (fun a c -> a * c) 1m

testData
|> parsePart1
|> run
|> printfn "Test data, part 1: %M"

[ testData |> parsePart2 ]
|> run
|> printfn "Test data, part 2: %M"

realData
|> parsePart1
|> run
|> printfn "Real data, part 1: %M"

[ realData |> parsePart2 ]
|> run
|> printfn "Real data, part 2: %M"
