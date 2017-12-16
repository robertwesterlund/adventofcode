using System;
using System.Linq;
using System.Collections.Generic;
using static C__10.Program;

namespace C__10
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunTest();
            RunRealThing();
        }



        private static void RunTest()
        {
            Console.WriteLine("-----------");
            Console.WriteLine("Test");
            Run(GetTestData(), logToConsole: true);
        }

        private static string[] GetTestData() => System.IO.File.ReadAllLines("testdata.txt");

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
        private static int[] Parse(string line)
        {
            return line.ToCharArray()
                .SelectMany(l => System.Text.Encoding.ASCII.GetBytes(l.ToString()))
                .Select(b => (int)b)
                .Concat(new int[]{17, 31, 73, 47, 23})
                .ToArray();
        }

        private static void Run(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
            _shouldLogToConsole = logToConsole;

            var hasher = new Hasher()
            {
                Numbers = Enumerable.Range(0, 256).ToArray(),
                CurrentPosition = 0,
                SkipSize = 0,
                Lengths = data[0]
            };
            for(var i = 0; i < 64;i++){
                hasher.HashStuff();
            }
            System.Console.WriteLine(string.Join(", ", hasher.Numbers));
            Console.WriteLine("Multiplication yields: " + (hasher.Numbers[0] * hasher.Numbers[1]));
            System.Console.WriteLine("Dense Hash Length: " + hasher.GetDenseHash().Length);
            System.Console.WriteLine(hasher.GetDenseHashInHex());
        }

    }

    public class Hasher
    {
        public int[] Numbers { get; set; }
        public int CurrentPosition { get; set; } = 0;
        public int SkipSize { get; set; } = 0;
        public int[] Lengths { get; set; }

        public void HashStuff()
        {
            foreach (var length in Lengths)
            {
                //System.Console.WriteLine(string.Join(", ", Numbers) + ", moving " + length);
                for (var i = 0; i < length / 2; i++)
                {
                    var rightPos = (CurrentPosition + length - i - 1) % Numbers.Length;
                    var leftPos = (CurrentPosition + i) % Numbers.Length;
                    var temp = Numbers[rightPos];
                    Numbers[rightPos] = Numbers[leftPos];
                    Numbers[leftPos] = temp;
                }
                CurrentPosition += length + SkipSize;
                SkipSize++;
                
            }
        }

        public int[] GetDenseHash(){

            Func<int, int, int> print = (len, index) => { System.Console.WriteLine(index);return len;};

            var res = from spot in Numbers.Select((value, index) => new {Length = value, Index = index})
                    let test = print(spot.Length, spot.Index)
                   group spot by spot.Index / 16 into myGroup
                        select myGroup;
            foreach(var g in res){
                System.Console.WriteLine(g.Key);
            }

            return (from spot in Numbers.Select((value, index) => new {Length = value, Index = index})
                   group spot by spot.Index / 16 into myGroup
                   select myGroup.Select(g => g.Length).Aggregate((prev, curr) => prev ^ curr)).ToArray();
        }

        public string GetDenseHashInHex(){
            return BitConverter.ToString(GetDenseHash().Select(b => (byte)b).ToArray()).Replace("-","");
        }
    }

    public class NumberList
    {
        public int[] Numbers { get; set; }
    }
}
