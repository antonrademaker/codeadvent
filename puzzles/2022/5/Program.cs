using System.Text.RegularExpressions;

var files = Directory.GetFiles("input", "*.txt");

Regex moveRegex = new(@"move (\d+) from (\d+) to (\d+)$", RegexOptions.NonBacktracking);
foreach (var file in files)
{
    var lines = File.ReadAllLines(file);

    Process(moveRegex, lines, true);
    Process(moveRegex, lines, false);
}
static void Process(Regex moveRegex, string[] lines, bool isPartOne)
{
    var invLines = new Stack<string[]>();

    var invStacks = new List<List<string>>();

    foreach (var line in lines)
    {
        if (!invStacks.Any())
        {
            SetupInitialState(invLines, invStacks, line);
        }
        else
        {
            CalculateMove(invStacks, line, moveRegex, isPartOne);
        }
    }
    Console.WriteLine($"Part {(isPartOne ? "1" : "2")}: {string.Join(null, invStacks.Select(t => t.Last()))}");
}

static void SetupInitialState(Stack<string[]> invLines, List<List<string>> invStacks, string line)
{
    if (line.Contains('1'))
    {
        invStacks.AddRange(invLines.Peek().Select(g => new List<string>()));

        while (invLines.Count != 0)
        {
            var invLine = invLines.Pop();
            for (var index = 0; index < invLine.Count(); index++)
            {
                if (!string.IsNullOrWhiteSpace(invLine[index]))
                {
                    invStacks[index].Add(invLine[index]);
                }
            }
        }
    }
    else
    {
        var cleanedLine = line.Replace("[", "").Replace("] ", ",").Replace("]", "").Replace("\t", ",").Replace("    ", ",");
        var cols = cleanedLine.Split(',');

        invLines.Push(cols);
    }
}

static void CalculateMove(List<List<string>> invStacks, string move, Regex moveRegex, bool isPartOne)
{
    if (move != string.Empty)
    {
        var match = moveRegex.Match(move);

        var count = int.Parse(match.Groups[1].Value);

        var source = int.Parse(match.Groups[2].Value) - 1;
        var target = int.Parse(match.Groups[3].Value) - 1;

        var pickup = invStacks[source].TakeLast(count);

        invStacks[target].AddRange(isPartOne ? pickup.Reverse() : pickup);
        invStacks[source].RemoveRange(invStacks[source].Count - count, count);
    }
}
