// Learn more about F# at http://fsharp.org

open System
open System.Collections.Generic
open System.Threading
open RoyT.AStar

let battleStartSample1 =
    [
        "#######"
        "#.G...#"
        "#...EG#"
        "#.#.#G#"
        "#..G#E#"
        "#.....#"
        "#######"
    ] |> List.toArray

let battleStartSample2 =
    [
        "#######"
        "#G..#E#"
        "#E#E.E#"
        "#G.##.#"
        "#...#E#"
        "#...E.#"
        "#######"
    ] |> List.toArray

let battleStartSample3 =
    [
        "#######"
        "#E..EG#"
        "#.#G.E#"
        "#E.##E#"
        "#G..#.#"
        "#..E#.#"
        "#######"
    ] |> List.toArray

let battleStartSample4 =
    [
        "#######"
        "#E.G#.#"
        "#.#G..#"
        "#G.#.G#"
        "#G..#.#"
        "#...E.#"
        "#######"
    ] |> List.toArray

let battleStartSample5 =
    [
        "#######"
        "#.E...#"
        "#.#..G#"
        "#.###.#"
        "#E#G#G#"
        "#...#G#"
        "#######"
    ] |> List.toArray

let battleStartSample6 =
    [
        "#########"
        "#G......#"
        "#.E.#...#"
        "#..##..G#"
        "#...##..#"
        "#...#...#"
        "#.G...G.#"
        "#.....G.#"
        "#########"
    ] |> List.toArray

let customSampleBattle =
    [
        "#########"
        "#.......#"
        "#.#..#..#"
        "#.#..E#.#"
        "#..#.#..#"
        "#...#...#"
        "#..#....#"
        "#.....G.#"
        "#########"
    ] |> List.toArray

let realBattleData =
    [
        "################################"
        "###########...G...#.##..########"
        "###########...#..G#..G...#######"
        "#########.G.#....##.#GG..#######"
        "#########.#.........G....#######"
        "#########.#..............#######"
        "#########.#...............######"
        "#########.GG#.G...........######"
        "########.##...............##..##"
        "#####.G..##G.......E....G......#"
        "#####.#..##......E............##"
        "#####.#..##..........EG....#.###"
        "########......#####...E.##.#.#.#"
        "########.#...#######......E....#"
        "########..G.#########..E...###.#"
        "####.###..#.#########.....E.####"
        "####....G.#.#########.....E.####"
        "#.........#G#########......#####"
        "####....###G#########......##..#"
        "###.....###..#######....##..#..#"
        "####....#.....#####.....###....#"
        "######..#.G...........##########"
        "######...............###########"
        "####.....G.......#.#############"
        "####..#...##.##..#.#############"
        "####......#####E...#############"
        "#.....###...####E..#############"
        "##.....####....#...#############"
        "####.########..#...#############"
        "####...######.###..#############"
        "####..##########################"
        "################################"
    ] |> List.toArray

type MapType =
    | Wall
    | Floor

[<StructuredFormatDisplay("{X}:{Y}")>]
type Position =
    {
        X : int
        Y : int
    }

let getReadOrderSortValue position =
    (99 - position.Y) * 100 + (99 - position.X)

let sortByReadingOrder positionAccessor collection =
    collection
        |> Seq.sortByDescending (fun x ->
            let p:Position = positionAccessor x
            p |> getReadOrderSortValue
        )

let ManhattanDistance p1 p2 =
    abs (p1.X - p2.X) + abs (p1.Y - p2.Y)

type CharacterType =
    | Elf
    | Goblin

let arePositionsAdjacent p1 p2 =
    if (p1.X = p2.X)
    then abs (p1.Y - p2.Y) = 1
    else if (p1.Y = p2.Y)
    then abs (p1.X - p2.X) = 1
    else false

