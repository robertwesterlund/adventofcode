open System

let part1DigitMap =
    [ 1 .. 9 ]
    |> Seq.fold (fun a c -> a |> Map.add (c.ToString()) c) Map.empty

let part2DigitMap =
    part1DigitMap
    |> Map.add "one" 1
    |> Map.add "two" 2
    |> Map.add "three" 3
    |> Map.add "four" 4
    |> Map.add "five" 5
    |> Map.add "six" 6
    |> Map.add "seven" 7
    |> Map.add "eight" 8
    |> Map.add "nine" 9

let getSum (digitMap: Map<string, int>) (input: string seq) =
    input
    |> Seq.map (fun line ->
        let inputSpan = line.AsSpan()
        let mutable numbers = []

        for i in 0 .. line.Length do
            let restOfString = inputSpan.Slice(i)

            for key in digitMap.Keys do
                if (restOfString.StartsWith(key, StringComparison.OrdinalIgnoreCase)) then
                    let foundNumber = [ digitMap.[key] ]
                    numbers <- foundNumber |> List.append numbers

        let first = numbers |> Seq.head
        let last = numbers |> Seq.rev |> Seq.head
        let result = first * 10 + last
        result)
    |> Seq.sum

Data.testData1
|> getSum part1DigitMap
|> printfn "Test data 1, part 1: %i"

Data.testData2
|> getSum part2DigitMap
|> printfn "Test data 2, part 2: %i"

Data.inputData
|> getSum part1DigitMap
|> printfn "Input data, part 1: %i"

Data.inputData
|> getSum part2DigitMap
|> printfn "Input data, part 2: %i"
