type Cave =
    | Start of Connections: string list
    | End
    | BigCave of letter: string * Connections: string list
    | SmallCave of letter: string * Connections: string list

type CaveSystem = { Caves: Map<string, Cave> }

let parse filename =
    let data =
        System.IO.File.ReadAllLines(filename)
        |> Array.toList

    let connections = data |> List.map (fun l -> l.Split('-'))

    let caves =
        connections
        |> List.map (fun l -> l |> Array.toList)
        |> List.concat
        |> List.distinct

    let map =
        caves
        |> List.map (fun name ->
            let conns =
                connections
                |> List.choose (fun c ->
                    match c with
                    | _ when c.[0] = name -> Some c.[1]
                    | _ when c.[1] = name -> Some c.[0]
                    | _ -> None)

            let cave =
                match name with
                | "start" -> Start(conns)
                | "end" -> End
                | c when System.Char.IsUpper(c.ToCharArray().[0]) -> BigCave(name, conns)
                | _ -> SmallCave(name, conns)

            (name, cave))
        |> Map.ofList

    { Caves = map }

let rec traverse canVisitSingleSmallCaveTwice currentCave currentPath caveSystem =
    let location = caveSystem.Caves.[currentCave]

    match location with
    | End -> [ location :: currentPath |> List.rev ]
    | Start conns ->
        match currentPath with
        | [] ->
            conns
            |> List.map (fun c -> traverse canVisitSingleSmallCaveTwice c [ location ] caveSystem)
            |> List.concat
        | _ -> []
    | BigCave (name, conns) ->
        conns
        |> List.map (fun c -> traverse canVisitSingleSmallCaveTwice c (location :: currentPath) caveSystem)
        |> List.concat
    | SmallCave (name, conns) ->
        let previousVisitsToThisCave =
            currentPath
            |> List.filter (fun cave ->
                match cave with
                | SmallCave (c, _) -> c = name
                | _ -> false)

        match previousVisitsToThisCave |> List.length with
        | 1 when canVisitSingleSmallCaveTwice ->
            let alreadyHasSmallCaveVisitedTwice =
                currentPath
                |> List.choose (fun c ->
                    match c with
                    | SmallCave (n, _) -> Some n
                    | _ -> None)
                |> List.groupBy (fun n -> n)
                |> List.filter (fun (n, l) -> l |> List.length > 1)
                |> List.length > 0

            if alreadyHasSmallCaveVisitedTwice then
                []
            else
                conns
                |> List.map (fun c -> traverse canVisitSingleSmallCaveTwice c (location :: currentPath) caveSystem)
                |> List.concat
        | 1 when not canVisitSingleSmallCaveTwice -> []
        | 0 ->
            conns
            |> List.map (fun c -> traverse canVisitSingleSmallCaveTwice c (location :: currentPath) caveSystem)
            |> List.concat
        | _ -> []

let part1 filename =
    filename
    |> parse
    |> traverse false "start" []
    |> List.length
    |> printfn "%s - Number of paths: %i" filename

let part2 filename =
    filename
    |> parse
    |> traverse true "start" []
    |> List.length
    |> printfn "%s - Number of paths: %i" filename

"testdata1.txt" |> part1
"testdata2.txt" |> part1
"testdata3.txt" |> part1
"input.txt" |> part1

"testdata1.txt" |> part2
"testdata2.txt" |> part2
"testdata3.txt" |> part2
"input.txt" |> part2