type Character(pauseAndDraw:bool, id:char, decisionEngines:seq<IDecisionEngine>, characterType, position) as self =
    let getLazyEnumerationOfNextActions map livingCharacters = 
        decisionEngines
            |> Seq.choose (fun ai -> ai.GetAction map livingCharacters self)
    member val Id = id
    member val AttackPower = 3 with get,set
    member val HP : int = 200 with get,set
    member val Position : Position = position with get,set
    member val Type : CharacterType = characterType

    member self.isAdjacentTo (otherCharacter:Character) =
        arePositionsAdjacent self.Position otherCharacter.Position
    member self.getEnemies (livingCharacters:seq<Character>) =
        livingCharacters |> Seq.filter (fun c -> c.Type <> self.Type)

    member self.GetPlannedActions map livingCharacters : IAction list =
        //This does not correctly apply actions between each decision engine
        let actions =
            getLazyEnumerationOfNextActions map livingCharacters
                |> Seq.toList
        if not (actions |> Seq.isEmpty)
        then
            actions
        else
            [new SkipTurnAction(self) :> IAction]

    member self.OneTurn map livingCharacters =
        getLazyEnumerationOfNextActions map livingCharacters
            |> Seq.iter (fun action ->
                if pauseAndDraw then action |> printf "%O. "
                action.ApplyAction()
            )
        if pauseAndDraw then printfn ""

    override self.ToString() =
        self.Type.ToString() + " (id: " + self.Id.ToString() + ")"

and IAction =
    abstract member ApplyAction : Unit -> Unit

and IDecisionEngine =
    abstract member GetAction : MapType array array -> List<Character> -> Character -> IAction option

and SkipTurnDecisionEngine() =
    interface IDecisionEngine with
        member this.GetAction map livingCharacters character=
            Some(new SkipTurnAction(character) :> IAction)

and SkipTurnAction(character) =
    override self.ToString() =
        character.ToString() + " is skipping its turn"
    interface IAction with
        member this.ApplyAction() = ()

type EndGameIfOutOfEnemiesDecisionEngine(onEndGameDetected: Unit -> Unit) =
    interface IDecisionEngine with
        member this.GetAction map livingCharacters character =
            if character.getEnemies(livingCharacters) |> Seq.isEmpty
            then
                Some(
                    {
                        new IAction with
                            member self.ApplyAction() = onEndGameDetected()
                    }
                )
            else None

let isFreePosition (map:MapType array array) (livingCharacters:seq<Character>) position =
    map.[position.Y].[position.X] = Floor && not (livingCharacters |> Seq.exists (fun c -> c.Position = position))

let getAdjacentFreePositions (map:MapType array array) (livingCharacters:seq<Character>) position =
    [
        {position with Position.Y = position.Y + 1}
        {position with Position.X = position.X - 1}
        {position with Position.X = position.X + 1}
        {position with Position.Y = position.Y - 1}
    ]
        |> Seq.filter (fun p -> isFreePosition map livingCharacters p)

type AttackAction(pauseAndDraw:bool, livingCharacters: List<Character>, attackingCharacter:Character, attackedCharacter:Character, attackPower) =
    override self.ToString() =
        attackingCharacter.ToString() + " plans to attack " + attackedCharacter.ToString()

    interface IAction with
        member this.ApplyAction() =
            attackedCharacter.HP <- attackedCharacter.HP - attackPower
            if attackedCharacter.HP <= 0
            then
                if (pauseAndDraw)
                then
                    printfn "%A dies" attackedCharacter.Type
                livingCharacters.Remove(attackedCharacter) |> ignore

type DefaultAttackingDecisionEngine(pauseAndDraw:bool) =
    interface IDecisionEngine with
        member this.GetAction map livingCharacters character=
            livingCharacters
                |> character.getEnemies
                |> Seq.filter (fun enemy -> character.isAdjacentTo(enemy))
                |> Seq.sortByDescending (fun enemy -> (999 - enemy.HP) * 1000000 + (enemy.Position |> getReadOrderSortValue))
                |> Seq.tryPick (fun closestEnemy -> Some(new AttackAction(pauseAndDraw, livingCharacters, character, closestEnemy, character.AttackPower) :> IAction))

