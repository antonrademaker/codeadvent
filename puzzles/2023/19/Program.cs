using System.Linq;
using System.Text.RegularExpressions;
using Range = AoC.Utilities.Range<int>;

namespace AoC.Puzzles.Y2023.D19;

public static partial class Program
{
    private static Regex RatingParser = RatingRegex();

    private static readonly Func<Rating, bool> Accept = (rating) => true;

    private static readonly char[] Categories = ['x', 'm', 'a', 's'];

    private const string Accepted = "A";
    private const string Rejected = "R";
    private const char LessThan = '<';
    private const char GreaterThan = '>';

    private const string StartWorkflow = "in";

    private static void Main(string[] args)
    {
        string[] inputFiles = [
            "input/example.txt"
            , "input/input.txt"
            ];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var part1 = CalculatePart1(inputs);

            Console.WriteLine($"Part 1: {part1}");

            var part2 = CalculatePart2(inputs);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static long CalculatePart1(string[] input)
    {
        var (workflows, ratings) = ParseInput(input);

        return ratings
            .Select(rating => (rating, result: CalculateRating(workflows, rating)))
            .Where(r => r.result == "A")
            .Sum(r => r.rating.Value());
    }

    private static long CalculatePart2(string[] input)
    {
        var (workflows, _) = ParseInput(input);

        // use ranges for each category

        var queue = new Queue<WorkflowRanges>();

        queue.Enqueue(new WorkflowRanges(StartWorkflow, Categories.ToDictionary(t => t, t => new Range(1, 4000))));

        var accepted = new List<WorkflowRanges>();

        while (queue.TryDequeue(out var current))
        {
            foreach (var rule in workflows[current.Name].Rules)
            {
                if (rule.Criterium == Accept)
                {
                    if (rule.NextWorkflow == Accepted)
                    {
                        accepted.Add(current);
                    }
                    else if (rule.NextWorkflow != Rejected)
                    {
                        queue.Enqueue(current with { Name = rule.NextWorkflow });
                    }
                }
                else if (rule.Compare == LessThan)
                {
                    if (current.Ranges[rule.Category].End < rule.Value)
                    {
                        if (rule.NextWorkflow == Accepted)
                        {
                            accepted.Add(current);
                        }
                        else if (rule.NextWorkflow != Rejected)
                        {
                            queue.Enqueue(current with { Name = rule.NextWorkflow });
                        }
                    }
                    else if (current.Ranges[rule.Category].Start < rule.Value)
                    {
                        var tmp = current.Copy();
                        tmp.Ranges[rule.Category] = new(tmp.Ranges[rule.Category].Start, rule.Value - 1);
                        current.Ranges[rule.Category] = new(rule.Value, current.Ranges[rule.Category].End);
                        if (rule.NextWorkflow == Accepted)
                        {
                            accepted.Add(tmp);
                        }
                        else if (rule.NextWorkflow != Rejected)
                        {
                            queue.Enqueue(new(rule.NextWorkflow, tmp.Ranges));
                        }
                    }
                }
                else
                {
                    if (current.Ranges[rule.Category].Start > rule.Value)
                    {
                        if (rule.NextWorkflow == Accepted)
                        {
                            accepted.Add(current);
                        }
                        else if (rule.NextWorkflow != Rejected)
                        {
                            queue.Enqueue(current with { Name = rule.NextWorkflow });
                        }
                    }
                    else if (current.Ranges[rule.Category].End > rule.Value)
                    {
                        var newRange = current.Copy();
                        newRange.Ranges[rule.Category] = new(rule.Value + 1, newRange.Ranges[rule.Category].End);
                        current.Ranges[rule.Category] = new(current.Ranges[rule.Category].Start, rule.Value);
                        if (rule.NextWorkflow == Accepted)
                        {
                            accepted.Add(newRange);
                        }
                        else if (rule.NextWorkflow != Rejected)
                        {
                            queue.Enqueue(new(rule.NextWorkflow, newRange.Ranges));
                        }
                    }
                }
            }
        }

        return accepted.Sum(range =>
        {
            return range.Ranges.Values.Aggregate(1L, (cur, next) => cur * next.Length);
        });
    }

    private static string CalculateRating(Dictionary<string, Workflow> workflows, Rating rating)
    {
        var current = StartWorkflow;

        while (current != Accepted && current != Rejected)
        {
            var workflow = workflows[current];
            current = workflow.Rules.First(rule => rule.Criterium(rating)).NextWorkflow;
        }

        return current;
    }

    private static (Dictionary<string, Workflow> workflows, Rating[] ratings) ParseInput(string[] input)
    {
        var workflows = new Dictionary<string, Workflow>();
        var ratings = new List<Rating>();
        var parsingWorkflows = true;

        for (var line = 0; line < input.Length; line++)
        {
            if (parsingWorkflows)
            {
                if (string.IsNullOrWhiteSpace(input[line]))
                {
                    parsingWorkflows = false;
                    continue;
                }

                var splitted = input[line].Split('{');

                var name = splitted[0];
                var definition = splitted[1][0..^1];
                var rules = definition.Split(',');

                var ruleDefinitions = new List<Rule>();

                foreach (var rule in rules)
                {
                    if (rule.Contains(':'))
                    {
                        var ruleDefinition = rule.Split(':');
                        var category = ruleDefinition[0][0];
                        var dataSelector = Selector(category);
                        var compare = ruleDefinition[0][1];
                        var value = int.Parse(ruleDefinition[0][2..]);
                        var target = ruleDefinition[1];

                        Func<Rating, bool> criterium = compare switch
                        {
                            LessThan => r => dataSelector(r) < value,
                            GreaterThan => r => dataSelector(r) > value,
                            _ => throw new Exception($"Unknown compare {compare}")
                        };

                        ruleDefinitions.Add(new Rule(criterium, category, compare, value, target));
                    }
                    else
                    {
                        ruleDefinitions.Add(new Rule(Accept, '_', '_', 0, rule));
                    }
                }

                workflows.Add(name, new Workflow(ruleDefinitions));
            }
            else
            {
                var match = RatingParser.Match(input[line]);

                if (!match.Success)
                {
                    throw new Exception($"Failed to parse rating on line {line}");
                }

                ratings.Add(new Rating(
                    int.Parse(match.Groups["x"].Value),
                    int.Parse(match.Groups["m"].Value),
                    int.Parse(match.Groups["a"].Value),
                    int.Parse(match.Groups["s"].Value)
                    ));
            }
        }

        return (workflows, ratings.ToArray());
    }

    [GeneratedRegex("\\{x=(?<x>\\d+),m=(?<m>\\d+),a=(?<a>\\d+),s=(?<s>\\d+)\\}", RegexOptions.Compiled)]
    private static partial Regex RatingRegex();

    private static Func<Rating, int> Selector(char category)
    {
        return category switch
        {
            'x' => r => r.X,
            'm' => r => r.M,
            'a' => r => r.A,
            's' => r => r.S,
            _ => throw new Exception($"Unknown category {category}")
        };
    }
}

public record Workflow(List<Rule> Rules);

public record struct Rating(int X, int M, int A, int S)
{
    public readonly long Value() => X + M + A + S;
};

public record Rule(Func<Rating, bool> Criterium, char Category, char Compare, int Value, string NextWorkflow);

public record WorkflowRanges(string Name, Dictionary<char, Range> Ranges)
{
    public WorkflowRanges Copy()
    {
        return this with { Ranges = Ranges.ToDictionary() };
    }
}