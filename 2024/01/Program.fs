let testData =
    System.IO.File.ReadAllLines("testdata.txt")

let realData =
    System.IO.File.ReadAllLines("realdata.txt")

type DataPair =
    { Left: int64
      Right: int64
      NumberOfOccurancesOfLeftValueInRightList: int64 }
    member this.Diff =
        if this.Left > this.Right then
            this.Left - this.Right
        else
            this.Right - this.Left

let parse (data: string array) =
    let (leftList, rightList) =
        data
        |> Array.fold
            (fun (leftList, rightList) line ->
                let parts =
                    line.Split([| ' ' |], System.StringSplitOptions.RemoveEmptyEntries)

                let left = int64 parts.[0]
                let right = int64 parts.[1]
                (leftList @ [ left ], rightList @ [ right ]))
            ([], [])

    let leftSorted = leftList |> List.sort
    let rightSorted = rightList |> List.sort

    let occuranceDictionary =
        rightSorted
        |> List.fold
            (fun acc x ->
                if (acc |> Map.containsKey x) then
                    acc |> Map.add x (acc.[x] + 1)
                else
                    acc |> Map.add x 1)
            Map.empty

    List.zip leftSorted rightSorted
    |> List.map (fun (left, right) ->
        { Left = left
          Right = right
          NumberOfOccurancesOfLeftValueInRightList =
            if occuranceDictionary.ContainsKey left then
                occuranceDictionary.[left]
            else
                0 })

testData
|> parse
|> List.sumBy (fun pair -> pair.Diff)
|> printfn "Part 1 - test data %i"

realData
|> parse
|> List.sumBy (fun pair -> pair.Diff)
|> printfn "Part 1 - real data %i"

testData
|> parse
|> List.sumBy (fun pair ->
    pair.Left
    * pair.NumberOfOccurancesOfLeftValueInRightList)
|> printfn "Part 2 - test data %i"

realData
|> parse
|> List.sumBy (fun pair ->
    pair.Left
    * pair.NumberOfOccurancesOfLeftValueInRightList)
|> printfn "Part 2 - real data %i"
