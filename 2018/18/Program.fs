// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Security.Cryptography.X509Certificates
open System.Text.RegularExpressions

let sampleData = File.ReadAllLines("sample.data")

let inputData = File.ReadAllLines("input.data")

type Position = 
    {
        X : int
        Y : int
    }

type AcreContent =
    | Open
    | Trees
    | Lumberyard

let isAcreMatching (area: 'T array array) position predicate =
    match position with
    | _ when position.X < 0 || position.Y < 0 -> false
    | _ when position.X >= area.[0].Length || position.Y >= area.Length -> false
    | _ -> area.[position.Y].[position.X] |> predicate

let countAdjacentMatching area centerPosition predicate =
    let test x y = isAcreMatching area {Position.X = x; Position.Y = y} predicate
    [
        test (centerPosition.X - 1) (centerPosition.Y - 1)
        test (centerPosition.X) (centerPosition.Y - 1)
        test (centerPosition.X + 1) (centerPosition.Y - 1)
        test (centerPosition.X - 1) (centerPosition.Y)
        test (centerPosition.X + 1) (centerPosition.Y)
        test (centerPosition.X - 1) (centerPosition.Y + 1)
        test (centerPosition.X) (centerPosition.Y + 1)
        test (centerPosition.X + 1) (centerPosition.Y + 1)
    ] |> Seq.filter (fun res -> res) |> Seq.length

let getResultingAcre area position =
    let numberOf = countAdjacentMatching area position
    match area.[position.Y].[position.X] with
    | Open -> if numberOf (fun t -> t = Trees) > 2 then Trees else Open
    | Trees -> if numberOf (fun t -> t = Lumberyard) > 2 then Lumberyard else Trees
    | Lumberyard -> 
        if numberOf (fun t -> t = Lumberyard) > 0 && numberOf (fun t -> t = Trees) > 0
        then Lumberyard
        else Open

let traverse transformation (area:AcreContent[][]) = 
    Array.init area.Length (fun y -> 
        Array.init area.[y].Length (fun x ->
            transformation {Position.X = x; Position.Y = y}
        )
    )    

let areaAfterOneMinute (area:AcreContent[][]) = 
    area |> traverse (fun p -> getResultingAcre area p)

let convertToOutput (area:AcreContent[][]) = 
    area |> traverse (fun p -> 
        match area.[p.Y].[p.X] with 
        | Open -> '.'
        | Trees -> '|'
        | Lumberyard -> '#'
    )

let parse (data:string[]) =
    Array.init data.Length (fun y ->
        Array.init data.[y].Length (fun x ->
            match data.[y].[x] with
            | '#' -> Lumberyard
            | '|' -> Trees
            | '.' -> Open
            | _ -> raise (Exception "Unexpected input, I don't care what...")
        )
    )

let printArea area =
    area 
        |> convertToOutput 
        |> Seq.iter (fun row ->
            row |> Seq.iter (fun cell -> cell |> printf "%c")
            printfn ""
        )

let count predicate (area:AcreContent[][]) =
    area |> Seq.map (fun row -> row |> Seq.filter predicate |> Seq.length) |> Seq.sum

let evolve minutes area =
    let rec evolve_rec minutes (alreadySeen:List<AcreContent[][]>) area = 
        if (minutes = 0)
        then area
        else 
            let areaAfterOneMinute = area |> areaAfterOneMinute 
            if (alreadySeen |> List.contains areaAfterOneMinute)
            then
                //We've found a loop, let's not evolve the rest, but just pick the end result
                let indexOfLoopStart = alreadySeen |> List.findIndex (fun arr -> arr = areaAfterOneMinute)
                let lengthOfLoop = (alreadySeen |> List.length) - indexOfLoopStart
                let offSet = (minutes - 1) % lengthOfLoop
                alreadySeen.[indexOfLoopStart + offSet]              
            else
                areaAfterOneMinute |> evolve_rec (minutes - 1) (List.append alreadySeen (List.singleton areaAfterOneMinute))
    area |> evolve_rec minutes List.empty                  

let getResourceValue area =
    (area |> count (fun t -> t = Trees)) * (area |> count (fun t -> t = Lumberyard))

[<EntryPoint>]
let main argv =
    printfn "Initial"
    sampleData |> parse |> printArea
    printfn "After one minute"
    sampleData |> parse |> areaAfterOneMinute |> printArea
    printfn "After 10 minutes"
    sampleData |> parse |> evolve 10 |> printArea
    
    let resultForInput = inputData |> parse |> evolve 1000000000
    //resultForInput |> printArea
    resultForInput |> getResourceValue |> printfn "Input resource count after 1000000000 minutes is: %i"

    0 // return an integer exit code