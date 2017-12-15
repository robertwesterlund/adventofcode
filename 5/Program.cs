using System;
using System.Linq;
using System.Collections.Generic;

namespace _5
{
    class Program
    {
        static void Main(string[] args)
        {
            RunThroughRealMaze(Maze.StepChangeStrategies.IncrementIfBelowThreeOtherwiseDecrement);
        }

        public static void RunThroughTestMaze(Func<int, int> stepChangeStrategy)
        {
            var maze = new Maze(new[] { 0, 3, 0, 1, -3 });
            maze.RunThroughMaze(stepChangeStrategy, printMazeStateAfterEachStep: true);
        }

        public static void RunThroughRealMaze(Func<int, int> stepChangeStrategy)
        {
            var maze = new Maze(
                System.IO.File.ReadAllLines("mazedata.txt")
                    .Select(line => int.Parse(line))
                    .ToArray()
            );
            maze.RunThroughMaze(stepChangeStrategy, printMazeStateAfterEachStep: false);
        }
    }

    public class Maze
    {
        private LinkedList<Room> _rooms;
        public bool IsOutside { get; private set; }
        private LinkedListNode<Room> _currentRoom;

        public static class StepChangeStrategies
        {
            public static int AlwaysIncrement(int previous) => previous + 1;
            public static int IncrementIfBelowThreeOtherwiseDecrement(int previous) => previous < 3 ? previous + 1 : previous - 1;
        }

        public void Step(Func<int, int> stepChangeStrategy)
        {
            if (IsOutside)
            {
                throw new Exception("Cannot step when we're already out of the maze");
            }
            var jumps = _currentRoom.Value.Jumps;
            _currentRoom.Value.Jumps = stepChangeStrategy(jumps);
            if (jumps > 0)
            {
                for (var i = 0; i < jumps; i++)
                {
                    _currentRoom = _currentRoom.Next;
                    if (_currentRoom == null)
                    {
                        this.IsOutside = true;
                        break;
                    }
                }
            }
            else if (jumps < 0)
            {
                for (var i = 0; i > jumps; i--)
                {
                    _currentRoom = _currentRoom.Previous;
                    if (_currentRoom == null)
                    {
                        throw new Exception("I doubt we're allowed to walk backwards out of the maze");
                    }
                }
            }
        }

        public Maze(int[] maze)
        {
            this._rooms = new LinkedList<Room>(maze.Select(r => new Room(r)));
            _currentRoom = this._rooms.First;
        }


        public override string ToString()
        {
            return string.Join(" ", _rooms.Select(r => (!IsOutside && r == _currentRoom.Value) ? $"({r.Jumps})" : r.Jumps.ToString()));
        }

        public void RunThroughMaze(Func<int, int> stepChangeStrategy, bool printMazeStateAfterEachStep = false)
        {
            var numberOfStepsTaken = 0;
            while (!this.IsOutside)
            {
                numberOfStepsTaken++;
                this.Step(stepChangeStrategy);
                if (printMazeStateAfterEachStep)
                {
                    System.Console.WriteLine(this);
                }
            }
            System.Console.WriteLine($"Left the maze after {numberOfStepsTaken} steps");
        }
    }

    public class Room
    {
        public Room(int jumps)
        {
            this.Jumps = jumps;
        }
        public int Jumps { get; set; }
    }
}
