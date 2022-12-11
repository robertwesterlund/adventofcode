type CommandType =
    | LS
    | CD of target: string

type File = { Name: string; Size: int64 }

type Line =
    | Command of command: CommandType
    | Directory of name: string
    | FileEntry of File

type Directory =
    { Name: string
      Parent: Directory option
      mutable Directories: Directory list
      mutable Files: File list }

    member this.getDirectorySize() =
        (this.Directories
         |> List.sumBy (fun d -> d.getDirectorySize ()))
        + (this.Files |> List.sumBy (fun f -> f.Size))

type Context =
    { Root: Directory
      mutable Cwd: Directory }

let parse (input: string) =
    input.Replace("\r\n", "\n").Split("\n")
    |> Array.toList
    |> List.map (fun text ->
        if text.[0] = '$' then
            let splitCommand = text.Split(" ")

            match splitCommand.[1] with
            | "cd" -> Command(CD splitCommand.[2])
            | "ls" -> Command(LS)
        elif text.StartsWith("dir") then
            Directory(text.Split(" ").[1])
        else
            let split = text.Split(" ")

            FileEntry
                { Name = split.[1]
                  Size = System.Int64.Parse(split.[0]) })

let createTree input =
    let root =
        { Name = "/"
          Parent = None
          Directories = []
          Files = [] }

    let rec interpret (context: Context) lines =
        match lines with
        | [] -> context
        | h :: t ->
            match h with
            | FileEntry file ->
                context.Cwd.Files <- context.Cwd.Files |> List.append [ file ]
                t |> interpret context
            | Directory dir ->
                context.Cwd.Directories <-
                    context.Cwd.Directories
                    |> List.append [ { Name = dir
                                       Parent = Some context.Cwd
                                       Directories = []
                                       Files = [] } ]

                t |> interpret context
            | Command command ->
                match command with
                | LS -> t |> interpret context
                | CD path ->
                    match path with
                    | "/" -> t |> interpret { context with Cwd = context.Root }
                    | ".." ->
                        t
                        |> interpret { context with Cwd = context.Cwd.Parent |> Option.get }
                    | x ->
                        t
                        |> interpret
                            { context with
                                Cwd =
                                    context.Cwd.Directories
                                    |> List.find (fun d -> d.Name = x) }

    input
    |> parse
    |> interpret { Root = root; Cwd = root }


let rec findDirs predicate cwd =
    let matchingChildren =
        cwd.Directories
        |> List.fold (fun acc curr -> acc |> List.append (findDirs predicate curr)) []

    if predicate cwd then
        cwd :: matchingChildren
    else
        matchingChildren

let part1 input =
    let tree = input |> createTree

    let mutableList =
        new System.Collections.Generic.List<Directory>()

    let dirs =
        findDirs (fun (d: Directory) -> d.getDirectorySize () <= 100000L) tree.Root

    dirs
    |> List.sumBy (fun d -> d.getDirectorySize ())

let part2 input =
    let tree = input |> createTree
    let totalSpace = 70000000L
    let usedSpace = tree.Root.getDirectorySize ()
    let availableSpace = totalSpace - usedSpace
    let neededSpace = 30000000L
    let missingSpace = -(availableSpace - neededSpace)

    let candidates =
        findDirs (fun d -> d.getDirectorySize () > missingSpace) tree.Root

    candidates
    |> List.map (fun d -> d.getDirectorySize ())
    |> List.min

Input.testData
|> part1
|> printfn "Part 1 - test data: %i"

Input.realData
|> part1
|> printfn "Part 1 - real data: %i"

Input.testData
|> part2
|> printfn "Part 2 - test data: %i"

Input.realData
|> part2
|> printfn "Part 2 - real data: %i"
