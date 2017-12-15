using System;
using System.Linq;
using System.Collections.Generic;
using static C_.Program;

namespace C_
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
            Run(@"b inc 5 if a > 1
a inc 1 if b < 5
c dec -10 if a >= 1
c inc -20 if c == 10".Split("\n"), logToConsole: true);
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

        private static Signal Parse(string line)
        {
            var split = line.Split(" ");
            return new Signal()
            {
                Affects = split[0],
                Direction = split[1],
                Value = int.Parse(split[2]),
                Condition = new Condition()
                {
                    Variable = split[4],
                    Comparison = split[5],
                    Value = int.Parse(split[6])
                }
            };
        }

        private static void Run(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
            _shouldLogToConsole = logToConsole;

            Dictionary<string, int> state = new Dictionary<string, int>();
            int highestEver = 0;
            foreach(var line in data){
                line.Execute(state);
                if (_shouldLogToConsole){
                    Log(string.Join(", ", state.Keys.Select(k => $"{k} = {state[k]}")));
                }
                if (state.Any()){
                    highestEver = Math.Max(highestEver, state.Values.Max());
                }
            }
            var max = state.Values.Max();
            Console.WriteLine($"Max value is: " + max);
            Console.WriteLine($"Highest value ever is: " + highestEver);
        }
    }

    public class Signal
    {
        public string Affects { get; set; }
        public string Direction { get; set; }
        public int Value { get; set; }
        public Condition Condition { get; set; }

        public void Execute(Dictionary<string, int> state){
            if (Condition.Test(state)){
                var varVal = 0;
                if (state.ContainsKey(Affects)){
                    varVal = state[Affects];
                }
                if (Direction == "inc"){
                    varVal += Value;
                }
                else{
                    varVal -= Value;
                }
                state[Affects] = varVal;
            }
        }
    }

    public class Condition
    {
        public string Variable { get; set; }
        public string Comparison { get; set; }
        public int Value { get; set; }

        public bool Test(Dictionary<string, int> state)
        {
            var varVal = 0;
            if (state.ContainsKey(Variable))
            {
                varVal = state[Variable];
            }
            switch (Comparison)
            {
                case "<":
                    return varVal < Value;
                case ">":
                    return varVal > Value;
                case "==":
                    return varVal == Value;
                case "!=":
                    return varVal != Value;
                case ">=":
                    return varVal >= Value;
                case "<=":
                    return varVal <= Value;
                default:
                    throw new Exception(Comparison + " is not supported");
            }
        }
    }
}
