using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__22.Program;

namespace C__22
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!long.TryParse(args.FirstOrDefault(), out long iterations))
            {
                iterations = 1000;
            }
            foreach (var testFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "test*.txt").OrderBy(t => t))
            {
                RunTest(File.ReadAllLines(testFile), iterations);
            }
            if (File.Exists("data.txt"))
            {
                RunRealThing(iterations);
            }
            else
            {
                Console.WriteLine("-------");
                Console.WriteLine("No real data file");
            }
        }

        private static void RunTest(string[] data, long iterations)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("Test");
            Console.WriteLine(string.Join("\n", data));
            Run(data, logToConsole: true, iterations: iterations);
        }

        private static bool _shouldLogToConsole = false;
        private static void Log(string text)
        {
            if (_shouldLogToConsole)
            {
                System.Console.WriteLine(text);
            }
        }

        private static void RunRealThing(long iterations)
        {
            Console.WriteLine("-----------");
            Console.WriteLine("Real Thing");
            Run(GetRealData(), logToConsole: false, iterations: iterations);
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole, long iterations)
        {
            _shouldLogToConsole = logToConsole;
            var sporifica = new Sporifica();

            var numberOfInitialRows = input.Length;
            var numberOfInitialColumns = input[0].Length;
            var topIndex = (int)Math.Floor((decimal)numberOfInitialRows / 2);
            var bottomIndex = -topIndex;
            var rightIndex = (int)Math.Floor((decimal)numberOfInitialColumns / 2);
            var leftIndex = -rightIndex;
            for (var row = topIndex; row >= bottomIndex; row--)
            {
                for (var column = leftIndex; column <= rightIndex; column++)
                {
                    if (input[input.Length - 1 - (row + topIndex)][column + rightIndex] == '#')
                    {
                        sporifica.AffectedNodes.Add(new Point(column, row), State.Infected);
                    }
                }
            }
            Log("Current position: " + sporifica.CurrentPosition);
            Log("Current direction: " + sporifica.CurrentDirection);
            Log($"Going for {iterations} iteration(s)");
            for (var i = 0; i < iterations; i++)
            {
                sporifica.Step();
            }
            Log("Current position: " + sporifica.CurrentPosition);
            Log("Current direction: " + sporifica.CurrentDirection);
            System.Console.WriteLine("Total Infections: " + sporifica.TotalInfections);
        }
    }

    public enum State
    {
        Clean,
        Weakened,
        Infected,
        Flagged
    }

    public class Sporifica
    {
        public Dictionary<Point, State> AffectedNodes { get; } = new Dictionary<Point, State>();

        public Point CurrentPosition { get; private set; } = new Point() { X = 0, Y = 0 };
        public int CurrentDirection { get; private set; } = 0;

        public int TotalInfections { get; private set; } = 0;
        public int TotalCleans { get; private set; } = 0;

        private State GetStateAt(Point position)
        {
            if (AffectedNodes.ContainsKey(position))
            {
                return AffectedNodes[position];
            }
            else
            {
                return State.Clean;
            }
        }

        public void Step()
        {
            switch (GetStateAt(CurrentPosition))
            {
                case State.Clean:
                    AffectedNodes.Add(CurrentPosition, State.Weakened);
                    CurrentDirection = (CurrentDirection - 1);
                    if (CurrentDirection < 0)
                    {
                        CurrentDirection = 3;
                    }
                    break;
                case State.Weakened:
                    TotalInfections++;
                    AffectedNodes[CurrentPosition] = State.Infected;
                    break;
                case State.Infected:
                    AffectedNodes[CurrentPosition] = State.Flagged;
                    CurrentDirection = (CurrentDirection + 1) % 4;
                    break;
                case State.Flagged:
                    AffectedNodes.Remove(CurrentPosition);
                    CurrentDirection = (CurrentDirection + 2) % 4;
                    break;
                    default:
                    throw new Exception("Unknown state");
            }
            CurrentPosition = GetNextPosition();
        }

        private Point GetNextPosition()
        {
            var pos = this.CurrentPosition;
            switch (CurrentDirection)

            {
                case 0:
                    return new Point(pos.X, pos.Y + 1);
                case 1:
                    return new Point(pos.X + 1, pos.Y);
                case 2:
                    return new Point(pos.X, pos.Y - 1);
                case 3:
                    return new Point(pos.X - 1, pos.Y);
                default:
                    throw new Exception("Invalid direction: " + CurrentDirection);
            }
        }
    }

    public struct Point
    {
        public Point(long X, long Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public long X { get; set; }
        public long Y { get; set; }

        public override string ToString()
        {
            return $"{{{X}:{Y}}}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Point p)
            {
                return Equals(p);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Point point)
        {
            return point.X == this.X && point.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
