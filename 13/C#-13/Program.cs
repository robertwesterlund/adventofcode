using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__13.Program;
using System.Threading.Tasks;

namespace C__13
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
            Run2(data, logToConsole: true);
        }

        private static bool _shouldLogToConsole = false;
        public static void Log(string text)
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
            Run2(GetRealData(), logToConsole: false);
        }

        private static Layer Parse(string line)
        {
            var split = line.Split(": ");
            return new Layer()
            {
                Depth = int.Parse(split[0]),
                Range = int.Parse(split[1])
            };
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");


        private static void Run2(string[] input, bool logToConsole)
        {

            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
            var highestLayer = data.Max(d => d.Depth);
            var layers = Enumerable.Range(0, highestLayer + 1)
                .Select(i => data.FirstOrDefault(d => d.Depth == i) ?? new Layer()
                {
                    Depth = i,
                    Range = 0,
                    ScannerPosition = 0
                }).ToArray();
            var fw = new Firewall2()
            {
                Layers = layers
            };
            long delay = 0;
            while(true){
                if (delay % 1000 == 0){
                    Console.WriteLine(delay);
                }
                if (!fw.IsFoundAfterDelay(delay)){
                    break;
                }
                delay++;
            }
            Console.WriteLine("Found solution after: " + delay);
        }

        private static void Run(string[] input, bool logToConsole)
        {
            //var delay = 0;
            var initialStatesTested = new HashSet<string>();
            var solutions = new List<long>();
            Parallel.For(0, 9999999999, (delay, loopState) =>
            {
                Console.WriteLine(delay.ToString());
                // if (delay % 1000 == 0)
                // {
                //     Console.WriteLine("Testing delay of " + delay);
                // }
                var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
                _shouldLogToConsole = logToConsole;

                var highestLayer = data.Max(d => d.Depth);
                var layers = Enumerable.Range(0, highestLayer + 1)
                    .Select(i => data.FirstOrDefault(d => d.Depth == i) ?? new Layer()
                    {
                        Depth = 1,
                        Range = 0,
                        ScannerPosition = 0
                    }).ToArray();

                var firewall = new Firewall()
                {
                    Layers = layers
                };
                for (var i = 0; i < delay; i++)
                {
                    foreach (var layer in firewall.Layers)
                    {
                        layer.MoveScanner();
                    }
                }
                // var initialStateAsString = string.Join(", ", firewall.Layers.Select(l => $"{l.ScannerPosition}:{l.Direction}"));
                // Log(initialStateAsString);
                // if (initialStatesTested.Contains(initialStateAsString))
                // {
                //     //throw new Exception("We have a circular calculation");
                //     Console.WriteLine("We have a circular calculation at " + delay);
                //     loopState.Break();
                //     return;
                // }
                //initialStatesTested.Add(initialStateAsString);
                while (!firewall.IsOutside)
                {
                    firewall.Move();
                }
                var severity = firewall.Layers.Select(l => l.HasFoundYou ? l.Depth * l.Range : 0).Sum();
                //Console.WriteLine("Severity: " + severity);
                if (!firewall.Layers.Any(l => l.HasFoundYou))
                {
                    Console.WriteLine("Found a solution with delay " + delay);
                    solutions.Add(delay);
                    loopState.Break();
                    return;
                    //break;
                }
                delay++;
            });

            if (solutions.Any())
            {
                Console.WriteLine("Fastest solution: " + solutions.Min());
            }
            //Console.WriteLine("Waiting " + delay + " solves the problem");
        }
    }

    public class Firewall2
    {
        public Layer[] Layers { get; set; }

        public bool IsFoundAfterDelay(long delay)
        {
            for (var i = 0; i < Layers.Length; i++)
            {
                var layer = Layers[i];
                var layerDelay = delay + i;
                if (layer.GetScannerPositionAfter(layerDelay) == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class Firewall
    {
        public int YourLayerIndex = -1;
        public Layer[] Layers { get; set; }
        public bool IsOutside { get; set; } = false;
        public void Move()
        {
            if (IsOutside)
            {
                throw new Exception("Can't move when you're already outside");
            }
            YourLayerIndex++;
            if (YourLayerIndex >= Layers.Length)
            {
                IsOutside = true;
                return;
            }
            if (Layers[YourLayerIndex].ScannerPosition == 1)
            {
                Log("Found at " + YourLayerIndex);
                Layers[YourLayerIndex].HasFoundYou = true;
            }
            //Log($"Position: {YourLayerIndex}, ScannerLocations: {string.Join(", ", Layers.Select(l => l.ScannerPosition))}");
            foreach (var layer in Layers)
            {
                layer.MoveScanner();
            }
        }
    }

    public class Layer
    {
        public int Depth { get; set; }
        public int Range { get; set; }
        public int ScannerPosition { get; set; } = 1;
        public int Direction = 1;

        public bool HasFoundYou { get; set; } = false;

        public long GetScannerPositionAfter(long delay)
        {
            if (Range == 0)
            {
                return 0;
            }
            var stepsLeft = delay % (Range - 1);
            var direction = delay / (Range - 1);
            if (direction % 2 == 0)
            {
                return 1 + stepsLeft;
            }
            else
            {
                return Range - stepsLeft;
            }
        }

        public void MoveScanner()
        {
            if (Range == 1)
            {
                ScannerPosition = 1;
            }
            else if (Range > 1)
            {
                if (ScannerPosition == Range && Direction == 1)
                {
                    Direction = -1;
                }
                else if (ScannerPosition == 1 && Direction == -1)
                {
                    Direction = 1;
                }
                ScannerPosition += Direction;
            }
        }
    }
}
