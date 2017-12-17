using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__17.Program;

namespace C__17
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
            Run(3, logToConsole: true);
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
            Run(324, logToConsole: false);
        }

        private static MyObject Parse(string line)
        {
            return null;
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(int input, bool logToConsole)
        {
            _shouldLogToConsole = logToConsole;

            var list = new LinkedList<int>();
            list.AddFirst(0);
            var current = list.First;
            const int iterations = 50_000_000;
            for (var nextInsertionNumber = 1; nextInsertionNumber <= iterations; nextInsertionNumber++)
            {
                if (nextInsertionNumber % 1_000_000 == 0){
                    System.Console.WriteLine(nextInsertionNumber);
                }
                for (var skippedSteps = 0; skippedSteps < input; skippedSteps++)
                {
                    if (current == list.Last)
                    {
                        current = list.First;
                    }
                    else
                    {
                        current = current.Next;
                    }
                }
                list.AddAfter(current, nextInsertionNumber);
                current = current.Next;
            }
            Console.WriteLine("Current: " + current.Value);
            Console.WriteLine("Next: " + current.Next.Value);
            Console.WriteLine("First Value: " + list.First.Value);
            Console.WriteLine("Second Value: " + list.First.Next.Value);
        }
    }

    public class MyObject
    {
        public MyObject()
        {
        }
    }
}
