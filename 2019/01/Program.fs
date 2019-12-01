module Program

open System

[<EntryPoint>]
let main argv =
    Part1.answer |> printfn "Part one answer: %.0f"
    Part2.answer |> printfn "Part two answer: %.0f"
    0 // return an integer exit code
