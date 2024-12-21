using System.Diagnostics;
using Coordinate = AoC.Utilities.Coordinate<int>;

namespace Solution;

public partial class Program
{
    private static readonly List<(string, int)> inputFiles = [("input/example1.txt", 50), ("input/input.txt", 100)];


    public static void Main(string[] _)
    {
        foreach (var (file, cutoff) in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input1 = ParseFile(file);

            var startTime = Stopwatch.GetTimestamp();

            var answer1 = CalculateAnswer1(input1, cutoff);
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"{file}: Answer 1: {answer1} ({elapsedTime.TotalMilliseconds}ms)");
            Console.WriteLine();

            var input2 = ParseFile(file);

            startTime = Stopwatch.GetTimestamp();
            var answer2 = CalculateAnswer2(input2, cutoff);
            elapsedTime = Stopwatch.GetElapsedTime(startTime);

            Console.WriteLine($"{file}: Answer 2: {answer2} ({elapsedTime.TotalMilliseconds}ms)");
        }
    }

    private static Input ParseFile(string file)
    {
        return new Input(File.ReadAllText(file));
    }

    public static int CalculateAnswer1(Input input, int cutoff)
    {
        return Calculate(input, cutoff, 2);
    }

    private static int Calculate(Input input, int cutoff, int maxShortcut)
    {
        var fromStartDistances = CalculateDistances(input, input.Start, input.End);
        var fromEndDistances = CalculateDistances(input, input.End, input.Start);

        var shortest = fromStartDistances.Values.Min();

        var distances = fromStartDistances.Keys
            .ToDictionary(k => k, k => int.Min(fromStartDistances[k], shortest - fromEndDistances[k]));

        var cheats = new Dictionary<(Coordinate cheatStart, Coordinate cheatEnd), int>();

        var cheatTargets = Enumerable.Range(-maxShortcut, maxShortcut * 2 + 1).SelectMany(
            y => Enumerable.Range(-maxShortcut, maxShortcut * 2 + 1).Select(x => new Coordinate(x, y))
            ).Select(p => (coord: p, distance: p.DistanceTo(Coordinate.Zero)))
            .Where(p => p.distance > 1 && p.distance <= maxShortcut)
            .Select(p => p.coord)
            .ToList();

        foreach (var point in distances)
        {
            foreach (var cheatTargetNormal in cheatTargets)
            {
                var cheatTarget = point.Key + cheatTargetNormal;

                if (!fromEndDistances.ContainsKey(cheatTarget))
                {
                    continue;
                }

                if (cheats.ContainsKey((point.Key, cheatTarget)) || cheats.ContainsKey((cheatTarget, point.Key)))
                {
                    continue;
                }

                var startDistance = distances[point.Key];
                var endDistance = distances[cheatTarget];

                if (endDistance - startDistance <= 2)
                {
                    // no point in cheating if it's not shorter
                    continue;
                }

                cheats[(point.Key, cheatTarget)] = endDistance - startDistance - cheatTargetNormal.DistanceTo(Coordinate.Zero); // + two for cheat ticks
            }
        }


        var cheatsBy = cheats.GroupBy(cheat => cheat.Value).OrderBy(group => group.Key);
        if (cheatsBy.Count() < 1000)
        {
            foreach (var c in cheatsBy)
            {
                Console.WriteLine($"{c.Count()} x {c.Key}");
            }
        }
        return cheats.Values.Count(x => x >= cutoff);
    }

    private static Dictionary<Coordinate, int> CalculateDistances(Input input, Coordinate origin, Coordinate target)
    {
        var visited = new Dictionary<Coordinate, int>();
        var queue = new Queue<(Coordinate, int)>();

        queue.Enqueue((origin, 0));

        while (queue.TryDequeue(out var result))
        {
            var (current, distance) = result;

            if (visited.TryGetValue(current, out var currentDistance) && currentDistance <= distance)
            {
                continue;
            }

            visited[current] = distance;

            foreach (var direction in Coordinate.Offsets)
            {
                var next = current + direction;
                if (input.Map.Contains(next))
                {
                    continue;
                }
                queue.Enqueue((next, distance + 1));
            }
        }

        return visited;
    }

    public static long CalculateAnswer2(Input input, int cutoff)
    {
        return Calculate(input, cutoff, 20);

    }

}

public readonly struct Input
{
    public int Height { get; }
    public int Width { get; }

    public Input(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Height = lines.Length;
        Width = lines[0].Length;

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (lines[y][x] == 'S')
                {
                    Start = new Coordinate(x, y);
                }
                else if (lines[y][x] == 'E')
                {
                    End = new Coordinate(x, y);
                }
                else if (lines[y][x] != '.')
                {
                    Map.Add(new Coordinate(x, y));
                }
            }

        }
    }

    public readonly HashSet<Coordinate> Map = [];
    public readonly Coordinate Start = default;
    public readonly Coordinate End = default;
}
