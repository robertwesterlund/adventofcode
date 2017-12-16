using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__11.Program;

namespace C__11
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach(var testFile in Directory.GetFiles(Directory.GetCurrentDirectory(), "test*.txt").OrderBy(t => t))
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

        private static Step[] Parse(string line)
        {
            return line.Split(",").Select(d => new Step { Direction = d }).ToArray();
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
            _shouldLogToConsole = logToConsole;

            var steps = data[0]; 
            var location = new Location{X = 0, Y = 0};
            var furthestDistance = 0;
            foreach(var step in steps){
                location = step.Move(location);
                furthestDistance = Math.Max(furthestDistance, location.GetDistanceFromOrigo());
            }
            
            Console.WriteLine("Closest distance: " + location.GetDistanceFromOrigo());
            Console.WriteLine("Furthest: " + furthestDistance);
        }

    }

    public class Step
    {
        public string Direction { get; set; }

        public Location Move(Location loc)
        {
            Func<int, int, Location> delta = (deltaX, deltaY) => new Location { X = loc.X + deltaX, Y = loc.Y + deltaY };

            switch (Direction)
            {
                case "n": return delta(0, 1);
                case "nw": return delta(-1, 1);
                case "sw": return delta(-1, 0);
                case "s": return delta(0, -1);
                case "se": return delta(1, -1);
                case "ne": return delta(1, 0);
                default:
                    throw new Exception("Invalid direction " + Direction);
            }
        }
    }

    public struct Location
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int GetDistanceFromOrigo(){
            if (X < 0 && Y < 0 || X > 0 && Y > 0)
            {
                //We can't move diagonally
                return Math.Abs(X) + Math.Abs(Y);
            }
            else{
                //We can move diagonally
                return Math.Max(Math.Abs(X), Math.Abs(Y));
            }
        }
    }

}
