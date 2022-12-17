let parse (input: string) =
    input.Replace("\r\n", "\n").Split("\n")
    |> Array.map (fun line ->
        line.ToCharArray()
        |> Array.map (fun c -> System.Int32.Parse(c.ToString())))

let part1 input =
    let map = input |> parse

    let highestTreesMap =
        map
        |> Array.mapi (fun y row ->
            row
            |> Array.mapi (fun x treeHeigh ->
                [ ([ 0 .. (y - 1) ]
                   |> List.fold (fun highest yCheck -> System.Math.Max(map.[yCheck].[x], highest)) -1)

                  ([ y + 1 .. map.Length - 1 ]
                   |> List.fold (fun highest yCheck -> System.Math.Max(map.[yCheck].[x], highest)) -1)

                  ([ 0 .. (x - 1) ]
                   |> List.fold (fun highest xCheck -> System.Math.Max(map.[y].[xCheck], highest)) -1)

                  ([ x + 1 .. map.[0].Length - 1 ]
                   |> List.fold (fun highest xCheck -> System.Math.Max(map.[y].[xCheck], highest)) -1) ]))

    map
    |> Array.mapi (fun y line ->
        line
        |> Array.mapi (fun x treeHeight ->
            match treeHeight with
            | height when
                (highestTreesMap.[y].[x]
                 |> List.exists (fun path -> path < height))
                ->
                height
            | _ -> -1))
    |> Array.collect id
    |> Array.filter (fun x -> x <> -1)
    |> Array.length

let part2 input =
    let map = input |> parse

    map
    |> Array.mapi (fun y row ->
        row
        |> Array.mapi (fun x treeHeight ->
            let res =
                [ ([ 0 .. (y - 1) ]
                   |> List.rev
                   |> List.takeWhile (fun yCheck ->
                       if yCheck + 1 = y then
                           true
                       else
                           map.[yCheck + 1].[x] < treeHeight)
                   |> List.length)

                  ([ y + 1 .. map.Length - 1 ]
                   |> List.takeWhile (fun yCheck ->
                       if yCheck - 1 = y then
                           true
                       else
                           map.[yCheck - 1].[x] < treeHeight)
                   |> List.length)

                  ([ 0 .. (x - 1) ]
                   |> List.rev
                   |> List.takeWhile (fun xCheck ->
                       if (xCheck + 1) = x then
                           true
                       else
                           map.[y].[xCheck + 1] < treeHeight)
                   |> List.length)

                  ([ x + 1 .. map.[0].Length - 1 ]
                   |> List.takeWhile (fun xCheck ->
                       if xCheck - 1 = x then
                           true
                       else
                           map.[y].[xCheck - 1] < treeHeight)
                   |> List.length) ]

            res |> List.fold (fun acc curr -> acc * curr) 1))
    |> Array.collect id
    |> Array.max

Input.testData
|> part1
|> printfn "Part 1 - test data %i"

Input.realData
|> part1
|> printfn "Part 1 - real data %i"

Input.testData
|> part2
|> printfn "Part 2 - test data %A"

Input.realData
|> part2
|> printfn "Part 2 - real data %i"
