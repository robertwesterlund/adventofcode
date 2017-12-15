// Learn more about F# at http://fsharp.org

open System


let testdata = 
    String.Split "pbga (66)
xhth (57)
ebii (61)
havc (66)
ktlj (57)
fwft (72) -> ktlj, cntj, xhth
qoyq (66)
padx (45) -> pbga, havc, qoyq
tknk (41) -> ugml, padx, fwft
jptl (61)
ugml (68) -> gyxo, ebii, jptl
gyxo (61)
cntj (57)" [|'\n'|]

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    0 // return an integer exit code