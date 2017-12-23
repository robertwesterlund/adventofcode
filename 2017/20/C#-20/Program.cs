using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
            Run2(data, logToConsole: true);
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
            Run2(GetRealData(), logToConsole: false);
        }
        private static Particle Parse(string line, int index)
        {
            var split = line.Split(">,");
            Vector3D getVector(string data)
            {
                data = data.Trim('p', 'a', 'v', ' ', '=', '<', '>');
                var splitData = data.Split(",");
                return new Vector3D()
                {
                    X = long.Parse(splitData[0]),
                    Y = long.Parse(splitData[1]),
                    Z = long.Parse(splitData[2]),
                };
            }
            return new Particle
            {
                Id = index,
                Position = getVector(split[0]),
                Velocity = getVector(split[1]),
                Acceleration = getVector(split[2])
            };
        }

        private static string[] GetRealData() => System.IO.File.ReadAllLines("data.txt");

        private static void Run(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select((l, index) => Parse(l, index)).ToArray();
            _shouldLogToConsole = logToConsole;

            int GetLeaderIndex(Particle[] particles)
            {
                var lowestDistance = long.MaxValue;
                var index = 0;
                for (var i = 0; i < particles.Length; i++)
                {
                    var distance = particles[i].Position.GetManhattanDistance();
                    if (distance < lowestDistance)
                    {
                        lowestDistance = distance;
                        index = i;
                    }
                }
                return index;
            }

            int leader = -1;
            long iterationsWithSameLeader = 0;
            var state = data;
            while (iterationsWithSameLeader < 1000)
            {
                iterationsWithSameLeader++;
                state = state.Select(p => p.TickVelocity()).ToArray();
                state = state.Select(p => p.TickPosition()).ToArray();
                var newLeader = GetLeaderIndex(state);
                if (newLeader != leader)
                {
                    Log("Changed leader to " + newLeader);
                    leader = newLeader;
                    iterationsWithSameLeader = 0;
                }
            }
            Console.WriteLine("Leader for the last long time was: " + leader);
        }
        private static void Run2(string[] input, bool logToConsole)
        {
            var data = input.Where(l => !string.IsNullOrWhiteSpace(l)).Select((l, index) => Parse(l, index)).ToArray();
            _shouldLogToConsole = true;

            bool HasCollided(Particle[] particles, Particle particle)
            {
                return particles.Any(p => p.Id != particle.Id && p.Position.Equals(particle.Position));
            }

            long iterationsWithoutCollisions = 0;
            var state = data;
            while (iterationsWithoutCollisions < 1000)
            {
                iterationsWithoutCollisions++;
                state = state.Select(p => p.TickVelocity()).ToArray();
                state = state.Select(p => p.TickPosition()).ToArray();
                int particlesBeforeCheck = state.Length;
                state = state.Where(p => !HasCollided(state, p)).ToArray();
                int particlesAfterCheck = state.Length;
                if (particlesAfterCheck != particlesBeforeCheck)
                {
                    Log($"Collisions detected after {iterationsWithoutCollisions} iterations without collision");
                    iterationsWithoutCollisions = 0;
                }
            }
            Console.WriteLine("Particles after all collisions: " + state.Length);
        }
    }

    public struct Particle
    {
        public int Id{get;set;}
        public Vector3D Position { get; set; }
        public Vector3D Velocity { get; set; }
        public Vector3D Acceleration { get; set; }

        public Particle TickVelocity()
        {
            return new Particle
            {
                Id = this.Id,
                Position = this.Position,
                Velocity = this.Velocity.Add(this.Acceleration),
                Acceleration = this.Acceleration
            };
        }

        public Particle TickPosition()
        {
            return new Particle
            {
                Id = this.Id,
                Position = this.Position.Add(this.Velocity),
                Velocity = this.Velocity,
                Acceleration = this.Acceleration
            };
        }
    }

    public struct Vector3D
    {
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        public Vector3D Add(Vector3D other)
        {
            return new Vector3D()
            {
                X = this.X + other.X,
                Y = this.Y + other.Y,
                Z = this.Z + other.Z
            };
        }

        public long GetManhattanDistance()
        {
            return Math.Abs(this.X) + Math.Abs(this.Y) + Math.Abs(this.Z);
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector3D)
            {
                return Equals((Vector3D)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(Vector3D other)
        {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
        }

        public override int GetHashCode()
        {
            return $"{X},{Y},{Z}".GetHashCode();
        }
    }
}
