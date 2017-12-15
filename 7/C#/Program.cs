using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using static _7.Program;

namespace _7
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
            Run(@"
pbga (66)
xhth (57)
ebii (61)
havc (66)
ktlj (57)
fwft (72) -> ktlj, cntj, xhth
qoyq (66)
padx (45) -> pbga, havc, qoyq
tknk (41) -> ugml, padx, fwft
jptl (61)
ugml (68) -> gyxo, ebii, jptl
gyxo (61)
cntj (57)
".Split("\n"), logToConsole: true);
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

        private static void Run(string[] data, bool logToConsole)
        {
            data = data.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
            _shouldLogToConsole = logToConsole;

            Dictionary<string, Prog> dict = new Dictionary<string, Prog>();
            foreach (var line in data)
            {
                var prog = Parse(line);
                dict.Add(prog.Name, prog);
            }
            foreach (var key in dict.Keys)
            {
                Log(dict[key].ToString());
            }
            foreach (var p in dict.Values)
            {
                p.Carries = p.CarriesNames.Select(n =>
                {
                    var carried = dict[n];
                    carried.CarriedBy = p;
                    return carried;
                }).ToArray();
            }
            var bottomProgram = dict.Values.Single(p => p.CarriedBy == null);
            Console.WriteLine(bottomProgram.Name);
            // var arr = dict.Values.Where(p => !p.IsBalanced()).ToArray();
            // foreach(var u in arr){
            //     PrintUnbalanced(u);
            // }
            var unbalancedProg = dict.Values.Single(p => !p.IsBalanced() && !p.Carries.Any(child => !child.IsBalanced()));
            PrintUnbalanced(unbalancedProg);
        }

        private static void PrintUnbalanced(Prog unbalancedProg)
        {
            System.Console.WriteLine("Unbalanced Prog: " + unbalancedProg.Name + $" ({unbalancedProg.Weight})");
            foreach( var c in unbalancedProg.Carries){
                System.Console.WriteLine($"Carries: {c.Name} - tree weight: {c.GetWeightOfSubtree()} - node weight: {c.Weight}");
            }
        }

        static System.Text.RegularExpressions.Regex _regex = new System.Text.RegularExpressions.Regex(@"^(?<name>[a-z]+) \((?<weight>\d+)\)(?: -> (?<carries>[a-z, ]*))?$", System.Text.RegularExpressions.RegexOptions.Compiled);

        private static Prog Parse(string line)
        {
            var match = _regex.Match(line.Trim());
            return new Prog
            {
                Name = match.Groups["name"].Value,
                Weight = long.Parse(match.Groups["weight"].Value),
                CarriesNames = match.Groups["carries"].Success ? match.Groups["carries"].Value.Split(",").Select(s => s.Trim()).ToArray() : new string[0]
            };
        }
    }

    public class Prog
    {
        public string Name { get; set; }
        public long Weight { get; set; }
        public Prog[] Carries { get; set; }
        public Prog CarriedBy { get; set; }
        public string[] CarriesNames { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Weight}) -> {string.Join(", ", CarriesNames)}";
        }

        public long GetWeightOfSubtree()
        {
            return this.Weight + this.Carries.Sum(c => c.GetWeightOfSubtree());
        }

        public bool IsBalanced()
        {
            var subtrees = this.Carries.Select(c => new
            {
                Node = c,
                TreeWeight = c.GetWeightOfSubtree()
            }).ToArray();

            if (subtrees.Any())
            {
                if (subtrees.Any(c => c.TreeWeight != subtrees[0].TreeWeight))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
