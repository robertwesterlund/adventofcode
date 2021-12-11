type InputLine =
    { SignalPatterns: list<string>
      Output: list<string>
      OutputDigits: list<int>
      OutputValue: int
      Mapping: Map<list<char>, int> }

let parse filename =
    System.IO.File.ReadAllLines(filename)
    |> Array.toList
    |> List.map (fun line ->
        let [| signalLine; outputLine |] = line.Split('|')

        let signals =
            signalLine.Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
            |> Array.toList

        let outputStrings =
            outputLine.Split(' ', System.StringSplitOptions.RemoveEmptyEntries)
            |> Array.toList

        let countMatches (string1: string) (string2: string) =
            string1.ToCharArray()
            |> Array.fold
                (fun acc c ->
                    match string2.Contains(c) with
                    | true -> acc + 1
                    | false -> acc)
                0

        let rec createMapping map signalsLeft =
            let findForMapping value (map: Map<int, string>) (signalsLeft: list<string>) =
                // This method expects the entries to be picked out of the list in the order this list is written
                match value with
                | 1 -> signalsLeft |> List.find (fun v -> v.Length = 2)
                | 4 -> signalsLeft |> List.find (fun v -> v.Length = 4)
                | 7 -> signalsLeft |> List.find (fun v -> v.Length = 3)
                | 8 -> signalsLeft |> List.find (fun v -> v.Length = 7)
                | 6 ->
                    signalsLeft
                    |> List.find (fun v -> v.Length = 6 && 1 = countMatches v map.[1])
                | 9 ->
                    signalsLeft
                    |> List.find (fun v -> v.Length = 6 && 4 = countMatches v map.[4])
                | 0 -> signalsLeft |> List.find (fun v -> v.Length = 6)
                | 5 ->
                    signalsLeft
                    |> List.find (fun v -> v.Length = 5 && 5 = countMatches v map.[6])
                | 3 ->
                    signalsLeft
                    |> List.find (fun v -> v.Length = 5 && 2 = countMatches v map.[1])
                | 2 -> signalsLeft |> List.find (fun v -> v.Length = 5)

            let (reverseMap, _) =
                [ 1; 4; 7; 8; 6; 9; 0; 5; 3; 2 ]
                |> List.fold
                    (fun acc value ->
                        let (m, s) = acc
                        let stringForValue = findForMapping value m s

                        let indexOfMatch =
                            s |> List.findIndex (fun v -> v = stringForValue)

                        (m |> Map.add value stringForValue, s |> List.removeAt indexOfMatch))
                    (Map.empty, signals)

            reverseMap
            |> Map.toList
            |> List.map (fun (v, s) -> (s.ToCharArray() |> Array.toList |> List.sort, v))
            |> Map.ofList

        let mapping = createMapping Map.empty signalLine

        let outputDigits =
            outputStrings
            |> List.map (fun v -> mapping.[v.ToCharArray() |> Array.toList |> List.sort])

        { SignalPatterns = signals
          Mapping = mapping
          OutputDigits = outputDigits
          OutputValue =
            outputDigits
            |> List.indexed
            |> List.sumBy (fun (index, v) -> (pown 10 (3 - index)) * v)
          Output = outputStrings })


let countUniqueLengthEntriesInOutput (entries: list<InputLine>) =
    entries
    |> List.fold
        (fun acc e ->
            let c =
                e.Output
                |> List.map (fun o ->
                    match o.Length with
                    | 3
                    | 4
                    | 7
                    | 2 -> 1
                    | _ -> 0)
                |> List.sum

            c + acc)
        0

"testdata.txt"
|> parse
|> countUniqueLengthEntriesInOutput
|> printfn "Part 1 Test: %i"

"input.txt"
|> parse
|> countUniqueLengthEntriesInOutput
|> printfn "Part 1 Real: %i"

"testdata.txt"
|> parse
|> List.sumBy (fun v -> v.OutputValue)
|> printfn "Part 2 Test: %i"

"input.txt"
|> parse
|> List.sumBy (fun v -> v.OutputValue)
|> printfn "Part 2 Real: %i"
