using System.Text.RegularExpressions;

Regex regex = new Regex(@"^Game\s(?<gameId>\d+):\w*(?<subsets>[\s\.\w,;]+)$");

string[] inputFiles = ["input/example.txt", "input/input.txt"];

var part1Requirements = new Dictionary<string, int> { { "red", 12 }, { "green", 13 }, { "blue", 14 } };

foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var parsed = inputs.Select(input => regex.Match(input))
        .Where(match => match.Success)
        .Select(match => new
        {
            gameId = int.Parse(match.Groups["gameId"].Value),
            subSets = match.Groups["subsets"].Value
                    .Split(";", StringSplitOptions.TrimEntries)
                    .Select(subset => subset
                                            .Split(",", StringSplitOptions.TrimEntries)
                                            .Select(c => c.Split(" "))
                                            .ToDictionary(c => c[1], c => int.Parse(c[0]))
                    )
        })
        .Select(row => new
        {
            row.gameId,
            maxSubset = part1Requirements.Keys.ToDictionary(key => key, key => row.subSets.Max(t => t.TryGetValue(key, out int value) ? value : 0))
        }
        ).ToList();

    var part1Rows = parsed.Where(game => part1Requirements.All(kv => game.maxSubset[kv.Key] <= kv.Value))
        .ToList();

    var part1 = part1Rows.Sum(t => t.gameId);

    Console.WriteLine($"Part 1: {part1}");

    var part2Rows = parsed.Select(r => r.maxSubset.Values.Aggregate(1, (cur, next) => cur * next));

    var part2 = part2Rows.Sum();
    Console.WriteLine($"Part 2: {part2}");
}

