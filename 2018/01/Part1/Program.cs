using System;
using System.IO;
using System.Linq;

namespace AdventOfCode2018_01
{
    class Program
    {
        static void Main(string[] args)
        {
            var sum = File.ReadAllLines(".\\input.txt")
                .Select(int.Parse)
                .Sum();

            System.Console.WriteLine(sum);
        }
    }
}
