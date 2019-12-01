// Learn more about F# at http://fsharp.org

open System

type Position = 
    {
        X : int
        Y : int
    }

type FuelCell = 
    {
        Position : Position
        PowerLevel : int
    }

let getFuelCells (powerlevelAlgorithm:Position -> int) : FuelCell array array =
    let fuelCells : FuelCell array array = Array.zeroCreate 300
    for y in 1 .. 300 do
        fuelCells.[y-1] <- Array.zeroCreate 300
        for x in 1 .. 300 do
            let position = {Position.X = x; Position.Y = y}
            fuelCells.[y-1].[x-1] <- 
                {
                    FuelCell.Position = position
                    FuelCell.PowerLevel = position |> powerlevelAlgorithm
                }
    fuelCells

let firstPowerLevelAlgorithm gridSerialNumber position =
    let rackId = position.X + 10
    let powerlevelString = ((rackId * position.Y + gridSerialNumber) * rackId).ToString()
    if powerlevelString.Length > 2
    then
        Int32.Parse(powerlevelString.[powerlevelString.Length - 3].ToString()) - 5
    else
        -5

let getFuelCellWindows windowSize (fuelCells:FuelCell array array) = 
    seq { 1 .. 300 } |> Seq.windowed windowSize |> Seq.collect (fun yWindow ->
        seq { 1 .. 300 } |> Seq.windowed windowSize |> Seq.map (fun xWindow ->
            yWindow |> Seq.collect (fun y -> xWindow |> Seq.map (fun x -> fuelCells.[y-1].[x-1]))
        ) |> Seq.cache
    ) |> Seq.cache

let getAllSizedFuelCellWindows fuelCells =
    seq { 1 .. 20 } |> Seq.collect (fun windowSize ->
            getFuelCellWindows windowSize fuelCells |> Seq.cache
        )

let findLargestFuelCellWindow fuelCells =
    let (largestWindow, _) = 
        fuelCells 
            |> getAllSizedFuelCellWindows
            |> Seq.map (fun window -> (window, window |> Seq.sumBy (fun f -> f.PowerLevel)))
            |> Seq.maxBy (fun (_, power) -> power)
    largestWindow

let firstTask gridSerialNumber =
    let highestPoweredWindow = firstPowerLevelAlgorithm gridSerialNumber |> getFuelCells |> findLargestFuelCellWindow
    let topLeftOfWindow = highestPoweredWindow |> Seq.head
    let numberOfFuelCellsInWindow = highestPoweredWindow |> Seq.length
    let sizeOfWindow = Math.Sqrt(numberOfFuelCellsInWindow |> float)
    printfn "Using grid serial number %i finds a position and size of %i,%i,%i" gridSerialNumber topLeftOfWindow.Position.X topLeftOfWindow.Position.Y (sizeOfWindow |> int)

[<EntryPoint>]
let main argv =
    {Position.X = 3; Position.Y = 5} |> firstPowerLevelAlgorithm 8 |> printfn "%i"
    {Position.X = 122; Position.Y = 79} |> firstPowerLevelAlgorithm 57 |> printfn "%i"
    {Position.X = 217; Position.Y = 196} |> firstPowerLevelAlgorithm 39 |> printfn "%i"
    {Position.X = 101; Position.Y = 153} |> firstPowerLevelAlgorithm 71 |> printfn "%i"

    firstTask 18
    firstTask 42
    firstTask 6042

    printfn "Hello World from F#!"
    0 // return an integer exit code
