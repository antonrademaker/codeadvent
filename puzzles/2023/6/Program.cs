using System.Text.RegularExpressions;

string[] inputFiles = ["input/example.txt", "input/input.txt"];

foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var location = CalculatePart1(inputs);

    var part1 = location;

    Console.WriteLine($"Part 1: {part1}");

    var part2 = CalculatePart2(inputs);

     Console.WriteLine($"Part 2: {part2}");
}

long CalculatePart1(string[] inputs)
{
    var times = Parse(inputs[0]);
    var distances = Parse(inputs[1]);
    return Calculate(times, distances);
}

long CalculatePart2(string[] inputs)
{
    long[] times = [ParsePart2(inputs[0])];
    long[] distances = [ParsePart2(inputs[1])];
    return Calculate(times, distances);
}

long[] Parse(string input)
{
    var regex = RowParser();
    var matches = regex.Matches(input);

    var result = new long[matches.Count];

    for (int i = 0; i < matches.Count; i++)
    {
        result[i] = long.Parse(matches[i].Value);
    }

    return result;
}

long ParsePart2(string input)
{
    var regex = RowParser();
    var matches = regex.Matches(input);

    return long.Parse(string.Join("", matches.Select(t => t.Value)));
}


static long Calculate(long[] times, long[] distances)
{
    long totalWins = 1;

    for (var index = 0; index < times.Length; index++)
    {
        var time = times[index];
        var distance = distances[index];
        var trust = 0L;
        var lower = 0L;
        var upper = 0L;

        var test = 0L;

        do
        {
            trust++;

            test = ((time - trust) * trust);
            //Console.WriteLine($"Trust: {trust}, {time}: test: {test}");
        } while (test <= distance);
        lower = trust;

        trust = time;
        do
        {
            trust--;
            test = ((time - trust) * trust);
            //Console.WriteLine($"Trust: {trust}, {time}: test: {test}");

        } while (test <= distance);
        upper = trust;

        var winning = upper - lower + 1;

        Console.WriteLine($"Time: {time}, max current distance: {distance}: {winning}");

        totalWins *= winning;
    }

    return totalWins;
}

partial class Program
{
    [GeneratedRegex(@"\d+")]
    private static partial Regex RowParser();
}
