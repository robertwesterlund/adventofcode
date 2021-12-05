open System

let testData =
    System.IO.File.ReadAllLines("testdata.txt")
    |> Array.toList

let input =
    System.IO.File.ReadAllLines("input.txt")
    |> Array.toList

type Position = { X: int32; Y: int32 }

let range a b =
    match a with
    | _ when a > b -> [ a .. -1 .. b ]
    | _ -> [ a .. b ]

let parse includeDiagonalLines lines =
    lines
    |> List.map (fun (line: string) ->
        let split =
            line.Split(" -> ")
            |> Array.map (fun pos ->
                let s = pos.Split(',')
                (Convert.ToInt32(s.[0]), Convert.ToInt32(s.[1]))
            //{X = Convert.ToInt32(s[0]); Y = Convert.ToInt32(s[1])}
            )

        match split with
        | [| (x1, y1); (x2, y2) |] when x1 = x2 && y1 = y2 -> [ (x1, y1) ]
        | [| (x1, y1); (x2, y2) |] when y1 = y2 -> range x1 x2 |> List.map (fun x -> (x, y1))
        | [| (x1, y1); (x2, y2) |] when x1 = x2 -> range y1 y2 |> List.map (fun y -> (x1, y))
        | [| (x1, y1); (x2, y2) |] ->
            match includeDiagonalLines with
            | false -> []
            | true -> List.zip (range x1 x2) (range y1 y2))

let countOverlappingStreamLocations includeDiagonalLines data =
    data
    |> parse includeDiagonalLines
    |> List.concat
    |> List.groupBy (fun v -> v)
    |> List.sumBy (fun (position, list) ->
        match list |> List.length with
        | l when l > 1 -> 1L
        | _ -> 0L)

testData
|> countOverlappingStreamLocations false
|> printfn "Test Data Overlapping Streams: %O"

input
|> countOverlappingStreamLocations false
|> printfn "Input Data Overlapping Streams: %O"

testData
|> countOverlappingStreamLocations true
|> printfn "Test Data Overlapping Streams (with diagonal): %O"

input
|> countOverlappingStreamLocations true
|> printfn "Input Data Overlapping Streams (with diagonal): %O"
