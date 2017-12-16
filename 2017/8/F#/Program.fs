// Learn more about F# at http://fsharp.org

open System
open System.Collections.Generic;
open System.IO;

let state = new Dictionary<string, int>();

let getComparer sign = 
    match sign with
        | "==" -> (=)
        | "<" -> (<)
        | ">" -> (>)
        | "!=" -> (<>)
        | "<=" -> (<=)
        | ">=" -> (>=)

let getAction name = 
    match name with
        | "inc" -> (+)
        | "dec" -> (-)

let getStateValue name =
    match state.ContainsKey(name) with 
        | true -> state.[name]
        | false -> 0

let parseInt (text:string) =
    Int32.Parse text

let getHighestValue (dict:Dictionary<string, int>) = 
    let hasValues = dict.Count > 0
    match dict.Count > 0 with
        | true -> System.Linq.Enumerable.Max dict.Values
        | false -> 0

let execute (line:string) = 
    Console.WriteLine line
    let split = line.Split [|' '|]
    let compareState = getStateValue split.[4]
    let comparisonOperator = getComparer split.[5]
    let comparisonTestValue = parseInt split.[6]
    if (comparisonOperator compareState comparisonTestValue) then
        let currentState = getStateValue split.[0]
        let action = getAction split.[1]
        let delta = parseInt split.[2]
        let newVal = action currentState delta
        state.[split.[0]] <- newVal
    getHighestValue state

let getStateAfterExecution datasetName =
    let filename = sprintf "%sdata.txt" datasetName
    let mutable highestOfAllTimes = 0
    IO.File.ReadAllLines(filename)
        |> Array.iter (fun line -> highestOfAllTimes <- Math.Max(execute line, highestOfAllTimes))
    //|> Array.iter execute
    //|> printfn "State is: %A" state
    state, highestOfAllTimes



[<EntryPoint>]
let main argv =
    let (finalState, highestOfAllTimes) = getStateAfterExecution argv.[0]
    let highestValue = getHighestValue finalState
    let message = sprintf "Highest value is %i" highestValue
    let message = sprintf "Highest value of all times is %i" highestOfAllTimes
    System.Console.WriteLine message
    printfn "%A" finalState
    0 // return an integer exit code
