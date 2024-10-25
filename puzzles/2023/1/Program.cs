using System.Text.RegularExpressions;

Regex regex = new Regex(@"[^0-9]");
Regex isNumber = new Regex(@"\d");

var textual = new Dictionary<string, int> {
    { "one", 1 }, { "two", 2 }, { "three", 3 }, { "four", 4 }, { "five", 5 }, { "six", 6 }, { "seven", 7 }, {  "eight", 8 }, {  "nine",9 },
    { "1", 1 }, { "2", 2 }, { "3", 3 }, { "4", 4 }, { "5", 5 }, { "6", 6 }, { "7", 7 }, { "8", 8 }, { "9",9 }
};

string[] inputFiles = [/*"input/example.txt",*/ "input/example2.txt", "input/input.txt"];

foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var parsed1 = inputs.Select(input => regex.Replace(input, string.Empty))
        .Where(value => !string.IsNullOrWhiteSpace(value))
        .Select(value => 10 * (value[0] - '0') + value[^1] - '0')
        .ToList();

    var part1 = parsed1.Sum();

    Console.WriteLine($"Part 1: {part1}");

    var parsed2 = inputs.Select(input => Parse(input))
        .Select(value => 10 * value[0] + value[^1])
        .ToList();

    var part2 = parsed2.Sum();

    Console.WriteLine($"Part 2: {part2}");
}

int[] Parse(string input)
{
    var result = new List<int>();
    for (int i = 0; i < input.Length; i++)
    {
        var current = input[i..];
        foreach (var opt in textual)
        {
            if (input.StartsWith(opt.Key))
            {
                result.Add(opt.Value);
                break;
            }
        }
    }
    if (result.Count < 2)
    {
        result.Add(result[0]);
    }
    return result.ToArray();
}