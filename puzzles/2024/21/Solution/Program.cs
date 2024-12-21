using System.Diagnostics;
using Coordinate = AoC.Utilities.Coordinate<int>;

namespace Solution;

public partial class Program
{
    private static readonly List<string> inputFiles = ["input/example1.txt", "input/input.txt"];

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
        }
    }

    public static readonly Dictionary<(char s, char t), int> cache = [];

    public static readonly Dictionary<char, Coordinate> numericKeypad = new()
    {
        ['7'] = new Coordinate(0, 0),
        ['8'] = new Coordinate(1, 0),
        ['9'] = new Coordinate(2, 0),
        ['4'] = new Coordinate(0, 1),
        ['5'] = new Coordinate(1, 1),
        ['6'] = new Coordinate(2, 1),
        ['1'] = new Coordinate(0, 2),
        ['2'] = new Coordinate(1, 2),
        ['3'] = new Coordinate(2, 2),
        [' '] = new Coordinate(0, 3),
        ['0'] = new Coordinate(1, 3),
        ['A'] = new Coordinate(2, 3),
    };

    public static readonly Dictionary<char, Coordinate> directionalKeypad = new()
    {
        [' '] = new Coordinate(0, 0),
        ['^'] = new Coordinate(1, 0),
        ['A'] = new Coordinate(2, 0),
        ['<'] = new Coordinate(0, 1),
        ['v'] = new Coordinate(1, 1),
        ['>'] = new Coordinate(2, 1),
    };

    private static Input ParseFile(string file)
    {
        return new Input(File.ReadAllText(file));
    }

    public static long CalculateAnswer1(Input input)
    {
        return Calculate(input, 2);
    }

    private static long Calculate(Input input, int layers)
    {
        Cache.Clear();

        var sum = 0L;
        foreach (var line in input.Lines)
        {
            var shortest = Shortest(line, 0, layers);

            sum += shortest * long.Parse(line[..3]);

            Console.WriteLine(line);
        }

        return sum;
    }

    public static long CalculateAnswer2(Input input)
    {
        return Calculate(input, 25);
    }

    public static readonly Dictionary<(string input, int depth), long> Cache = [];

    public static long Shortest(string input, int depth, int layers)
    {
        if (Cache.TryGetValue((input, depth), out var res))
        {
            return res;
        }

        if (depth > layers)
        {
            var result = input.Length;
            Cache[(input, depth)] = result;
            return result;
        }

        var keypadLocation = 'A';

        var shortest = 0L;

        foreach (var c in input)
        {
            var instructions = ToKeypadInstructions(keypadLocation, c, depth == 0 ? numericKeypad : directionalKeypad);

            shortest += instructions.Min(instructionSet => Shortest(instructionSet, depth + 1, layers));

            keypadLocation = c;
        }

        Cache[(input, depth)] = shortest;

        return shortest;
    }

    private static List<string> ToKeypadInstructions(char source, char target, Dictionary<char, Coordinate> keypad)
    {
        var start = keypad[source];
        var end = keypad[target];

        var presses = end - start;

        List<char> result = [];

        if (start.X == end.X)
        {
            result.AddRange(Enumerable.Repeat(presses.Y > 0 ? 'v' : '^', int.Abs(presses.Y)));
            result.Add('A');
            return [new string([.. result])];
        }

        if (start.Y == end.Y)
        {
            result.AddRange(Enumerable.Repeat(presses.X > 0 ? '>' : '<', int.Abs(presses.X)));
            result.Add('A');

            return [new string([.. result])];
        }

        var c1 = new Coordinate(start.X, end.Y);
        var c2 = new Coordinate(end.X, start.Y);

        var candidates = new List<string>();

        if (c1 != keypad[' '])
        {
            result.AddRange(Enumerable.Repeat(presses.Y > 0 ? 'v' : '^', int.Abs(presses.Y)));
            result.AddRange(Enumerable.Repeat(presses.X > 0 ? '>' : '<', int.Abs(presses.X)));
            result.Add('A');
            candidates.Add(new string([.. result]));
        }

        if (c2 != keypad[' '])
        {
            List<char> result2 = [];
            result2.AddRange(Enumerable.Repeat(presses.X > 0 ? '>' : '<', int.Abs(presses.X)));
            result2.AddRange(Enumerable.Repeat(presses.Y > 0 ? 'v' : '^', int.Abs(presses.Y)));
            result2.Add('A');

            candidates.Add(new string([.. result2]));
        }

        return candidates;
    }
}

public readonly struct Input(string input)
{
    public readonly string[] Lines = input.Split(Environment.NewLine);
}