// Learn more about F# at http://fsharp.org

open System

let input = "<x=14, y=4, z=5>
<x=12, y=10, z=8>
<x=1, y=7, z=-10>
<x=16, y=-5, z=3>".Replace("\r", "").Split("\n")

let sample1 = "<x=-1, y=0, z=2>
<x=2, y=-10, z=-7>
<x=4, y=-8, z=8>
<x=3, y=5, z=-1>".Replace("\r", "").Split("\n")

let sample2 = "<x=-8, y=-10, z=0>
<x=5, y=5, z=10>
<x=2, y=-7, z=3>
<x=9, y=-8, z=-3>".Replace("\r", "").Split("\n")

type IVector3D =
    abstract X : int64
    abstract Y : int64
    abstract Z : int64

type Velocity = 
    {
        X : int64
        Y : int64
        Z : int64
    }

    member this.KineticEnergy = 
        abs this.X + abs this.Y + abs this.Z

    interface IVector3D with
        member this.X = this.X
        member this.Y = this.Y
        member this.Z = this.Z

type Position =
    {
        X : int64
        Y : int64
        Z : int64
    }

    member this.PotentialEnergy =
        abs this.X + abs this.Y + abs this.Z

    member this.ApplyVelocity (velocity:Velocity) = 
        {
            X = this.X + velocity.X
            Y = this.Y + velocity.Y
            Z = this.Z + velocity.Z
        }

    interface IVector3D with
        member this.X = this.X
        member this.Y = this.Y
        member this.Z = this.Z

type Moon =
    {
        Position : Position
        Velocity : Velocity
    }

    member this.ApplyGravity moons = 
        let getDeltaForAxis thisVal otherVal = 
            match thisVal - otherVal with
            | x when x < 0L -> 1L
            | x when x > 0L -> -1L
            | _ -> 0L
        {
            Position = this.Position;
            Velocity = Array.fold (fun v moon -> 
                    {
                        X = v.X + getDeltaForAxis this.Position.X moon.Position.X;
                        Y = v.Y + getDeltaForAxis this.Position.Y moon.Position.Y;
                        Z = v.Z + getDeltaForAxis this.Position.Z moon.Position.Z;
                    }
                ) this.Velocity moons
        }
    
    member this.ApplyVelocity() =
        {
            Position = this.Position.ApplyVelocity this.Velocity
            Velocity = this.Velocity
        }

    member this.TotalEnergy = 
        this.Position.PotentialEnergy * this.Velocity.KineticEnergy

let ParseRow line =
    let m = System.Text.RegularExpressions.Regex.Match(line, "<x=(?<x>-?\d+), y=(?<y>-?\d+), z=(?<z>-?\d+)>")
    let getValue (groupName:string) = Convert.ToInt64(m.Groups.[groupName].Value)
    let moon = 
        {
            Position = { X = getValue("x"); Y = getValue("y"); Z = getValue("z") }
            Velocity = { X = 0L; Y = 0L; Z = 0L }
        }
    moon

let ParseRows lines =
    lines |> Array.map ParseRow

let ApplyGravity moons = 
    moons |> Array.map (fun (moon:Moon) -> moon.ApplyGravity moons)

let ApplyVelocity moons = 
    moons |> Array.map (fun (moon:Moon) -> moon.ApplyVelocity())

let rec ApplyTurns numberOfTurns moons =
    match numberOfTurns with 
    | 0L -> moons
    | _ -> moons |> ApplyGravity |> ApplyVelocity |> ApplyTurns (numberOfTurns - 1L)

let GetNumberOFTurnsBeforeRecurring moons = 
    let gcd (a:int64) (b:int64) = 
        let rec gcdInner a b =
            match b with
            | 0L -> a
            | _ -> gcdInner b (a % b)
        gcdInner (Math.Max(a, b)) (Math.Min(a, b))
    let lcm a b =
        (abs (a * b)) / (gcd a b)
    let rec findAxisLoop turnCount previousValues getSummaryFunc moons =
        let summary = getSummaryFunc moons
        if previousValues |> Set.contains summary then turnCount
        else 
            let newListOfPreviousValues = previousValues |> Set.add summary
            moons |> ApplyGravity |> ApplyVelocity |> findAxisLoop (turnCount + 1L) newListOfPreviousValues getSummaryFunc
    let inline getSummaryForAxis (axisGetter: IVector3D -> int64) moons = 
        moons |> Array.map (fun m -> (axisGetter m.Position).ToString() + "," + (axisGetter m.Velocity).ToString()) |> String.concat ";"
    let findLoopingTurncountForAxis axisGetter =
        findAxisLoop 0L Set.empty (getSummaryForAxis axisGetter) moons
    let xLoop = findLoopingTurncountForAxis (fun a -> a.X)
    let yLoop = findLoopingTurncountForAxis (fun a -> a.Y)
    let zLoop = findLoopingTurncountForAxis (fun a -> a.Z)
    lcm xLoop (lcm yLoop zLoop)

[<EntryPoint>]
let main argv =
    sample1 |> ParseRows |> ApplyTurns 10L |> Array.fold (fun energy moon -> energy + moon.TotalEnergy) 0L |> printfn "Sample1 total energy after 10 turns: %A"
    sample2 |> ParseRows |> ApplyTurns 100L |> Array.fold (fun energy moon -> energy + moon.TotalEnergy) 0L |> printfn "Sample2 total energy after 100 turns: %A"
    input |> ParseRows |> ApplyTurns 1000L |> Array.fold (fun energy moon -> energy + moon.TotalEnergy) 0L |> printfn "Input total energy after 1000 turns: %A"
    sample1 |> ParseRows |> GetNumberOFTurnsBeforeRecurring |> printfn "Sample1 loops after %i turns"
    input |> ParseRows |> GetNumberOFTurnsBeforeRecurring |> printfn "Input loops after %i turns"
    //input |> ParseRows |> GetNumberOfTurnsBeforeRecurring |> printfn "Input loops after %i turns"
    0 // return an integer exit code
