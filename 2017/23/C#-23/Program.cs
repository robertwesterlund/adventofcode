using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static C__23.Runner;

namespace C__23
{
    class Runner
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
                case "snd": return new SendInst(new ValueReference(split[1]));
                case "set": return new SetInst(split[1][0], new ValueReference(split[2]));
                case "sub": return new SubInst(split[1][0], new ValueReference(split[2]));
                case "add": return new AddInst(split[1][0], new ValueReference(split[2]));
                case "mul": return new MultInst(split[1][0], new ValueReference(split[2]));
                case "mod": return new ModInst(split[1][0], new ValueReference(split[2]));
                case "rcv": return new RecInst(split[1][0]);
                case "jnz":
                case "jgz": return new JumpInst(new ValueReference(split[1]), new ValueReference(split[2]));
                default: throw new Exception("Invalid instruction: " + line);
            }
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => Parse(l)).ToArray();
            _shouldLogToConsole = logToConsole;

            var program = new Program(0, data, new Queue<long>(), new Queue<long>());
            program.SetRegisterValue('a', 1);
            long counter = 0; 
            while (!program.IsFinished)
            {
                if (counter % 1_000_000 == 0){
                    Console.WriteLine("Counter " + counter + ", Value of register H is: " + program.GetRegisterValue('h'));
                }
                program.TakeOneStep();
                counter++;
            }

            Console.WriteLine("Done");
            var numberOfMulOps = program.Instructions.OfType<MultInst>().Select(i => i.NumberOfInvocations).Sum();
            Console.WriteLine("Number of mul ops: " + numberOfMulOps);
            Console.WriteLine("Value of register H is: " + program.GetRegisterValue('h'));
        }
    }



    public class Program
    {
        public Program(long programId, IInstruction[] instructions, Queue<long> readQueue, Queue<long> sendQueue)
        {
            this.ProgramId = programId;
            this.Instructions = instructions;
            this.Registers = new Dictionary<char, long>();
            this.SetRegisterValue('p', programId);
            this.ReadQueue = readQueue;
            this.SendQueue = sendQueue;
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
        public long ProgramId { get; }
        public IInstruction[] Instructions { get; set; }
        public long LastFrequencyPlayed { get; set; }
        public long CurrentIndex { get; set; } = 0;
        public bool IsFinished => CurrentIndex < 0 || CurrentIndex >= Instructions.Length;
        public bool IsWaiting { get; set; }
        public long CountOfSendOperations { get; set; }
        public Queue<long> ReadQueue { get; }
        public Queue<long> SendQueue { get; }

        public void Send(long value)
        {
            CountOfSendOperations++;
            SendQueue.Enqueue(value);
        }

        public long? Read()
        {
            if (ReadQueue.Any())
            {
                return ReadQueue.Dequeue();
            }
            else
            {
                return null;
            }
        }

        public void TakeOneStep()
        {
            var currentInstruction = Instructions[CurrentIndex];
            var indexBeforeChange = CurrentIndex;
            currentInstruction.Perform(this);
            if (CurrentIndex == indexBeforeChange)
            {
                CurrentIndex++;
            }
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

        public long GetValue(Program program)
        {
            return Value.HasValue ? Value.Value : program.GetRegisterValue(Register);
        }
    }

    public interface IInstruction
    {
        void Perform(Program program);
    }

    public class SendInst : IInstruction
    {
        public SendInst(ValueReference frequency)
        {
            this.Value = frequency;
        }

        public ValueReference Value { get; }

        public void Perform(Program synth)
        {
            synth.Send(Value.GetValue(synth));
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
        public OpInst(char register, Func<Program, long, long> op)
        {
            this.Register = register;
            this.Op = op;
        }

        public char Register { get; }
        public Func<Program, long, long> Op { get; }
        public long NumberOfInvocations { get; private set; }

        public void Perform(Program program)
        {
            NumberOfInvocations++;
            program.SetRegisterValue(Register, Op(program, program.GetRegisterValue(Register)));
        }
    }

    public class AddInst : OpInst
    {
        public AddInst(char register, ValueReference value)
        : base(register, (synth, oldValue) => oldValue + value.GetValue(synth))
        { }
    }

    public class SubInst : OpInst
    {
        public SubInst(char register, ValueReference value)
        : base(register, (synth, oldValue) => oldValue - value.GetValue(synth))
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

        public void Perform(Program program)
        {
            var received = program.Read();
            if (received.HasValue)
            {
                program.SetRegisterValue(Register, received.Value);
            }
            else
            {
                program.IsWaiting = true;
            }
        }
    }

    public class JumpInst : IInstruction
    {
        public JumpInst(ValueReference register, ValueReference jumpLength)
        {
            this.Register = register;
            this.JumpLength = jumpLength;
        }

        public ValueReference Register { get; }
        public ValueReference JumpLength { get; }

        public void Perform(Program program)
        {
            if (Register.GetValue(program) != 0)
            {
                program.CurrentIndex += JumpLength.GetValue(program);
            }
        }
    }
}
