[<CustomEquality;CustomComparison>]
type Path = 
    {
        Position: int32 * int32
        Cost : int32
        MinCost : int32
        Path: (int32 * int32) list
    }
    override this.Equals(obj) =
        match obj with
        //| :? Path as p -> this.Position = p.Position && this.Cost = p.Cost && this.MinCost = p.MinCost && this.Path = p.Path
        | :? Path as p -> this.Path = p.Path
        | _ -> false

    override this.GetHashCode() =
        this.Path.GetHashCode()

    interface System.IComparable with
        override this.CompareTo(obj: obj): int = 
            match obj with
            | :? Path as p -> 
                match this.MinCost.CompareTo(p.MinCost) with
                | x when x = 0 -> this.Path.GetHashCode().CompareTo(p.Path.GetHashCode())
                | x -> x
            | _ -> 1

let parse filename = 
    filename 
    |> System.IO.File.ReadAllLines
    |> Array.toList
    |> List.indexed
    |> List.map (fun (rowIndex, line) ->
        line.ToCharArray()
        |> Array.toList
        |> List.indexed
        |> List.map (fun (columnIndex, c) -> (columnIndex, rowIndex), System.Int32.Parse(c.ToString()))
    )
    |> List.concat
    |> Map.ofList

let repeatMap5Times data =
    let (dimX, dimY) = (
        (data |> Map.toList |> List.map (fun ((x, _), _) -> x) |> List.max) + 1,
        (data |> Map.toList |> List.map (fun ((_, y), _) -> y) |> List.max) + 1
    )
    data 
    |> Map.toList 
    |> List.fold (fun acc ((x, y), cost) -> 
        [for row in 0..4 do
            for column in 0..4 do
                let addition = row + column
                ((x + column * dimX, y + row * dimY), (((cost - 1) + addition) % 9) + 1)]
                |> List.fold (fun a (p, c) -> a |> Map.add p c) acc
    ) Map.empty

let rec search destination (map: Map<int * int, int>) =
    let mutable counter = 0
    let mutable alreadyVisited = Map.empty
    let isAlreadyVisitedWithLowerOrSameCost position minCost =
        alreadyVisited |> Map.containsKey position && alreadyVisited.[position] <= minCost
    let rec search_inner destination (map: Map<int * int, int>) paths =
        let (destinationX, destinationY) = destination
        match 
            paths |> Set.minElement
        with
        | p when p.Position = destination -> (p.Path |> List.rev, p.Cost)
        | p -> 
            // counter <- counter + 1
            // if (counter % 100 = 0) then
            //     printfn "%O" (p.Position, p.Cost, p.MinCost)
            if (alreadyVisited.ContainsKey p.Position && alreadyVisited.[p.Position] > p.MinCost) then
                alreadyVisited <- alreadyVisited |> Map.add p.Position p.MinCost
            let (x, y) = p.Position
            [(x-1, y); (x+1, y); (x, y+1); (x, y-1)]
            |> List.filter (fun (x, y) -> x >= 0 && x <= destinationX && y >= 0 && y <= destinationY)
            |> List.map (fun pos -> {Position = pos; Path = pos :: p.Path; Cost = p.Cost + map.[pos]; MinCost = p.Cost + map.[pos] + destinationX - x + destinationY - y} )
            |> List.filter (fun p -> not (isAlreadyVisitedWithLowerOrSameCost p.Position p.MinCost ))
            |> List.fold (fun acc v -> acc |> Set.add v) (paths |> Set.remove p)
            |> search_inner destination map
    search_inner destination map (Set.empty |> Set.add {Position = (0,0); MinCost = 0; Cost = 0; Path = [(0,0)]})
        
let part1 filename =
    let map = filename |> parse
    let destination = map.Keys |> Seq.maxBy (fun (x, y) -> x * 1000 + y)
    search destination map |> printfn "%s part 1: %O" filename
let part2 filename =
    let map = filename |> parse |> repeatMap5Times
    let destination = map.Keys |> Seq.maxBy (fun (x, y) -> x * 1000 + y)
    search destination map |> printfn "%s part 1: %O" filename

"testdata.txt" |> part1
"input.txt" |> part1
"testdata.txt" |> part2
"input.txt" |> part2

// //[((0,0),8)] 
// |> Map.ofList 
// "testdata.txt" 
// |> parse
// |> repeatMap5Times 
// |> Map.toList 
// |> List.groupBy (fun ((x, y), _) -> y)
// |> List.sortBy (fun (y, _) -> y)
// |> List.iter (fun (y, list) ->
//     list 
//     |> List.sortBy (fun ((x, _),_) -> x)
//     |> List.iter (fun ((x, y), c) -> c |> printf "%i")
//     printfn ""
// )