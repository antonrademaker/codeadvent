using System.Text.RegularExpressions;

Regex parser = new Regex(@"^Card\s+(?<cardId>\d+):(?<winning>(\s+\d+)+)\s*\|(?<own>(\s+\d+)+)\s*$", RegexOptions.Compiled | RegexOptions.Multiline);
string[] inputFiles = ["input/example.txt", "input/input.txt"];

foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var scratchPoints = inputs.Select(input => CalculateScratchPoints(input));

    var part1 = scratchPoints.Sum();

    Console.WriteLine($"Part 1: {part1}");

    //   var part2 = gears.Sum();
    //   Console.WriteLine($"Part 2: {part2}");
}

int CalculateScratchPoints(string input)
{
    Console.WriteLine(input);

    var match = parser.Match(input);

    if (!match.Success)
    {
        throw new Exception($"Invalid input: {input}");
    }

    var cardId = int.Parse(match.Groups["cardId"].Value);
    var winning = match.Groups["winning"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToHashSet();
    var own = match.Groups["own"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToHashSet();

    var winningScratchcards = own.Intersect(winning);
    var matches = winningScratchcards.Count();
    var score = matches > 0 ? Enumerable.Range(1, matches - 1).Aggregate(1, (cur, next) => cur * 2) : 0;
    Console.WriteLine($"\tWinning: {string.Join(",", winningScratchcards)}, points: {score}");

    return score;

}