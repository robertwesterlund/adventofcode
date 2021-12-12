let parse filename =
    System.IO.File.ReadAllLines(filename)
    |> Array.toList
    |> List.map (fun l -> l.ToCharArray() |> Array.toList)

type ChunkStart =
    | ProgramStart
    | Parenthesis
    | SquareBracket
    | Braces
    | AngleBracket

type Context =
    { Prev: Context option
      Chunk: ChunkStart }
    member this.canBeClosedWith(c: char) =
        match (this.Chunk, c) with
        | (Parenthesis, ')') -> true
        | (SquareBracket, ']') -> true
        | (Braces, '}') -> true
        | (AngleBracket, '>') -> true
        | _ -> false

    member this.tryInterpret(c: char) =
        match c with
        | '(' ->
            Some
                { Prev = Some this
                  Chunk = Parenthesis }
        | '[' ->
            Some
                { Prev = Some this
                  Chunk = SquareBracket }
        | '{' -> Some { Prev = Some this; Chunk = Braces }
        | '<' ->
            Some
                { Prev = Some this
                  Chunk = AngleBracket }
        | _ ->
            match this.canBeClosedWith c with
            | true -> this.Prev
            | false -> None

let rec getFirstIllegalCharacter (context: Context) line =
    match line with
    | [] -> None
    | h :: t ->
        match context.tryInterpret h with
        | Some c -> getFirstIllegalCharacter c t
        | None -> Some(h, t, context)

let part1 filename =
    filename
    |> parse
    |> List.map (fun line ->
        match line
              |> getFirstIllegalCharacter { Prev = None; Chunk = ProgramStart }
            with
        | None ->
            //printfn "%O did not break" line
            0L
        | Some (error, _, context) ->
            //printfn "%O did break. Invalid character %c detected." line error
            match error with
            | ')' -> 3L
            | ']' -> 57L
            | '}' -> 1197L
            | '>' -> 25137L)
    |> List.sum
    |> printfn "%s part 1: %i" filename

let part2 filename =
    let completionScores =
        filename
        |> parse
        // First remove corrupt lines
        |> List.filter (fun line ->
            line
            |> getFirstIllegalCharacter { Prev = None; Chunk = ProgramStart }
            |> Option.isNone)
        // Then interpret the chunks
        |> List.map (fun line ->
            line
            |> List.fold
                (fun (acc: Context) curr -> (acc.tryInterpret (curr)).Value)
                { Prev = None; Chunk = ProgramStart })
        // Then generate the completions
        |> List.map (fun context ->
            let rec flatten l c =
                match c.Chunk with
                | ProgramStart -> l
                | Parenthesis -> flatten (')' :: l) c.Prev.Value
                | SquareBracket -> flatten (']' :: l) c.Prev.Value
                | Braces -> flatten ('}' :: l) c.Prev.Value
                | AngleBracket -> flatten ('>' :: l) c.Prev.Value

            flatten [] context |> List.rev)
        // Then calculate "score" of completions
        |> List.map (fun completion ->
            completion
            |> List.fold
                (fun acc c ->
                    acc * 5L
                    + match c with
                      | ')' -> 1L
                      | ']' -> 2L
                      | '}' -> 3L
                      | '>' -> 4L)
                0L)
        |> List.sort

    let len = completionScores |> List.length

    let middle =
        completionScores
        |> List.skip (len / 2)
        |> List.head

    middle |> printfn "%s part 2: %i" filename

"testdata.txt" |> part1
"input.txt" |> part1

"testdata.txt" |> part2
"input.txt" |> part2
