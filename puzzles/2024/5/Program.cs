

string[] inputFiles = ["input/example.txt", "input/input.txt"];



static (bool isValid, string[] repaired) Validate(Input input, string[] update, bool fix = false)
{
    pages.Clear();

    var isValid = true;

    for (var i = update.Length - 1; i >= 0; i--)
    {
        if (pages.Contains(update[i]))
        {
            if (!fix)
            {
                isValid = false;
                break;
            }
            // swap
            (update[i], update[i + 1]) = (update[i + 1], update[i]);

            var (_, repaired) = Validate(input, update, fix);

            return (false, repaired);

        }

        pages.Add(update[^1]);

        if (input.PageOrderingRules.TryGetValue(update[i], out var rules))
        {
            foreach (var page in rules)
            {
                pages.Add(page);
            }
        }
    }

    return (isValid, update);
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

static int CalculateAnswer1(Input input)
{
    var currentSum = 0;

    foreach (var update in input.Updates)
    {
        var (isValid, _) = Validate(input, update);

        if (isValid)
        {
            currentSum += int.Parse(update[update.Length / 2]);
        }
    }


    return currentSum;
}

static int CalculateAnswer2(Input input)
{
    var currentSum = 0;

    foreach (var update in input.Updates)
    {
        var (isValid, repaired) = Validate(input, update, true);

        if (!isValid)
        {
            currentSum += int.Parse(repaired[repaired.Length / 2]);
        }
    }
    return currentSum;
}

static Input ParseFile(string file)
{
    return new Input(File.ReadAllLines(file));
}

public class Input
{
    public readonly Dictionary<string, List<string>> PageOrderingRules = [];
    public readonly List<string[]> Updates = [];
    public Input(string[] inputs)
    {
        var scanningOrdering = true;
        foreach (var input in inputs)
        {
            if (scanningOrdering)
            {
                if (string.IsNullOrEmpty(input))
                {
                    scanningOrdering = false;
                    continue;
                }

                var parts = input.Split("|");
                if (PageOrderingRules.TryGetValue(parts[0], out var current))
                {
                    current.Add(parts[1]);
                }
                else
                {
                    PageOrderingRules[parts[0]] = [parts[1]];
                }
            }
            else
            {
                if (input.StartsWith("//"))
                {
                    continue;
                }
                Updates.Add(input.Split(","));
            }
        }
    }
}

partial class Program
{
    static HashSet<string> pages = [];
}