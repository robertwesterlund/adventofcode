using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__14.Program;

namespace C__14
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

        private static string Parse(string line)
        {
            return line;
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
            _shouldLogToConsole = logToConsole;

            var key = data[0];
            var knots = new string[128];
            for (var i = 0; i < 128; i++)
            {
                knots[i] = Hasher.GetKnotHashInHex($"{key}-{i}");
            }

            var bits = new bool[128][];
            for (var i = 0; i < 128; i++)
            {
                bits[i] = GetBoolArrayWithHighBitFirst(knots[i]);
            }

            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    if (bits[i][j])
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }

            var currentRegionNumber = 0;
            var regionMap = new int[128][];
            for (var i = 0; i < 128; i++)
            {
                regionMap[i] = new int[128];
                Array.Fill(regionMap[i], 0);
            }
            for (var i = 0; i < bits.Length; i++)
            {
                for (var j = 0; j < bits[i].Length; j++)
                {
                    if (bits[i][j] && regionMap[i][j] == 0)
                    {
                        currentRegionNumber++;
                        CreateRegion(bits, regionMap, currentRegionNumber, i, j);
                    }
                }
            }
            for(var i = 0; i  <8; i++){
                for (var j = 0; j < 8; j++){
                    if (regionMap[i][j] == 0){
                        Console.Write(".");
                    }
                    else{
                        Console.Write(regionMap[i][j]);
                    }
                }
                Console.WriteLine();
            }

            var used = bits.Sum(b => b.Count(isUsed => isUsed));
            Console.WriteLine($"The number of used squares is: {used}");
            Console.WriteLine($"The number of regions identified is: {currentRegionNumber}");
        }

        public static void CreateRegion(bool[][] bits, int[][] regionMap, int regionNumber, int rowIndex, int columnIndex)
        {
            if (regionMap[rowIndex][columnIndex] == regionNumber){
                return;
            }
            if (regionMap[rowIndex][columnIndex] != 0){
                throw new Exception($"We're trying to create a region where the item is already in a region... (group index was: {regionMap[rowIndex][columnIndex]}, wanted to set {regionNumber})");
            }
            regionMap[rowIndex][columnIndex] = regionNumber;
            if (rowIndex > 0 && bits[rowIndex-1][columnIndex])
            {
                CreateRegion(bits, regionMap, regionNumber, rowIndex -1, columnIndex);
            }
            if (bits.Length > rowIndex +1 && bits[rowIndex+1][columnIndex])
            {
                CreateRegion(bits, regionMap, regionNumber, rowIndex +1, columnIndex);
            }
            if (columnIndex > 0 && bits[rowIndex][columnIndex-1])
            {
                CreateRegion(bits, regionMap, regionNumber, rowIndex, columnIndex-1);
            }
            if (bits[rowIndex].Length > columnIndex + 1 && bits[rowIndex][columnIndex+1])
            {
                CreateRegion(bits, regionMap, regionNumber, rowIndex, columnIndex+1);
            }
        }

        public static byte[] ConvertHexToByteArray(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static BitArray GetBitArray(string hex)
        {
            return new BitArray(ConvertHexToByteArray(hex));
        }

        public static bool[] GetBoolArrayWithHighBitFirst(string hex)
        {
            var bitArray = GetBitArray(hex);
            bool[] bits = new bool[bitArray.Length];
            for (var i = 0; i < bitArray.Length; i += 8)
            {
                for (var j = 0; j < 8; j++)
                {
                    bits[i + j] = bitArray[i + 7 - j];
                }
            }
            return bits;
        }
    }


    public class MyObject
    {
        public MyObject()
        {
        }
    }

    public class Hasher
    {
        public static string GetKnotHashInHex(string input)
        {
            var hasher = new Hasher()
            {
                Numbers = Enumerable.Range(0, 256).ToArray(),
                Lengths = input.ToCharArray().Select(c => (byte)c).Concat(new byte[] { 17, 31, 73, 47, 23 }).Select(c => (int)c).ToArray(),
                CurrentPosition = 0,
                SkipSize = 0
            };
            for (var i = 0; i < 64; i++)
            {
                hasher.HashStuff();
            }
            return hasher.GetDenseHashInHex();
        }

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

        public int[] GetDenseHash()
        {

            Func<int, int, int> print = (len, index) => { System.Console.WriteLine(index); return len; };

            var res = from spot in Numbers.Select((value, index) => new { Length = value, Index = index })
                      let test = print(spot.Length, spot.Index)
                      group spot by spot.Index / 16 into myGroup
                      select myGroup;
            // foreach(var g in res){
            //     System.Console.WriteLine(g.Key);
            // }

            return (from spot in Numbers.Select((value, index) => new { Length = value, Index = index })
                    group spot by spot.Index / 16 into myGroup
                    select myGroup.Select(g => g.Length).Aggregate((prev, curr) => prev ^ curr)).ToArray();
        }

        public string GetDenseHashInHex()
        {
            return BitConverter.ToString(GetDenseHash().Select(b => (byte)b).ToArray()).Replace("-", "");
        }
    }
}