type MoveAction(movingCharacter:Character, newPosition, plannedToReachPosition)=
    member val Destination = newPosition
    member val Character = movingCharacter
    member val PlannedTarget = plannedToReachPosition
    override self.ToString() =
        movingCharacter.ToString() + " is moving from " + movingCharacter.Position.ToString() + " to " + newPosition.ToString() + " (planning to reach position: " + plannedToReachPosition.ToString() + ")"

    interface IAction with
        member this.ApplyAction() = movingCharacter.Position <- newPosition

type aStarResult =
    {
        Steps : Position array
    }
let aStarPlanning (character:Character) (map:MapType array array) (livingCharacters:List<Character>) (origin:Position) (targets:Position array) : seq<aStarResult option> =
    let canWalkOn = isFreePosition map livingCharacters
    printfn "Test walking for %c" character.Id
    let candidates = new List<Position array * string>()
    let getShortestVerticalPathBetween p1 p2 =
        match p1.Y - p2.Y with
        | delta when delta < 0 -> seq { 1 .. abs delta } |> Seq.map (fun d -> {p1 with Position.Y = p1.Y + d})
        | delta when delta > 0 -> seq { 1 .. delta } |> Seq.map (fun d -> {p1 with Position.Y = p1.Y - d})
        | _ -> Seq.empty
    let getShortestHorizontalPathBetween p1 p2 =
        match p1.X - p2.X with
        | delta when delta < 0 -> seq { 1 .. abs delta } |> Seq.map (fun d -> {p1 with Position.X = p1.X + d})
        | delta when delta > 0 -> seq { 1 .. delta } |> Seq.map (fun d -> {p1 with Position.X = p1.X - d})
        | _ -> Seq.empty
    let getSortString (path: Position array) =
        let costOfWalking = path.Length
        let pathReadingOrderSortValue = String.Join(".", path |> Seq.map (fun s -> s.Y.ToString() + ":" + s.X.ToString()))
        costOfWalking.ToString("000") + "." + pathReadingOrderSortValue
    candidates.AddRange(
        targets
            |> Seq.map (fun t ->
                let verticalPath = (getShortestVerticalPathBetween origin t) |> Seq.toArray
                let horizontalPath =
                    if (verticalPath |> Array.isEmpty)
                    then
                        getShortestHorizontalPathBetween origin t
                    else
                        getShortestHorizontalPathBetween (verticalPath.[verticalPath.Length - 1]) t
                let steps = Seq.concat [Seq.singleton origin; verticalPath |> Array.toSeq; horizontalPath] |> Seq.toArray
                let h = horizontalPath |> Seq.toArray
                let sortValue = getSortString steps
                (steps, sortValue)
            )
        )
    Seq.initInfinite (fun _ ->
        if candidates.Count = 0
        then
            None
        else
            let nextCandidate = candidates |> Seq.minBy (fun (_, s) -> s)
            candidates.Remove(nextCandidate) |> ignore
            let (c, s) = nextCandidate
            if character.Id = 'A' //|| character.Id = '1'
            then
                printfn "Next candidate: %A %s" c s
                candidates |> Seq.iter (fun (c, s) -> printfn "Alternative candidate: %A %s" c s)
            Some(nextCandidate)
    )
        |> Seq.takeWhile (fun c -> c <> None)
        |> Seq.choose (id)
        |> Seq.map(fun ((steps:Position array), _) ->
            let mutable currentIndex = 1 //Skip the first position, since it's the origin point which we'll remove later
            let mutable isPossibleToWalk = true
            while isPossibleToWalk && currentIndex < steps.Length do
                isPossibleToWalk <- canWalkOn steps.[currentIndex]
                if (not isPossibleToWalk)
                then
                     //Don't like have side effecs in a map, but lets go ahead
                    steps.[currentIndex - 1]
                        |> getAdjacentFreePositions map livingCharacters
                        |> Seq.except steps
                        |> Seq.iter (fun newPartOrigin ->
                            targets
                                |> Seq.map (fun t ->
                                    let verticalPath = (getShortestVerticalPathBetween newPartOrigin t) |> Seq.toArray
                                    let horizontalPath =
                                        if (verticalPath |> Array.isEmpty)
                                        then
                                            getShortestHorizontalPathBetween newPartOrigin t
                                        else
                                            getShortestHorizontalPathBetween (verticalPath.[verticalPath.Length - 1]) t
                                    let secondPartOfSteps = Seq.concat [Seq.singleton newPartOrigin;verticalPath |> Array.toSeq;horizontalPath]
                                    let firstPartOfSteps = steps |> Seq.take currentIndex
                                    let newSteps = Seq.concat [firstPartOfSteps;secondPartOfSteps] |> Seq.toArray
                                    let newSortValue = getSortString newSteps
                                    (newSteps, newSortValue)
                                )
                                |> Seq.iter (fun c -> candidates.Add(c))
                        )
                currentIndex <- currentIndex + 1
            if isPossibleToWalk
            then
                printfn "Found path: %A" steps
                Some({aStarResult.Steps = steps})
            else
                None
        )

