// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Text
open System.ComponentModel
open System.Reflection

type Verdict =
    | Planted
    | Empty

type Generation =
    {
        GenerationNumber : int64        
        PotsWithPlants : int64[]
    }

let convertStringToFilledPots str =
    str
        |> Seq.mapi (fun i c -> 
            if c = '#' 
            then Some(i |> int64) 
            else None
        ) 
        |> Seq.choose (id)
        |> Seq.toArray

let parseRules (data:seq<string>) = 
    data 
        |> Seq.filter (fun s -> s.EndsWith("#"))
        |> Seq.map (fun s -> s.Substring(0, 5) |> convertStringToFilledPots |> Array.map (fun i -> (i |> int64) - 2L)) 
        |> Seq.cache

let sampleInitialState = {Generation.GenerationNumber = 0L; Generation.PotsWithPlants = "#..#.#..##......###...###" |> convertStringToFilledPots;}
let realInitialState = {Generation.GenerationNumber = 0L; Generation.PotsWithPlants = "##.##..#.#....#.##...###.##.#.#..###.#....##.###.#..###...#.##.#...#.#####.###.##..#######.####..#" |> convertStringToFilledPots;}
let sampleRules = 
    [
        "...## => #"
        "..#.. => #"
        ".#... => #"
        ".#.#. => #"
        ".#.## => #"
        ".##.. => #"
        ".#### => #"
        "#.#.# => #"
        "#.### => #"
        "##.#. => #"
        "##.## => #"
        "###.. => #"
        "###.# => #"
        "####. => #"
    ] |> parseRules
let realRules = 
    [
        ".##.. => #"
        "#...# => ."
        "####. => #"
        "##..# => #"
        "..##. => ."
        ".###. => ."
        "..#.# => ."
        "##### => ."
        "##.#. => #"
        "...## => #"
        ".#.#. => ."
        "##.## => #"
        "#.##. => ."
        "#.... => ."
        "#..## => ."
        "..#.. => #"
        ".#..# => #"
        ".#.## => #"
        "...#. => ."
        ".#... => #"
        "###.# => #"
        "#..#. => #"
        ".#### => #"
        "#.### => #"
        ".##.# => #"
        "#.#.. => ."
        "###.. => #"
        "..... => ."
        "##... => ."
        "....# => ."
        "#.#.# => #"
        "..### => #"
    ] |> parseRules

let judge rules =
    fun (previousGeneration:Generation) index -> 
        let slice  = previousGeneration.PotsWithPlants |> Seq.filter (fun i -> index - 2L <= i && index + 2L >= i) |> Seq.map (fun i -> i - index) |> Seq.toArray
        let foundRule = rules |> Seq.tryFind (fun r -> r = slice)
        match foundRule with
        | None -> Empty
        | Some(_) -> Planted

let getNextGeneration (trainedJudge:Generation -> int64 -> Verdict) previousGeneration =
    let shouldIndexBePlanted = trainedJudge previousGeneration
    let minIndex = previousGeneration.PotsWithPlants |> Seq.min
    let maxIndex = previousGeneration.PotsWithPlants |> Seq.max

    {
        Generation.GenerationNumber = previousGeneration.GenerationNumber + 1L
        Generation.PotsWithPlants = 
            seq { minIndex - 4L .. maxIndex + 4L} 
                |> Seq.choose (fun index -> if shouldIndexBePlanted index = Planted then Some(index) else None)
                |> Seq.toArray
    }

type LoopInfo = 
    {
        LoopFromGeneration : int64
        LoopDetectedAtGeneration : int64
        Skew : int64
    }

let areSameButSkewed (old:int64 array) (newItem:int64 array) = 
    let delta = newItem.[0] - old.[0]
    let adjustedNew = newItem |> Seq.map (fun v -> v - delta) |> Seq.toArray
    old = adjustedNew

