// Learn more about F# at http://fsharp.org

open System

let input = [|3L; 8L; 1005L; 8L; 330L; 1106L; 0L; 11L; 0L; 0L; 0L; 104L; 1L; 104L; 0L; 3L; 8L; 102L; -1L; 8L; 10L; 101L; 1L; 10L; 10L; 4L; 10L; 1008L; 8L; 0L; 10L; 4L; 10L; 102L; 1L; 8L; 29L; 2L; 9L; 4L; 10L; 1006L; 0L; 10L; 1L; 1103L; 17L; 10L; 3L; 8L; 102L; -1L; 8L; 10L; 101L; 1L; 10L; 10L; 4L; 10L; 108L; 0L; 8L; 10L; 4L; 10L; 101L; 0L; 8L; 61L; 1006L; 0L; 21L; 1006L; 0L; 51L; 3L; 8L; 1002L; 8L; -1L; 10L; 101L; 1L; 10L; 10L; 4L; 10L; 108L; 1L; 8L; 10L; 4L; 10L; 1001L; 8L; 0L; 89L; 1L; 102L; 19L; 10L; 1L; 1107L; 17L; 10L; 1006L; 0L; 18L; 3L; 8L; 1002L; 8L; -1L; 10L; 1001L; 10L; 1L; 10L; 4L; 10L; 1008L; 8L; 1L; 10L; 4L; 10L; 1001L; 8L; 0L; 123L; 1L; 9L; 2L; 10L; 2L; 1105L; 10L; 10L; 2L; 103L; 9L; 10L; 2L; 1105L; 15L; 10L; 3L; 8L; 102L; -1L; 8L; 10L; 1001L; 10L; 1L; 10L; 4L; 10L; 1008L; 8L; 0L; 10L; 4L; 10L; 102L; 1L; 8L; 161L; 3L; 8L; 102L; -1L; 8L; 10L; 101L; 1L; 10L; 10L; 4L; 10L; 108L; 1L; 8L; 10L; 4L; 10L; 101L; 0L; 8L; 182L; 3L; 8L; 1002L; 8L; -1L; 10L; 101L; 1L; 10L; 10L; 4L; 10L; 1008L; 8L; 0L; 10L; 4L; 10L; 101L; 0L; 8L; 205L; 2L; 1102L; 6L; 10L; 1006L; 0L; 38L; 2L; 1007L; 20L; 10L; 2L; 1105L; 17L; 10L; 3L; 8L; 102L; -1L; 8L; 10L; 1001L; 10L; 1L; 10L; 4L; 10L; 108L; 1L; 8L; 10L; 4L; 10L; 1001L; 8L; 0L; 241L; 3L; 8L; 102L; -1L; 8L; 10L; 101L; 1L; 10L; 10L; 4L; 10L; 108L; 1L; 8L; 10L; 4L; 10L; 101L; 0L; 8L; 263L; 1006L; 0L; 93L; 2L; 5L; 2L; 10L; 2L; 6L; 7L; 10L; 3L; 8L; 102L; -1L; 8L; 10L; 101L; 1L; 10L; 10L; 4L; 10L; 108L; 0L; 8L; 10L; 4L; 10L; 1001L; 8L; 0L; 296L; 1006L; 0L; 81L; 1006L; 0L; 68L; 1006L; 0L; 76L; 2L; 4L; 4L; 10L; 101L; 1L; 9L; 9L; 1007L; 9L; 1010L; 10L; 1005L; 10L; 15L; 99L; 109L; 652L; 104L; 0L; 104L; 1L; 21102L; 825594262284L; 1L; 1L; 21102L; 347L; 1L; 0L; 1105L; 1L; 451L; 21101L; 0L; 932855939852L; 1L; 21101L; 358L; 0L; 0L; 1106L; 0L; 451L; 3L; 10L; 104L; 0L; 104L; 1L; 3L; 10L; 104L; 0L; 104L; 0L; 3L; 10L; 104L; 0L; 104L; 1L; 3L; 10L; 104L; 0L; 104L; 1L; 3L; 10L; 104L; 0L; 104L; 0L; 3L; 10L; 104L; 0L; 104L; 1L; 21102L; 1L; 235152649255L; 1L; 21101L; 405L; 0L; 0L; 1105L; 1L; 451L; 21102L; 235350879235L; 1L; 1L; 21102L; 416L; 1L; 0L; 1106L; 0L; 451L; 3L; 10L; 104L; 0L; 104L; 0L; 3L; 10L; 104L; 0L; 104L; 0L; 21102L; 988757512972L; 1L; 1L; 21101L; 439L; 0L; 0L; 1106L; 0L; 451L; 21102L; 1L; 988669698828L; 1L; 21101L; 0L; 450L; 0L; 1106L; 0L; 451L; 99L; 109L; 2L; 22101L; 0L; -1L; 1L; 21102L; 40L; 1L; 2L; 21102L; 1L; 482L; 3L; 21102L; 472L; 1L; 0L; 1106L; 0L; 515L; 109L; -2L; 2105L; 1L; 0L; 0L; 1L; 0L; 0L; 1L; 109L; 2L; 3L; 10L; 204L; -1L; 1001L; 477L; 478L; 493L; 4L; 0L; 1001L; 477L; 1L; 477L; 108L; 4L; 477L; 10L; 1006L; 10L; 509L; 1101L; 0L; 0L; 477L; 109L; -2L; 2106L; 0L; 0L; 0L; 109L; 4L; 1202L; -1L; 1L; 514L; 1207L; -3L; 0L; 10L; 1006L; 10L; 532L; 21102L; 1L; 0L; -3L; 21202L; -3L; 1L; 1L; 21202L; -2L; 1L; 2L; 21102L; 1L; 1L; 3L; 21102L; 1L; 551L; 0L; 1106L; 0L; 556L; 109L; -4L; 2105L; 1L; 0L; 109L; 5L; 1207L; -3L; 1L; 10L; 1006L; 10L; 579L; 2207L; -4L; -2L; 10L; 1006L; 10L; 579L; 22101L; 0L; -4L; -4L; 1105L; 1L; 647L; 21201L; -4L; 0L; 1L; 21201L; -3L; -1L; 2L; 21202L; -2L; 2L; 3L; 21102L; 598L; 1L; 0L; 1105L; 1L; 556L; 21202L; 1L; 1L; -4L; 21101L; 0L; 1L; -1L; 2207L; -4L; -2L; 10L; 1006L; 10L; 617L; 21102L; 1L; 0L; -1L; 22202L; -2L; -1L; -2L; 2107L; 0L; -3L; 10L; 1006L; 10L; 639L; 21202L; -1L; 1L; 1L; 21102L; 1L; 639L; 0L; 105L; 1L; 514L; 21202L; -2L; -1L; -2L; 22201L; -4L; -2L; -4L; 109L; -5L; 2105L; 1L; 0L |]

