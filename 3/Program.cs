using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace _3
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || !args.Any() || !int.TryParse(args[0], out int value))
            {
                Console.WriteLine("Need to provide a value argument");
                return;
            }

            PrintFirstValueHigherThan(value);
        }

        private static void PrintFirstValueHigherThan(int value)
        {
            var spiral = new Spiral();
            var cell = spiral.AddNewCellWithSummedValueWhileValueIsLowerThanOrEqualTo(value);

            Console.WriteLine($"First cell with value higher than {value} has a value of {cell.Value}");
        }

        private static void PrintManhattanDistanceForValue(int value)
        {
            var spiral = new Spiral();
            Spiral.Cell lastCell = default(Spiral.Cell);
            for (var i = 2; i <= value; i++)
            {
                lastCell = spiral.AddNewCell(i);
            }
            Console.WriteLine($"Manhattan distance for {lastCell.Value} is {Math.Abs(lastCell.X) + (Math.Abs(lastCell.Y))}");
        }
    }

    public class Spiral : IEnumerable<Spiral.Cell>
    {
        private LinkedList<Cell> _cells;
        public Spiral()
        {
            this._cells = new LinkedList<Cell>();
            this._cells.AddLast(new Cell(1, 0, 0));
        }

        public Spiral.Cell AddNewCell(int value)
        {
            var nextIndex = GetNextCellIndex();
            return _cells.AddLast(new Cell(value, x: nextIndex.x, y: nextIndex.y)).Value;
        }

        public Spiral.Cell AddNewCellWithSummedValueWhileValueIsLowerThanOrEqualTo(int threshold)
        {
            if (threshold <= _cells.Last.Value.Value)
            {
                throw new Exception("Cannot add to a value lower than already in spiral");
            }
            while (_cells.Last.Value.Value <= threshold)
            {
                var nextIndex = GetNextCellIndex();
                var value = _cells.Where(c =>
                {
                    var xDistance = Math.Abs(nextIndex.x - c.X);
                    var yDistance = Math.Abs(nextIndex.y - c.Y);
                    return (xDistance == 1 || xDistance == 0) && (yDistance == 1 || yDistance == 0);
                }).Sum(cell => cell.Value);
                AddNewCell(value);
            }
            return _cells.Last.Value;
        }

        public override string ToString()
        {
            var highestValueLength = _cells.Last.Value.Value.ToString().Length;
            var arraySize = (_cells.Last.Value.GetLayerIndex() * 2) + 1;
            var mid = (int)Math.Floor((decimal)arraySize / (decimal)2);
            var array = new string[arraySize, arraySize];
            System.Console.WriteLine($"ArraySize: {arraySize}, Mid: {mid}");
            foreach (var cell in _cells)
            {
                array[mid - cell.Y, mid + cell.X] = cell.Value.ToString().PadLeft(highestValueLength);
            }
            StringBuilder builder = new StringBuilder();
            for (var y = 0; y < arraySize; y++)
            {
                for (var x = 0; x < arraySize; x++)
                {
                    if (x != 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(array[y, x]);
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }

        IEnumerator<Cell> IEnumerable<Spiral.Cell>.GetEnumerator()
        {
            return _cells.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_cells).GetEnumerator();
        }

        private (int x, int y) GetNextCellIndex()
        {
            var currentCell = _cells.Last.Value;
            var layer = currentCell.GetLayerIndex();
            if (currentCell.X == layer) // right side
            {
                if (currentCell.Y == -layer)
                {
                    //Will create new layer
                    return (x: layer + 1, y: currentCell.Y);
                }
                else if (currentCell.Y == layer)
                {
                    return (x: currentCell.X - 1, y: currentCell.Y);
                }
                else
                {
                    return (x: currentCell.X, y: currentCell.Y + 1);
                }
            }
            else if (currentCell.Y == layer) // top side
            {
                if (currentCell.X == -layer)
                {
                    return (x: currentCell.X, y: currentCell.Y - 1);
                }
                else
                {
                    return (x: currentCell.X - 1, y: currentCell.Y);
                }
            }
            else if (currentCell.X == -layer) // left side
            {
                if (currentCell.Y == -layer)
                {
                    return (x: currentCell.X + 1, y: currentCell.Y);
                }
                else
                {
                    return (x: currentCell.X, y: currentCell.Y - 1);
                }
            }
            else if (currentCell.Y == -layer) // bottom side (except for the case where X == layer, that has already been handled)
            {
                return (x: currentCell.X + 1, y: currentCell.Y);
            }
            else
            {
                throw new Exception("This is weird. The cell is not actually on the spiral...");
            }
        }

        public struct Cell
        {
            public Cell(int value, int x, int y)
            {
                this.Value = value;
                this.X = x;
                this.Y = y;
            }

            public int Value { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public int GetLayerIndex()
            {
                return Math.Max(Math.Abs(X), Math.Abs(Y));
            }
        }
    }
}
