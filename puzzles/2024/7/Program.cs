

string[] inputFiles = ["input/example.txt", "input/input.txt"];

static void ParseLine(string line, out long target, out ReadOnlySpan<long> values)
{
    var parts = line.Split(":");
    target = long.Parse(parts[0]);
    values = parts[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray().AsSpan();    
}

foreach (string file in inputFiles)
{
    Console.WriteLine($"Reading: {file}");

    var input = ParseFile(file);

    var answer1 = CalculateAnswer1(input);
    Console.WriteLine($"{file}: Answer 1: {answer1}");

    var answer2 = CalculateAnswer2(input);
    Console.WriteLine($"{file}: Answer 2: {answer2}");
}

static long CalculateAnswer1(string[] input)
{
    var currentSum = 0L;

    foreach (var line in input)
    {
        ParseLine(line, out var target, out var values);

        if (Valid(target, values[0], values[1..]))
        {
            currentSum += target;
        }
    }

    return currentSum;
}

static bool Valid(long target, long value, ReadOnlySpan<long> values)
{

    if (values.Length == 0)
    {
        return target == value;
    }

    return Valid(target, value * values[0], values[1..]) || Valid(target, value + values[0], values[1..]);
}

static bool ValidPart2(long target, long value, ReadOnlySpan<long> values)
{
    if (values.Length == 0)
    {
        return target == value;
    }

    return ValidPart2(target, value * values[0], values[1..]) || ValidPart2(target, value + values[0], values[1..]) || ValidPart2(target, long.Parse(string.Concat(value, values[0])), values[1..]);
}

static long CalculateAnswer2(string[] input)
{
    var currentSum = 0L;

    foreach (var line in input)
    {
        ParseLine(line, out var target, out var values);

        if (ValidPart2(target, values[0], values[1..]))
        {
            currentSum += target;
        }
    }

    return currentSum;
}

static string[] ParseFile(string file)
{
    return File.ReadAllLines(file);
}

partial class Program
{
    static HashSet<string> pages = [];
}