let rec getNumberOfGenerationsBeforeLoop evolutionaryTheory previousGenerations lastGeneration =
    //File.AppendAllText(".\\generations.log", lastGeneration.GenerationNumber.ToString() + ": " + String.Join(", ", lastGeneration.PotsWithPlants |> Seq.map (fun p -> p.ToString())) + Environment.NewLine)
    let nextGeneration = getNextGeneration evolutionaryTheory lastGeneration
    if nextGeneration.GenerationNumber % 1000L = 0L then printfn "Testing generation %i" nextGeneration.GenerationNumber
    let loopRootResult = previousGenerations |> Seq.tryFind (fun p -> areSameButSkewed p.PotsWithPlants nextGeneration.PotsWithPlants)
    match loopRootResult with 
    | Some(loopRoot) ->
        {
            LoopInfo.LoopFromGeneration = loopRoot.GenerationNumber
            LoopInfo.LoopDetectedAtGeneration = nextGeneration.GenerationNumber
            LoopInfo.Skew = nextGeneration.PotsWithPlants.[0] - loopRoot.PotsWithPlants.[0]
        } 
    | None ->
        let allGenerations = previousGenerations |> Seq.append [nextGeneration] |> Seq.cache
        getNumberOfGenerationsBeforeLoop evolutionaryTheory allGenerations nextGeneration

let rec darwinObserve (evolutionaryTheory: Generation -> int64 -> Verdict) (generationsLeft:int64) (generation:Generation)  =
    if (generationsLeft = 0L)
    then
        [generation]
    else
        let nextGeneration = getNextGeneration evolutionaryTheory generation
        generation :: (nextGeneration |> darwinObserve evolutionaryTheory (generationsLeft - 1L))

let getResults rules initialState generations =
    let trainedJudge = judge rules
    let survivalOfTheRules = darwinObserve trainedJudge generations
    initialState |> survivalOfTheRules 

let sumValue generation = 
    generation.PotsWithPlants |> Seq.sum

let sampleEvolution = getResults sampleRules sampleInitialState

let realEvolution = getResults realRules realInitialState
    
[<EntryPoint>]
let main argv =
    sampleEvolution 20L |> Seq.rev |> Seq.head |> sumValue |> printfn "Sample 20 gen value: %i"
    
    realEvolution 20L |> Seq.rev |> Seq.head |> sumValue |> printfn "Real 20 gen value: %i"
    
    let loopInfo = getNumberOfGenerationsBeforeLoop (judge realRules) (Seq.singleton realInitialState) realInitialState 
    printfn "Real evolution lopp info: %A " loopInfo
    let generationStartingLoop = realEvolution loopInfo.LoopFromGeneration |> Seq.rev |> Seq.head
    let numberOfGenerationsLooping = loopInfo.LoopDetectedAtGeneration - loopInfo.LoopFromGeneration
    let generationAfterOneLoop = getResults realRules generationStartingLoop numberOfGenerationsLooping |> Seq.rev |> Seq.head
    let targetGeneration = 50000000000L
    let numberOfLoops = (targetGeneration - loopInfo.LoopFromGeneration) / numberOfGenerationsLooping
    let skewPerLoop = generationAfterOneLoop.PotsWithPlants.[0] - generationStartingLoop.PotsWithPlants.[0]
    let generationsAfterLoop = targetGeneration - numberOfLoops * numberOfGenerationsLooping - loopInfo.LoopFromGeneration
    let skew = numberOfLoops * (skewPerLoop |> int64)
    let result = 
        {
            Generation.GenerationNumber = targetGeneration
            Generation.PotsWithPlants =
                getResults realRules generationStartingLoop generationsAfterLoop 
                    |> Seq.map (fun g -> g.PotsWithPlants)
                    |> Seq.rev
                    |> Seq.head
                    |> Seq.map (fun i -> i + skew) 
                    |> Seq.toArray
        }
    result |> sumValue |> printfn "Real %i gen value: %i" targetGeneration

    0 // return an integer exit code