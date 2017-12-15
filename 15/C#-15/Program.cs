using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__15.Program;

namespace C__15
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

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            var a = long.Parse(input[0].Split(" ")[0]);
            var b = long.Parse(input[0].Split(" ")[1]);
            _shouldLogToConsole = logToConsole;

            var generatorA = new Generator()
            {
                Multiplier = 16807,
                InitialValue = a
            };
            var generatorB = new Generator()
            {
                Multiplier = 48271,
                InitialValue = b
            };

            System.Console.WriteLine("Part 1 number of matches: " + Judge.GetNumberOfMatches(generatorA, generatorB, 40_000_000));

            generatorA.Criteria = 4;
            generatorB.Criteria = 8;
            System.Console.WriteLine("Part 2 number of matches: " + Judge.GetNumberOfMatches(generatorA, generatorB, 5_000_000));
        }
    }

    public static class Judge
    {
        private const int BitMask = 0b1111111111111111;

        public static int GetNumberOfMatches(Generator a, Generator b, int comparisonCount)
        {
            var aEnumerator = a.GetGeneratedValues(comparisonCount).GetEnumerator();
            var bEnumerator = b.GetGeneratedValues(comparisonCount).GetEnumerator();
            int matches = 0;
            while (aEnumerator.MoveNext() && bEnumerator.MoveNext())
            {
                if ((aEnumerator.Current & BitMask) == (bEnumerator.Current & BitMask))
                {
                    matches++;
                }
            }
            return matches;
        }
    }

    public class Generator
    {
        public long InitialValue { get; set; }
        public int Multiplier { get; set; }
        public int Criteria { get; set; } = 1;
        public IEnumerable<long> GetGeneratedValues(int count)
        {
            long value = InitialValue;
            while (count > 0)
            {
                value = (value * Multiplier) % 2147483647;
                if (value % Criteria == 0)
                {
                    yield return value;
                    count--;
                }
            }
        }
    }
}
