using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__21.Program;

namespace C__21
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var testFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "test*.txt").OrderBy(t => t))
            {
                //RunTest(File.ReadAllLines(testFile));
            }
            if (File.Exists("data.txt"))
            {
                RunRealThing();
            }
            else
            {
                Console.WriteLine("-------");
                Console.WriteLine("No real data file");
            }
        }

        private static void RunTest(string[] data)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("Test");
            Console.WriteLine(string.Join("\n", data));
            Run(data, logToConsole: true);
        }

        private static bool _shouldLogToConsole = false;
        private static void Log(string text)
        {
            if (_shouldLogToConsole)
            {
                System.Console.WriteLine(text);
            }
        }

        private static void RunRealThing()
        {
            Console.WriteLine("-----------");
            Console.WriteLine("Real Thing");
            Run(GetRealData(), logToConsole: false);
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            var rules = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
            _shouldLogToConsole = logToConsole;

            var grid = new Grid(new bool[,]{
                {false, true, false},
                {false, false, true},
                {true, true, true}
            });

            System.Console.WriteLine("A nice rule:");
            if (rules.Length > 13)
            {
                System.Console.WriteLine(rules[13].From);
            }

            var current = grid;
            var iterations = 18;//5;
            for (var i = 0; i < iterations; i++)
            {
                current = current.Split().Select(
                    g =>
                    {
                        var matchingRule = rules.FirstOrDefault(r => r.From.Equals(g));
                        if (matchingRule == null)
                        {
                            System.Console.WriteLine("Didn't match anything: " + Environment.NewLine + g.ToString());
                            return g;
                        }
                        else
                        {

                            return matchingRule.To;
                        }
                    }).Join();
                Console.WriteLine("Iterations completed: " + (i+1));
            }

            System.Console.WriteLine("-----");
            System.Console.WriteLine(current);
            System.Console.WriteLine($"Number of on bits: " + current.Bits.Cast<bool>().Count(b => b));

            // var otherGrid = new Grid(new bool[,]{
            //     {true, false, false},
            //     {true, false, true},
            //     {true, true, false}
            // });

            //Console.WriteLine(grid.ToString());
            // System.Console.WriteLine();
            // foreach (var ruleGrid in data)
            // {
            //     System.Console.WriteLine(ruleGrid.From.ToString());
            //     System.Console.WriteLine(grid.Equals(ruleGrid.From));
            //     System.Console.WriteLine(otherGrid.Equals(ruleGrid.From));
            // }
            // System.Console.WriteLine(otherGrid);
            // System.Console.WriteLine();
            // System.Console.WriteLine(otherGrid.GetTransformedGrid(Grid.GetTransformedValue90));
            // System.Console.WriteLine();
            // System.Console.WriteLine(otherGrid.GetTransformedGrid(Grid.GetTransformedValue180));
            // System.Console.WriteLine();
            // System.Console.WriteLine(otherGrid.GetTransformedGrid(Grid.GetTransformedValue270));
            // System.Console.WriteLine();
        }

        private static GridTransformationRule Parse(string line)
        {
            (var fromString, var toString) = line.Split(" => ");
            Grid ParseGrid(string data)
            {
                var rowData = data.Split("/");
                var bits = new bool[rowData.Length, rowData.Length];
                for (var row = 0; row < rowData.Length; row++)
                {
                    for (var column = 0; column < rowData.Length; column++)
                    {
                        bits[row, column] = rowData[row][column] == '#';
                    }
                }
                return new Grid(bits);
            }
            return new GridTransformationRule()
            {
                From = ParseGrid(fromString),
                To = ParseGrid(toString)
            };
        }
    }

    public class GridTransformationRule
    {
        public Grid From { get; set; }
        public Grid To { get; set; }
    }

    public class Grid
    {
        public Grid(bool[,] bits)
        {
            this.Bits = bits;
        }

        public bool[,] Bits { get; }

        public int Size => Bits.GetLength(0);

        public IEnumerable<Grid> Split()
        {
            var sizePerSubgrid = Size % 2 == 0 ? 2 : 3;
            var numberOfSubgridsPerDimension = Size / sizePerSubgrid;
            for (var row = 0; row < numberOfSubgridsPerDimension; row++)
            {
                for (var column = 0; column < numberOfSubgridsPerDimension; column++)
                {
                    var bits = new bool[sizePerSubgrid, sizePerSubgrid];
                    for (var rowInSubgrid = 0; rowInSubgrid < sizePerSubgrid; rowInSubgrid++)
                    {
                        for (var columnInSubgrid = 0; columnInSubgrid < sizePerSubgrid; columnInSubgrid++)
                        {
                            bits[rowInSubgrid, columnInSubgrid] = Bits[row * sizePerSubgrid + rowInSubgrid, column * sizePerSubgrid + columnInSubgrid];
                        }
                    }
                    yield return new Grid(bits);
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Grid grid)
            {
                return this.Equals(grid);
            }
            else
            {
                return false;
            }
        }

        public Grid GetTransformedGrid(Func<Grid, int, int, bool> transformationFunction)
        {
            var bits = new bool[Size, Size];
            for (var row = 0; row < Size; row++)
            {
                for (var column = 0; column < Size; column++)
                {
                    bits[row, column] = transformationFunction(this, row, column);
                }
            }
            return new Grid(bits);
        }

        public Grid GetFlippedGrid()
        {
            var bits = new bool[Size, Size];
            for (var row = 0; row < Size; row++)
            {
                for (var column = 0; column < Size; column++)
                {
                    bits[row, column] = this.Bits[row, Size - 1 - column];
                }
            }
            return new Grid(bits);
        }

        public static bool GetTransformedValue0(Grid grid, int row, int column) => grid.Bits[row, column];
        public static bool GetTransformedValue90(Grid grid, int row, int column) => grid.Bits[column, row];
        public static bool GetTransformedValue180(Grid grid, int row, int column) => grid.Bits[grid.Size - (row + 1), grid.Size - (column + 1)];
        public static bool GetTransformedValue270(Grid grid, int row, int column) => grid.Bits[grid.Size - (column + 1), grid.Size - (row + 1)];

        private string[] _stringRepresentations = null;
        private string[] StringRepresentations
        {
            get
            {
                if (_stringRepresentations == null)
                {
                    _stringRepresentations = new string[]{
                        this.ToString(),
                        this.GetFlippedGrid().ToString(),
                        this.GetTransformedGrid(GetTransformedValue90).ToString(),
                        this.GetTransformedGrid(GetTransformedValue90).GetFlippedGrid().ToString(),
                        this.GetTransformedGrid(GetTransformedValue180).ToString(),
                        this.GetTransformedGrid(GetTransformedValue180).GetFlippedGrid().ToString(),
                        this.GetTransformedGrid(GetTransformedValue270).ToString(),
                        this.GetTransformedGrid(GetTransformedValue270).GetFlippedGrid().ToString()
                    };
                }
                return _stringRepresentations;
            }
        }

        public bool Equals(Grid otherGrid)
        {
            if (this.Size != otherGrid.Size)
            {
                return false;
            }
            return this.StringRepresentations.Any(s => otherGrid.StringRepresentations.Any(r => s == r));
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine,
                Enumerable.Range(0, Size).Select(rowIndex => string.Join("",
                    Enumerable.Range(0, Size).Select(columnIndex => Bits[rowIndex, columnIndex] ? "#" : "."))
                )
            );
        }
    }

    public static class Extensions
    {
        public static Grid Join(this IEnumerable<Grid> grids)
        {
            var allGrids = grids.ToArray();
            var numberOfSubgridsPerRow = (int)Math.Sqrt(allGrids.Length);
            var subgridSize = allGrids.First().Size;
            var newSize = numberOfSubgridsPerRow * subgridSize;
            var newBits = new bool[newSize, newSize];
            for (var row = 0; row < newSize; row++)
            {
                for (var column = 0; column < newSize; column++)
                {
                    var gridsFromTop = (int)Math.Floor((decimal)row / subgridSize);
                    var gridsFromLeft = (int)Math.Floor((decimal)column / subgridSize);
                    var rowInGrid = row - gridsFromTop * subgridSize;
                    var columnInGrid = column - gridsFromLeft * subgridSize;
                    newBits[row, column] = allGrids[gridsFromTop * numberOfSubgridsPerRow + gridsFromLeft].Bits[rowInGrid, columnInGrid];
                }
            }
            return new Grid(newBits);
        }

        public static void Deconstruct<T>(this IEnumerable<T> list, out T first, out T second)
        {
            var arr = list.ToArray();
            first = arr[0];
            second = arr[1];
        }
    }
}
