using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _11
{
    internal class Program
    {
        private static void Main()
        {
            string filePath = GetInputFilePath();

            var instructions = File.ReadAllLines(filePath);

            Measure(() => Part1(instructions));

            Measure(() => Part2(instructions));
        }

        #region Part 1

        private static void Part1(string[] lines)
        {
            Dictionary<(int, int, int), int> tiles = Calculate(lines);

            var black = tiles.Values.Count(t => t % 2 == 1);
            var white = tiles.Values.Count(t => t % 2 == 0);

            Console.WriteLine($"Blacks: {black}, white: {white} total tiles: {tiles.Count}");
        }

        private static Dictionary<(int, int, int), int> Calculate(string[] lines)
        {
            var tiles = new Dictionary<(int, int, int), int>();

            foreach (var line in lines)
            {
                var input = Parse(line);

                var endPosition = input.Aggregate(
                    (0, 0, 0),
                    (acc, direction) => (acc.Item1 + direction.Item1, acc.Item2 + direction.Item2, acc.Item3 + direction.Item3));

                var flipped = 1;

                if (tiles.TryGetValue(endPosition, out var alreadyflipped))
                {
                    flipped += alreadyflipped;
                }
                Console.WriteLine($"Flipped: {flipped}");
                tiles[endPosition] = flipped;
            }

            return tiles;
        }

        private static IEnumerable<(int, int, int)> Parse(string input)
        {
            for (var pos = 0; pos < input.Length; pos++)
            {
                var lookup = input.Substring(pos, pos < (input.Length - 1) ? 2 : 1);

                if (directions.TryGetValue(lookup, out var dir))
                {
                    yield return dir;
                    pos++;
                }
                else if (directions.TryGetValue(lookup[0].ToString(), out var dir2))
                {
                    yield return dir2;
                }
                else
                {
                    throw new Exception($"Lookup not found: {lookup}");
                }
            }
        }

        private static readonly Dictionary<string, (int, int, int)> directions = new Dictionary<string, (int, int, int)>()
        {
            {  "e" ,  (-1,1,0) },
            {  "se" , (-1,0,1) },
            {  "sw" , (0,-1,1) },
            {  "w" ,  (1,-1,0) },
            {  "nw" , (1,0,-1) },
            {  "ne" , (0,1,-1) },
        };

        private static readonly (int, int, int)[] neighbours = directions.Values.ToArray();

        private record Direction(int X, int Y, int Z);

        #endregion Part 1

        #region Part 2

        private static void Part2(string[] lines)
        {
            Dictionary<(int, int, int), bool> tiles = Calculate(lines).Where(t => (t.Value % 2) == 1).ToDictionary(kvp => kvp.Key, kvp => true);

            for (var round = 1; round <= 100; round++)
            {
                tiles = CalculateNext(tiles);
                Console.WriteLine($"Day {round}: {tiles.Values.Count(t => t)}");
            }
        }

        public static Dictionary<(int, int, int), bool> CalculateNext(Dictionary<(int, int, int), bool> current)
        {
            var next = new Dictionary<(int, int, int), bool>();
            foreach (var tile in current)
            {
                var tilePosition = tile.Key;

                if (!next.ContainsKey(tilePosition))
                {
                    next[tilePosition] = Calculate(current, tilePosition);
                }

                foreach (var neighbour in neighbours)
                {
                    var location = (
                        tilePosition.Item1 + neighbour.Item1,
                        tilePosition.Item2 + neighbour.Item2,
                        tilePosition.Item3 + neighbour.Item3);

                    if (!next.ContainsKey(location))
                    {
                        next[location] = Calculate(current, location);
                    }
                }
            }
            return next;
        }

        public static bool Calculate(Dictionary<(int, int, int), bool> current, (int, int, int) location)
        {
            var black = neighbours.Select(
                neighbour => (
                    location.Item1 + neighbour.Item1,
                    location.Item2 + neighbour.Item2,
                    location.Item3 + neighbour.Item3)
                ).Select(l => current.TryGetValue(l, out var isBlack) ? (isBlack ? 1 : 0) : 0).Sum();

            current.TryGetValue(location, out var isBlack);

            return isBlack ? ((black == 0 || black > 2) ? false : true) : (black == 2);
        }

        #endregion Part 2

        private static string GetInputFilePath()
        {
            string path = Directory.GetCurrentDirectory();
            var filePath = path + "\\input.txt";
            while (!File.Exists(filePath))
            {
                var dirInfo = Directory.GetParent(path);

                if (dirInfo?.Exists ?? false)
                {
                    path = dirInfo.FullName;
                    filePath = path + "\\input.txt";
                }
                else
                {
                    throw new Exception($"Path not found {path}");
                }
            }

            return filePath;
        }

        public static void Measure(Action action)
        {
            var sw = new Stopwatch();
            Console.WriteLine($"Start");
            sw.Start();
            action();
            sw.Stop();
            Console.WriteLine($"Ended in {sw.ElapsedMilliseconds}ms");
        }
    }
}