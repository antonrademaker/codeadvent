using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Coordinate = AoC.Utilities.Coordinate<long>;
using Direction = AoC.Utilities.Coordinate<long>;

namespace Solution;

public partial class Program
{
    private static readonly List<string> inputFiles = ["input/example1.txt", "input/example2.txt", "input/input.txt"];

    private const int MoveAheadCost = 1;
    private const int TurnCost = 1000;

    public static void Main(string[] _)
    {
        foreach (var file in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input = ParseFile(file);

            var startTime = Stopwatch.GetTimestamp();

            var answer1 = CalculateAnswer1(input);
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"{file}: Answer 1: {answer1} ({elapsedTime.TotalMilliseconds}ms)");
            Console.WriteLine();

            startTime = Stopwatch.GetTimestamp();
            var answer2 = CalculateAnswer2(input);
            elapsedTime = Stopwatch.GetElapsedTime(startTime);

            Console.WriteLine($"{file}: Answer 2: {answer2} ({elapsedTime.TotalMilliseconds}ms)");
        }
    }

    private static Input ParseFile(string file)
    {
        return new Input(File.ReadAllLines(file));
    }

    public static long CalculateAnswer1(Input input)
    {
        var (answer, _) = Calculate(ref input, input.Start, Coordinate.OffsetRight);

        return answer;
    }

    private static (long, HashSet<Coordinate>) Calculate(ref Input input, Coordinate reindeer, Direction reindeerDirection)
    {
        PriorityQueue<(Coordinate loc, Direction dir, ImmutableHashSet<Coordinate> path), int> queue = new();

        var currentPath = new List<Coordinate> { reindeer }.ToImmutableHashSet();


        queue.Enqueue((reindeer, reindeerDirection, currentPath), 0);

        Dictionary<(Coordinate, Direction), int> reachable = [];

        var minScore = long.MaxValue;

        var onBestPaths = new List<ImmutableHashSet<Coordinate>>();

        while (queue.TryDequeue(out var element, out var score))
        {
            var (loc, dir, path) = element;

            if (reachable.TryGetValue((loc, dir), out var currentScore) && score > currentScore)
            {
                continue;
            }

            reachable[(loc, dir)] = score;

            minScore = Calculate(ref input, ref queue, minScore, score, loc, dir, path, 0, ref onBestPaths);

            minScore = Calculate(ref input, ref queue, minScore, score, loc, dir.RotateLeft, path, TurnCost, ref onBestPaths);
            minScore = Calculate(ref input, ref queue, minScore, score, loc, dir.RotateRight, path, TurnCost, ref onBestPaths);

        }

        var bestPathCoordinates = new HashSet<Coordinate>();

        foreach (var path in onBestPaths)
        {
            foreach (var point in path)
            {
                bestPathCoordinates.Add(point);
            }
        }

        return (minScore, bestPathCoordinates);
    }

    private static long Calculate(ref Input input, ref PriorityQueue<(Coordinate loc, Coordinate dir, ImmutableHashSet<Coordinate>), int> queue, long minScore, int currentSteps, Coordinate loc, Coordinate newDir, ImmutableHashSet<Coordinate> path, int hit, ref List<ImmutableHashSet<Coordinate>> foundPaths)
    {
        if (input.Map.TryGetValue(loc + newDir, out var mapObject2))
        {
            if (mapObject2 == 'E')
            {
                var newPath = path.Add(loc + newDir);

                var toBeat = currentSteps + hit + MoveAheadCost;
                if (minScore > toBeat)
                {
                    minScore = currentSteps + hit + MoveAheadCost;
                    foundPaths.Clear();
                    foundPaths.Add(newPath);
                }
                else if (minScore == toBeat)
                {
                    foundPaths.Add(newPath);

                }
            }

            // we hit a wall, stop this movement

        }
        else
        {
            var newPath = path.Add(loc + newDir);
            queue.Enqueue((loc + newDir, newDir, newPath), currentSteps + hit + MoveAheadCost);
        }

        return minScore;
    }

    public static void PrintMap(ref Input input, Coordinate start, HashSet<Coordinate>? bestPathCoordinates = null)
    {
        StringBuilder sb = PositionsToStringBuilder(ref input, start, bestPathCoordinates);

        Console.WriteLine(sb.ToString());
    }

    private static StringBuilder PositionsToStringBuilder(ref Input input, Coordinate reindeer, HashSet<Coordinate>? bestPathCoordinates = null)
    {
        var sb = new StringBuilder();

        for (var y = 0; y < input.Height; y++)
        {
            for (var x = 0; x < input.Width; x++)
            {
                var coordinate = new Coordinate(x, y);
                if (bestPathCoordinates?.Contains(coordinate) ?? false)
                {
                    sb.Append('O');
                }
                else if (input.Map.TryGetValue(new Coordinate(x, y), out var value))
                {
                    sb.Append(value);
                }
                else if (reindeer.X == x && reindeer.Y == y)
                {
                    sb.Append('@');
                }
                else
                {
                    sb.Append('.');
                }
            }

            sb.AppendLine();
        }

        return sb;
    }

    public static long CalculateAnswer2(Input input)
    {
        var (_, onBestPaths) = Calculate(ref input, input.Start, Coordinate.OffsetRight);

        return onBestPaths.Count;
    }

}


public readonly struct Input
{
    public int Height { get; }
    public int Width { get; }

    public Input(string[] input)
    {
        Width = input[0].Length;
        Height = input.Length;

        for (var y = 0; y < input.Length; y++)
        {
            for (var x = 0; x < input[y].Length; x++)
            {
                var val = input[y][x];
                if (val == 'S')
                {
                    Start = new Coordinate(x, y);
                }
                else if (val != '.')
                {
                    var coord = new Coordinate(x, y);
                    if (val == 'E')
                    {
                        End = coord;
                    }
                    Map[coord] = val;
                }
            }
        }
    }


    public readonly Dictionary<Coordinate, char> Map = [];
    public readonly Coordinate Start = default;
    public readonly Coordinate End = default;
}