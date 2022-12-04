type HandShape =
    | Rock
    | Paper
    | Scissors

type RoundOutcome =
    | Win
    | Draw
    | Loss

exception InvalidInput of string

let parseHandshape value =
    match value with
    | "A" -> Rock
    | "B" -> Paper
    | "C" -> Scissors
    | "X" -> Rock
    | "Y" -> Paper
    | "Z" -> Scissors
    | _ -> raise (InvalidInput value)

let parseExpectedOutcome value =
    match value with
    | "X" -> Loss
    | "Y" -> Draw
    | "Z" -> Win
    | _ -> raise (InvalidInput value)

let parseInputPart1 (input: string) =
    input.Replace("\r\n", "\n").Split("\n")
    |> Array.map (fun line -> line.Split(" ") |> Array.map parseHandshape)

type strategy = Round of HandShape * RoundOutcome

let parseInputPart2 (input: string) =
    input.Replace("\r\n", "\n").Split("\n")
    |> Array.map (fun line ->
        let [| column1; column2 |] = line.Split(" ")
        Round(column1 |> parseHandshape, column2 |> parseExpectedOutcome))

let getOutcomeForPlayer1 player1 player2 =
    match player1 with
    | Rock ->
        match player2 with
        | Rock -> Draw
        | Paper -> Loss
        | Scissors -> Win
    | Paper ->
        match player2 with
        | Rock -> Win
        | Paper -> Draw
        | Scissors -> Loss
    | Scissors ->
        match player2 with
        | Rock -> Loss
        | Paper -> Win
        | Scissors -> Draw

let getOutcomeValue outcome =
    match outcome with
    | Win -> 6L
    | Draw -> 3L
    | Loss -> 0L

let getPointsForChoice choice =
    match choice with
    | Rock -> 1L
    | Paper -> 2L
    | Scissors -> 3L

let part1 input =
    input
    |> parseInputPart1
    |> Array.map (fun [| p1; p2 |] ->
        (getPointsForChoice p2)
        + (getOutcomeForPlayer1 p2 p1 |> getOutcomeValue))
    |> Array.sum

let part2 input =
    input
    |> parseInputPart2
    |> Array.map (fun strategy ->
        match strategy with
        | Round (opponent, expectedOutcome) ->
            [ Rock; Paper; Scissors ]
            |> List.filter (fun choice -> expectedOutcome = (getOutcomeForPlayer1 choice opponent))
            |> List.map (fun choice ->
                (getPointsForChoice choice)
                + (getOutcomeValue expectedOutcome))
            |> List.head)
    |> Array.sum

Input.testData
|> part1
|> printfn "Part 1 - test data: %A"

Input.realData
|> part1
|> printfn "Part 1 - real data: %A"

Input.testData
|> part2
|> printfn "Part 2 - test data: %A"

Input.realData
|> part2
|> printfn "Part 2 - real data: %A"
