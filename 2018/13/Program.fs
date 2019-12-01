// Learn more about F# at http://fsharp.org

open System
open System.IO

let sampleData = 
    File.ReadAllLines(".\\sample.data")
let inputData = 
    File.ReadAllLines(".\\input.data")
let collisionSampleData = 
    File.ReadAllLines(".\\collisionsample.data")

type Position = 
    {
        X : int
        Y : int
    }

type Direction = 
    | North
    | West
    | South
    | East

type TurnDirection =
    | Left
    | Straight
    | Right

let getNewDirection direction turnDirection =
    match turnDirection with
    | Straight -> direction
    | Left ->
        match direction with 
        | North -> West 
        | West -> South
        | South -> East
        | East -> North
    | Right ->
        match direction with 
        | North -> East
        | West -> North
        | South -> West
        | East -> South

type TrackType =
    | StraightTrack
    | StraightTrackWithCart of initialDirection : Direction
    | CurveSlash
    | CurveBackslash
    | Intersection  
    | EmptySpace  

let getNextTurnDirection previousTurnDirection =
    match previousTurnDirection with
    | Straight -> Right
    | Right -> Left
    | Left -> Straight

let getNextPosition position direction =
    match direction with 
    | North -> { position with Position.Y = position.Y - 1}
    | South -> { position with Position.Y = position.Y + 1}
    | West -> { position with Position.X = position.X - 1}
    | East -> { position with Position.X = position.X + 1}

[<StructuredFormatDisplay("Cart Id: {Id}, Position: {Position}")>]
type Cart(id, initialPosition, initialDirection) = 
    let mutable nextTurnDirection = Left
    member self.Id = id
    member val Position : Position = initialPosition with get, set
    member val Direction : Direction = initialDirection with get, set
    member val HasCollided : bool = false with get, set

    member self.Move(trackTypeAtPosition) =
        match trackTypeAtPosition with
        | EmptySpace -> raise (Exception("We shouldn't be able to end up on an empty space"))
        | StraightTrack -> ()
        //We treat places which originally had carts as if they are ordinary straight tracks
        | StraightTrackWithCart _ -> ()
        | CurveSlash ->
            self.Direction <-
                match self.Direction with
                | North -> East
                | South -> West
                | West -> South
                | East -> North
        | CurveBackslash ->
            self.Direction <-
                match self.Direction with
                | North -> West
                | South -> East
                | West -> North
                | East -> South
        | Intersection ->
            self.Direction <- getNewDirection self.Direction nextTurnDirection
            nextTurnDirection <- getNextTurnDirection nextTurnDirection
        self.Position <- getNextPosition self.Position self.Direction

type Collision = 
    {
        Position : Position
        JustMoved : Cart
        AlreadyThere : Cart
    }

type Map(trackData:string[]) =
    let tracks = 
        seq {0 .. trackData.Length - 1} |> Seq.map (fun y ->
            let line = trackData.[y]
            seq {0 .. line.Length - 1} |> Seq.map (fun x ->
                match line.[x] with 
                | '/' -> CurveSlash
                | '\\' -> CurveBackslash
                | '|' -> StraightTrack
                | '-' -> StraightTrack
                | '+' -> Intersection
                | '<' -> StraightTrackWithCart West
                | 'v' -> StraightTrackWithCart South
                | '^' -> StraightTrackWithCart North
                | '>' -> StraightTrackWithCart East
                | ' ' -> EmptySpace
            ) |> Seq.toArray
        ) |> Seq.toArray

    let carts = 
        seq {0 .. tracks.Length - 1} 
            |> Seq.collect (fun y -> 
                let row = tracks.[y]
                seq {0 .. row.Length - 1} 
                    |> Seq.choose (fun x ->
                        let t = row.[x]
                        match t with
                        | StraightTrackWithCart direction -> 
                            Some( 
                                new Cart(
                                    x.ToString() + "," + y.ToString(), 
                                    { Position.Y = y; Position.X = x},
                                    direction
                                )
                            )                            
                        | _ -> None
                    )
            ) |> Seq.toArray

    let getNonCrashedCarts() =
        carts |> Seq.filter (fun c -> not c.HasCollided)        

    let mutable ticks = 0L
    member self.TickUntilCollision() =
        ticks <- ticks + 1L
        carts 
            |> Seq.sortBy (fun c -> c.Position.Y * 10000 + c.Position.X) 
            |> Seq.tryPick (fun c -> 
                c.Move(tracks.[c.Position.Y].[c.Position.X])
                let collisionCheck = carts |> Seq.tryFind (fun otherCart -> otherCart.Id <> c.Id && c.Position = otherCart.Position)
                match collisionCheck with
                | None -> None
                | Some(collidedWith) -> 
                    Some({
                        Collision.Position = c.Position
                        Collision.AlreadyThere = collidedWith
                        Collision.JustMoved = c
                    })
            )

    member self.TickUntilOnlyOneCart() =
        ticks <- ticks + 1L
        getNonCrashedCarts()
            |> Seq.sortBy (fun c -> c.Position.Y * 10000 + c.Position.X) 
            |> Seq.iter (fun c -> 
                if c.HasCollided
                then ()
                else
                    c.Move(tracks.[c.Position.Y].[c.Position.X])
                    let collisionCheck = getNonCrashedCarts() |> Seq.tryFind (fun otherCart -> otherCart.Id <> c.Id && c.Position = otherCart.Position)
                    match collisionCheck with
                    | None -> ()
                    | Some(collidedWith) -> 
                        c.HasCollided <- true
                        collidedWith.HasCollided <- true                                       
            )
        if getNonCrashedCarts() |> Seq.length = 1
        then 
            Some(getNonCrashedCarts() |> Seq.head)
        else
            None 

let getFirstCollision data =
    let map = new Map(data)
    Seq.initInfinite (fun _ -> map.TickUntilCollision())
        |> Seq.pick (id)

let getLastCart data =
    let map = new Map(data)
    Seq.initInfinite (fun _ -> map.TickUntilOnlyOneCart())
        |> Seq.pick (id)

[<EntryPoint>]
let main argv =
    
    sampleData |> getFirstCollision |> printfn "%A"
    inputData |> getFirstCollision |> printfn "%A"
    collisionSampleData |> getLastCart |> printfn "%A"
    //printfn "Cart Id: %s, Position: %i,%i" sampleCollisionLastCart.Id sampleCollisionLastCart.Position.X sampleCollisionLastCart.Position.Y
    inputData |> getLastCart |> printfn "%A"

    0 // return an integer exit code
