type Entry =
    { Data: int [] []
      Row: int
      Column: int }
    member this.getValue() = this.Data.[this.Row].[this.Column]

    member this.getSiblings() =
        [ this.Row, this.Column - 1
          this.Row, this.Column + 1
          this.Row - 1, this.Column
          this.Row + 1, this.Column ]
        |> List.filter (fun (r, c) ->
            r >= 0
            && c >= 0
            && r < this.Data.Length
            && c < this.Data.[0].Length)
        |> List.map (fun (r, c) -> { this with Row = r; Column = c })

let parse filename =
    { Row = 0
      Column = 0
      Data =
        System.IO.File.ReadAllLines(filename)
        |> Array.map (fun line ->
            line.ToCharArray()
            |> Array.map (fun a -> System.Convert.ToInt32(a.ToString()))) }

let findLowPoints firstEntry =
    [ for row in [ 0 .. (firstEntry.Data.Length - 1) ] do
          for column in [ 0 .. (firstEntry.Data.[0].Length - 1) ] do
              let e =
                  { firstEntry with
                      Row = row
                      Column = column }

              if (e.getSiblings ()
                  |> List.tryFind (fun s -> s.getValue () <= e.getValue ())
                  |> Option.isNone) then
                  e ]

let findBasins firstEntry =
    let lowPoints = firstEntry |> findLowPoints

    let getBasin point =
        let mutable basin = List.empty

        let rec fillBasin (p: Entry) =
            match basin |> List.contains p with
            | true -> ()
            | false ->
                match p.getValue () with
                | 9 -> ()
                | _ ->
                    basin <- p :: basin

                    p.getSiblings ()
                    |> List.iter (fun s -> fillBasin s)

        fillBasin point
        basin

    lowPoints |> List.map getBasin

"testdata.txt"
|> parse
|> findLowPoints
|> List.sumBy (fun v -> v.getValue () + 1)
|> printfn "Testdata Part 1: %i"

"input.txt"
|> parse
|> findLowPoints
|> List.sumBy (fun v -> v.getValue () + 1)
|> printfn "Real Part 1: %i"

"testdata.txt"
|> parse
|> findBasins
|> List.map (fun b -> b |> List.length)
|> List.sortDescending
|> List.take 3
|> List.fold (fun acc v -> acc * (int64) v) 1L
|> printfn "Testdata Part 2: %i"

"input.txt"
|> parse
|> findBasins
|> List.map (fun b -> b |> List.length)
|> List.sortDescending
|> List.take 3
|> List.fold (fun acc v -> acc * (int64) v) 1L
|> printfn "Input Part 2: %i"
