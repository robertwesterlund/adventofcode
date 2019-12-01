// Learn more about F# at http://fsharp.org

open System
open System.IO

let sampleData = 
    File.ReadAllLines(".\\sample.data") |> Seq.head

let inputData = 
    File.ReadAllLines(".\\input.data") |> Seq.head

let getEnumerator (data:string) =
    let split = data.Split(" ")
    let mutable currentIndex = -1
    fun () -> currentIndex <- currentIndex + 1; if currentIndex < split.Length then split.[currentIndex] else raise (System.Exception("We tried reading too many times..."))

type Node =
    {
        Children : seq<Node>
        Metadata : seq<int>
    }

let rec convertToNode enumerator =
    let childCount = enumerator() |> int
    let metadataCount = enumerator() |> int
    //printfn "Node has %3i children and %3i pieces of metadata" childCount metadataCount
    let children = Seq.init childCount (fun _ -> convertToNode enumerator) |> Seq.cache
    let metadata = Seq.init metadataCount (fun _ -> enumerator() |> int) |> Seq.cache
    //Since parsing has side effects (the enumerator), we want to do this eagerly
    //Running Seq.length over the collections trigger their evaluation
    children |> Seq.length |> ignore
    metadata |> Seq.length |> ignore
    {
        Node.Children = children
        Node.Metadata = metadata
    }

let parseInput data = 
    let enumerator = data |> getEnumerator
    enumerator |> convertToNode

let doWork (treeWalker:Node -> Unit) data=
    let rootNode = data |> parseInput
    rootNode |> treeWalker

let rec sumMetadata node = 
    (node.Metadata |> Seq.sum) + (node.Children |> Seq.sumBy (sumMetadata))

let rec sumNodeValues node =
    if (node.Children |> Seq.isEmpty) 
    then node.Metadata |> Seq.sum
    else 
        node.Metadata 
        |> Seq.map (fun md -> 
                if md > 0 && md <= (node.Children |> Seq.length) 
                then node.Children |> Seq.item (md - 1) |> sumNodeValues 
                else 0
            ) |> Seq.sum

let walker calculator dataName = doWork (fun node -> node |> calculator |> printfn "sum for %s was %i" dataName)
let firstTask dataName = printf "Using the first algorithm the "; walker sumMetadata dataName
let secondTask dataName = printf "Using the second algorithm the "; walker sumNodeValues dataName

[<EntryPoint>]
let main argv =
    sampleData |> firstTask "sample data"
    inputData |> firstTask "input data"
    sampleData |> secondTask "sample data"
    inputData |> secondTask "input data"

    0 // return an integer exit code
