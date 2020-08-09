module Turtle

open System.Drawing

type Context =
    { PaintedOn: Set<int * int>
      WhiteSquares: Set<int * int>
      CurrentDirection: int
      CurrentPosition: int * int }

    member this.TurnRight() = { this with CurrentDirection = (this.CurrentDirection + 1 + 4) % 4 }

    member this.TurnLeft() = { this with CurrentDirection = (this.CurrentDirection - 1 + 4) % 4 }

    member this.MoveForward() =
        { this with
              CurrentPosition =
                  match this.CurrentDirection, this.CurrentPosition with
                  | 0, (x, y) -> (x, y + 1)
                  | 1, (x, y) -> (x + 1, y)
                  | 2, (x, y) -> (x, y - 1)
                  | 3, (x, y) -> (x - 1, y)
                  | _ -> failwith ("Invalid direction " + this.CurrentDirection.ToString()) }

    member this.IsCurrentSquareWhite() = this.WhiteSquares |> Set.contains this.CurrentPosition

    member private this.TurnByNumber number =
        match number with
        | 0 -> this.TurnLeft()
        | 1 -> this.TurnRight()
        | _ -> failwith ("Invalid turn number: " + number.ToString())



    member this.PaintBlack() =
        { this with
              WhiteSquares = (this.WhiteSquares |> Set.remove this.CurrentPosition)
              PaintedOn = (this.PaintedOn |> Set.add this.CurrentPosition) }



    member this.PaintWhite() =
        { this with
              WhiteSquares = (this.WhiteSquares |> Set.add this.CurrentPosition)
              PaintedOn = (this.PaintedOn |> Set.add this.CurrentPosition) }



    member private this.PaintByNumber number =
        match number with
        | 1 -> this.PaintWhite()
        | 0 -> this.PaintBlack()
        | _ -> failwith ("Invalid paint number: " + number.ToString())

    member this.Execute(paintInstruction, turnInstruction) =
        this.PaintByNumber(paintInstruction).TurnByNumber(turnInstruction).MoveForward()

    member this.ExecuteList instructions =
        match instructions with
        | [] -> this
        | head :: tail -> this.Execute(head).ExecuteList tail

    static member Empty =
        { CurrentPosition = (0, 0)
          CurrentDirection = 0
          PaintedOn = Set.empty<int * int>
          WhiteSquares = Set.empty<int * int> }

    member this.Draw() =
        let yMin =
            this.WhiteSquares
            |> Set.map (fun (x, y) -> y)
            |> Seq.min

        let yMax =
            this.WhiteSquares
            |> Set.map (fun (x, y) -> y)
            |> Seq.max

        let xMin =
            this.WhiteSquares
            |> Set.map (fun (x, y) -> x)
            |> Seq.min

        let xMax =
            this.WhiteSquares
            |> Set.map (fun (x, y) -> x)
            |> Seq.max

        for y in seq { yMax + 1 .. -1 .. yMin - 1 } do
            for x in seq { xMin - 1 .. xMax + 1 } do
                match this.WhiteSquares |> Set.contains (x, y) with
                | true -> printf "#"
                | false -> printf "."
            printfn ""
 