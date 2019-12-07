using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace _02
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test("sample1.txt", "sample1.answer.txt");
            // Test("sample2.txt", "sample2.answer.txt");
            // Test("sample3.txt", "sample3.answer.txt");
            Run("Part1", "input.txt", 12, 2);
            for (var noun = 0; noun <= 99; noun++)
            {
                for (var verb = 0; verb <= 99; verb++)
                {
                    var result = GetResults("input.txt", noun, verb);
                    if (result[0] == 19690720)
                    {
                        System.Console.WriteLine("Part 2");
                        System.Console.WriteLine($"Found {noun.ToString("00")}{verb.ToString("00")}");
                        System.Console.WriteLine(string.Join(",", result));
                        return;
                    }
                }
            }
        }

        private static void Test(string filename, string answerFilename)
        {
            var opCodes = string.Join(",", GetResults(filename));
            var expected = File.ReadAllLines(answerFilename).First();
            System.Console.WriteLine(filename);
            if (opCodes == expected)
            {
                System.Console.WriteLine("Success");
            }
            else
            {
                System.Console.WriteLine($"FAILURE!");
                System.Console.WriteLine(opCodes);
                System.Console.WriteLine(expected);
            }
        }

        private static void Run(string testName, string filename, int noun, int verb)
        {
            var opCodes = GetResults(filename, noun, verb);
            System.Console.WriteLine(testName);
            System.Console.WriteLine(string.Join(",", opCodes));
        }

        private static int[] GetResults(string filename, int? noun = null, int? verb = null)
        {
            var input = File.ReadAllLines(filename).First();
            var opCodes = input.Split(",").Select(int.Parse).ToArray();
            if (noun.HasValue)
            {
                opCodes[1] = noun.Value;
            }
            if (verb.HasValue)
            {
                opCodes[2] = verb.Value;
            }
            var program = new IntOpsProgram(opCodes);
            var operation = program.GetCurrentOperation();
            while (!operation.IsStopOperation)
            {
                program.Apply(operation);
                program.HopToNextOperation();
                operation = program.GetCurrentOperation();
            }
            return opCodes;
        }
    }

    public class IntOpsProgram
    {
        private int CurrentPosition = 0;
        private int[] _opCodes;

        public IntOpsProgram(int[] opCodes)
        {
            _opCodes = opCodes;
        }

        public void HopToNextOperation()
        {
            CurrentPosition += 4;
        }

        public Operation GetCurrentOperation()
        {
            if (_opCodes[CurrentPosition] == 99)
            {
                return new Operation { OpCode = _opCodes[CurrentPosition] };
            }
            return new Operation
            {
                OpCode = _opCodes[CurrentPosition],
                Param1 = _opCodes[_opCodes[CurrentPosition + 1]],
                Param2 = _opCodes[_opCodes[CurrentPosition + 2]],
                Param3 = _opCodes[CurrentPosition + 3],
            };
        }

        public void Apply(Operation operation)
        {
            operation.Apply(_opCodes);
        }
    }

    public struct Operation
    {
        public int OpCode { get; set; }
        public int Param1 { get; set; }
        public int Param2 { get; set; }
        public int Param3 { get; set; }

        public bool IsStopOperation { get => OpCode == 99; }

        public void Apply(int[] opCodes)
        {
            Func<int, int, int> operation;
            switch (OpCode)
            {
                case 1: operation = (a, b) => a + b; break;
                case 2: operation = (a, b) => a * b; break;
                case 99: throw new ApplicationHaltException();
                default:
                    throw new InvalidOperationException($"OpCode {OpCode} is not supported");
            };
            opCodes[Param3] = operation(Param1, Param2);
        }

        public override string ToString() => $"{OpCode},{Param1},{Param2},{Param3}";
    }

    internal class ApplicationHaltException : Exception
    {
    }
}
