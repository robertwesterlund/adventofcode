using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2018_01
{
    class Program
    {
        static void Main(string[] args)
        {
            var frequencyChanges = File.ReadAllLines(".\\input.txt")
                .Select(int.Parse)
                .ToArray();
            var observedFrequencies = new List<int>();
            var frequency = 0;
            for (var i = 0; i < frequencyChanges.Length; i = (i + 1) % frequencyChanges.Length)
            {
                frequency += frequencyChanges[i];
                if (observedFrequencies.Contains(frequency))
                {
                    System.Console.WriteLine(frequency);
                    return;
                }
                observedFrequencies.Add(frequency);
            }
        }
    }
}
