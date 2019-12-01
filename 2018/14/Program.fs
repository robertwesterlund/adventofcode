// Learn more about F# at http://fsharp.org

open System
open System.Collections.Generic

type Worker = 
    {
        mutable Index : int
    }

let getHotChocolateEnumerator numberOfWorkers =
    let workers = seq { 0 .. numberOfWorkers - 1} |> Seq.map (fun index -> { Worker.Index = index }) |> Seq.toArray
    let mutable currentIndex:int = (-1)
    let mutable recipies = new List<int>()
    recipies.AddRange([3;7])
    let getWorkerResult() =
        let workersAndScores = workers |> Seq.map (fun w -> (w, recipies.[w.Index])) |> Seq.cache
        let totalScore = workersAndScores |> Seq.sumBy (fun (worker, score) -> score)
        let results = 
            if totalScore < 10
            then [totalScore]
            else
                let firstDigit = totalScore / 10
                [firstDigit; totalScore - (firstDigit * 10)]
        workersAndScores |> Seq.iter (fun (w,s) -> w.Index <- (w.Index + 1 + s) % ((recipies.Count) + (results |> List.length)))
        results

    fun () -> 
        currentIndex <- currentIndex + 1
        if currentIndex >= recipies.Count
        then 
            recipies.AddRange(getWorkerResult())
        recipies.[currentIndex]

let getInititeAmountsOfHotChocolate workerCount = 
    let enumerator = getHotChocolateEnumerator workerCount 
    Seq.initInfinite (fun i -> enumerator()) |> Seq.cache

let printNextRecipies workerCount numberOfTestRecipies numberOfRecipiesToTake= 
    printf "Next %i recipies, after %i workers have done %i test recipies are: " numberOfRecipiesToTake workerCount numberOfTestRecipies
    getInititeAmountsOfHotChocolate workerCount |> Seq.skip numberOfTestRecipies |> Seq.take numberOfRecipiesToTake |> Seq.iter (printf "%i")
    printfn ""

let getIndexOfPattern (pattern:int[]) =
    let enumerator = getHotChocolateEnumerator 2
    Seq.initInfinite (fun i -> (i,enumerator())) 
        |> Seq.cache 
        |> Seq.windowed pattern.Length 
        |> Seq.pick (fun window -> 
            if pattern = (window |> Seq.map (fun (_, value) -> value) |> Seq.toArray)
            then Some(window |> Seq.map (fun (i, _) -> i) |> Seq.head)
            else None
        )        

let printRecipiesBeforePattern pattern =
    let patternAsIntArray = pattern |> Seq.map (fun c -> Char.GetNumericValue(c) |> int) |> Seq.toArray
    let index = getIndexOfPattern patternAsIntArray
    printf "Found pattern "
    patternAsIntArray |> Seq.iter (printf "%i")
    printfn " with %i recipies before" index

[<EntryPoint>]
let main argv =
    
    printNextRecipies 2 9 10
    printNextRecipies 2 5 10
    printNextRecipies 2 18 10
    printNextRecipies 2 2018 10
    printNextRecipies 2 894501 10

    printRecipiesBeforePattern "51589"
    printRecipiesBeforePattern "01245"
    printRecipiesBeforePattern "92510"
    printRecipiesBeforePattern "59414"
    printRecipiesBeforePattern "894501"

    0 // return an integer exit code
