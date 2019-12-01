// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Text.RegularExpressions

let sampleData = 
    File.ReadLines(".\\sample.data") |> Seq.cache

let inputData = 
    File.ReadLines(".\\input.data") |> Seq.cache

type TimeTravel =
    | FastForward
    | Next
    | Previous
    | Quit

type Position = 
    {
        X : int
        Y : int
    }

type Velocity = 
    {
        X : int
        Y : int
    }

type Light(initialPosition, initialVelocity) =
    let mutable position = initialPosition
    let velocity = initialVelocity

    member self.GetPosition() = position

    member self.Move(direction) =
        let op = 
            match direction with
            | Next -> (+)
            | Previous -> (-)
        position <- {Position.X = op position.X velocity.X; Position.Y = op position.Y velocity.Y}

type Bounds = 
    {
        Top : int
        Bottom : int
        Left : int
        Right : int
        YSize : int
        XSize : int
    }

type Sky(lights: seq<Light>) =

    let mutable seconds = 0L
    let getBounds() = 
        let topBound = (lights |> Seq.map (fun l -> l.GetPosition().Y) |> Seq.min)
        let leftBound = (lights |> Seq.map (fun l -> l.GetPosition().X) |> Seq.min)
        let bottomBound = (lights |> Seq.map (fun l -> l.GetPosition().Y) |> Seq.max)
        let rightBound = (lights |> Seq.map (fun l -> l.GetPosition().X) |> Seq.max)
        {
            Bounds.Bottom = bottomBound
            Bounds.Top = topBound
            Bounds.Left = leftBound
            Bounds.Right = rightBound
            Bounds.YSize = bottomBound - topBound + 1
            Bounds.XSize = rightBound - leftBound + 1
        }

    member self.FastForward() =
        Console.Clear() 
        let mutable bounds = getBounds()
        let mutable counter = 0
        while bounds.XSize > 100 && bounds.YSize > 100 do
            counter <- (counter + 1) % 500
            if (counter = 0)
            then 
                Console.Clear()
                printfn "Hidden due to size. YSize: %i, XSize: %i" bounds.YSize bounds.XSize  
            // else
            //     printf "."            
            self.Move(Next)
            bounds <- getBounds()

    member self.GetSecondsPassed = seconds

    member self.Move(direction) =
        let timeDirection = if direction = Next then (+) else if direction = Previous then (-) else raise (Exception("Invalid direction passed in"))
        seconds <- timeDirection seconds 1L
        lights |> Seq.iter (fun l -> l.Move(direction))    

    member self.Draw() =
        let bounds = getBounds()

        if (bounds.YSize < 400 && bounds.XSize < 400)
        then
            let canvas:bool array array = Array.zeroCreate bounds.YSize
            for y in 0 .. (bounds.YSize-1) do
                canvas.[y] <- Array.zeroCreate bounds.XSize
            //printfn "YSize: %i, XSize: %i" ySize xSize        
            lights |> Seq.iter (fun l -> 
                //printfn "Light position: %i, %i, using index %i, %i" l.Position.Y l.Position.X (l.Position.Y - topBound) (l.Position.X - leftBound)
                let p = l.GetPosition()
                canvas.[p.Y - bounds.Top].[p.X - bounds.Left] <- true
            )
            Console.Clear()
            for y in 0 .. (bounds.YSize-1) do
                for x in 0 .. (bounds.XSize-1) do
                    printf (if canvas.[y].[x] then "X" else ".")
                printfn ""  
            printfn "Seconds %i" seconds                                  
        else
            Console.Clear()
            printfn "Hidden due to size. YSize: %i, XSize: %i" bounds.YSize bounds.XSize                    

let rec getInput() = 
    match Console.ReadKey() with
    | keyInfo when keyInfo.Key = ConsoleKey.F -> FastForward
    | keyInfo when keyInfo.Key = ConsoleKey.RightArrow -> Next
    | keyInfo when keyInfo.Key = ConsoleKey.LeftArrow -> Previous
    | keyInfo when keyInfo.Key = ConsoleKey.Q -> Quit
    | _ -> getInput()

let parseData (data:seq<string>) =
    data 
        |> Seq.map (fun line -> Regex.Match(line.Trim(), "^position=<(?<x>[^,]+),(?<y>[^>]+)> velocity=<(?<velx>[^,]+),(?<vely>[^>]+)>$"))
        |> Seq.cast<Match>
        |> Seq.map (fun m -> 
            new Light(
                { Position.X = m.Groups.["x"].Value |> int; Position.Y = m.Groups.["y"].Value |> int},
                { Velocity.X = m.Groups.["velx"].Value |> int; Velocity.Y = m.Groups.["vely"].Value |> int}
            ))

let rec emulate (sky:Sky) =
    sky.Draw()
    match getInput() with
    | input when input = Quit -> ()
    | input when input = FastForward ->
        sky.FastForward()
        sky |> emulate
    | input -> 
        sky.Move(input)
        sky |> emulate
    
let watchMessage data = 
    let lights = data |> parseData |> Seq.cache
    let sky = new Sky(lights)
    sky |> emulate 

[<EntryPoint>]
let main argv =
    sampleData |> watchMessage
    inputData |> watchMessage
    0 // return an integer exit code
