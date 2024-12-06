let testdata =
    System.IO.File.ReadAllLines("testdata.txt")

let realdata =
    System.IO.File.ReadAllLines("realdata.txt")

type Direction =
    | North
    | East
    | South
    | West

type Space =
    | Empty
    | Visited of List<Direction>
    | Blocked

type EndPosition =
    | Loop
    | MadeItOut

let parse (data: string array) =
    let mutable guardPosition = (0, 0)

    let map =
        data
        |> Array.mapi (fun y row ->
            row.ToCharArray()
            |> Array.mapi (fun x c ->
                match c with
                | '.' -> Empty
                | '^' ->
                    guardPosition <- (x, y)
                    Visited [ North ]
                | '#' -> Blocked
                | _ -> failwith "Invalid character"))

    (guardPosition, map)

let turn previousDirection =
    match previousDirection with
    | North -> East
    | East -> South
    | South -> West
    | West -> North

let getNextPosition (x, y) direction =
    match direction with
    | North -> (x, y - 1)
    | East -> (x + 1, y)
    | South -> (x, y + 1)
    | West -> (x - 1, y)

let walk (startPosition, (map: Space array array)) =
    let rec walk currentDirection currentPosition =
        let nextPosition =
            getNextPosition currentPosition currentDirection

        match nextPosition with
        | (x, y) when
            x < 0
            || y < 0
            || x >= map.[0].Length
            || y >= map.Length
            ->
            (map, MadeItOut)
        | (x, y) when map.[y].[x] = Blocked -> walk (currentDirection |> turn) currentPosition
        | (x, y) ->
            match map.[y].[x] with
            | Empty ->
                map.[y].[x] <- Visited [ currentDirection ]
                walk currentDirection nextPosition
            | Visited directions ->
                if directions |> List.contains currentDirection then
                    (map, Loop)
                else
                    map.[y].[x] <- Visited(currentDirection :: directions)
                    walk currentDirection nextPosition
            | Blocked -> failwith "Should not happen"

    walk North startPosition

let part1 data =
    data
    |> walk
    |> fst
    |> Array.concat
    |> Array.filter (fun x ->
        match x with
        | Visited _ -> true
        | _ -> false)
    |> Array.length

let part2 (initialPosition, originalMap) =
    let getMapClone () =
        originalMap
        |> Array.map (fun row -> row |> Array.map id)

    (initialPosition, getMapClone ())
    |> walk
    |> fst
    |> Array.mapi (fun y row -> (y, row))
    |> Array.fold
        (fun acc (y, row) ->
            row
            |> Array.mapi (fun x cell -> (x, cell))
            |> Array.fold
                (fun acc (x, cell) ->
                    match cell with
                    | Visited _ ->
                        [ North; East; South; West ]
                        |> List.fold
                            (fun acc d ->
                                let mapClone = getMapClone ()
                                let (nextX, nextY) = getNextPosition (x, y) d

                                if nextX >= 0
                                   && nextY >= 0
                                   && nextX < mapClone.[0].Length
                                   && nextY < mapClone.Length then
                                    mapClone.[nextY].[nextX] <- Blocked
                                    let (_, result) = walk (initialPosition, mapClone)

                                    match result with
                                    | Loop -> acc |> Set.add (nextX, nextY)
                                    | MadeItOut -> acc
                                else
                                    acc)
                            acc
                    | _ -> acc)
                acc)
        Set.empty
    |> Set.count

testdata
|> parse
|> part1
|> printfn "part 1 - testdata: %i"

realdata
|> parse
|> part1
|> printfn "part 1 - realdata: %i"

testdata
|> parse
|> part2
|> printfn "part 2 - testdata: %i"

realdata
|> parse
|> part2
|> printfn "part 2 - realdata: %i"
