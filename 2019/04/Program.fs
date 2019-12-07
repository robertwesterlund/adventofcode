// Learn more about F# at http://fsharp.org

open System
open Xunit;

let rec isAnyDigitDouble (chars: char list) = 
    match chars with
    | head :: tail when tail |> List.isEmpty -> false
    | head :: tail when head.Equals(tail |> List.head) -> true
    | head :: tail -> isAnyDigitDouble tail

let isAnyDigitDecreasing chars =
    let rec inner prev charsLeft =
        match charsLeft with
        | head :: tail when prev > head -> true
        | head :: tail -> inner head tail
        | [] -> false
    match chars with
    | head :: tail -> inner head tail

let isLackingDecreasingDigits chars =
    not (isAnyDigitDecreasing chars)

let isMatchPart1 input = 
    let chars = input.ToString().ToCharArray() |> Seq.toList
    isAnyDigitDouble chars && isLackingDecreasingDigits chars

let isAnyDigitDoubleButNotTriple chars =
    let rec inner prevPrev prev rest = 
        match rest with
        | head :: tail when tail |> List.isEmpty -> head.Equals(prev) && not(head.Equals(prevPrev))
        | head :: second :: tail when head.Equals(prev) && not(head.Equals(prevPrev)) && not(head.Equals(second)) -> true
        | head :: second :: tail -> inner prev head (second::tail)
    inner ('-') (chars |> List.head) (chars |> List.skip 1)    

let isMatchPart2 input = 
    let chars = input.ToString().ToCharArray() |> Seq.toList
    isAnyDigitDoubleButNotTriple chars &&  isLackingDecreasingDigits chars

[<Theory>]
[<InlineData(111111, true)>]
[<InlineData(123455, true)>]
[<InlineData(223450, false)>]
[<InlineData(123789, false)>]
[<InlineData(153517, false)>]
[<InlineData(630395, false)>]
let ``part1 samples provide expected result`` input expectedResult=
    Assert.Equal(expectedResult, input |> isMatchPart1)

[<Theory>]
[<InlineData(112233, true)>]
[<InlineData(111122, true)>]
[<InlineData(112345, true)>]
[<InlineData(122345, true)>]
[<InlineData(123345, true)>]
[<InlineData(123445, true)>]
[<InlineData(123455, true)>]
[<InlineData(111123, false)>]
[<InlineData(123444, false)>]
let ``part2 samples provide expected result`` input expectedResult=
    Assert.Equal(expectedResult, input |> isMatchPart2)

let input = 
    seq { 153517 .. 630395 } 

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    printfn "Part 1 - Number of matches: %i" (input |> Seq.filter isMatchPart1 |> Seq.length)
    printfn "Part 2 - Number of matches: %i" (input |> Seq.filter isMatchPart2 |> Seq.length)
    0 // return an integer exit code
