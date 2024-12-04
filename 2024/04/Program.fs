let minidata =
    [| "..X..."
       ".SAMX."
       ".A..A."
       "XMAS.S"
       ".X...." |]

let testdata =
    System.IO.File.ReadAllLines("testdata.txt")

let realdata =
    System.IO.File.ReadAllLines("realdata.txt")

type Direction =
    | North
    | NorthEast
    | East
    | SouthEast
    | South
    | SouthWest
    | West
    | NorthWest

type Position =
    | RealPosition of x: int * y: int // { x: int; y: int }
    | NullPosition

type X_Y = { x: int; y: int }

let tryGetNextPosition (grid: char array array) direction position =
    match position with
    | NullPosition -> NullPosition
    | RealPosition (x, y) ->
        let desiredPosition =
            match direction with
            | North -> { x = x; y = y - 1 }
            | NorthEast -> { x = x + 1; y = y - 1 }
            | East -> { x = x + 1; y = y }
            | SouthEast -> { x = x + 1; y = y + 1 }
            | South -> { x = x; y = y + 1 }
            | SouthWest -> { x = x - 1; y = y + 1 }
            | West -> { x = x - 1; y = y }
            | NorthWest -> { x = x - 1; y = y - 1 }

        if desiredPosition.x >= 0
           && desiredPosition.x < grid.[0].Length
           && desiredPosition.y >= 0
           && desiredPosition.y < grid.Length then
            RealPosition(desiredPosition.x, desiredPosition.y)
        else
            NullPosition


let allDirections =
    [ North
      NorthEast
      East
      SouthEast
      South
      SouthWest
      West
      NorthWest ]

let matchesWord (word: string) (grid: char array array) direction (position: Position) =
    let characters = word.ToCharArray()

    match position with
    | NullPosition -> false
    | RealPosition (x, y) ->
        if grid.[y].[x] <> characters.[0] then
            false
        else
            characters
            |> Array.fold
                (fun (position, isMatchSoFar) letter ->
                    match position with
                    | NullPosition -> (position, false)
                    | RealPosition (x, y) ->
                        match grid.[y].[x] with
                        | x when x = letter -> (tryGetNextPosition grid direction position, isMatchSoFar)
                        | _ -> (position, false))
                (position, true)
            |> snd

let parse (data: string array) =
    data |> Array.map (fun line -> line.ToCharArray())

let getAllPositions (grid: char array array) =
    [ for y in 0 .. grid.Length - 1 do
          for x in 0 .. grid.[0].Length - 1 do
              yield RealPosition(x, y) ]

let part1 (grid: char array array) =
    getAllPositions grid
    |> List.fold
        (fun count position ->
            allDirections
            |> List.fold
                (fun count direction ->
                    if matchesWord "XMAS" grid direction position then
                        // printfn "found XMAS at %A %A" position direction
                        count + 1L
                    else
                        count)
                count)
        0L

let part2 (grid: char array array) =
    grid
    |> getAllPositions
    |> List.fold
        (fun count position ->
            match position with
            | NullPosition -> count
            | RealPosition (x, y) ->
                match grid.[y].[x] with
                | 'A' ->
                    if ((matchesWord "MAS" grid SouthEast (tryGetNextPosition grid NorthWest position)
                         || matchesWord "MAS" grid NorthWest (tryGetNextPosition grid SouthEast position))
                        && (matchesWord "MAS" grid NorthEast (tryGetNextPosition grid SouthWest position)
                            || matchesWord "MAS" grid SouthWest (tryGetNextPosition grid NorthEast position))) then
                        count + 1L
                    else
                        count
                | _ -> count)
        0L

minidata
|> parse
|> part1
|> printfn "part 1 - mini data %i"

testdata
|> parse
|> part1
|> printfn "part 1 - test data %i"

realdata
|> parse
|> part1
|> printfn "part 1 - real data %i"

testdata
|> parse
|> part2
|> printfn "part 2 - test data %i"

realdata
|> parse
|> part2
|> printfn "part 2 - real data %i"
