// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Threading.Tasks.Dataflow

let sampleData = 
    File.ReadLines(".\\sample.data")

let inputData = 
    File.ReadLines(".\\input.data")

type Rect =
    {
        Top : int
        Left : int
        Bottom : int
        Right : int
    }

type Point = 
    {
        Y : int
        X : int
    }

let convertToPoints (lines:seq<string>) = 
    lines |> Seq.map (fun line -> 
        let split = line.Split(',')
        {
            Point.X = split.[0].Trim() |> int
            Point.Y = split.[1].Trim() |> int
        }
    )

let getSquareBorderSize (data:seq<Point>) = 
    {
        Rect.Top = (data |> Seq.minBy (fun s -> s.Y)).Y
        Rect.Bottom = (data |> Seq.maxBy (fun s -> s.Y)).Y
        Rect.Left = (data |> Seq.minBy (fun s -> s.X)).X
        Rect.Right = (data |> Seq.maxBy (fun s -> s.X)).X
    }

let getBorderPoints data : seq<Point>= 
    let border = data |> getSquareBorderSize
    let topAndBottom = 
        seq {border.Top .. border.Bottom} |> Seq.collect (fun y ->        
            [
                {Point.Y = y; Point.X = border.Left}
                {Point.Y = y; Point.X = border.Right}
            ]        
        ) 
    let rightAndLeft =    
        seq {border.Left .. border.Right} |> Seq.collect (fun x ->
            [
                {Point.X = x; Point.Y = border.Top}
                {Point.X = x; Point.Y = border.Bottom}
            ]
        )
    Seq.concat [topAndBottom;rightAndLeft] |> Seq.distinct

let getFullBoardPoints data : seq<Point> = 
    let border = data |> getSquareBorderSize
    seq {border.Top .. border.Bottom} |> Seq.collect (fun y ->
        seq {border.Left .. border.Right} |> Seq.map (fun x ->
            {Point.Y = y; Point.X = x}
        )        
    )    

let manhattanDistance (pointA:Point) (pointB:Point) =
    (abs (pointA.Y - pointB.Y)) + (abs (pointA.X - pointB.X)) 

let getClosestToPointIfSingle (point:Point) data : Option<(Point * Point)> =
    let dataWithDistance = data |> Seq.map (fun p -> (p, manhattanDistance p point))
    let (_, lowestDistance) = dataWithDistance |> Seq.minBy (fun (_, distance) -> distance)
    let entriesWithLowestDistance = dataWithDistance |> Seq.filter (fun (_, distance) -> distance = lowestDistance)
    if (entriesWithLowestDistance |> Seq.length = 1)
    then Some(point, entriesWithLowestDistance |> Seq.map (fun (p, _) -> p) |> Seq.head)
    else None

//Any spaces that are the closest to any of the outer bounds will be infinite, any other will be finite
let getPointsWithInfiniteArea (data:seq<Point>) : seq<Point>=
    let borderPoints = data |> getBorderPoints
    borderPoints 
        |> Seq.choose (fun borderP -> data |> getClosestToPointIfSingle borderP) 
        |> Seq.map (fun (_, point) -> point)
        |> Seq.distinct

let getPointWithLargestArea (data:seq<Point>) =
    let pointsWithInfiniteArea = data |> getPointsWithInfiniteArea
    data 
        |> getFullBoardPoints 
        |> Seq.choose (fun p -> data |> getClosestToPointIfSingle p)
        |> Seq.filter (fun (_, point) -> not (pointsWithInfiniteArea |> Seq.exists (fun p -> p = point)))
        |> Seq.groupBy (fun (_, point) -> point)
        |> Seq.map (fun (key, group) -> (key, group |> Seq.length))
        |> Seq.maxBy (fun (key, size) -> size)

let runFirstTask data =     
    let (point, area) = data |> convertToPoints |> getPointWithLargestArea 
    printfn "Point: %d, %d, size: %d" point.X point.Y area

let runSecondTask maxDistance data = 
    let dataPoints = data |> convertToPoints
    dataPoints 
        |> getFullBoardPoints
        |> Seq.map (fun p -> (p, dataPoints |> Seq.sumBy (fun dp -> manhattanDistance dp p)))
        |> Seq.filter (fun (p, sum) -> sum < maxDistance)
        |> Seq.length
        |> printfn "Size of area with less than %d MD from all points: %d" maxDistance

[<EntryPoint>]
let main argv =
    //sampleData |> convertToPoints |> getBorderPoints |> Seq.iter (fun p -> printfn "%d, %d" p.X p.Y)
    //sampleData |> convertToPoints |> filterInfiniteAreas |> Seq.iter (fun p -> printfn "%d, %d" p.X p.Y)
    printfn "Sample data"
    //sampleData |> runFirstTask
    sampleData |> runSecondTask 32
    printfn "Input data"
    inputData |> runSecondTask 10000
    //inputData |> runFirstTask
    0 // return an integer exit code
