using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static C_.Program;

namespace C_
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

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            _shouldLogToConsole = logToConsole;
            var startStateName = input[0].Substring("Begin in state ".Length, 1)[0];
            var numberOfStepsBeforeChecksum = int.Parse(Regex.Match(input[1], @"Perform a diagnostic checksum after (?<steps>\d+) steps.").Groups["steps"].Value);
            var machine = new Machine();

            ActionDescription GetActionDescription(int offset)
            {
                var valueToSet = Regex.Match(input[offset], @"Write the value (?<value>\d)").Groups["value"].Value == "1" ? true : false;
                var moveAction = Regex.Match(input[offset + 1], @"Move one slot to the (?<direction>right|left)").Groups["direction"].Value == "right" ? (Action)machine.MoveRight : machine.MoveLeft;
                var newStateName = Regex.Match(input[offset + 2], @"Continue with state (?<statename>.)").Groups["statename"].Value[0];
                return new ActionDescription()
                {
                    NameOfNextState = newStateName,
                    SetValueAction = () => machine.SetValueAtCurrentPosition(valueToSet),
                    MoveAction = moveAction
                };
            }

            IEnumerable<State> ParseStates()
            {
                for (var offset = 3; offset < input.Length; offset += 10)
                {
                    var stateName = input[offset].Substring("In state ".Length, 1)[0];
                    yield return new State(machine)
                    {
                        Name = stateName,
                        ActionIfValueIsOff = GetActionDescription(offset + 2),
                        ActionIfValueIsOn = GetActionDescription(offset + 6)
                    };
                }
            }

            var states = ParseStates().ToDictionary(s => s.Name);

            var currentState = states[startStateName];
            for (var i = 0; i < numberOfStepsBeforeChecksum; i++)
            {
                var newStateName = currentState.Execute();
                currentState = states[newStateName];
            }

            Console.WriteLine("Number of set slots " +  machine.CountOfSetStates);
        }
    }

    public class Machine
    {
        private HashSet<long> _statesSetToAOne = new HashSet<long>();

        public int CountOfSetStates => _statesSetToAOne.Count;
        public bool GetValueAtIndex(long index)
        {
            return _statesSetToAOne.Contains(index) ? true : false;
        }
        public void SetValueAtIndex(long index, bool value)
        {
            if (value)
            {
                _statesSetToAOne.Add(index);
            }
            else
            {
                _statesSetToAOne.Remove(index);
            }
        }
        public void SetValueAtCurrentPosition(bool value)
        {
            SetValueAtIndex(CurrentPosition, value);
        }

        public bool GetValueAtCurrentPosition()
        {
            return GetValueAtIndex(CurrentPosition);
        }

        public long CurrentPosition { get; private set; } = 0;

        public void MoveLeft()
        {
            CurrentPosition--;
        }

        public void MoveRight()
        {
            CurrentPosition++;
        }
    }

    public class State
    {
        public State(Machine machine)
        {
            this.Machine = machine;
        }

        public char Name { get; set; }
        public ActionDescription ActionIfValueIsOff { get; set; }
        public ActionDescription ActionIfValueIsOn { get; set; }
        public Machine Machine { get; }

        public char Execute()
        {
            var currentValue = Machine.GetValueAtCurrentPosition();
            if (currentValue)
            {
                return ActionIfValueIsOn.Execute();
            }
            else
            {
                return ActionIfValueIsOff.Execute();
            }
        }
    }

    public class ActionDescription
    {
        public Action SetValueAction { get; set; }
        public Action MoveAction { get; set; }
        public char NameOfNextState { get; set; }

        public char Execute()
        {
            SetValueAction();
            MoveAction();
            return NameOfNextState;
        }
    }
}
