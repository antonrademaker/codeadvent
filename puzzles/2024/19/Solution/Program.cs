using System.Diagnostics;
using AoC.Utilities;

namespace Solution;

public partial class Program
{
    private static readonly List<string> inputFiles = ["input/example1.txt", "input/input.txt"];

    private static readonly Dictionary<string, bool> cache = [];
    private static readonly DefaultDictionary<string, long> counts = [];

    public static void Main(string[] _)
    {
        foreach (var file in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input1 = ParseFile(file);

            var startTime = Stopwatch.GetTimestamp();

            var answer1 = CalculateAnswer1(input1);
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"{file}: Answer 1: {answer1} ({elapsedTime.TotalMilliseconds}ms)");
            Console.WriteLine();

            var input2 = ParseFile(file);

            startTime = Stopwatch.GetTimestamp();
            var answer2 = CalculateAnswer2(input2);
            elapsedTime = Stopwatch.GetElapsedTime(startTime);

            Console.WriteLine($"{file}: Answer 2: {answer2} ({elapsedTime.TotalMilliseconds}ms)");

            // reset after each file
            cache.Clear();
            counts.Clear();
        }
    }

    private static Input ParseFile(string file)
    {
        return new Input(File.ReadAllText(file));
    }

    public static int CalculateAnswer1(Input input)
    {
        var counter = 0;

        foreach (var design in input.Designs)
        {
            (bool isPossible, long _) = IsPossible(input, design);
            if (isPossible)
            {
                counter++;
            }
        }

        return counter;
    }

    public static long CalculateAnswer2(Input input)
    {
        var counter = 0L;

        foreach (var design in input.Designs)
        {
            (bool isPossible, long resultCount) = IsPossible(input, design);
            if (isPossible)
            {
                counter += resultCount;
            }
        }

        return counter;
    }

    public static (bool possible, long count) IsPossible(Input input, string design)
    {
        if (cache.TryGetValue(design, out bool value))
        {
            return (value, counts[design]);
        }

        if (input.Towels.Contains(design))
        {
            cache[design] = true;
            counts[design]++;
        }

        foreach (var t in input.Towels.Where(t => t.Length < design.Length && design.Substring(0, t.Length) == t))
        {
            string sub = design.Substring(t.Length);
            var (possible, count) = IsPossible(input, sub);
            if (possible)
            {
                cache[design] = true;
                counts[design] += count;
            }
        }

        cache[design] = counts[design] > 0;

        return (cache[design], counts[design]);
    }
}

public readonly struct Input
{
    public Input(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Towels = lines[0].Split(", ");

        Designs = lines[1..];
    }

    public string[] Towels { get; }
    public string[] Designs { get; }
}