// Learn more about F# at http://fsharp.org

open System
open System.Drawing
open System.Linq;

type Content =
    | Empty 
    | Asteroid

let parse (input:string) = 
    let data = input.Split('\n').Select(fun s -> s.ToCharArray() |> Array.toList |> List.filter (fun c -> c <> '\r')).ToArray() |> Array.toList
    data |> List.mapi (fun y row -> 
        row |> List.mapi (fun x column -> 
            (   
                (decimal(x), decimal(y)), 
                match column with 
                    | '.' -> Empty
                    | '#' -> Asteroid
                    | _ -> failwith ("Invalid content '" + column.ToString() + "'")
            )     
        )
    ) |> List.collect id |> List.filter (fun (_, content) -> 
        match content with
        | Empty -> false
        | Asteroid -> true
    ) |> Map.ofList

let getPotentialBlockersBetween (ax, ay) (bx, by) =
    let deltaX = bx - ax
    let deltaY = by - ay
    let getXDiff diff = if deltaX < 0m then -diff else diff
    let getYDiff diff = if deltaY < 0m then -diff else diff
    if deltaX = 0m then seq { 0m .. (getYDiff 1m) .. deltaY} |> Seq.map (fun ydiff -> (ax, ay + ydiff))
    else if deltaY = 0m then seq { 0m .. (getXDiff 1m) .. deltaX} |> Seq.map (fun xdiff -> (ax + xdiff, ay))
    else 
        let rec gcd a b =
            match b with 
            | 0m -> a
            | _ -> gcd b (a % b)
        let highestStepCount = max (abs deltaX) (abs deltaY)
        let lowestStepCount = min (abs deltaX) (abs deltaY)
        let numberOfSteps = gcd highestStepCount lowestStepCount
        let (xdiff, ydiff) =
            match (highestStepCount, lowestStepCount) with
            | (x, y) when (abs deltaX) > (abs deltaY) -> ((getXDiff (highestStepCount / numberOfSteps)), (getYDiff (lowestStepCount / numberOfSteps)))
            | (x, y) -> (((getXDiff (lowestStepCount / numberOfSteps))), (getYDiff (highestStepCount / numberOfSteps)))
        seq {0m .. numberOfSteps} 
            |> Seq.map (fun step -> ((ax + xdiff * step), (ay + ydiff * step))) 
            |> Seq.filter (fun (dx, dy) -> dx % 1m = 0m && dy % 1m = 0m)

let mapVisibility (asteroids: Map<(decimal * decimal), Content>) =
    asteroids |> Map.map (fun aPosition _ ->
        let (ax, ay) = aPosition
        asteroids |> Seq.map (fun b ->
            if aPosition = b.Key
            then None
            else 
                let blockingPlaces = getPotentialBlockersBetween aPosition b.Key
                match blockingPlaces |> Seq.tryFind (fun p -> p <> aPosition && p <> b.Key && asteroids |> Map.containsKey p) with
                | Some _ -> None
                | None -> Some b.Key
        ) |> Seq.collect (fun p -> match p with | Some c -> [c]; | None -> []) |> List.ofSeq
    )

let getBestPosition (visibilityMap:Map<(decimal * decimal), (decimal * decimal) list>) =
    let best = visibilityMap |> Seq.maxBy (fun entry -> entry.Value |> List.length)
    (best.Key, best.Value |> Seq.length, best.Value)

let sortTargets ((originX, originY), _, targets) =
    let toDegrees radians = radians * (180.0 / Math.PI)
    let turnAngle angle = 
        match angle + 90.0 with
        | a when a < 0.0 -> a + 360.0
        | a -> a
    targets |> Seq.sortBy (fun (targetX, targetY) -> atan2 (double (targetY - originY)) (double (targetX - originX)) |> toDegrees |> turnAngle)

let sample1 = 
    ".#..#
.....
#####
....#
...##"

let sample2 = 
    "......#.#.
#..#.#....
..#######.
.#.#.###..
.#..#.....
..#....#.#
#..#....#.
.##.#..###
##...#..#.
.#....####"

let sample3 = 
    "#.#...#.#.
.###....#.
.#....#...
##.#.#.#.#
....#.#.#.
.##..###.#
..#...##..
..##....##
......#...
.####.###."

let sample4 = 
    ".#..#..###
####.###.#
....###.#.
..###.##.#
##.##.#.#.
....###..#
..#.#..#.#
#..#.#.###
.##...##.#
.....#.#.."

let sample5 = 
    ".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##"

let input = 
    ".#..#..##.#...###.#............#.
.....#..........##..#..#####.#..#
#....#...#..#.......#...........#
.#....#....#....#.#...#.#.#.#....
..#..#.....#.......###.#.#.##....
...#.##.###..#....#........#..#.#
..#.##..#.#.#...##..........#...#
..#..#.......................#..#
...#..#.#...##.#...#.#..#.#......
......#......#.....#.............
.###..#.#..#...#..#.#.......##..#
.#...#.................###......#
#.#.......#..####.#..##.###.....#
.#.#..#.#...##.#.#..#..##.#.#.#..
##...#....#...#....##....#.#....#
......#..#......#.#.....##..#.#..
##.###.....#.#.###.#..#..#..###..
#...........#.#..#..#..#....#....
..........#.#.#..#.###...#.....#.
...#.###........##..#..##........
.###.....#.#.###...##.........#..
#.#...##.....#.#.........#..#.###
..##..##........#........#......#
..####......#...#..........#.#...
......##...##.#........#...##.##.
.#..###...#.......#........#....#
...##...#..#...#..#..#.#.#...#...
....#......#.#............##.....
#......####...#.....#...#......#.
...#............#...#..#.#.#..#.#
.#...#....###.####....#.#........
#.#...##...#.##...#....#.#..##.#.
.#....#.###..#..##.#.##...#.#..##"

let part2Sample = 
    ".#....#####...#..
##...##.#####..##
##...#...#.#####.
..#.....#...###..
..#.#.....#....##"

[<EntryPoint>]
let main argv =
    printfn "Part 1"
    let part1 title data =
        data |> parse |> mapVisibility |> getBestPosition |> printfn "%s: %A" title
    sample1 |> part1 "Sample 1"
    sample2 |> part1 "Sample 2"
    sample3 |> part1 "Sample 3"
    sample4 |> part1 "Sample 4"
    sample5 |> part1 "Sample 5"
    input |> part1 "Input"

    printfn "\n\n"

    printfn "Part 2"
    
    part2Sample |> parse |> mapVisibility |> getBestPosition |> sortTargets |> printfn "Part 2 sample ordered: %A"
    let part2 title data = 
        data |> parse |> mapVisibility |> getBestPosition |> sortTargets |> Seq.skip 199 |> Seq.take 1 |> printfn "%s: %A" title
    sample5 |> part2 "Part 2 big sample result"
    input |> part2 "Part 2 input data"
    0 // return an integer exit code
