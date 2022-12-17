module Part2

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
    { Points: Point list }

    member this.moveHeadInDirection direction =
        { Points =
            this.Points
            |> List.fold
                (fun acc curr ->
                    match acc with
                    | [] ->
                        match direction with
                        | "U" -> [ { curr with Y = curr.Y + 1 } ]
                        | "D" -> [ { curr with Y = curr.Y - 1 } ]
                        | "R" -> [ { curr with X = curr.X + 1 } ]
                        | "L" -> [ { curr with X = curr.X - 1 } ]
                    | _ ->
                        let parent = acc |> List.last
                        acc @ [ curr.getPointCloseTo parent ])
                [] }

type Context =
    { PointsVisitedByTail: Set<Point>
      CurrentPosition: Positions }

    member this.moveHead direction numberOfSteps =
        [ 1 .. numberOfSteps ]
        |> List.fold
            (fun context stepNumber ->
                let newPosition =
                    context.CurrentPosition.moveHeadInDirection direction

                let tail = newPosition.Points |> List.last

                if context.PointsVisitedByTail.Contains tail then
                    { context with CurrentPosition = newPosition }
                else
                    { CurrentPosition = newPosition
                      PointsVisitedByTail = context.PointsVisitedByTail.Add tail })
            this

let parse (input: string) =
    input.Replace("\r\n", "\n").Split("\n")
    |> Array.toList
    |> List.map (fun line ->
        let [| direction; stepCount |] = line.Split(" ")
        (direction, System.Int32.Parse(stepCount)))

let part2 input =
    let start = { X = 0; Y = 0 }

    let result =
        input
        |> parse
        |> List.fold
            (fun (context: Context) (direction, count) -> context.moveHead direction count)
            { CurrentPosition = { Points = [ 1 .. 10 ] |> List.map (fun _ -> start) }
              PointsVisitedByTail = [ start ] |> Set.ofList }

    result.PointsVisitedByTail.Count

let execute =
    Input.testData
    |> part2
    |> printfn "Part 2 - test data: %i"

    @"R 5
U 8
L 8
D 3
R 17
D 10
L 25
U 20"
    |> part2
    |> printfn "Part 2 - longer test data: %i"

    Input.realData
    |> part2
    |> printfn "Part 2 - real data: %i"
