using System.Text.RegularExpressions;

var files = Directory.GetFiles("input", "*.txt");

var regex = new Regex(@"Monkey (?<index>\d):\s+Starting items: (?<items>[\d,\s]+)\s+Operation: new = old (?<operation>(old|\d+|\s|[\+\-\*\\])+)\s+Test: divisible by (?<test>\d+)\s+If true: throw to monkey (?<targetTrue>\d)\s+If false: throw to monkey (?<targetFalse>\d)\s*", RegexOptions.NonBacktracking);

foreach (var file in files.OrderBy(t => t))
{
    var fileInput = File.ReadAllText(file);
    Console.WriteLine($"File: {file}");

    var part1 = CalculateRounds(regex, fileInput, 20, true);

    Console.WriteLine($"Part 1: {part1}");
    var part2 = CalculateRounds(regex, fileInput, 10000, false);
    Console.WriteLine($"Part 2: {part2}{Environment.NewLine}");
}

static long CalculateRounds(Regex regex, string fileInput, int rounds, bool decrease)
{
    var monkeys = ParseMonkeys(regex, fileInput).ToList();

    var inspections = monkeys.ToDictionary(k => k, v => 0L);

    var boundary = monkeys.Aggregate(1L, (cur, next) => cur * next.Test);

    for (var round = 1; round <= rounds; round++)
    {
        foreach (var monkey in monkeys)
        {
            var monkeyItems = monkey.Items.ToArray();
            monkey.Items.Clear();

            inspections[monkey] += monkeyItems.Length;

            foreach (var item in monkeyItems)
            {
                var newItem = monkey.Operation(item) % boundary;
                if (decrease)
                {
                    newItem = (int)Math.Floor((newItem / 3f));
                }                

                monkeys[(newItem % monkey.Test == 0) ? monkey.TargetTrue : monkey.TargetFalse].Items.Add(newItem);
            }
        }
    }

    foreach (var monkey in monkeys)
    {
        Console.WriteLine($"Monkey {monkey.Index}: {monkey.Items.Count}, inspections: {inspections[monkey]}");
    }

    var topTwoMonkeys = inspections.Values.OrderByDescending(t => t).Take(2).Aggregate(1L, (cur, val) => cur * val);
    return topTwoMonkeys;
}

static Func<long, long> ParseOperation(string input)
{
    if (input.Contains("old"))
    {
        return old => old * old;
    }

    var number = int.Parse(input[2..]);

    return input switch
    {
        string s when s.Contains("*") => old => old * number,
        _ => old => old + number
    };
}

static IEnumerable<Monkey> ParseMonkeys(Regex regex, string fileInput)
{
    var monkeyMatches = regex.Matches(fileInput);

    foreach (var input in monkeyMatches.Cast<Match>().Select(m => m.Groups))
    {
        var monkey = new Monkey()
        {
            Index = int.Parse(input["index"].Value),
            Items = input["items"].Value.Split(',').Select(s => long.Parse(s.Trim())).ToList(),
            Operation = ParseOperation(input["operation"].Value),
            Test = int.Parse(input["test"].Value),
            TargetTrue = int.Parse(input["targetTrue"].Value),
            TargetFalse = int.Parse(input["targetFalse"].Value)
        };
        yield return monkey;
    }
}

public class Monkey
{
    public int Index { get; init; }

    public List<long> Items { get; init; }

    public Func<long, long> Operation { get; init; }

    public long Test { get; init; }

    public int TargetTrue { get; init; }
    public int TargetFalse { get; init; }
}