module Part1

open System
open Xunit

let ``calculate fuel consumption`` mass = Math.Floor(mass / 3m) - 2m

let ``sum fuel consumption`` input = input |> List.sumBy ``calculate fuel consumption``

let answer = Data.input |> ``sum fuel consumption``

[<Theory>]
[<InlineData(12, 2)>]
[<InlineData(14, 2)>]
[<InlineData(1969, 654)>]
[<InlineData(100756, 33583)>]
let ``test part one samples`` input expectedResult =
    Assert.Equal(expectedResult, input |> ``calculate fuel consumption``)

[<Fact>]
let ``test part one sum``() =
    let samples = [ 12m; 14m; 1969m; 100756m ]
    let expectedResult = 34241m
    Assert.Equal(expectedResult, (samples |> ``sum fuel consumption``))
