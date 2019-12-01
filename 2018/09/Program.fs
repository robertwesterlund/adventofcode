// Learn more about F# at http://fsharp.org

open System
open System.Collections.Generic

type GameContext() = 
    let board = new List<int>()
    let mutable currentMarbleIndex = 0
    let mutable nextMarbleValue = 0

    let getMarble() = 
        let value = nextMarbleValue
        nextMarbleValue <- nextMarbleValue + 1
        value

    let changeMarbleIndex delta = 
        if (board.Count = 0)
        then currentMarbleIndex <- 0
        else currentMarbleIndex <- (currentMarbleIndex + delta + board.Count) % board.Count

    member self.lastMarblePlaced() =    
        nextMarbleValue - 1
    
    member self.PlaceMarble() = 
        let marble = getMarble()
        match marble with
        | _ when marble = 0 ->
            board.Add(0)
            None
        | value when value % 23 = 0 -> 
            changeMarbleIndex -7
            let removedItem = board.[currentMarbleIndex]
            board.RemoveAt(currentMarbleIndex)
            Some(value + removedItem)
        | _ -> 
            changeMarbleIndex 2
            board.Insert(currentMarbleIndex, marble)
            None

let play numberOfPlayers lastMarbleValue = 
    let context = new GameContext()
    let mutable currentPlayer = 0
    let scores:int64 array = Array.zeroCreate numberOfPlayers
    for i in 0 .. (lastMarbleValue + 1) do
        match context.PlaceMarble() with
        | None -> None |> ignore
        | Some(value) -> scores.[currentPlayer] <- scores.[currentPlayer] + (value |> int64)
        currentPlayer <- (currentPlayer + 1) % numberOfPlayers
    let maxScore = scores |> Seq.max    
    printfn "Playing with %i number of players and last marble value of %i resulted in max score of %i" numberOfPlayers lastMarbleValue maxScore
    maxScore

let playAndCheck numberOfPlayers lastMarbleValue expectedOutput =
    let result = play numberOfPlayers lastMarbleValue
    if (result = expectedOutput)
    then printfn "Correct output"
    else printfn "Fail! Expected %i but received %i, with %i number of players and %i lastMarbleValue" expectedOutput result numberOfPlayers lastMarbleValue

[<EntryPoint>]
let main argv =

    playAndCheck 9 25 32L
    playAndCheck 10 1618 8317L
    playAndCheck 13 7999 146373L
    playAndCheck 17 1104 2764L
    playAndCheck 21 6111 54718L
    playAndCheck 30 5807 37305L
    play 425 70848 |> ignore
    play 425 (70848*100) |> ignore

    0 // return an integer exit code
