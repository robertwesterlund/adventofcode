let readLines filename =
    filename
    |> System.IO.File.ReadAllLines
    |> Array.toList

let readDots filename =
    filename
    |> readLines
    |> List.filter (fun line -> line.Length > 0 && System.Char.IsDigit(line.[0]))
    |> List.map (fun line ->
        let split = line.Split(',')
        (split.[0] |> System.Int32.Parse, split.[1] |> System.Int32.Parse))

let readFoldingInstructions filename =
    filename
    |> readLines
    |> List.filter (fun line ->
        line.Length > 0
        && not (System.Char.IsDigit(line.[0])))
    |> List.map (fun line ->
        let split =
            line.Substring("fold along ".Length).Split("=")

        (split.[0].[0], split.[1] |> System.Int32.Parse))

let foldPaper foldingInstruction (data: List<int32 * int32>) =
    match foldingInstruction with
    | ('x', pos) ->
        data
        |> List.map (fun (x, y) -> ((if x > pos then pos * 2 - x else x), y))
    | ('y', pos) ->
        data
        |> List.map (fun (x, y) -> (x, (if y > pos then pos * 2 - y else y)))
    |> List.distinct

let part1 filename =
    let foldingInstructions = filename |> readFoldingInstructions

    filename
    |> readDots
    |> foldPaper foldingInstructions.[0]
    |> List.length
    |> printfn "%s result: %O" filename

let part2 filename =
    let result =
        filename
        |> readFoldingInstructions
        |> List.fold (fun dots instruction -> dots |> foldPaper instruction) (filename |> readDots)

    let (xMax, yMax) =
        result
        |> List.fold (fun (foundx, foundy) (x, y) -> (max foundx x, max foundy y)) (0, 0)

    printf "\nResults for %s:" filename

    [ for y in 0 .. yMax do
          for x in 0 .. xMax do
              if x = 0 then printfn ""

              match result |> List.contains (x, y) with
              | true -> printf "#"
              | false -> printf "." ]
    |> ignore

    printfn ""

"testdata.txt" |> part1
"input.txt" |> part1
"testdata.txt" |> part2
"input.txt" |> part2
