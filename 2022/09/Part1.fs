module Part1

open System

type Point =
    { X: int32
      Y: int32 }

    member this.getPointCloseTo target =
        match (target.X, target.Y) with
        | (x, y) when
            Math.Abs(x - this.X) <= 1
            && Math.Abs(y - this.Y) <= 1
            ->
            this
        | (x, y) when x = this.X ->
            { this with
                Y =
                    if y < this.Y then
                        this.Y - 1
                    else
                        this.Y + 1 }
        | (x, y) when y = this.Y ->
            { this with
                X =
                    if x < this.X then
                        this.X - 1
                    else
                        this.X + 1 }
        | (x, y) ->
            { Y =
                if y < this.Y then
                    this.Y - 1
                else
                    this.Y + 1
              X =
                if x < this.X then
                    this.X - 1
                else
                    this.X + 1 }

type Positions =
    { Head: Point
      Tail: Point }

    member this.moveHeadInDirection direction =
        let targetPosition =
            match direction with
            | "U" -> { this.Head with Y = this.Head.Y + 1 }
            | "D" -> { this.Head with Y = this.Head.Y - 1 }
            | "R" -> { this.Head with X = this.Head.X + 1 }
            | "L" -> { this.Head with X = this.Head.X - 1 }

        { Head = targetPosition
          Tail = this.Tail.getPointCloseTo targetPosition }

type Context =
    { PointsVisitedByTail: Set<Point>
      CurrentPosition: Positions }

    member this.moveHead direction numberOfSteps =
        [ 1 .. numberOfSteps ]
        |> List.fold
            (fun context stepNumber ->
                let newPosition =
                    context.CurrentPosition.moveHeadInDirection direction

                if context.PointsVisitedByTail.Contains newPosition.Tail then
                    { context with CurrentPosition = newPosition }
                else
                    { CurrentPosition = newPosition
                      PointsVisitedByTail = context.PointsVisitedByTail.Add newPosition.Tail })
            this

let parse (input: string) =
    input.Replace("\r\n", "\n").Split("\n")
    |> Array.toList
    |> List.map (fun line ->
        let [| direction; stepCount |] = line.Split(" ")
        (direction, System.Int32.Parse(stepCount)))

let part1 input =
    let start = { X = 0; Y = 0 }

    let result =
        input
        |> parse
        |> List.fold
            (fun (context: Context) (direction, count) -> context.moveHead direction count)
            { CurrentPosition = { Head = start; Tail = start }
              PointsVisitedByTail = [ start ] |> Set.ofList }

    result.PointsVisitedByTail.Count

let execute =
    Input.testData
    |> part1
    |> printfn "Part 1 - test data: %i"

    Input.realData
    |> part1
    |> printfn "Part 1 - real data: %i"
