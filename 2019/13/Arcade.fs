module Arcade

open System

type Position = { X: int64; Y: int64 }

type Tile =
    | Empty
    | Wall
    | Block
    | HorizontalPaddle
    | Ball

type Context =
    { Screen: Map<Position, Tile>
      Score: int64 }

    static member Empty =
        { Screen = Map.empty<Position, Tile>
          Score = 0L }

let Paint position tile context =
    { context with
          Screen = (context.Screen |> Map.add position tile) }

let PaintByNumbers (numbers: int64 list) context =
    let x = numbers.[2]
    let y = numbers.[1]
    let tile = numbers.[0]
    if x = -1L && y = 0L then
        { context with Score = tile }
    else
        let getTileByNumber number =
            match number with
            | 0L -> Empty
            | 1L -> Wall
            | 2L -> Block
            | 3L -> HorizontalPaddle
            | 4L -> Ball
            | _ ->
                failwith
                    ("Invalid number provided: "
                     + number.ToString()
                     + " ("
                     + x.ToString()
                     + ", "
                     + y.ToString()
                     + ")")

        context
        |> Paint { X = x; Y = y } (getTileByNumber tile)

let Draw context =
    Threading.Thread.Sleep(20)
    Console.Clear()
    Console.SetCursorPosition(0, 0)
    context.Score |> printfn "Score: %i"
    context.Screen
    |> Map.iter (fun k v ->
        Console.SetCursorPosition((int) k.X, (int) k.Y + 1)
        match v with
        | Empty -> printf " "
        | Wall -> printf "#"
        | Block -> printf "~"
        | HorizontalPaddle -> printf "_"
        | Ball -> printf "o")
