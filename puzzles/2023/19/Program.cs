using AoC.Utilities;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Coordinate = AoC.Utilities.Coordinate<long>;

namespace AoC.Puzzles.Y2023.D19;

public static partial class Program
{
    private static Regex RatingParser = RatingRegex();

    private static readonly Func<Rating, bool> Accept = (rating) => true;
    private static readonly Func<Rating, bool> Reject = (rating) => false;

    private const string Accepted = "A";
    private const string Rejected = "R";

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
        return 0;
    }

    private static string CalculateRating(Dictionary<string, Workflow> workflows, Rating rating)
    {
        var current = "in";

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
                        var dataSelector = Selector(ruleDefinition[0][0]);
                        var compare = ruleDefinition[0][1];
                        var value = int.Parse(ruleDefinition[0][2..]);
                        var target = ruleDefinition[1];

                        Func<Rating, bool> criterium = compare switch
                        {
                            '<' => r => dataSelector(r) < value,
                            '>' => r => dataSelector(r) > value,
                            _ => throw new Exception($"Unknown compare {compare}")
                        };

                        ruleDefinitions.Add(new Rule(criterium, target));
                    }
                    else
                    {
                        ruleDefinitions.Add(new Rule(Accept, rule));
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

    private static Func<Rating, int> Selector(char categorie)
    {
        return categorie switch
        {
            'x' => r => r.X,
            'm' => r => r.M,
            'a' => r => r.A,
            's' => r => r.S,
            _ => throw new Exception($"Unknown categorie {categorie}")
        };
    }
}

public record Workflow(List<Rule> Rules);

public record struct Rating(int X, int M, int A, int S)
{
    public readonly long Value() => X + M + A + S;
};

public record Rule(Func<Rating, bool> Criterium, string NextWorkflow);