// let aStarPlanning character (map:MapType array array) (livingCharacters:List<Character>) (origin:Position) (targets:Position array) : seq<aStarResult option> =
    // let candidates = new List<aStarResult * string>()
    // let adjacentFreePositions = origin |> getAdjacentFreePositions map livingCharacters |> Seq.toArray
    // let getSortValue = (fun c ->
    //     if c.Steps.[0] = {Position.X = 7; Position.Y = 7}
    //     then
    //         printfn "Allowing breakpoint for %A" c.Steps
    //         3 = 8 |> ignore
    //     let minCostOfWalking = (targets |> Seq.map (fun t -> ManhattanDistance c.Steps.[c.Steps.Length - 1] t) |> Seq.min) + c.Steps.Length
    //     let pathReadingOrderSortValue = String.Join(".", c.Steps |> Seq.map (fun s -> s.Y.ToString() + ":" + s.X.ToString()))
    //     minCostOfWalking.ToString("000") + "." + pathReadingOrderSortValue
    // )
    // candidates.AddRange(adjacentFreePositions |> Seq.map (fun p -> ({aStarResult.Steps = [|p|]}, getSortValue {aStarResult.Steps = [|p|]})))
    // Seq.initInfinite (fun _ ->
    //     if candidates.Count > 0
    //     then
    //         let nextCandidate =
    //             candidates
    //                 |> Seq.minBy (fun (_,sortValue) -> sortValue)

    //         let (c, sortValue) = nextCandidate
    //         if c.Steps.[0] = {Position.X = 7; Position.Y = 7}
    //         then
    //             printfn "Allowing breakpoint for candidate %A : %s" c.Steps sortValue
    //             candidates |> Seq.iter (fun (c, s) -> printfn "Entries of candidates were: %A : %s" c s)
    //             3 = 8 |> ignore
    //         candidates.Remove(nextCandidate) |> ignore
    //         Some(nextCandidate)
    //     else None
    // )
    //     |> Seq.takeWhile (fun c -> c <> None)
    //     |> Seq.choose (id)
    //     |> Seq.map (fun (c, sortValue) ->
    //         printfn "Testing candidate %A : %s" c.Steps  sortValue
    //         let lastPositionInPlan = c.Steps.[c.Steps.Length - 1]
    //         if targets |> Seq.exists (fun t -> t = lastPositionInPlan)
    //         then
    //             printfn "Candidate accepted"
    //             Some(c)
    //         else
    //             let otherWaysToThisPoint = candidates |> Seq.filter (fun (cand, _) -> cand.Steps |> Seq.contains lastPositionInPlan) |> Seq.toArray
    //             let isThisWayBetterThanAllOtherWays = not (otherWaysToThisPoint |> Seq.exists (fun (_, candSort) -> candSort < sortValue))
    //             if isThisWayBetterThanAllOtherWays
    //             then
    //                 //Side effects in our tryFind? Ah, well...
    //                 otherWaysToThisPoint |> Seq.iter (fun cand -> candidates.Remove(cand) |> ignore)
    //                 lastPositionInPlan
    //                     |> getAdjacentFreePositions map livingCharacters
    //                     |> Seq.filter (fun p -> not (c.Steps |> Seq.contains p))
    //                     |> Seq.iter (fun p ->
    //                         let newAStarResult = {aStarResult.Steps = ([|p|] |> Array.append c.Steps)}
    //                         candidates.Add(newAStarResult, getSortValue newAStarResult)
    //                     )
    //             None
    //     )

