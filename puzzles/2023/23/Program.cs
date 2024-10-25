using System.Text;

using Coordinate = AoC.Utilities.Coordinate<int>;

namespace AoC.Puzzles.Y2023.D23;

public static partial class Program
{
    private static readonly IReadOnlyDictionary<char, Coordinate> Directions = new Dictionary<char, Coordinate>
    {
        { '>', Coordinate.OffsetRight},
        { '<', Coordinate.OffsetLeft},
        { 'v', Coordinate.OffsetDown},
        { '^', Coordinate.OffsetUp},
    };

    private static void Main(string[] args)
    {
        List<string> inputFiles = [
            "input/example.txt",
            "input/input.txt"
           ];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var part1 = CalculateHike(inputs, false);

            Console.WriteLine($"Part 1: {part1}");

            var part2 = CalculateHike(inputs, true);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static int CalculateHike(string[] inputs, bool enableClimbingSteepSlopes)
    {
        var map = ParseInput(inputs);

        var path = new HashSet<Coordinate>() { new(1, 0) };

        var startPosition = new Coordinate(1, 1);

        var endPosition = new Coordinate(inputs.Length - 2, inputs.Length - 1);

        var queue = new Queue<(HashSet<Coordinate> path, Coordinate position, Coordinate lastPosition, int length)>();

        queue.Enqueue((path, startPosition, new Coordinate(1, 0), 1));

        var longestPath = new HashSet<Coordinate>();

        var longestPathCount = 0;

        while (queue.TryDequeue(out var state))
        {
            while (true)
            {
                var nextStates = new List<(HashSet<Coordinate> path, Coordinate position, Coordinate lastPosition, int length)>();

                foreach (var direction in Coordinate.Offsets)
                {
                    var next = state.position + direction;

                    if (next == state.lastPosition)
                    {
                        continue;
                    }
                    else if (next == endPosition)
                    {
                        if (state.length + 1 > longestPathCount)
                        {
                            longestPathCount = state.length + 1;
                        }
                    }
                    else if (!state.path.Contains(next))
                    {
                        var mapData = map[next];

                        if (mapData == '#')
                        {
                            // wall
                        }
                        else if (mapData == '.' || enableClimbingSteepSlopes)
                        {
                            var newPath = new HashSet<Coordinate>(state.path);
                            if (newPath.Add(next))
                            {
                                nextStates.Add((newPath, next, state.position, state.length + 1));
                            }
                        }
                        else
                        {
                            if (Directions.TryGetValue(mapData, out var requiredDirection) && direction == requiredDirection)
                            {
                                var newPosition = next + direction;
                                var newPath = new HashSet<Coordinate>(state.path)
                                {
                                    next,
                                    newPosition
                                };
                                if (!state.path.Contains(newPosition))
                                {
                                    nextStates.Add((newPath, newPosition, next, state.length + 2));
                                }
                            }
                        }
                    }
                }
                if (nextStates.Count == 0)
                {
                    break;
                }
                else if (nextStates.Count == 1)
                {
                    // just continue
                    state = (state.path, nextStates[0].position, nextStates[0].lastPosition, nextStates[0].length);
                }
                else
                {
                    // multiple options to continue
                    foreach (var nextState in nextStates)
                    {
                        if (!state.path.Contains(state.position))
                        {
                            var newPath = new HashSet<Coordinate>(state.path)
                            {
                                state.position
                            };
                            queue.Enqueue((path: newPath, nextState.position, nextState.lastPosition, nextState.length));
                        }
                    }
                    break;
                }
            }
        }

        Print(map, longestPath);

        return longestPathCount;
    }

    private static Dictionary<Coordinate, char> ParseInput(string[] inputs)
    {
        var map = new Dictionary<Coordinate, char>();

        for (int y = 0; y < inputs.Length; y++)
        {
            for (int x = 0; x < inputs[y].Length; x++)
            {
                map.Add(new Coordinate(x, y), inputs[y][x]);
            }
        }
        return map;
    }

    private static void Print(Dictionary<Coordinate, char> map, HashSet<Coordinate> path)
    {
        var maxX = map.Keys.Max(t => t.X) + 1;
        var maxY = map.Keys.Max(t => t.Y) + 1;

        var sb = new StringBuilder();

        for (var y = 0; y < maxY; y++)
        {
            for (var x = 0; x < maxX; x++)
            {
                var coordinate = new Coordinate(x, y);
                if (path.Contains(coordinate))
                {
                    sb.Append('O');
                }
                else
                {
                    sb.Append(map[coordinate]);
                }
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }
}