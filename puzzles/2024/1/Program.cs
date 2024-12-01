using System.Text.RegularExpressions;

Regex parser = Parser();

string[] inputFiles = { "input/example.txt", "input/input.txt" };

foreach (string file in inputFiles)
{
    var inputs = ParseFile(file, parser);

    var left = GetOrdered(x => x.Left, inputs);
    var right = GetOrdered(x => x.Right, inputs);

    var answer1 = CalculateAnswer1(left, right);
    Console.WriteLine("Answer 1: {0}", answer1);

    var answer2 = CalculateAnswer2(left, right);
    Console.WriteLine("Answer 2: {0}", answer2);
}

static List<(int Left, int Right)> ParseFile(string file, Regex parser)
{
    return File.ReadAllLines(file)
        .Select(line => parser.Match(line).Groups)
        .Select(m => (Left: int.Parse(m[1].Value), Right: int.Parse(m[2].Value)))
        .ToList();
}

static List<int> GetOrdered(Func<(int Left, int Right), int> selector, List<(int Left, int Right)> inputs)
{
    return inputs.Select(selector).OrderBy(v => v).ToList();
}

static int CalculateAnswer1(List<int> left, List<int> right)
{
    return left.Zip(right, (l, r) => Math.Abs(l - r)).Sum();
}

static int CalculateAnswer2(List<int> left, List<int> right)
{
    var leftDict = left.GroupBy(v => v).ToDictionary(g => g.Key, g => g.Count());
    var rightDict = right.GroupBy(v => v).ToDictionary(g => g.Key, g => g.Count());

    return leftDict.Aggregate(0, (acc, kv) => acc + (kv.Key * kv.Value * rightDict.GetValueOrDefault(kv.Key, 0)));
}

partial class Program
{
    [GeneratedRegex(@"(\d+)\s+(\d+)")]
    private static partial Regex Parser();
}