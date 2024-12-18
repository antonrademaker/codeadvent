using System.Diagnostics;
using System.Text;
using Coordinate = AoC.Utilities.Coordinate<long>;

namespace Solution;

public partial class Program
{
    private static readonly List<(string file, int width, int height, int first)> inputFiles = [("input/example1.txt", 6, 6, 12), ("input/input.txt", 70, 70, 1024)];

    public static void Main(string[] _)
    {
        foreach (var (file, width, height, first) in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input1 = ParseFile(file, width, height, first);

            var startTime = Stopwatch.GetTimestamp();

            var answer1 = CalculateAnswer1(input1);
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"{file}: Answer 1: {answer1} ({elapsedTime.TotalMilliseconds}ms)");
            Console.WriteLine();

            var input2 = ParseFile(file, width, height, int.MaxValue);


            startTime = Stopwatch.GetTimestamp();
            var answer2 = CalculateAnswer2(input2);
            elapsedTime = Stopwatch.GetElapsedTime(startTime);

            Console.WriteLine($"{file}: Answer 2: {answer2} ({elapsedTime.TotalMilliseconds}ms)");
        }
    }

    private static Input ParseFile(string file, int width, int height, int first)
    {
        return new Input(File.ReadAllLines(file), width, height, first);
    }

    public static long CalculateAnswer1(Input input)
    {
        var position = new Coordinate(0, 0);

        var end = new Coordinate(input.Width, input.Height);

        var queue = new PriorityQueue<Coordinate, int>();

        queue.Enqueue(position, 0);

        var visited = new HashSet<Coordinate>();

        while (queue.TryDequeue(out var pos, out int currentSteps))
        {
            if (visited.Contains(pos))
            {
                continue;
            }

            visited.Add(pos);

            if (pos == end)
            {
                return currentSteps;
            }

            foreach (var direction in Coordinate.Offsets)
            {
                var next = pos + direction;
                if (visited.Contains(next))
                {
                    continue;
                }
                if (input.IsCandidate(next))
                {
                    queue.Enqueue(next, currentSteps + 1);
                }
            }
        }
        return 0;
    }

    public static void PrintMap(HashSet<Coordinate> map, Coordinate robot, int width, int height)
    {
        StringBuilder sb = PositionsToStringBuilder(map, robot, width + 1, height + 1);
        Console.WriteLine(sb.ToString());
    }

    public static void PrintMap(Dictionary<Coordinate, int> map, Coordinate robot, int width, int height)
    {
        PrintMap([.. map.Keys], robot, width + 1, height + 1);
    }

    private static StringBuilder PositionsToStringBuilder(HashSet<Coordinate> map, Coordinate robot, int width, int height)
    {
        var sb = new StringBuilder();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (map.TryGetValue(new Coordinate(x, y), out var _))
                {
                    sb.Append('X');
                }
                else if (robot.X == x && robot.Y == y)
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

    public static string CalculateAnswer2(Input input)
    {
        for (var step = 0; step < input.Map.Count; step++)
        {
             if (!CalculatePath(input, step))
            {
                return input.Lines[step - 1].ToString();
            }

        }
        return "No solution";
    }

    private static bool CalculatePath(Input input, int step)
    {
        var position = new Coordinate(0, 0);

        var end = new Coordinate(input.Width, input.Height);

        var queue = new PriorityQueue<Coordinate, int>();

        queue.Enqueue(position, 0);

        var visited = new HashSet<Coordinate>();

        while (queue.TryDequeue(out var pos, out int currentSteps))
        {
            if (visited.Contains(pos))
            {
                continue;
            }

            visited.Add(pos);

            if (pos == end)
            {
                return true;
            }

            foreach (var direction in Coordinate.Offsets)
            {
                var next = pos + direction;
                if (visited.Contains(next))
                {
                    continue;
                }
                if (input.IsCandidate(next, step))
                {
                    queue.Enqueue(next, currentSteps + 1);
                }
            }
        }
        return false;
    }
}

public readonly struct Input
{
    public int Height { get; }
    public int Width { get;  }

    public string[] Lines { get; }

    public Input(string[] input, int width, int height, int first)
    {
        Width = width;
        Height = height;
        Lines = input;

        foreach (var (line, index) in input.Take(first).Select((line, index) => (line, index)))
        {
            var parts = line.Split(',');
            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);
            Map.Add(new Coordinate(x, y), index);
        }
    }

    public readonly Dictionary<Coordinate, int> Map = [];

    public bool IsCandidate(Coordinate key, int currentSteps)
    {
        if (key.X < 0 || key.Y < 0 || key.X > Width || key.Y > Height)
        {
            return false;
        }

        if (Map.TryGetValue(key, out var visibleFrom) && visibleFrom < currentSteps)
        {
            return false;
        }

        return true;
    }

    public bool IsCandidate(Coordinate key)
    {
        if (key.X < 0 || key.Y < 0 || key.X > Width || key.Y > Height)
        {
            return false;
        }

        if (Map.TryGetValue(key, out var _))
        {
            return false;
        }

        return true;
    }

}