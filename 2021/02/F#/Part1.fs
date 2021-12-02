module Part1

open Shared

let ExecuteAction position action =
    match action with
    | Down v -> { position with Y = position.Y + v }
    | Up v -> { position with Y = position.Y - v }
    | Forward v -> { position with X = position.X + v }

let ExecutePlan position actions =
    actions
    |> List.fold (fun pos action -> action |> ExecuteAction pos) position

let ExecuteFromOrigo = ExecutePlan { X = 0; Y = 0 }

let CalculateResultFromPosition position = position.X * position.Y

let ParseAndExecute data =
    data |> Parse |> ExecuteFromOrigo |> printfn "%O"

    data
    |> Parse
    |> ExecuteFromOrigo
    |> CalculateResultFromPosition
    |> printfn "%O"