// let aStarPlanning character (map:MapType array array) (livingCharacters:List<Character>) (origin:Position) (targets:Position array) : seq<aStarResult option> =
//     let candidates = new List<aStarResult * string>()
//     let adjacentFreePositions = origin |> getAdjacentFreePositions map livingCharacters |> Seq.toArray
//     let getSortValue = (fun c ->
//         let minCostOfWalking = (targets |> Seq.map (fun t -> ManhattanDistance c.Steps.[c.Steps.Length - 1] t) |> Seq.min) + c.Steps.Length
//         let pathReadingOrderSortValue = String.Join(".", c.Steps |> Seq.map (fun s -> s.Y.ToString() + ":" + s.X.ToString()))
//         minCostOfWalking.ToString("000") + "." + pathReadingOrderSortValue
//     )
//     candidates.AddRange(adjacentFreePositions |> Seq.map (fun p -> ({aStarResult.Steps = [|p|]}, getSortValue {aStarResult.Steps = [|p|]})))
//     let mutable printJu = false
//     Seq.initInfinite (fun _ ->
//         if candidates.Count > 0
//         then
//             let nextCandidate =
//                 candidates
//                     |> Seq.sortBy (fun (_,sortValue) -> sortValue)
//                     |> Seq.head
//             candidates.Remove(nextCandidate) |> ignore
//             Some(nextCandidate)
//         else None
//     )
//         |> Seq.takeWhile (fun c -> c <> None)
//         |> Seq.choose (id)
//         |> Seq.map (fun (c, sortValue) ->
//             let lastPositionInPlan = c.Steps.[c.Steps.Length - 1]
//             if targets |> Seq.exists (fun t -> t = lastPositionInPlan)
//             then
//                 Some(c)
//             else
//                 let otherWaysToThisPoint = candidates |> Seq.filter (fun (cand, _) -> cand.Steps |> Seq.contains lastPositionInPlan) |> Seq.toArray
//                 let isThisWayBetterThanAllOtherWays = not (otherWaysToThisPoint |> Seq.exists (fun (_, candSort) -> candSort < sortValue))
//                 if isThisWayBetterThanAllOtherWays
//                 then
//                     //Side effects in our tryFind? Ah, well...
//                     otherWaysToThisPoint |> Seq.iter (fun cand -> candidates.Remove(cand) |> ignore)
//                     lastPositionInPlan
//                         |> getAdjacentFreePositions map livingCharacters
//                         |> Seq.filter (fun p -> not (c.Steps |> Seq.contains p))
//                         |> Seq.iter (fun p ->
//                             let newAStarResult = {aStarResult.Steps = ([|p|] |> Array.append c.Steps)}
//                             candidates.Add(newAStarResult, getSortValue newAStarResult)
//                         )
//                 None
//         )

