using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Coordinate = AoC.Utilities.Coordinate<long>;
using Direction = AoC.Utilities.Coordinate<long>;

namespace Solution;

public partial class Program
{
    private static readonly List<(string file, int width, int height)> inputFiles = [("input/example1.txt", 11, 7), ("input/input.txt", 101, 103)];

    public static void Main(string[] args)
    {
        foreach (var (file, width, height) in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input = ParseFile(file);

            var startTime = Stopwatch.GetTimestamp();

            var answer1 = CalculateAnswer1(input, width, height, 100);
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"{file}: Answer 1: {answer1} ({elapsedTime.TotalMilliseconds}ms)");
            Console.WriteLine();
            if (width > 100)
            {
                startTime = Stopwatch.GetTimestamp();
                var answer2 = CalculateAnswer2(input, width, height);
                elapsedTime = Stopwatch.GetElapsedTime(startTime);

                Console.WriteLine($"{file}: Answer 2: {answer2} ({elapsedTime.TotalMilliseconds}ms)");
            }
        }
    }

    static Input ParseFile(string file)
    {
        return new Input(File.ReadAllLines(file));
    }

    public static long CalculateAnswer1(Input input, int width, int height, int seconds)
    {
        var positions = CalculateRobotPositions(input, width, height, seconds);

        PrintPositions(positions, width, height);
        int[] quadrants = CalculateQuadrants(positions, width, height);

        return quadrants.Aggregate(1, (acc, cur) => acc * cur);
    }

    private static int[] CalculateQuadrants(Dictionary<Robot, Coordinate> positions, int width, int height)
    {
        var xMiddle = width / 2;
        var yMiddle = height / 2;

        var quadrants = new int[4];

        foreach (var robotPosition in positions.Values)
        {
            if (robotPosition.X < xMiddle)
            {
                if (robotPosition.Y < yMiddle)
                {
                    quadrants[0]++;
                }
                else if (robotPosition.Y > yMiddle)
                {
                    quadrants[1]++;
                }
            }
            else if (robotPosition.X > xMiddle)
            {
                if (robotPosition.Y < yMiddle)
                {
                    quadrants[2]++;
                }
                else if (robotPosition.Y > yMiddle)
                {
                    quadrants[3]++;
                }
            }
        }

        return quadrants;
    }

    public static Dictionary<Robot, Coordinate> CalculateRobotPositions(Input input, int width, int height, int seconds)
    {
        return input.GetMachines().ToDictionary(t => t, t => (t.Start + t.Direction * seconds).ResetToBoundary(width, height));
    }

    public static void PrintPositions(Dictionary<Robot, Coordinate> robots, int width, int height)
    {
        StringBuilder sb = PositionsToStringBuilder(robots, width, height);

        Console.WriteLine(sb.ToString());
    }

    private static StringBuilder PositionsToStringBuilder(Dictionary<Robot, Coordinate> robots, int width, int height)
    {
        var positions = robots.Values.GroupBy(t => t).ToDictionary(t => t.Key, t => t.Count());

        var sb = new StringBuilder();

        var xMiddle = width / 2;
        var yMiddle = height / 2;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (xMiddle == x || yMiddle == y)
                {
                    sb.Append(' ');
                }
                else if (positions.TryGetValue(new Coordinate(x, y), out var count))
                {
                    sb.Append(count);
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


    public static long CalculateAnswer2(Input input, int width, int height)
    {
        var robots = input.GetMachines().ToDictionary(t => t, t => t.Start);

        var secondsPassed = 0L;

        while (secondsPassed++ < width * height)
        {
            foreach (var (robot, position) in robots)
            {
                robots[robot] = (position + robot.Direction).ResetToBoundary(width, height);
            }

            var map = PositionsToStringBuilder(robots, width, height).ToString();
            if (map.Contains("11111111111111"))
            {
                Console.WriteLine(map);
                return secondsPassed;
            }
        }

        return 0;
    }

    public static readonly Regex ParseRegex = Parser();

    [GeneratedRegex(@"p=(?<px>\d+),(?<py>\d+) v=(?<vx>-?\d+),(?<vy>-?\d+)", RegexOptions.Compiled)]
    private static partial Regex Parser();
}

public readonly ref struct Input(string[] input)
{
    private readonly string[] StringInput = input;

    public List<Robot> GetMachines()
    {
        return StringInput.Select(GetRobot).ToList();
    }

    private static Robot GetRobot(string input)
    {
        var coords = Program.ParseRegex.Match(input);

        return new Robot(new Coordinate(int.Parse(coords.Groups["px"].Value), int.Parse(coords.Groups["py"].Value)),
            new Coordinate(int.Parse(coords.Groups["vx"].Value), int.Parse(coords.Groups["vy"].Value)));
    }
}

public readonly record struct Robot(Coordinate Start, Direction Direction)
{

}
