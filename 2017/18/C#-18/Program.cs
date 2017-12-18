using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__18.Program;

namespace C__18
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
            Run(GetRealData(), logToConsole: false);
        }

        private static IInstruction Parse(string line)
        {
            var split = line.Split(" ");
            switch (split[0])
            {
                case "snd": return new SoundInst(new ValueReference(split[1]));
                case "set": return new SetInst(split[1][0], new ValueReference(split[2]));
                case "add": return new AddInst(split[1][0], new ValueReference(split[2]));
                case "mul": return new MultInst(split[1][0], new ValueReference(split[2]));
                case "mod": return new ModInst(split[1][0], new ValueReference(split[2]));
                case "rcv": return new RecInst(split[1][0]);
                case "jgz": return new JumpInst(split[1][0], new ValueReference(split[2]));
                default: throw new Exception("Invalid instruction: " + line);
            }
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
            _shouldLogToConsole = logToConsole;

            var synth = new Synth(data);
            while (!synth.IsFinished)
            {
                var instruction = synth.TakeOneStep();
                if (instruction is RecInst recaller && recaller.RecalledValue.HasValue)
                {
                    Console.WriteLine("Recalled value is: " + recaller.RecalledValue.Value);
                    break;
                }
            }
        }
    }

    public class Synth
    {
        public Synth(IInstruction[] instructions)
        {
            this.Instructions = instructions;
            this.Registers = new Dictionary<char, long>();
        }

        public long GetRegisterValue(char key)
        {
            if (Registers.ContainsKey(key))
            {
                return Registers[key];
            }
            else
            {
                return 0;
            }
        }

        public void SetRegisterValue(char key, long value)
        {
            Registers[key] = value;
        }

        private Dictionary<char, long> Registers { get; set; }
        public IInstruction[] Instructions { get; set; }
        public long LastFrequencyPlayed { get; set; }
        public long CurrentIndex { get; set; } = 0;
        public bool IsFinished => CurrentIndex < 0 || CurrentIndex >= Instructions.Length;
        public void Play(long frequency)
        {
            LastFrequencyPlayed = frequency;
        }

        public IInstruction TakeOneStep()
        {
            var currentInstruction = Instructions[CurrentIndex];
            var indexBeforeChange = CurrentIndex;
            currentInstruction.Perform(this);
            if (CurrentIndex == indexBeforeChange)
            {
                CurrentIndex++;
            }
            return currentInstruction;
        }
    }

    public class ValueReference
    {
        public ValueReference(string str)
        {
            if (long.TryParse(str, out long val))
            {
                this.Value = val;
            }
            else
            {
                this.Register = str[0];
            }
        }

        public ValueReference(char register)
        {
            this.Register = register;
        }

        public ValueReference(long value)
        {
            this.Value = value;
        }

        public char Register { get; }
        public long? Value { get; }

        public long GetValue(Synth synth)
        {
            return Value.HasValue ? Value.Value : synth.GetRegisterValue(Register);
        }
    }

    public interface IInstruction
    {
        void Perform(Synth synth);
    }

    public class SoundInst : IInstruction
    {
        public SoundInst(ValueReference frequency)
        {
            this.Frequency = frequency;
        }

        public ValueReference Frequency { get; }

        public void Perform(Synth synth)
        {
            synth.Play(Frequency.GetValue(synth));
        }
    }

    public class SetInst : OpInst
    {
        public SetInst(char register, ValueReference value)
            : base(register, (synth, _) => value.GetValue(synth))
        { }
    }

    public class OpInst : IInstruction
    {
        public OpInst(char register, Func<Synth, long, long> op)
        {
            this.Register = register;
            this.Op = op;
        }

        public char Register { get; }
        public Func<Synth, long, long> Op { get; }

        public void Perform(Synth synth)
        {
            synth.SetRegisterValue(Register, Op(synth, synth.GetRegisterValue(Register)));
        }
    }

    public class AddInst : OpInst
    {
        public AddInst(char register, ValueReference value)
        : base(register, (synth, oldValue) => oldValue + value.GetValue(synth))
        { }
    }

    public class MultInst : OpInst
    {
        public MultInst(char register, ValueReference value)
        : base(register, (synth, oldValue) => oldValue * value.GetValue(synth))
        { }
    }

    public class ModInst : OpInst
    {
        public ModInst(char register, ValueReference value)
        : base(register, (synth, oldValue) => oldValue % value.GetValue(synth))
        { }
    }

    public class RecInst : IInstruction
    {
        public RecInst(char register)
        {
            this.Register = register;
        }

        public char Register { get; }
        public long? RecalledValue { get; set; }

        public void Perform(Synth synth)
        {
            if (synth.GetRegisterValue(Register) != 0)
            {
                this.RecalledValue = synth.LastFrequencyPlayed;
            }
        }
    }

    public class JumpInst : IInstruction
    {
        public JumpInst(char register, ValueReference jumpLength)
        {
            this.Register = register;
            this.JumpLength = jumpLength;
        }

        public char Register { get; }
        public ValueReference JumpLength { get; }

        public void Perform(Synth synth)
        {
            if (synth.GetRegisterValue(Register) > 0)
            {
                synth.CurrentIndex += JumpLength.GetValue(synth);
            }
        }
    }

    public class MyObject
    {
        public MyObject()
        {
        }
    }
}
