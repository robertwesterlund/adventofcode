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
            Run(GetRealData(), logToConsole: false);
        }

        private static (int id, int left, int right) Parse(string line, int id)
        {
            var split = line.Split("/");
            return (id: id, left: int.Parse(split[0]), right: int.Parse(split[1]));
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select((l, index) => Parse(l, index)).ToArray();
            _shouldLogToConsole = logToConsole;

            IEnumerable<PortNode> GetSublist(int currentPortValue, IEnumerable<(int id, int left, int right)> availablePorts)
            {
                var validPorts = availablePorts.Where(p => p.left == currentPortValue || p.right == currentPortValue);
                foreach (var validPort in validPorts)
                {
                    var connectsToLeftPort = validPort.left == currentPortValue;
                    var openPort = connectsToLeftPort ? validPort.right : validPort.left;
                    var closedPort = connectsToLeftPort ? validPort.left : validPort.right;
                    foreach (var validSublist in GetSublist(openPort, availablePorts.Where(p => p.id != validPort.id)))
                    {
                        yield return new PortNode()
                        {
                            ClosedPort = closedPort,
                            OpenPort = openPort,
                            Connected = validSublist
                        };
                    }
                    yield return new PortNode()
                    {
                        ClosedPort = closedPort,
                        OpenPort = openPort,
                        Connected = default(PortNode)
                    };
                }
            }

            var validLists = GetSublist(0, data).Select(list => new PortNode { OpenPort = 0, ClosedPort = 0, Connected = list }).ToArray();

            Console.WriteLine("Max value of all bridges: " + validLists.Select(list => list.SumSubtree()).Max());
            var maxLength = validLists.OrderByDescending(l => l.GetLength()).First().GetLength();
            var max = validLists.Where(l => l.GetLength() == maxLength).Select(list => list.SumSubtree()).Max();
            Console.WriteLine("Max value of longest bridge: " + max);
        }
    }

    public class PortNode
    {
        public int ClosedPort { get; set; }
        public int OpenPort { get; set; }
        public PortNode Connected { get; set; }

        public int SumSubtree()
        {
            return ClosedPort + OpenPort + (Connected?.SumSubtree() ?? 0);
        }

        public int GetLength()
        {
            return 1 + (Connected?.GetLength() ?? 0);
        }
    }
}
