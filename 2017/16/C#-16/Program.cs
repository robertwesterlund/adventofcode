using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__16.Program;
using System.Threading.Tasks;

namespace C__16
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
            Run2(new char[] { 'a', 'b', 'c', 'd', 'e' }, data, logToConsole: true);
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
            var progs = Enumerable.Range((int)'a', (int)'p' - (int)'a' + 1)
                .Select(c => (char)c)
                .ToArray();
            Run2(progs, GetRealData(), logToConsole: false);
        }

        private static IDanceInstruction[] Parse(Lineup lineup, string line)
        {
            return line.Split(",")
                .Select(inst =>
                {
                    switch (inst[0])
                    {
                        case 's':
                            return (IDanceInstruction)new Spin(lineup, int.Parse(inst.Substring(1)));
                        case 'x':
                            var split = inst.Substring(1).Split("/");
                            return new Exchange(lineup, int.Parse(split[0]), int.Parse(split[1]));
                        case 'p':
                            return new Partner(lineup, inst[1], inst[3]);
                        default:
                            throw new Exception($"I have no idea what to do with {inst}");
                    }
                }).ToArray();
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static BaseMove Parse2(string instruction)
        {
            switch (instruction[0])
            {
                case 's':
                    return new Spin2 { Steps = int.Parse(instruction.Substring(1)) };
                case 'x':
                    var split = instruction.Substring(1).Split("/");
                    return new Exchange2 { Index1 = int.Parse(split[0]), Index2 = int.Parse(split[1]) };
                case 'p':
                    return new Partner2 { Name1Value = getValueForName(instruction[1]), Name2Value = getValueForName(instruction[3]) };
                default:
                    throw new Exception($"I have no idea what to do with {instruction}");
            }
        }

        private abstract class BaseMove { }
        private class Spin2 : BaseMove { public int Steps { get; set; } }
        private class Exchange2 : BaseMove { public int Index1 { get; set; } public int Index2 { get; set; } }
        private class Partner2 : BaseMove { public int Name1Value { get; set; } public int Name2Value { get; set; } }

        private static int getValueForName(char name) => ((int)name) - ((int)'a');
        private static char GetNameForValue(int value) => (char)(((int)'a') + value);

        private static void Run2(char[] progs, string[] input, bool logToConsole)
        {
            _shouldLogToConsole = logToConsole;
            var dancesDone = new Dictionary<string, string>();
            var instructions = input[0].Split(',').Select(Parse2).ToArray();
            var nameToIndex = progs.Select(getValueForName).ToArray();
            Log("NameToIndex: " + string.Join(", ", nameToIndex));
            var indexToName = Enumerable.Range(0, progs.Length).ToArray();
            Log("IndexToName: " + string.Join(", ", indexToName));
            var numberOfDancers = progs.Length;
            string state = null;
            const int NumberOfIterations = 1_000_000_000;
            for (var iteration = 1; iteration <= NumberOfIterations; iteration++)
            {
                if (iteration % 10_000_000 == 0)
                {
                    Console.WriteLine(iteration);
                }
                if (state != null)
                {
                    state = dancesDone[state];
                    continue;
                }
                var startState = string.Join("", indexToName.Select(GetNameForValue));
                if (dancesDone.ContainsKey(startState))
                {
                    //Since we now have a loop, let's jump ahead in the iterations
                    var iterationsPerLoop = iteration - 1;
                    int loopsToJumpOver = (int)Math.Floor(((decimal)NumberOfIterations / iterationsPerLoop) - 1);
                    if (NumberOfIterations % iterationsPerLoop == 0){
                        loopsToJumpOver--;
                    }
                    Console.WriteLine("Index before jump: " + iteration);
                    iteration += loopsToJumpOver * iterationsPerLoop;
                    Console.WriteLine("Index after jump: " + iteration);
                    state = dancesDone[startState];
                }
                foreach (var inst in instructions)
                {
                    switch (inst)
                    {
                        case Spin2 spin:
                            for (var i = 0; i < numberOfDancers; i++)
                            {
                                nameToIndex[i] = (nameToIndex[i] + spin.Steps) % numberOfDancers;
                                indexToName[nameToIndex[i]] = i;
                            }
                            break;
                        case Exchange2 exchange:
                            var first = indexToName[exchange.Index1];
                            var second = indexToName[exchange.Index2];
                            nameToIndex[first] = exchange.Index2;
                            nameToIndex[second] = exchange.Index1;
                            indexToName[exchange.Index1] = second;
                            indexToName[exchange.Index2] = first;
                            break;
                        case Partner2 partner:
                            var firstIndex = nameToIndex[partner.Name1Value];
                            var secondIndex = nameToIndex[partner.Name2Value];
                            indexToName[firstIndex] = partner.Name2Value;
                            indexToName[secondIndex] = partner.Name1Value;
                            nameToIndex[partner.Name1Value] = secondIndex;
                            nameToIndex[partner.Name2Value] = firstIndex;
                            break;
                    }
                }
                var endState = string.Join("", indexToName.Select(GetNameForValue));
                dancesDone[startState] = endState;
            }
            if (state == null)
            {
                Console.WriteLine("Result: " + string.Join("", indexToName.Select(GetNameForValue)));
            }
            else
            {
                Console.WriteLine("Endstate: " + state);
            }
        }

        private static void Run(char[] progs, string[] input, bool logToConsole)
        {
            var lineup = new Lineup(progs);
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(lineup, l)).ToArray();
            _shouldLogToConsole = false;

            var dance = data[0];
            const int iterations = 1_000_000_000;
            for (var i = 0; i < iterations; i++)
            {
                if ((i % 1_000_000) == 0)
                {
                    System.Console.WriteLine(i);
                }
                foreach (var instruction in dance)
                {
                    instruction.Dance();
                }
            }
            var result = lineup.Dancers.OrderBy(d => d.Position).Select(d => d.Name).ToArray();

            Console.WriteLine(string.Join("", result));
        }
    }

    public interface IDanceInstruction
    {
        void Dance();
    }

    public class Spin : IDanceInstruction
    {
        public Lineup Lineup { get; }
        public int Steps { get; }
        public Spin(Lineup lineup, int steps)
        {
            this.Lineup = lineup;
            this.Steps = steps;
        }

        public void Dance()
        {
            var numberOfDancers = Lineup.Dancers.Length;
            foreach (var dancer in Lineup.Dancers)
            {
                dancer.Position = (dancer.Position + Steps) % numberOfDancers;
            }
        }
    }

    public class Exchange : IDanceInstruction
    {
        public Exchange(Lineup lineup, int position1, int position2)
        {
            this.Position1 = position1;
            this.Position2 = position2;
            this.Lineup = lineup;
        }

        public int Position1 { get; }
        public int Position2 { get; }
        public Lineup Lineup { get; }

        public void Dance()
        {
            var dancersFound = new Dancer[2];
            foreach (var dancer in Lineup.Dancers)
            {
                if (dancer.Position == Position1)
                {
                    dancersFound[0] = dancer;
                    if (dancersFound[1] != null)
                    {
                        break;
                    }
                }
                else if (dancer.Position == Position2)
                {
                    dancersFound[1] = dancer;
                    if (dancersFound[0] != null)
                    {
                        break;
                    }
                }
            }
            dancersFound[0].Swap(dancersFound[1]);
        }
    }

    public class Partner : IDanceInstruction
    {
        public Partner(Lineup lineup, char first, char second)
        {
            this.First = lineup.Dancers.First(d => d.Name == first);
            this.Second = lineup.Dancers.First(d => d.Name == second);
        }

        public Dancer First { get; }
        public Dancer Second { get; }

        public void Dance()
        {
            First.Swap(Second);
        }
    }

    public class Lineup
    {
        public Lineup(char[] dancers)
        {
            this.Dancers = dancers.Select((name, index) => new Dancer(name, index)).ToArray();
        }

        public Dancer[] Dancers { get; set; }
    }

    public class Dancer
    {
        public char Name { get; set; }
        public int Position { get; set; }

        public Dancer(char name, int index)
        {
            this.Name = name;
            this.Position = index;
        }
    }

    public static class Extensions
    {
        public static void Swap(this Dancer first, Dancer second)
        {
            var temp = first.Position;
            first.Position = second.Position;
            second.Position = temp;
        }
    }
}
