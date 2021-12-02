module Shared

type Action =
    | Forward of Distance: int64
    | Down of Distance: int64
    | Up of Distance: int64

type Position = { X: int64; Y: int64 }

let Parse input =
    input
    |> List.map (fun (line: string) ->
        let split = line.Split(' ')
        let value = split.[1] |> System.Convert.ToInt64

        match split.[0] with
        | "forward" -> Forward value
        | "down" -> Down value
        | "up" -> Up value
        | _ -> raise (System.Exception("Invalid input string " + line)))