type DefaultMovingDecisionEngine() =
    interface IDecisionEngine with
        member this.GetAction map livingCharacters character =
            let isAdjacentToEnemy =
                livingCharacters
                    |> character.getEnemies
                    |> Seq.exists (fun enemy -> character.isAdjacentTo(enemy))
            if isAdjacentToEnemy
            then
                None
            else
                let grid = new RoyT.AStar.Grid(map.[0].Length, map.Length)
                let convertToLibraryPosition position =
                    new RoyT.AStar.Position(position.X, position.Y)
                for y in { 0 .. map.Length - 1} do
                    for x in { 0 .. map.[0].Length - 1} do
                        let p = {Position.X = x; Position.Y = y}
                        if not (isFreePosition map livingCharacters p) && p <> character.Position
                        then
                            grid.BlockCell(p |> convertToLibraryPosition)

                let options =
                    character.getEnemies(livingCharacters)
                        |> Seq.collect (fun e -> getAdjacentFreePositions map livingCharacters e.Position)
                        |> Seq.map (fun target ->
                            (
                                grid.GetPath(character.Position |> convertToLibraryPosition, target |> convertToLibraryPosition, RoyT.AStar.MovementPatterns.LateralOnly),
                                target
                            )
                        )
                        |> Seq.filter (fun (p, _) -> p <> null && p.Length > 0)
                        |> Seq.sortBy (fun (p, _) -> p.Length)
                        |> Seq.toArray

                if options.Length = 0
                then
                    None
                else
                    let minLength = options |> Seq.map (fun (p, _) -> p.Length) |> Seq.min
                    let shortestOptions = options |> Seq.filter (fun (p, _) -> p.Length = minLength)
                    let selectedTarget = shortestOptions |> sortByReadingOrder (fun (_, t) -> t) |> Seq.map (fun (_, t) -> t) |> Seq.head
                    let possiblePathsToTarget =
                        character.Position
                            |> getAdjacentFreePositions map livingCharacters
                            |> Seq.map (fun candidateStep ->
                                if (candidateStep = selectedTarget)
                                then
                                    (
                                        [|candidateStep |> convertToLibraryPosition|],
                                        candidateStep                                        
                                    )                                    
                                else
                                    (
                                        grid.GetPath(candidateStep |> convertToLibraryPosition, selectedTarget |> convertToLibraryPosition, RoyT.AStar.MovementPatterns.LateralOnly),
                                        candidateStep
                                    )
                            )
                            |> Seq.toArray
                    let minLength = possiblePathsToTarget |> Seq.map (fun (p, _) -> p.Length) |> Seq.min
                    let pathsOfShortestLength = possiblePathsToTarget |> Seq.filter (fun (p, _) -> p.Length = minLength)
                    let (plan, destination) = pathsOfShortestLength |> sortByReadingOrder (fun (_, d) -> d) |> Seq.head
                    Some(new MoveAction(character, destination, selectedTarget) :> IAction)

// type DefaultMovingDecisionEngine() =
//     interface IDecisionEngine with
//         member this.GetAction map livingCharacters character =
//             let isAdjacentToEnemy =
//                 livingCharacters
//                     |> character.getEnemies
//                     |> Seq.exists (fun enemy -> character.isAdjacentTo(enemy))
//             if isAdjacentToEnemy
//             then
//                 None
//             else
//                 let pathPlanning = aStarPlanning character map livingCharacters character.Position
//                 let possibleTargetPositions =
//                     character.getEnemies(livingCharacters)
//                         |> Seq.map (fun c -> c.Position)
//                         |> Seq.collect (fun p -> getAdjacentFreePositions map livingCharacters p)
//                         |> Seq.sortBy (fun p -> ManhattanDistance p character.Position)
//                         |> Seq.toArray
//                 let walkingPlan =
//                     possibleTargetPositions
//                         |> pathPlanning
//                         |> Seq.choose (id)
//                         |> Seq.tryHead

//                 match walkingPlan with
//                 | Some(plan) ->
//                     //printfn "Choose plan %A" plan.Steps
//                     Some(new MoveAction(character, plan.Steps.[0], plan.Steps) :> IAction)
//                 | None -> None

let sortCharactersInReadingOrder = sortByReadingOrder (fun (c:Character) -> c.Position)

let firstInReadingOrder positionAccessor collection =
    collection |> sortByReadingOrder positionAccessor |> Seq.head

type BattleResult =
    {
        FullTurnsCompleted : int
        HPLeft : int
    }

