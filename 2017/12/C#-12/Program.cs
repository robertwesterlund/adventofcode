using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__12.Program;

namespace C__12
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

        private static Prog Parse(string line)
        {
            var split = line.Split("<->");
            var prog = new Prog();
            prog.Id = split[0].Trim();
            foreach (var connector in split[1].Split(","))
            {
                prog.CanConnectTo.Add(connector.Trim());
            }
            return prog;
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
            _shouldLogToConsole = logToConsole;

            var alreadyTested = new HashSet<string>();
            var stillToTest = new Stack<Prog>();
            stillToTest.Push(data.First(p => p.Id == "0"));
            int progsThatCanConnectTo0 = 0;
            var groups = 0;
            while (alreadyTested.Count != data.Length)
            {
                groups++;
                var groupStarter = data.First(p => !alreadyTested.Contains(p.Id));
                stillToTest.Push(groupStarter);
                Console.WriteLine("Group starts with " + groupStarter.Id);
                while (stillToTest.Any())
                {
                    var current = stillToTest.Pop();
                    if (alreadyTested.Contains(current.Id))
                    {
                        continue;
                    }
                    progsThatCanConnectTo0++;
                    alreadyTested.Add(current.Id);
                    foreach (var child in current.CanConnectTo)
                    {
                        stillToTest.Push(data.First(p => p.Id == child));
                    }
                }
                Console.WriteLine("Progs in group: " + progsThatCanConnectTo0);
            }

            Console.WriteLine("Groups: " + groups);
        }
    }

    public class Prog
    {
        public string Id { get; set; }

        public List<string> CanConnectTo { get; } = new List<string>();
    }
}
