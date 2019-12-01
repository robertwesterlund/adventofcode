
// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Text.RegularExpressions

let sampleData = 
    File.ReadAllLines(".\\sample.data")
let inputData = 
    File.ReadAllLines(".\\input.data")

type Instruction =
    {
        Successor : char
        Predecessor : char
    }

let parseInstructions lines = 
    lines 
        |> Seq.map (fun line -> Regex.Match(line, "^Step (?<predecessor>.) must be finished before step (?<successor>.) can begin\\.$"))
        |> Seq.cast<Match>
        |> Seq.map (fun m -> 
            {
                Instruction.Predecessor = m.Groups.["predecessor"].Value.[0]
                Instruction.Successor = m.Groups.["successor"].Value.[0]
            }
        ) 

let getAllStepNames instructions = 
    instructions |> Seq.collect (fun i -> [i.Predecessor; i.Successor]) |> Seq.distinct

type Worker = 
    {
        WorkingOnStep : Option<char>
        CurrentWorkDone : int
    }

type WorkState = 
    {
        SecondsWorked : int
        CostCalculator : char -> int
        Workers : seq<Worker>
        InstructionsLeft : seq<Instruction>
        FinishedWorkOrder : seq<char>
    }

let getCandidateForWork instructions =
    instructions 
        |> getAllStepNames
        |> Seq.filter (fun c -> not (instructions |> Seq.exists (fun s -> s.Successor = c)))
        |> Seq.sort

let isWorkFinishedAfterThisSecond costCalculator worker =
    match worker.WorkingOnStep with
    | None -> false
    | Some(step) -> worker.CurrentWorkDone >= (step |> costCalculator) - 1

let getFinishedWorkAfterSecond workstate = 
    Seq.concat [
        workstate.FinishedWorkOrder
        workstate.Workers 
            |> Seq.filter (isWorkFinishedAfterThisSecond workstate.CostCalculator) 
            |> Seq.choose (fun w -> w.WorkingOnStep)
    ]               

let getInstructionsLeftAfterSecond workstate =
    workstate.InstructionsLeft 
        |> Seq.filter (fun i -> not (workstate |> getFinishedWorkAfterSecond |> Seq.contains i.Predecessor))

let hasOngoingWorkAfterThisSecond costCalculator worker =
    worker.WorkingOnStep <> None && not (worker |> isWorkFinishedAfterThisSecond costCalculator)

let getWorkerStateAfterSecond workstate = 
    Seq.concat [
        workstate.Workers 
            |> Seq.filter (fun w -> not (hasOngoingWorkAfterThisSecond workstate.CostCalculator w))
            |> Seq.zip (
                workstate
                    |> getInstructionsLeftAfterSecond 
                    |> getCandidateForWork 
                    |> Seq.filter (fun c -> 
                        not (workstate.Workers 
                            |> Seq.exists(fun w -> 
                                w.WorkingOnStep = Some(c)
                            )
                        )
                    )
                )
            |> Seq.map (fun (step, _) ->
            {
                Worker.CurrentWorkDone = 0
                Worker.WorkingOnStep = Some(step)
            })
        workstate.Workers
            |> Seq.filter (hasOngoingWorkAfterThisSecond workstate.CostCalculator)
            |> Seq.map (fun w -> 
            {
                w with
                    Worker.CurrentWorkDone = w.CurrentWorkDone + 1
            })
        Seq.initInfinite (fun _ -> 
            {
                Worker.CurrentWorkDone = 0
                Worker.WorkingOnStep = None
            }
        )        
    ] |> Seq.take (workstate.Workers |> Seq.length)

let WorkOneSecond workState =
    {
        workState with
            WorkState.SecondsWorked = workState.SecondsWorked + 1
            WorkState.FinishedWorkOrder = workState |> getFinishedWorkAfterSecond |> Seq.cache
            WorkState.InstructionsLeft = workState |> getInstructionsLeftAfterSecond |> Seq.cache
            WorkState.Workers = workState |> getWorkerStateAfterSecond |> Seq.cache
    }

let rec DoAllTheWork workstate = 
    printf "%3i - State: %d instructions left, finished work: " workstate.SecondsWorked (workstate.InstructionsLeft |> Seq.length)
    workstate.FinishedWorkOrder |> Seq.iter (printf "%c")
    printfn ""
    workstate.Workers |> Seq.iter (fun w -> printfn "Work done %2i, working on %A" w.CurrentWorkDone w.WorkingOnStep)
    match workstate with
    //Only once there are no more instructions left and none of the workers have any work left
    | _ when workstate.InstructionsLeft |> Seq.isEmpty && not (workstate.Workers |> Seq.exists (fun w -> w.WorkingOnStep <> None)) -> workstate
    | _ -> workstate |> WorkOneSecond |> DoAllTheWork

let getWorkOrder costCalculator workerCount data =
    let instructions = data |> parseInstructions |> Seq.cache
    (
        DoAllTheWork 
            {
                //The sample lets work be performed during second 0
                WorkState.SecondsWorked = -1 
                //The instructions state to calculate cost of work based on character code + base cost
                WorkState.CostCalculator = costCalculator
                WorkState.FinishedWorkOrder = Seq.empty
                WorkState.InstructionsLeft = 
                    Seq.concat [
                        instructions
                        //Due to how we handle instruction removal, add "empty" successors to all entries which don't have any successors
                        instructions 
                            |> getAllStepNames 
                            |> Seq.filter (fun c -> not (instructions |> Seq.exists (fun i -> i.Predecessor = c)))
                            |> Seq.map (fun c -> {Instruction.Predecessor = c; Instruction.Successor = '-'})
                    ] |> Seq.cache                    
                WorkState.Workers = Seq.init workerCount (fun _ -> {Worker.CurrentWorkDone = 0; Worker.WorkingOnStep = None})
            }
    )

let runSecondTask baseCost workerCount data =
    let result = data |> getWorkOrder (fun c -> baseCost + (int c) - (int 'A') + 1) workerCount
    result.SecondsWorked |> printfn "Performing work will take %i seconds"
    result.FinishedWorkOrder |> Seq.iter (printf "%c")
    printfn ""
    
let runFirstTask data = 
    let result = data |> getWorkOrder (fun c -> 1) 1
    result.FinishedWorkOrder |> Seq.iter (printf "%c")
    printfn ""    

[<EntryPoint>]
let main argv =
    // printfn "Sample data"
    // sampleData |> runFirstTask
    // printfn "Input data"
    // inputData |> runFirstTask
    
    printfn "Sample data"
    sampleData |> runSecondTask 0 2
    // sampleData |> runSecondTask 60 2
    printfn "Input data"
    inputData |> runSecondTask 60 5
    0 // return an integer exit code
