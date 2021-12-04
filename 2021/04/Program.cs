using System.Collections.Immutable;

(int[] numbersDrawn, ImmutableArray<Board> boards) Parse(string[] input)
{
    var numbersDrawn = input[0].Split(',').Select(int.Parse).ToArray();
    var boards = Enumerable.Range(0, (input.Length - 2) / 6).Select(boardNumber =>
    {
        var startIndex = 2 + boardNumber * 6;
        var cells = input[startIndex..(startIndex + 5)]
            .Select(r =>
                r.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(c => new Cell(Value: int.Parse(c), IsMarked: false))
                    .ToImmutableArray()
            ).ToImmutableArray();
        return new Board(Cells: cells);
    });
    return (numbersDrawn, boards.ToImmutableArray());
}

void Execute(int[] numbersDrawn, ImmutableArray<Board> initialBoards)
{
    var firstWinner = numbersDrawn.Aggregate((boards: initialBoards, lastNumber: 0), (state, drawnNumber) =>
    {
        return state.boards.Any(b => b.HasWon()) switch
        {
            true => state,
            false => (
                boards: state.boards.Select(b => b.Mark(drawnNumber)).ToImmutableArray(),
                lastNumber: drawnNumber
            )
        };
    });
    var lastWinner = numbersDrawn.Aggregate((boards: initialBoards, lastNumber: 0), (state, drawnNumber) =>
        state.boards switch
        {
            { Length: 1 } when state.boards[0].HasWon() => state,
            _ => (
                boards: state.boards.Where(b => !b.HasWon()).Select(b => b.Mark(drawnNumber)).ToImmutableArray(),
                lastNumber: drawnNumber
            )
        }
    );

    var winner = firstWinner.boards.Single(b => b.HasWon());
    var winningScore = winner.Cells.SelectMany(row => row).Sum(c => c.IsMarked ? 0 : c.Value) * firstWinner.lastNumber;
    var lastWinnerScore = lastWinner.boards.Single().Cells.SelectMany(row => row).Sum(c => c.IsMarked ? 0 : c.Value) * lastWinner.lastNumber;
    System.Console.WriteLine();
    System.Console.WriteLine("Winning Number: " + firstWinner.lastNumber);
    System.Console.WriteLine("Winning Score: " + winningScore);
    System.Console.WriteLine("Winner: \n" + winner);
    System.Console.WriteLine("Last Number: " + lastWinner.lastNumber);
    System.Console.WriteLine("Last Score: " + lastWinnerScore);
    System.Console.WriteLine("Last Winner: \n" + lastWinner.boards.Single());
}

System.Console.WriteLine("Test");
System.Console.WriteLine("----------------");
var testData = Parse(File.ReadAllLines("TestData.txt"));
Execute(testData.numbersDrawn, testData.boards);
System.Console.WriteLine("----------------");
System.Console.WriteLine("----------------");
System.Console.WriteLine("Real");
var input = Parse(File.ReadAllLines("Input.txt"));
Execute(input.numbersDrawn, input.boards);

record Cell(
    int Value,
    bool IsMarked
);

record Board(
    ImmutableArray<ImmutableArray<Cell>> Cells
)
{
    private (int rowIndex, int cellIndex)? FindIndex(int value)
    {
        for (var i = 0; i < Cells.Length; i++)
        {
            for (var j = 0; j < Cells[i].Length; j++)
            {
                if (Cells[i][j].Value == value)
                {
                    return (rowIndex: i, cellIndex: j);
                }
            }
        }
        return null;
    }

    public Board Mark(int value)
    {
        var index = FindIndex(value);
        if (index.HasValue)
        {
            var row = this.Cells[index.Value.rowIndex];
            var cell = row[index.Value.cellIndex];
            var newRow = row.SetItem(index.Value.cellIndex, cell with { IsMarked = true });
            var newCellCollection = this.Cells.SetItem(index.Value.rowIndex, newRow);
            return this with { Cells = newCellCollection };
        }
        else
        {
            return this;
        }
    }

    public bool HasWon()
    {
        // Any full row?
        if (Cells.Any(row => row.All(c => c.IsMarked)))
        {
            return true;
        }
        // Any full column?
        if (Enumerable.Range(0, this.Cells[0].Length).Any(columnIndex => Cells.All(row => row[columnIndex].IsMarked)))
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        var builder = new System.Text.StringBuilder();
        foreach (var row in this.Cells)
        {
            foreach (var cell in row)
            {
                builder.Append(cell.IsMarked ? $"-{cell.Value.ToString("00")}- " : $"[{cell.Value.ToString("00")}] ");
            }
            builder.AppendLine();
        }
        return builder.ToString();
    }
}
