var actions = new Action[]
{
    new Action.Forward(Distance: 3L),
    new Action.Down(Distance: 4L),
    new Action.Up(Distance: 5L)
};

Action Parse(string line)
{
    var split = line.Split(' ');
    var value = Int64.Parse(split[1]);
    return (split[0] switch
    {
        "forward" => new Action.Forward(Distance: value),
        "down" => new Action.Down(Distance: value),
        "up" => new Action.Up(Distance: value),
        _ => throw new Exception("Failed parsing line: " + line)
    });
}

Int64 CalculateEndResult(Position position) => position.X * position.Y;

void RunPart1(string[] input)
{
    var origo = new Position(X: 0L, Y: 0L);
    var actions = input.Select(l => Parse(l));
    var result = actions.Aggregate(origo, (pos, action) => action switch
    {
        Action.Forward f => pos with { X = pos.X + f.Distance },
        Action.Down f => pos with { Y = pos.Y + f.Distance },
        Action.Up f => pos with { Y = pos.Y - f.Distance },
        _ => throw new Exception("Unknown action " + action)
    });

    System.Console.WriteLine(result);
    System.Console.WriteLine(CalculateEndResult(result));
}

void RunPart2(string[] input)
{
    var origo = new Position(X: 0L, Y: 0L);
    var actions = input.Select(l => Parse(l));
    var result = actions.Aggregate(
        new Submarine(Aim: 0, Position: origo),
        (sub, action) => action switch
        {
            Action.Down f => sub with { Aim = sub.Aim + f.Distance },
            Action.Up f => sub with { Aim = sub.Aim - f.Distance },
            Action.Forward f => sub with
            {
                Position = new Position(
                    X: sub.Position.X + f.Distance,
                    Y: sub.Position.Y + f.Distance * sub.Aim
                )
            },
            _ => throw new Exception("Unknown action " + action)
        }
    );

    System.Console.WriteLine(result);
    System.Console.WriteLine(CalculateEndResult(result.Position));
}

RunPart1(Data.TestData);
RunPart1(Data.Input);
RunPart2(Data.TestData);
RunPart2(Data.Input);

record Position(Int64 X, Int64 Y);

record Submarine(Int64 Aim, Position Position);

internal abstract record Action
{
    public record Forward(Int64 Distance) : Action;
    public record Down(Int64 Distance) : Action;
    public record Up(Int64 Distance) : Action;
}