type Cave(pauseAndDraw:bool, initialData:string[], elfAttackPower) =
    let mutable isGameEnded = false
    let characterAI: IDecisionEngine list =
        [
            new EndGameIfOutOfEnemiesDecisionEngine(fun () -> isGameEnded <- true)
            new DefaultMovingDecisionEngine()
            new DefaultAttackingDecisionEngine(pauseAndDraw)
        ]
    let elfIds = new Queue<char>(['1';'2';'3';'4';'5';'6';'7';'8';'9';'0';'@';])
    let goblinIds = new Queue<char>(['A';'B';'C';'D';'E';'F';'G';'H';'J';'K';'L';'M';'N';'P';'R';'S';'T';'U';'V';'W';'X';'Y';'Z';])
    let livingCharacters = new List<Character>()
    let map =
        seq { 0 .. initialData.Length - 1 }
            |> Seq.map (fun y ->
                let line = initialData.[y]
                seq { 0 .. line.Length - 1 }
                    |> Seq.map (fun x ->
                        let createCharacterInfo characterType attackPower =
                            let characterId =
                                match characterType with
                                | Elf -> elfIds.Dequeue()
                                | Goblin -> goblinIds.Dequeue()
                            let character = new Character(pauseAndDraw, characterId, characterAI, characterType, {Position.X = x; Position.Y = y})                            
                            character.AttackPower <- attackPower
                            character
                        match line.[x] with
                        | '#' -> Wall
                        | '.' -> Floor
                        | 'G' ->
                            livingCharacters.Add(createCharacterInfo Goblin 3)
                            Floor
                        | 'E' ->
                            livingCharacters.Add(createCharacterInfo Elf elfAttackPower)
                            Floor
                    )|> Seq.toArray
            )
            |> Seq.toArray

    let existsCharactersOfBothTypes() =
        livingCharacters |> Seq.exists (fun c -> c.Type = Elf) && livingCharacters |> Seq.exists (fun c -> c.Type = Goblin)

    let mutable fullTurnsCompleted = 0

    member val LivingCharacters = livingCharacters
    member self.getActionPlans() =
        livingCharacters
            |> sortCharactersInReadingOrder
            |> Seq.map (fun c -> c.GetPlannedActions map livingCharacters)

    member self.OneTurn() =
        if isGameEnded
        then
            Some({
                BattleResult.FullTurnsCompleted = fullTurnsCompleted
                BattleResult.HPLeft = livingCharacters |> Seq.sumBy (fun c -> c.HP)
            })
        else
            let mutable turnOrder = 0
            livingCharacters
                |> sortCharactersInReadingOrder
                |> Seq.iter (fun c ->
                    if c.HP > 0
                    then
                        turnOrder <- turnOrder + 1
                        if pauseAndDraw
                        then
                            printf "%2i. " turnOrder
                        c.OneTurn map livingCharacters
                )
            if isGameEnded
            then
                Some({
                    BattleResult.FullTurnsCompleted = fullTurnsCompleted
                    BattleResult.HPLeft = livingCharacters |> Seq.sumBy (fun c -> c.HP)
                })
            else
                fullTurnsCompleted <- fullTurnsCompleted + 1
                None

    member self.Draw() =
        for y in { 0 .. map.Length - 1} do
            let row = map.[y]
            for x in {0 .. row.Length - 1} do
                let characterOnPosition = livingCharacters |> Seq.tryFind (fun c -> c.Position.Y = y && c.Position.X = x)
                let mutable c = ' '
                match characterOnPosition with
                | Some(character) ->
                    c <- character.Id.ToString().[0]
                        // if character.Type = Elf
                        // then 'E'
                        // else 'G'
                | None ->
                    c <-
                        if row.[x] = Floor
                        then '.'
                        else '#'
                printf "%c" c
            printfn ""
        printfn ""
        printfn "After %i turns" fullTurnsCompleted
        livingCharacters |> Seq.iter (fun c -> printfn "%O: %i" c c.HP)
        printfn ""

let runBattle pauseAndDraw elfAttackPower battleData =
    let cave = new Cave(pauseAndDraw, battleData, elfAttackPower)
    let result =
        Seq.initInfinite (fun _ ->
            if pauseAndDraw
            then
                Console.ReadLine() |> ignore
                Console.Clear()
                cave.Draw()
            cave.OneTurn()
        )
        |> Seq.pick (id)
    result |> printfn "Battle result %A"
    result.HPLeft * result.FullTurnsCompleted |> printfn "Battle outcome: %A"
    result

let runAndVerify runner  expectedOutcome data =
    let result = data |> runner
    let outcome = result.HPLeft * result.FullTurnsCompleted
    if outcome = expectedOutcome
    then
        printfn "Met expectations"
    else
        printfn "-------- FAILED !!!!! ------------ expected %i, received %i" expectedOutcome outcome

