let minidata =
    System.IO.File.ReadAllLines("minidata.txt")

let testdata =
    System.IO.File.ReadAllLines("testdata.txt")

let realdata =
    System.IO.File.ReadAllLines("realdata.txt")

type Antenna = { frequency: char; x: int; y: int }
type Antinode = { antiFrequency: char; x: int; y: int }

type MapSize = { width: int; height: int }

let parse (input: string array) =
    let data =
        input
        |> Array.map (fun line -> line.ToCharArray())

    let mapSize =
        { width = data.[0].Length
          height = data.Length }

    let antennas =
        data
        |> Seq.mapi (fun rowIndex row ->
            row
            |> Seq.mapi (fun colIndex cell ->
                match cell with
                | '.' -> None
                | x ->
                    Some
                        { frequency = x
                          x = colIndex
                          y = rowIndex }))
        |> Seq.collect id
        |> Seq.choose id
        |> Seq.toList

    (mapSize, antennas)

let printMap mapSize (antennas: Antenna list) antinodelist =
    [ 0 .. mapSize.height - 1 ]
    |> List.iter (fun y ->
        [ 0 .. mapSize.width - 1 ]
        |> List.iter (fun x ->
            match antennas
                  |> List.tryFind (fun a -> a.x = x && a.y = y)
                with
            | Some a -> printf "%c" a.frequency
            | None ->
                match antinodelist
                      |> List.tryFind (fun b -> b.x = x && b.y = y)
                    with
                | Some _ -> printf "#"
                | None -> printf ".")

        printfn "")

let getAntinodesPart1 (mapSize, antennas) =
    antennas
    |> List.groupBy (fun a -> a.frequency)
    |> List.collect (fun (frequency, antennas) ->
        let allCombinations =
            [ for i in 0 .. antennas.Length - 1 do
                  for j in i + 1 .. antennas.Length - 1 do
                      yield (antennas.[i], antennas.[j]) ]

        allCombinations
        |> List.collect (fun (antenna1, antenna2) ->
            let diffX = antenna1.x - antenna2.x
            let diffY = antenna1.y - antenna2.y

            [ (antenna1.x + diffX, antenna1.y + diffY)
              (antenna2.x - diffX, antenna2.y - diffY) ])
        |> List.filter (fun (x, y) ->
            x >= 0
            && x < mapSize.width
            && y >= 0
            && y < mapSize.height)
        |> List.distinct
        |> List.map (fun (x, y) ->
            { antiFrequency = frequency
              x = x
              y = y }))

let isInsideMap mapSize (x, y) =
    x >= 0
    && x < mapSize.width
    && y >= 0
    && y < mapSize.height

let getAntinodesPart2 (mapSize, antennas) =
    antennas
    |> List.groupBy (fun a -> a.frequency)
    |> List.collect (fun (frequency, antennas) ->
        let allCombinations =
            [ for i in 0 .. antennas.Length - 1 do
                  for j in i + 1 .. antennas.Length - 1 do
                      yield (antennas.[i], antennas.[j]) ]

        allCombinations
        |> List.collect (fun (antenna1, antenna2) ->
            let diffX = antenna1.x - antenna2.x
            let diffY = antenna1.y - antenna2.y

            [ antenna1; antenna2 ]
            |> List.collect (fun a ->
                [ 1; -1 ]
                |> List.collect (fun dx ->
                    let mutable counter = 0

                    [ while isInsideMap mapSize (a.x + (dx * counter * diffX), a.y + (dx * counter * diffY)) do
                          counter <- counter + 1
                          yield (a.x + (dx * counter * diffX), a.y + (dx * counter * diffY)) ])))
        |> List.filter (isInsideMap mapSize)
        |> List.distinct
        |> List.map (fun (x, y) ->
            { antiFrequency = frequency
              x = x
              y = y }))

let part1 data =
    data
    |> getAntinodesPart1
    |> List.map (fun antinode -> antinode.x, antinode.y)
    |> List.distinct
    |> List.length

let part2 data =
    data
    |> getAntinodesPart2
    |> List.map (fun antinode -> antinode.x, antinode.y)
    |> List.distinct
    |> List.length


testdata
|> parse
|> part1
|> printfn "part 1 - testdata - %i"

realdata
|> parse
|> part1
|> printfn "part 1 - realdata - %i"

minidata
|> parse
|> part2
|> printfn "part 2 - minidata - %i"

testdata
|> parse
|> part2
|> printfn "part 2 - testdata - %i"

realdata
|> parse
|> part2
|> printfn "part 2 - realdata - %i"
