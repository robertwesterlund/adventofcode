// Learn more about F# at http://fsharp.org

open System
open System.IO
open System

let sampleData = 
    File.ReadLines(".\\sample.data") |> Seq.head

let inputData = 
    File.ReadLines(".\\input.data") |> Seq.head

type Polarity = 
    | Upper
    | Lower

type PolymerUnit =
    {      
        Id: Guid
        Visual: char
        Type : char
        Polarity : Polarity
    }

// type PolymerTree = 
//     | Leaf of Unit:PolymerUnit
//     | Branch of Left:PolymerTree * Right:PolymerTree

// type ReactionResult =
//     | PolymerUnit
//     | PolymerTree

let convertToPolymerUnit char =
    {
        Id = Guid.NewGuid()
        PolymerUnit.Visual = char
        PolymerUnit.Type = Char.ToUpper(char)
        PolymerUnit.Polarity = if Char.IsUpper(char) then Upper else Lower
    }

let convertToPolymer (str:seq<char>) =
    str |> Seq.toList |> List.map (fun c -> c |> convertToPolymerUnit)

// let rec convertToPolymer (str:seq<char>) : PolymerTree =
//     match str with
//     | _ when Seq.isEmpty str -> raise (System.Exception("Should not get here"))
//     | _ when Seq.length str = 1 -> convertToPolymerUnit (Seq.head str) |> Leaf
//     | _ -> 
//         let middle = (Seq.length str) / 2
//         (str |> Seq.take(middle) |> convertToPolymer, str |> Seq.skip(middle) |> convertToPolymer) |> Branch

let areSameType pol1 pol2 =
    pol1.Type = pol2.Type

let areSamePolarity pol1 pol2 = 
    pol1.Polarity = pol2.Polarity

// let rec removeLastPolymerUnit (polymer:PolymerTree) : Option<PolymerTree> =
//     match polymer with
//     | Leaf _ -> None
//     | Branch (left, right) ->
//         let childResult = right |> removeLastPolymerUnit
//         match childResult with
//         | None -> Some(left)
//         | Some(newRightTree) -> Some((left, newRightTree) |> Branch)

// let rec removeFirstPolymerUnit (polymer:PolymerTree) : Option<PolymerTree> = 
//     match polymer with
//     | Leaf _ -> None
//     | Branch (left, right) ->
//         let childResult = right |> removeFirstPolymerUnit
//         match childResult with
//         | None -> Some(right)
//         | Some(newLeftTree) -> Some((newLeftTree, right) |> Branch)    

let willReact pol1 pol2 = 
    areSameType pol1 pol2 && not (areSamePolarity pol1 pol2)

let rec react polymer =
    let reaction = polymer |> List.windowed 2 |> List.tryPick (fun [w1; w2] -> if (willReact w1 w2) then Some([w1; w2]) else None)
    //reaction |> printfn "Found reaction %A"
    match reaction with
    | None -> polymer
    | Some (window) -> polymer |> List.except window |> react

// let rec reactOrGetLastPolymerUnit (polymerUnitToReactWith:Option<PolymerUnit>) (polymer:PolymerTree) : TreeOrUnit = 
//     match polymer with
//     | Leaf _ -> polymer
//     | Branch (left, right)

let runFirstTask (name:string) (data:string) =
    printfn "Running %s" name
    let result = data |> convertToPolymer |> react
    //result |> Seq.iter (fun pol -> printf "%c" pol.Visual)
    printfn ""
    printfn "Length was %d" (Seq.length result)
    printfn "--------------"

type ResultsWithIgnoredType =
    {
        IgnoredType : char
        LengthAfterReaction : int
    }

let runSecondTask (name:string) (data:string) = 
    printfn "Running Second Task with %s" name
    let polymer = data |> convertToPolymer 
    let uniquePolymerTypes = data |> Seq.map (fun c -> Char.ToUpper(c)) |> Seq.distinct
    let results = uniquePolymerTypes |> Seq.toArray |> Array.Parallel.map (fun ignoreType ->
        let entriesToIgnore = polymer |> List.filter (fun u -> u.Type = ignoreType)
        let results = polymer |> List.except entriesToIgnore |> react
        {
            ResultsWithIgnoredType.IgnoredType = ignoreType
            ResultsWithIgnoredType.LengthAfterReaction = Seq.length results
        }
    )
    printfn ""
    results |> Seq.iter (fun r -> printfn "Length, when ignoring %c, was %d" r.IgnoredType r.LengthAfterReaction)
    printfn "--------------"

[<EntryPoint>]
let main argv =
    sampleData |> runFirstTask "sample data"
    //inputData |> runFirstTask "input data"
    sampleData |> runSecondTask "sample data"
    inputData |> runSecondTask "input data"
    0 // return an integer exit code
