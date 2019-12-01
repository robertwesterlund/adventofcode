// Learn more about F# at http://fsharp.org

open System
open System.Collections.Generic
open System.Globalization
open System.Runtime.CompilerServices
open System.Collections.Generic

let sampleData = 
    System.IO.File.ReadLines(".\\sample.data")

let unorderedSampleData = 
    System.IO.File.ReadLines(".\\unorderedsample.data")

let inputData = 
    System.IO.File.ReadLines(".\\input.data")

type Action = 
    | ShiftStart of id: int
    | FallsAsleep
    | WakesUp

type Event = 
    {
        Date : System.DateTime        
        Action : Action
    }

type Sleep =
    {
        FallsAsleep : DateTime
        WakesUp : DateTime
    }

type SleepSummary = 
    {
        GuardId : int
        TotalSleep : TimeSpan
    }

let getIdPartFromShiftStart (actionPartOfLine:string) : string = 
    let idPartStartIndex = "Guard #".Length
    actionPartOfLine.Substring(idPartStartIndex, actionPartOfLine.IndexOf(" ", idPartStartIndex) - idPartStartIndex)

let parseAction (actionPartOfLine:string) : Action  = 
    if actionPartOfLine.StartsWith("Guard #") then 
        actionPartOfLine |> getIdPartFromShiftStart |> int |> ShiftStart
    else if actionPartOfLine.StartsWith("falls asleep") then 
        FallsAsleep
    else
        WakesUp    

let parseData (line:string) : Event  = 
    line.Substring(1, "1518-11-03 00:24".Length) 
    |> (fun (str:string) -> DateTime.ParseExact(str, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.CurrentCulture)) 
    |> (fun date -> { Event.Date = date; Event.Action = line.Substring("[1518-11-03 00:24] ".Length) |> parseAction})

let convertToSleepEntries (evts:seq<Event>) =
    let mutable currentGuardId = -1
    let mutable lastTimeFallenAsleep = DateTime.MinValue
    let mutable sleepEntries = Map.empty<int, seq<Sleep>>
    let sortedEntries = evts |> Seq.sortBy (fun e -> e.Date) 
    for evt in sortedEntries do 
        match evt.Action with
        | ShiftStart id -> currentGuardId <- id
        | FallsAsleep -> lastTimeFallenAsleep <- evt.Date
        | WakesUp -> let sleep = { Sleep.FallsAsleep = lastTimeFallenAsleep; Sleep.WakesUp = evt.Date }
                     if sleepEntries.ContainsKey(currentGuardId) 
                     then 
                        let newEntries = sleepEntries.[currentGuardId] |> Seq.append (Seq.singleton sleep)
                        sleepEntries <- sleepEntries |> Map.remove currentGuardId |> Map.add currentGuardId newEntries
                     else sleepEntries <-sleepEntries |> Map.add currentGuardId (Seq.singleton sleep)
    sleepEntries             

type TimeSleptDuringMinute = 
    {
        Minute : int
        TimesSlept : int   
    }

let getMostSleptMinute (sleep: seq<Sleep>) : TimeSleptDuringMinute =
    sleep 
        |> Seq.collect (fun s -> [| 0 .. 200 |] |> Seq.map (fun x -> s.FallsAsleep.AddMinutes((float)x)) |> Seq.takeWhile (fun time -> time < s.WakesUp))
        |> Seq.filter (fun s -> s.Hour = 0)
        |> Seq.groupBy (fun s -> s.Minute)
        |> Seq.map (fun (key, group) -> {TimeSleptDuringMinute.Minute = key; TimeSleptDuringMinute.TimesSlept = Seq.length group})
        |> Seq.sortByDescending (fun entry -> entry.TimesSlept)
        |> Seq.head 

let sumSleepMinutesPerGuard (sleepEntries : Map<int, seq<Sleep>>) = //: seq<SleepSummary> = 
    sleepEntries
        |> Seq.collect (fun x -> x.Value |> Seq.map (fun s -> {SleepSummary.GuardId = x.Key; SleepSummary.TotalSleep = s.WakesUp - s.FallsAsleep}))
        |> Seq.groupBy (fun x -> x.GuardId)
        |> Seq.map (fun (id, entries) ->
            { 
                SleepSummary.GuardId = id
                SleepSummary.TotalSleep = (entries |> Seq.fold (fun  prev sl -> prev + sl.TotalSleep) TimeSpan.Zero)
            }
        )

let printSleepPattern (sleepEntries: Map<int, seq<Sleep>>) : Unit =
    sleepEntries 
        |> Seq.collect (fun x -> x.Value |> Seq.map (fun s -> (x.Key, s))) 
        |> Seq.sortBy (fun (_, s) -> s.FallsAsleep) 
        |> Seq.iter (printfn "%A")

let runStrategyOne (dataName:string) (data:seq<string>) : Unit =
    let sleepEntries = data |> Seq.map parseData |> convertToSleepEntries
    let mostSleepingGuard = sleepEntries |> sumSleepMinutesPerGuard |> Seq.maxBy (fun s -> s.TotalSleep)
    let mostSleptMinute = sleepEntries.[mostSleepingGuard.GuardId] |> getMostSleptMinute
    printfn "%s, guard %d, best minute: %d, value: %d" dataName mostSleepingGuard.GuardId mostSleptMinute.Minute (mostSleepingGuard.GuardId * mostSleptMinute.Minute)

let runStrategyTwo (dataName:string) (data:seq<string>) : Unit = 
    let sleepEntries = data |> Seq.map parseData |> convertToSleepEntries
    let (guardId, _)  = sleepEntries |> Map.toSeq |> Seq.maxBy (fun (g, s) -> (s |> getMostSleptMinute).TimesSlept)
    let mostSleptMinute = sleepEntries.[guardId] |> getMostSleptMinute
    printfn "%s, guard %d, best minute: %d, value: %d" dataName guardId mostSleptMinute.Minute (guardId * mostSleptMinute.Minute)

[<EntryPoint>]
let main argv =
    sampleData |> runStrategyOne "Sample data"
    unorderedSampleData |> runStrategyOne "Unordered sample data"
    inputData |> runStrategyOne "Input data"
    sampleData |> runStrategyTwo "Sample data"
    unorderedSampleData |> runStrategyTwo "Unordered sample data"
    inputData |> runStrategyTwo "Input data"
    0 // return an integer exit code
