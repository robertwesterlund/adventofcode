module Part2

open System
open Xunit

let rec ``calculate fuel consumption recursively`` mass =
    match (Part1.``calculate fuel consumption`` mass) with
    | fuel when fuel > 0m -> fuel + (fuel |> ``calculate fuel consumption recursively``)
    | _ -> 0m

let ``sum fuel consumption recursively`` input = input |> List.sumBy ``calculate fuel consumption recursively``

let answer = Data.input |> ``sum fuel consumption recursively``

[<Theory>]
[<InlineData(12, 2)>]
[<InlineData(14, 2)>]
[<InlineData(1969, 966)>]
[<InlineData(100756, 50346)>]
let ``test part two samples`` input expectedResult =
    Assert.Equal(expectedResult, input |> ``calculate fuel consumption recursively``)

[<Fact>]
let ``test part two sum``() =
    let samples = [ 12m; 14m; 1969m; 100756m ]
    let expectedResult = 51316m
    Assert.Equal(expectedResult, (samples |> ``sum fuel consumption recursively``))
