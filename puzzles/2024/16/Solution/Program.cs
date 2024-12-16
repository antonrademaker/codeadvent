using System.Diagnostics;
using System.Text;
using System.Threading;
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
        var map = input.Map;

        var reindeer = input.Start;

        var reindeerDirection = Coordinate.OffsetRight;
        var answer = Calculate(map, reindeer, reindeerDirection);

        return answer;
    }

    private static long Calculate(Dictionary<Coordinate, char> map, Coordinate reindeer, Direction reindeerDirection)
    {
        PriorityQueue<(Coordinate loc, Direction dir), int> queue = new();
        queue.Enqueue((reindeer, reindeerDirection), 0);

        Dictionary<(Coordinate, Direction), int> reachable = [];

        var minScore = long.MaxValue;

        while (queue.TryDequeue(out var element, out var currentSteps))
        {
            var (loc, dir) = element;

            if (reachable.TryGetValue((loc,dir), out var currentScore) && currentSteps > currentScore)
            {
                continue;
            }

            reachable[(loc, dir)] = currentScore;

            minScore = Calculate(map, queue, minScore, currentSteps, loc, dir, 0);
            minScore = Calculate(map, queue, minScore, currentSteps, loc, dir.RotateLeft, TurnCost);
            minScore = Calculate(map, queue, minScore, currentSteps, loc, dir.RotateRight, TurnCost);

        }

        return minScore;
    }

    private static long Calculate(Dictionary<Coordinate, char> map, PriorityQueue<(Coordinate loc, Coordinate dir), int> queue, long minScore, int currentSteps, Coordinate loc, Coordinate newDir, int hit)
    {
        if (map.TryGetValue(loc + newDir, out var mapObject2))
        {
            if (mapObject2 == 'E')
            {
                if (minScore > currentSteps + hit + MoveAheadCost)
                {
                    minScore = currentSteps + hit + MoveAheadCost;
                }
            }

            // we hit a wall, stop this movement

        }
        else
        {
            queue.Enqueue((loc + newDir, newDir), currentSteps + hit + MoveAheadCost);
        }

        return minScore;
    }

    public static void PrintMap(Dictionary<Coordinate, char> map, Coordinate robot, int width, int height)
    {
        StringBuilder sb = PositionsToStringBuilder(map, robot, width, height);

        Console.WriteLine(sb.ToString());
    }

    private static StringBuilder PositionsToStringBuilder(Dictionary<Coordinate, char> map, Coordinate reindeer, int width, int height)
    {
        var sb = new StringBuilder();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (map.TryGetValue(new Coordinate(x, y), out var value))
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
        return 0;
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
                if (input[y][x] == 'S')
                {
                    Start = new Coordinate(x, y);
                }
                else if (input[y][x] != '.')
                {
                    Map[new Coordinate(x, y)] = input[y][x];
                }
            }
        }
    }


    public readonly Dictionary<Coordinate, char> Map = [];
    public readonly Coordinate Start = default;
}