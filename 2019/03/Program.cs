using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace _03
{
    class Program
    {
        static void Main(string[] args)
        {
            Draw("sample1.txt");
            Run("sample1.txt");
            Run("sample2.txt");
            Run("sample3.txt");
            Draw("custom.txt");
            Run("custom.txt");
            Run("input.txt");
            //Draw("input.txt", writeToFile: true);
        }

        static void Draw(string filename, bool writeToFile = false)
        {
            var content = File.ReadAllLines(filename);
            var wire1 = new Wire(content[0].Split(','));
            var wire2 = new Wire(content[1].Split(','));
            var origo = new Point(0, 0);
            var intersections = wire1.GetIntersections(wire2).Where(i => i != origo).ToArray();
            var top = wire1.Lines.Concat(wire2.Lines).SelectMany(l => new[] { l.Start.Y, l.End.Y }).Max() + 1;
            var bottom = wire1.Lines.Concat(wire2.Lines).SelectMany(l => new[] { l.Start.Y, l.End.Y }).Min() - 1;
            var left = wire1.Lines.Concat(wire2.Lines).SelectMany(l => new[] { l.Start.X, l.End.X }).Min() - 1;
            var right = wire1.Lines.Concat(wire2.Lines).SelectMany(l => new[] { l.Start.X, l.End.X }).Max() + 1;
            var chars = new char[Math.Abs(left) + right + 1, Math.Abs(bottom) + top + 1];
            for (var x = 0; x < chars.GetLength(0); x++) for (var y = 0; y < chars.GetLength(1); y++) chars[x, y] = '.';
            var xOffset = -left;
            var yOffset = top;
            Action<Point, char> setCharAt = (p, c) => chars[p.X + xOffset, -p.Y + yOffset] = c;
            foreach (var line in wire1.Lines.Concat(wire2.Lines))
            {
                var overlappingPoints = line.OverlappingLines.Select(l => Wire.GetIntersectionPoint(l, line));
                for (var x = line.LowestPoint.X; x <= line.HighestPoint.X; x++)
                    for (var y = line.LowestPoint.Y; y <= line.HighestPoint.Y; y++)
                    {
                        char c;
                        var point = new Point(x, y);
                        if (intersections.Contains(point)) c = 'X';
                        else if (overlappingPoints.Contains(point)) c = '+';
                        else if (point == line.Start || point == line.End) c = '+';
                        else if (line.Orientation == Orientation.Vertical) c = '|';
                        else if (line.Orientation == Orientation.Horizontal) c = '-';
                        else throw new Exception("Haven't handled this case");
                        setCharAt(point, c);
                    }
            }
            setCharAt(origo, 'o');
            Action<char> writer = c => Console.Write(c);
            StreamWriter mapWriter = null;
            if (writeToFile)
            {
                mapWriter = File.CreateText(filename + ".map.txt");
                writer = c => mapWriter.Write(c);
            }
            for (var y = 0; y < chars.GetLength(1); y++)
            {
                for (var x = 0; x < chars.GetLength(0); x++)
                {
                    writer(chars[x, y]);
                }
                writer('\n');
            }
        }

        static void Run(string filename)
        {
            var content = File.ReadAllLines(filename);
            var wire1 = new Wire(content[0].Split(','));
            var wire2 = new Wire(content[1].Split(','));
            var origo = new Point(0, 0);
            var intersections = wire1.GetIntersections(wire2).Where(i => i != origo).ToArray();
            var manhattanDistanceFromCentralPort = intersections
                .Select(p => GetManhattanDistance(p, origo))
                .Min();
            System.Console.WriteLine($"{filename} - Part 1: {manhattanDistanceFromCentralPort}");
            if (content.Length > 2)
            {
                System.Console.WriteLine("Success: " + (manhattanDistanceFromCentralPort.ToString() == content[2]));
            }
            var wire1CostToTargets = WalkTheLine(wire1, intersections);
            var wire2CostToTargets = WalkTheLine(wire2, intersections);
            var shortestPath = intersections
                .Select(i => wire1CostToTargets[i] + wire2CostToTargets[i])
                .Min();
            System.Console.WriteLine($"{filename} - Part 2: {shortestPath}");
            if (content.Length > 3)
            {
                System.Console.WriteLine("Success: " + (shortestPath.ToString() == content[3]));
            }
        }

        internal static int GetManhattanDistance(Point p1, Point p2) => Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);

        private static Dictionary<Point, int> WalkTheLine(Wire wire, Point[] targets)
        {
            Dictionary<Point, int> costToTarget = new Dictionary<Point, int>();
            var targetsLeft = new List<Point>(targets);
            var firstCostForIntersection = new Dictionary<Point, int>();
            var currentCost = 0;
            foreach (var line in wire.Lines)
            {
                var targetsOnLine = targetsLeft.Where(t => line.IsPointOnLine(t)).ToArray();
                foreach (var t in targetsOnLine)
                {
                    costToTarget.Add(t, firstCostForIntersection.ContainsKey(t) ? firstCostForIntersection[t] : currentCost + GetManhattanDistance(line.Start, t));
                }
                foreach (var intersection in line.OverlappingLines
                    .Where(l => line.IsBeforeLine(l, wire))
                    .Select(l => Wire.GetIntersectionPoint(line, l)))
                {
                    if (!firstCostForIntersection.ContainsKey(intersection))
                    {
                        firstCostForIntersection.Add(intersection, currentCost + GetManhattanDistance(line.Start, intersection));
                    }
                    else
                    {
                        throw new Exception("We have tried adding the same intersection twice. Something is wrong.");
                    }
                }
                currentCost = currentCost + GetManhattanDistance(line.Start, line.End);
            }
            return costToTarget;
        }
    }

    public class Wire
    {
        private List<Line> _lines = new List<Line>();
        public IEnumerable<Line> Lines { get { return _lines.AsReadOnly(); } }

        public Wire(string[] directions)
        {
            Point position = new Point(0, 0);
            foreach (var dir in directions)
            {
                var value = int.Parse(dir.Substring(1));
                var c = dir[0];
                var deltaX = c == 'R' ? value : c == 'L' ? -value : 0;
                var deltaY = c == 'U' ? value : c == 'D' ? -value : 0;
                Point end = new Point(
                    x: position.X + deltaX,
                    y: position.Y + deltaY
                );
                _lines.Add(new Line(position, end));
                position = end;
            }
            for (var i = 0; i < _lines.Count; i++)
            {
                for (var j = i + 1; j < _lines.Count; j++)
                {
                    if (_lines[i].IntersectsWith(_lines[j]))
                    {
                        var intersectionPoint = GetIntersectionPoint(_lines[i], _lines[j]);
                        if (intersectionPoint != _lines[i].Start && intersectionPoint != _lines[i].End)
                        {
                            _lines[i].OverlappingLines.Add(_lines[j]);
                            _lines[j].OverlappingLines.Add(_lines[i]);
                        }
                    }
                }
            }
        }

        public IEnumerable<Point> GetIntersections(Wire wire2)
        {
            foreach (var line1 in _lines)
            {
                foreach (var line2 in wire2._lines)
                {
                    if (line1.IntersectsWith(line2))
                    {
                        yield return GetIntersectionPoint(line1, line2);
                    }
                }
            }
        }

        public static Point GetIntersectionPoint(Line l1, Line l2)
        {
            if (l1.Orientation == Orientation.Horizontal)
            {
                return new Point(x: l2.Start.X, y: l1.Start.Y);
            }
            else
            {
                return new Point(x: l1.Start.X, y: l2.Start.Y);
            }
        }
    }

    public class Line
    {
        public Line(Point start, Point end)
        {
            this.Orientation = start.X == end.X ? Orientation.Vertical : Orientation.Horizontal;
            this.Start = start;
            this.End = end;
            this.LowestPoint = start.X < end.X || start.Y < end.Y ? start : end;
            this.HighestPoint = this.LowestPoint == start ? end : start;
        }

        public Point LowestPoint { get; }
        public Point HighestPoint { get; }
        public Point Start { get; }
        public Point End { get; }
        public Orientation Orientation { get; }

        public List<Line> OverlappingLines = new List<Line>();

        public bool IntersectsWith(Line line)
        {
            if (Orientation == line.Orientation)
            {
                return false;
            }
            if (Orientation == Orientation.Vertical)
            {
                return this.HighestPoint.Y >= line.LowestPoint.Y
                    && this.LowestPoint.Y <= line.LowestPoint.Y
                    && this.LowestPoint.X >= line.LowestPoint.X
                    && this.LowestPoint.X <= line.HighestPoint.X;
            }
            else
            {
                return this.HighestPoint.X >= line.LowestPoint.X
                    && this.LowestPoint.X <= line.LowestPoint.X
                    && this.LowestPoint.Y >= line.LowestPoint.Y
                    && this.LowestPoint.Y <= line.HighestPoint.Y;
            }
        }

        public bool IsBeforeLine(Line line, Wire wire)
        {
            foreach (var l in wire.Lines)
            {
                if (l == this)
                {
                    return true;
                }
                if (l == line)
                {
                    return false;
                }
            }
            throw new Exception("Neither line was part of the wire");
        }

        public Point FirstOnLine(Point p1, Point p2)
        {
            return Program.GetManhattanDistance(Start, p1) < Program.GetManhattanDistance(Start, p2) ? p1 : p2;
        }

        public bool IsPointOnLine(Point point)
        {
            if (this.Orientation == Orientation.Vertical)
            {
                return point.X == this.LowestPoint.X
                    && point.Y >= this.LowestPoint.Y
                    && point.Y <= this.HighestPoint.Y;
            }
            else
            {
                return point.Y == this.LowestPoint.Y
                    && point.X >= this.LowestPoint.X
                    && point.X <= this.HighestPoint.X;
            }
        }
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }
}
