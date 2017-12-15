using System;
using System.Linq;
using System.Collections.Generic;
using static C__09.Program;

namespace C__09
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest();
            RunRealThing();
        }



        private static void RunTest()
        {
            Console.WriteLine("-----------");
            Console.WriteLine("Test");
            Run(@"{{<!!>},{<!!>},{<!!>},{<!!>}}".Split("\n"), logToConsole: true);
            Run(@"{{<ab>},{<ab>},{<ab>},{<ab>}}".Split("\n"), logToConsole: true);
            Run(@"{{<a!>},{<a!>},{<a!>},{<ab>}}".Split("\n"), logToConsole: true);
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
        private static Group Parse(string line)
        {
            var root = new Group()
            {
                Score = 1
            };
            var stack = new Stack<Group>();
            stack.Push(root);
            var current = root;
            var isReadingGarbage = false;
            var numberOfCancelledCharacters = 0;
            for (var i = 1; i < line.Length - 2; i++)
            {
                var currentChar = line[i];
                if (currentChar == '!')
                {
                    i++;
                    continue;
                }
                if (isReadingGarbage && currentChar == '>')
                {
                    isReadingGarbage = false;
                    continue;
                }
                if (isReadingGarbage)
                {
                    numberOfCancelledCharacters++;
                    continue;
                }
                switch (currentChar)
                {
                    case ',':
                        continue;
                    case '<':
                        isReadingGarbage = true;
                        continue;
                    case '{':
                        var newGroup = new Group()
                        {
                            Score = current.Score + 1
                        };
                        stack.Push(current);
                        current.Children.Add(newGroup);
                        current = newGroup;
                        continue;
                    case '}':
                        current = stack.Pop();
                        continue;
                    default:
                        throw new Exception("Got character which was invalid: " + currentChar);
                }
            }
            System.Console.WriteLine("Number of Cancelled: " + numberOfCancelledCharacters);
            return root;
        }

        private static void Run(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
            _shouldLogToConsole = logToConsole;

            Stack<Group> stack = new Stack<Group>();
            stack.Push(data[0]);
            var sum = 0;
            while (stack.Any())
            {
                var current = stack.Pop();
                foreach (var child in current.Children)
                {
                    stack.Push(child);
                }
                sum += current.Score;
            }
            System.Console.WriteLine(data[0]);
            System.Console.WriteLine($"Score was {sum}");
        }

    }

    public class Group
    {
        public List<Group> Children { get; set; } = new List<Group>();
        public int Score { get; set; }

        public override string ToString()
        {
            return $"{{{Score}{string.Join("", Children.Select(c => c.ToString()))}}}";
        }
    }
}
