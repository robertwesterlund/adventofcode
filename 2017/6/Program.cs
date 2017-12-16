using System;
using System.Linq;
using System.Collections.Generic;

namespace _6
{
    class Program
    {
        static void Main(string[] args)
        {
            RunRealData();
        }

        private static void RunRealData()
        {
            var data = System.IO.File.ReadAllLines("data.txt")[0].Split("\t").Select(line => int.Parse(line)).ToArray();
            var steps = Debugger.GetStepsTakenForRedistribution(data, logEachStep: false);
            Console.WriteLine("Steps taken before found loop: " + steps.steps + ", loopsize: " + steps.loopSize);
        }

        private static void RunTestData()
        {
            var steps = Debugger.GetStepsTakenForRedistribution(new int[]{0,2,7,0}, logEachStep: true);
            Console.WriteLine("Steps taken before found loop: " + steps.steps + ", loopsize: " + steps.loopSize);
        }
    }

    class Bank
    {
        public Bank(int blocks)
        {
            this.Blocks = blocks;
        }

        public int Blocks { get; set; }

    }

    class Debugger
    {
        public Debugger(int[] blocks)
        {
            this.Banks = blocks.Select(b => new Bank(b)).ToArray();
        }
        public Bank[] Banks { get; set; }

        private int GetIndexToRedistribute()
        {
            var max = Banks.Select(b => b.Blocks).Max();
            for (var i = 0; i < Banks.Length; i++)
            {
                if (Banks[i].Blocks == max)
                {
                    return i;
                }
            }
            throw new Exception("We shouldn't be here");
        }

        public override string ToString()
        {
            return string.Join(", ", Banks.Select(b => b.Blocks));
        }

        public void Redistribute()
        {
            var index = GetIndexToRedistribute();
            var blocks = Banks[index].Blocks;
            Banks[index].Blocks = 0;
            while (blocks > 0)
            {
                index = (index + 1) % Banks.Length;
                Banks[index].Blocks++;
                blocks--;
            }
        }

        public static (int steps, int loopSize) GetStepsTakenForRedistribution(int[] blocks, bool logEachStep)
        {
            HashSet<string> alreadySeen = new HashSet<string>();
            var debugger = new Debugger(blocks);
            var memoryDump = debugger.ToString();
            var redistributionCount = 0;
            while (!alreadySeen.Contains(memoryDump))
            {
                if (logEachStep)
                {
                    Console.WriteLine(memoryDump);
                }
                alreadySeen.Add(memoryDump);
                debugger.Redistribute();
                redistributionCount++;
                memoryDump = debugger.ToString();
            }
            var loopSize = alreadySeen.Count - Array.IndexOf(alreadySeen.ToArray(), memoryDump);
            if (logEachStep)
            {
                Console.WriteLine(memoryDump + " has been seen before");
            }
            return (steps: redistributionCount, loopSize: loopSize);
        }
    }
}