[<EntryPoint>]
let main argv =
    ([(1,0); (0,0); (1,0); (1,0); (0,1); (1,0); (1,0)] |> Turtle.Context.Empty.ExecuteList).Draw()
    ([(1,0); (0,0); (1,0); (1,0); (0,1); (1,0); (1,0)] |> Turtle.Context.Empty.ExecuteList).PaintedOn |> Set.count |> printfn "Painted on: %i"


    let paintTheShip (turtle:Turtle.Context) (computer:Computer.Context) = 
        let mutable currentTurtle = turtle
        let mutable currentComputer = computer
        while not (currentComputer.HasHalted) do
            currentComputer <- currentComputer |> Computer.runProgram
            if (currentComputer.OutputStream |> List.length) % 2 <> 0 then failwith "We do not currently support getting partial commands, they must come in pairs"
            let turtleInput = 
                currentComputer.OutputStream 
                    |> List.rev 
                    |> List.map int32 
                    |> List.chunkBySize 2 
                    |> List.map (fun l -> (l.[0], l.[1])) 
            currentTurtle <-
                turtleInput |> currentTurtle.ExecuteList

            currentComputer <- { currentComputer with InputStream = [ if currentTurtle.IsCurrentSquareWhite() then 1L else 0L ]; OutputStream = [] } 
        currentTurtle                

    let sampleComputer = [|104L;1L;104L;0L;104L; 0L;104L;0L;104L; 1L;104L;0L;104L; 1L;104L;0L;104L; 0L;104L;1L;104L; 1L;104L;0L;104L; 1L;104L;0L;99L|] |> Computer.Context.CreateFromDataArray
    let sampleTurtle = sampleComputer |> paintTheShip Turtle.Context.Empty
    sampleTurtle.Draw()
    sampleTurtle.PaintedOn |> Set.count |> printfn "Painted on: %i"

    let computer = input |> Computer.Context.CreateFromDataArray
    let turtle = Turtle.Context.Empty
    let finalTurtle = computer |> paintTheShip turtle
    finalTurtle.Draw()
    finalTurtle.PaintedOn |> Set.count |> printfn "Painted on: %i"
    
    let computer_part2 = input |> Computer.Context.CreateFromDataArray
    let turtle_part2 = Turtle.Context.Empty.PaintWhite()
    let finalTurtle_part2 = computer_part2 |> paintTheShip turtle_part2
    finalTurtle_part2.Draw()


    printfn "Hello World from F#!"
    0 // return an integer exit code
