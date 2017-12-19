using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C_.Program;

namespace C_
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var testFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "test*.txt").OrderBy(t => t))
            {
                RunTest(File.ReadAllLines(testFile));
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

        private static PipingSystem Parse(string[] lines)
        {
            var pipes = new Pipe[lines[0].Length, lines.Length];
            for (var y = 0; y < lines.Length; y++)
            {
                for (var x = 0; x < lines[y].Length; x++)
                {
                    var pipe = GetPipe(lines[y][x]);
                    pipe.X = x;
                    pipe.Y = y;
                    pipes[x, y] = pipe;
                }
            }
            return new PipingSystem(pipes);
        }

        private static Pipe GetPipe(char character)
        {
            switch (character)
            {
                case '-':
                case '|':
                    return new Pipe()
                    {
                        CanWalk = true
                    };
                case '+':
                    return new Pipe()
                    {
                        CanWalk = true,
                        CanTurn = true
                    };
                case ' ':
                    return new Pipe();
                default:
                    return new Pipe()
                    {
                        CanWalk = true,
                        Letter = character
                    };
            }
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            var pipingSystem = Parse(input);
            _shouldLogToConsole = logToConsole;

            var letters = pipingSystem.Navigate()
                .Where(pipe => pipe.Letter.HasValue)
                .Select(pipe => pipe.Letter.Value);
            var result = new string(letters.ToArray());

            Console.WriteLine("Result: " + result);
            Console.WriteLine("Number of steps: " + pipingSystem.Navigate().Count());
        }
    }

    public class PipingSystem
    {
        public PipingSystem(Pipe[,] pipes)
        {
            this.Pipes = pipes;
        }

        public Pipe[,] Pipes { get; }

        public int XSize { get => Pipes.GetLength(0); }
        public int YSize { get => Pipes.GetLength(1); }

        private Pipe GetStartPipe()
        {
            for (var i = 0; i < XSize; i++)
            {
                if (Pipes[i, 0].CanWalk)
                {
                    return Pipes[i, 0];
                }
            }
            throw new Exception("Couldn't find a starting pipe");
        }

        public IEnumerable<Pipe> Navigate()
        {
            var currentPipe = GetStartPipe();
            Func<Pipe, Pipe> GetNorthernPipe = current => current.Y == 0 ? new Pipe() : Pipes[current.X, current.Y - 1];
            Func<Pipe, Pipe> GetSouthernPipe = current => current.Y == YSize - 1 ? new Pipe() : Pipes[current.X, current.Y + 1];
            Func<Pipe, Pipe> GetEasternPipe = current => current.X == XSize - 1 ? new Pipe() : Pipes[current.X + 1, current.Y];
            Func<Pipe, Pipe> GetWesternPipe = current => current.X == 0 ? new Pipe() : Pipes[current.X - 1, current.Y];
            var directionMovers = new[] { GetNorthernPipe, GetSouthernPipe, GetEasternPipe, GetWesternPipe };
            var moveInCurrentDirection = GetSouthernPipe;
            while (currentPipe.CanWalk)
            {
                yield return currentPipe;
                if (currentPipe.CanTurn)
                {
                    moveInCurrentDirection = directionMovers
                        .SingleOrDefault(mover => {
                            var pipe = mover(currentPipe);
                            return pipe.CanWalk && moveInCurrentDirection(pipe) != currentPipe;
                        });
                }
                if (moveInCurrentDirection == null) { currentPipe = new Pipe(); }
                else { currentPipe = moveInCurrentDirection(currentPipe); }
            }
        }
    }

    public class Pipe
    {
        public bool CanWalk { get; set; } = false;
        public bool CanTurn { get; set; } = false;
        public char? Letter { get; set; } = null;
        public int X { get; internal set; }
        public int Y { get; internal set; }
    }
}