let testCustomInitialPlan() =
    let cave = new Cave(true, customSampleBattle, 3)
    cave.Draw()
    let p x y = { Position.X = x; Position.Y = y}
    let expectedGoblinPlan =
        new Queue<Position>(
            [
                p 6 6
                p 6 5
                p 6 4
                p 7 4
                p 7 3
                p 7 2
                p 7 1
                p 6 1
                p 5 1
                p 4 1
                p 4 2
                p 4 3
            ]
        )
    while expectedGoblinPlan.Count > 0 do
        let expectedDestination = expectedGoblinPlan.Dequeue()
        let goblinMovementAction =
            cave.getActionPlans()
                |> Seq.collect (fun p -> p)
                |> Seq.cast<MoveAction>
                |> Seq.filter (fun a -> a.Character.Type = Goblin)
                |> Seq.head
        if goblinMovementAction.Destination = expectedDestination
        then
            printfn "Correctly got the expected %A" expectedDestination
        else
            printfn "!!!!!!!!!!!!!! Expected %A but got %A" expectedDestination goblinMovementAction.Destination
        (goblinMovementAction :> IAction).ApplyAction()

let cheatForElves pauseAndDraw battleData =
    let (attackPower, result) =
        Seq.initInfinite (fun attackPower -> 
            let cave = new Cave(pauseAndDraw, battleData, attackPower)
            let elvesBeforeBattleStarted = cave.LivingCharacters |> Seq.filter (fun c -> c.Type = Elf) |> Seq.length
            let result =
                Seq.initInfinite (fun _ ->
                    if pauseAndDraw
                    then
                        Console.ReadLine() |> ignore
                        Console.Clear()
                        cave.Draw()
                    cave.OneTurn()
                )
                |> Seq.pick (id)
            let elfCountAfterBattleEnded = cave.LivingCharacters |> Seq.filter (fun c -> c.Type = Elf) |> Seq.length
            (attackPower, elvesBeforeBattleStarted = elfCountAfterBattleEnded, result)
        )
            |> Seq.pick (fun (attackPower, didAllElvesSurvive, battleResult) ->
                if didAllElvesSurvive
                then 
                    Some(attackPower, battleResult)
                else
                    None                
            )    

    result |> printfn "Attack power %i gave battle result %A" attackPower
    result.HPLeft * result.FullTurnsCompleted |> printfn "Attack power %i gave battle outcome: %A" attackPower
    result

let cheatAndVerify pauseAndDraw expectedOutcome battleData =
    let result = cheatForElves pauseAndDraw battleData
    let outcome = result.HPLeft * result.FullTurnsCompleted    
    if outcome = expectedOutcome
    then
        printfn "Met expectations"
    else
        printfn "-------- FAILED !!!!! ------------ expected %i, received %i" expectedOutcome outcome

[<EntryPoint>]
let main argv =
    let pauseAndDraw = ((argv |> Seq.rev |> Seq.tryHead) = Some("--pause-battles"))
    let runBattleWithCorrectPauseSetting = runBattle pauseAndDraw 3
    let testBattle = runAndVerify runBattleWithCorrectPauseSetting
    let cheatWithCorrectPauseSetting = cheatForElves pauseAndDraw 
    let verifyCheat = cheatAndVerify pauseAndDraw
    // testCustomInitialPlan()
    // customSampleBattle |> runBattleWithCorrectPauseSetting |> ignore
    battleStartSample1 |> testBattle 27730
    battleStartSample2 |> testBattle 36334
    battleStartSample3 |> testBattle 39514
    battleStartSample4 |> testBattle 27755
    battleStartSample5 |> testBattle 28944
    battleStartSample6 |> testBattle 18740
    realBattleData |> runBattleWithCorrectPauseSetting |> ignore
    
    battleStartSample1 |> verifyCheat 4988
    battleStartSample3 |> verifyCheat 31284
    battleStartSample4 |> verifyCheat 3478
    battleStartSample5 |> verifyCheat 6474
    battleStartSample6 |> verifyCheat 1140

    realBattleData |> cheatWithCorrectPauseSetting |> ignore



    0 // return an integer exit code
