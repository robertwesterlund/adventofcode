let getData filename =
    let fishes =
        System.IO.File.ReadAllText(filename).Split(',')
        |> Array.toList
        |> List.map (fun v -> System.Convert.ToInt32(v))
        |> List.groupBy (fun v -> v)
        |> List.map (fun (days, fishes) -> (days, fishes |> List.length |> System.Convert.ToInt64))
        |> Map.ofList

    [ 0 .. 8 ]
    |> List.map (fun v ->
        match fishes |> Map.containsKey v with
        | true -> (v, fishes.[v])
        | false -> (v, 0))
    |> Map.ofList

let letOneDayPass (fishes: Map<int, int64>) =
    [ 0 .. 8 ]
    |> List.fold
        (fun acc day ->
            match day with
            | 8 -> acc |> Map.add 8 fishes.[0]
            | 6 -> acc |> Map.add 6 (fishes.[0] + fishes.[7])
            | d -> acc |> Map.add d fishes.[d + 1])
        Map.empty

let letManyDaysPass numberOfDays (fishes: Map<int, int64>) =
    [ 1 .. numberOfDays ]
    |> List.fold (fun acc dayNumber -> letOneDayPass acc) fishes

let run numberOfDays filename =
    filename
    |> getData
    |> letManyDaysPass numberOfDays
    |> Map.fold (fun acc key value -> acc + value) 0L
    |> printfn "%s number of fish after %i days %O" filename numberOfDays

"testdata.txt" |> run 80
"input.txt" |> run 80
"testdata.txt" |> run 256
"input.txt" |> run 256
