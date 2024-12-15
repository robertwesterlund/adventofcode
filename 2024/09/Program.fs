let testdata =
    System.IO.File.ReadAllText("testdata.txt")

let minidata =
    System.IO.File.ReadAllText("minidata.txt")

let realdata =
    System.IO.File.ReadAllText("realdata.txt")

type ValueEntry =
    { Id: int64
      mutable EntriesLeft: int64 }

let part1 (input: string) =
    let data =
        input.ToCharArray()
        |> Array.map (fun c -> System.Int64.Parse(c.ToString()))

    let values =
        new System.Collections.Generic.LinkedList<ValueEntry>(
            data
            |> Seq.mapi (fun i c -> (i, c))
            |> Seq.filter (fun (i, c) -> i % 2 = 0)
            |> Seq.mapi (fun i (_, c) -> { Id = i; EntriesLeft = c })
        )

    let spaces =
        new System.Collections.Generic.LinkedList<int64>(
            data
            |> Seq.mapi (fun i c -> (i, c))
            |> Seq.filter (fun (i, c) -> i % 2 = 1)
            |> Seq.map (fun (_, c) -> c)
        )

    let driveContent =
        [ let mutable currentEnd = values.Last
          values.RemoveLast()

          while values.Count > 0 do
              let current = values.First
              values.RemoveFirst()

              for _ in 1L .. current.Value.EntriesLeft do
                  yield current.Value.Id

              if spaces.Count > 0 then
                  let currentSpace = spaces.First
                  spaces.RemoveFirst()

                  for _ in 1L .. currentSpace.Value do
                      if currentEnd.Value.EntriesLeft = 0
                         && values.Count > 0 then
                          currentEnd <- values.Last
                          values.RemoveLast()

                      if currentEnd.Value.EntriesLeft > 0 then
                          currentEnd.Value.EntriesLeft <- currentEnd.Value.EntriesLeft - 1L
                          yield currentEnd.Value.Id

          for _ in 1L .. currentEnd.Value.EntriesLeft do
              yield currentEnd.Value.Id ]

    driveContent
    |> Seq.mapi (fun i c -> (i, c))
    |> Seq.sumBy (fun (i, c) -> int64 i * c)

let part2 (input: string) =
    let data =
        input.ToCharArray()
        |> Array.map (fun c -> System.Int64.Parse(c.ToString()))

    let values =
        data
        |> Seq.mapi (fun i c -> (i, c))
        |> Seq.filter (fun (i, c) -> i % 2 = 0)
        |> Seq.mapi (fun i (_, c) -> { Id = i; EntriesLeft = c })
        |> Seq.toList

    let spaces =
        [ 0L ]
        |> Seq.append (
            data
            |> Seq.mapi (fun i c -> (i, c))
            |> Seq.filter (fun (i, c) -> i % 2 = 1)
            |> Seq.map (fun (_, c) -> c)
        )
        |> Seq.toArray

    let driveContent =
        [

          let mutable valueIndex = values.Length - 1
          let mutable movedEntries = []

          while valueIndex > 0 do
              let current = values.[valueIndex]

              let spaceIndex =
                  spaces
                  |> Array.tryFindIndex (fun s -> s >= current.EntriesLeft)

              match spaceIndex with
              | Some i when i < valueIndex ->
                  spaces.[i] <- spaces.[i] - current.EntriesLeft
                  spaces.[int current.Id - 1] <- spaces.[int current.Id - 1] + current.EntriesLeft
                  movedEntries <- movedEntries @ [ (i, current) ]
              | _ -> ()

              valueIndex <- valueIndex - 1

          while valueIndex < values.Length - 1 do
              let current = values.[valueIndex]

              if movedEntries
                 |> List.tryFind (fun (_, e) -> e.Id = current.Id) = None then
                  for _ in 1L .. current.EntriesLeft do
                      yield current.Id

              let entriesMovedToThisSpace =
                  movedEntries
                  |> List.filter (fun (i, _) -> i = valueIndex)

              for (_, e) in entriesMovedToThisSpace do
                  for _ in 1L .. e.EntriesLeft do
                      yield e.Id

              for _ in 1L .. spaces.[valueIndex] do
                  yield 0L

              valueIndex <- valueIndex + 1

         ]

    driveContent
    |> Seq.mapi (fun i c -> (i, c))
    |> Seq.sumBy (fun (i, c) -> int64 i * c)

minidata
|> part1
|> printfn "part 1 - minidata - %i"

testdata
|> part1
|> printfn "part 1 - testdata - %i"

realdata
|> part1
|> printfn "part 1 - realdata - %i"

testdata
|> part2
|> printfn "part 2 - testdata - %i"

realdata
|> part2
|> printfn "part 2 - realdata - %i"

// 6467290913294 is too high
