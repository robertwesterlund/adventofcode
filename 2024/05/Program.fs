let testdata =
    System.IO.File.ReadAllLines("testdata.txt")

let realdata =
    System.IO.File.ReadAllLines("realdata.txt")

type Input =
    { Rules: Map<int64, Set<int64>>
      Updates: int64 list list }
    member this.isMatchingAllRules update =
        update
        |> this.getFirstRulebreaker
        |> Option.isNone

    member this.getFirstRulebreaker update =
        update
        |> Seq.fold
            (fun (prev, ruleBreaker) value ->
                match ruleBreaker with
                | Some breaker -> (prev, ruleBreaker)
                | None ->
                    match this.Rules |> Map.tryFind value with
                    | Some list ->
                        let isBreakingRule =
                            prev
                            |> Set.ofList
                            |> Set.intersect list
                            |> Set.isEmpty
                            |> not

                        if isBreakingRule then
                            (prev, Some(value, list))
                        else
                            (value :: prev, None)
                    | None -> (value :: prev, None))
            ([], None)
        |> snd

let parse data =
    let rules = data |> Seq.takeWhile (fun x -> x <> "")

    let update =
        data
        |> Seq.skipWhile (fun x -> x <> "")
        |> Seq.skip 1

    { Rules =
        rules
        |> Seq.fold
            (fun acc line ->
                let parts = line.Split("|")
                let predessor = int64 parts.[0]
                let successor = int64 parts.[1]

                let s =
                    match acc |> Map.tryFind predessor with
                    | Some list ->
                        acc
                        |> Map.add predessor (list |> Set.add successor)
                    | None ->
                        acc
                        |> Map.add predessor (Set.empty |> Set.add successor)

                s)
            Map.empty
      Updates =
        update
        |> Seq.map (fun x -> x.Split(",") |> Seq.map int64 |> List.ofSeq)
        |> List.ofSeq }

let part1 data =
    data
    |> Seq.ofArray
    |> parse
    |> fun input ->

        input.Updates
        |> Seq.filter input.isMatchingAllRules
        |> Seq.sumBy (fun update -> update.[(update.Length - 1) / 2])

let part2 data =
    data
    |> Seq.ofArray
    |> parse
    |> fun input ->

        input.Updates
        |> Seq.filter (fun update -> not (input.isMatchingAllRules update))
        |> Seq.map (fun update ->
            let rec fixNextBreak values =
                match input.getFirstRulebreaker values with
                | Some (value, list) ->
                    let originalIndex =
                        values |> List.findIndex (fun x -> x = value)

                    let newIndex =
                        values
                        |> List.findIndex (fun x -> list |> Set.contains x)

                    let firstPart = values |> List.take newIndex

                    let middlePart =
                        values
                        |> List.skip (newIndex)
                        |> List.take (originalIndex - newIndex)

                    let lastPart = values |> List.skip (originalIndex + 1)

                    let newValues =
                        firstPart @ [ value ] @ middlePart @ lastPart

                    newValues |> fixNextBreak
                | None -> values

            fixNextBreak update)
        |> Seq.sumBy (fun update -> update.[(update.Length - 1) / 2])

testdata
|> part1
|> printfn "part 1 - testdata %i"

realdata
|> part1
|> printfn "part 1 - realdata %i"

testdata
|> part2
|> printfn "part 2 - testdata %i"

realdata
|> part2
|> printfn "part 2 - realdata %i"
