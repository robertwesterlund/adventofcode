let parse filename =
    let data = filename |> System.IO.File.ReadAllLines

    (data.[0].ToCharArray()
     |> Array.toList
     |> List.windowed 2
     |> List.groupBy id
     |> List.map (fun (key, list) -> (key, (int64) (list |> List.length))),
     data
     |> Array.toList
     |> List.skip 2
     |> List.map (fun line ->
         let [| s; t |] = line.Split(" -> ")
         (s.ToCharArray() |> Array.toList, t.[0]))
     |> Map.ofList)

let step (map: Map<char list, char>) (data: (char list * int64) list) =
    let res =
        data
        |> List.map (fun ([ a; b ], count) ->
            let c = map.[[ a; b ]]
            [ ([ a; c ], count); ([ c; b ], count) ])
        |> List.concat

    let ((firstChars, _), (lastChars, _)) = (res |> List.head, res |> List.last)

    res
    |> List.groupBy (fun (key, count) -> key)
    |> List.map (fun (key, entries) -> (key, entries |> List.sumBy (fun (_, count) -> count)))
    // Ugly way to Keep the order of the first and last entry, but doesn't matter
    |> List.sortBy (fun (key, _) ->
        match key with
        | x when x = firstChars -> -1
        | x when x = lastChars -> 1
        | _ -> 0)

let run times filename =
    let (data, map) = filename |> parse

    let rec doXTimes x func data =
        match x with
        | 0 -> data
        | _ -> doXTimes (x - 1) func (data |> func)

    let mapTheRightNumberOfTimes = doXTimes times (step map)

    let result = data |> mapTheRightNumberOfTimes

    let ([ fst; _ ], _) = result |> List.head
    let ([ _; lst ], _) = result |> List.last

    let charCounts =
        result
        |> List.map (fun ([ a; b ], count) -> [ (a, count); (b, count) ])
        |> List.concat
        |> List.groupBy (fun (key, _) -> key)
        |> List.map (fun (key, list) -> (key, list |> List.sumBy (fun (_, c) -> c)))
        |> List.map (fun (k, c) -> (k, (if k = fst || k = lst then 1L + c else c)))
        |> Map.ofList

    let sorted =
        charCounts
        |> Map.toList
        |> List.map snd
        |> List.map (fun v -> v / 2L)
        |> List.sortDescending

    let (max, min) =
        (sorted |> List.head, sorted |> List.last)

    // firstLetter |> printfn "%O"
    // lastLetter |> printfn "%O"

    // adjustedCharCounts
    // |> Map.toList
    // |> List.iter (fun (c, count) -> printfn "%c: %i" c count)

    max - min
    |> printfn "%s after %i runs: %i" filename times

"testdata.txt" |> run 10
"input.txt" |> run 10
"testdata.txt" |> run 40
"input.txt" |> run 40
