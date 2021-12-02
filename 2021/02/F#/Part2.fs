module Part2

open Shared

type Submarine = { Position: Position; Aim: int64 }

let ExecuteAction submarine action =
    match action with
    | Down v -> { submarine with Aim = submarine.Aim + v }
    | Up v -> { submarine with Aim = submarine.Aim - v }
    | Forward v ->
        { submarine with
            Position =
                { X = submarine.Position.X + v
                  Y = submarine.Position.Y + submarine.Aim * v } }

let ExecutePlan position actions =
    actions
    |> List.fold (fun pos action -> action |> ExecuteAction pos) position

let ExecuteFromOrigo =
    ExecutePlan { Aim = 0; Position = { X = 0; Y = 0 } }

let CalculateResultFromPosition submarine =
    submarine.Position.X * submarine.Position.Y

let ParseAndExecute data =
    data |> Parse |> ExecuteFromOrigo |> printfn "%O"

    data
    |> Parse
    |> ExecuteFromOrigo
    |> CalculateResultFromPosition
    |> printfn "%O"
