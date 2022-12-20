using System.Collections.Immutable;
using System.Text.RegularExpressions;

internal class Program
{
    private static readonly Regex regex = new Regex(@"^Blueprint (?<id>\d+):\s+Each ore robot costs (?<ore>\d+) ore.\s+Each clay robot costs (?<clay>\d+) ore.\s+Each obsidian robot costs (?<obsidianore>\d+) ore and (?<obsidianclay>\d+) clay.\s+Each geode robot costs (?<geodeore>\d+) ore and (?<geodeobsidian>\d+) obsidian.", RegexOptions.Multiline | RegexOptions.NonBacktracking);
    private const int MaxTime = 24;

    private static void Main(string[] args)
    {
        var files = Directory.GetFiles("input", "*.txt");

        foreach (var file in files.OrderBy(t => t).Take(1))
        {
            Console.WriteLine($"{file}");

            var fileContent = File.ReadAllText(file);

            var blueprints = regex.Matches(fileContent).Cast<Match>().Select(Parse);

          //  Console.WriteLine(string.Join("\r\n", blueprints));

            var scores = blueprints.ToDictionary(t => t, t => 0L);

            foreach (var blueprint in blueprints)
            {
                var (round,hist) = CalculateBlueprint(blueprint);
                Console.WriteLine($"Blueprint {blueprint.Id}: {round.StoredGeode}");

                var score = round.StoredGeode;
                Console.WriteLine(round);
                Console.WriteLine(string.Join("\r\n", hist));
                Console.WriteLine(score);

               // Console.WriteLine(string.Join("\r\n", hist.Select(h => $"T: {24-h.TimeLeft}: resources {h.StoredOre},{h.StoredClay},{h.StoredObsidian},{h.StoredGeode} - robots: {h.ProductionOre},{h.ProductionClay},{h.ProductionObsidian},{h.ProductionGeode} ")));
                scores[blueprint] = score;
            }
            var part1 = scores.Aggregate(0L, (current, scoredBlueprint) => current + scoredBlueprint.Key.Id * scoredBlueprint.Value);

            Console.WriteLine($"Part 1: {part1}");
        }
    }

    private static (Round, ImmutableList<string>) CalculateBlueprint(Blueprint blueprint)
    {
        var startRound = new Round() { TimeLeft = MaxTime  };
        var candidate = startRound;

        var queue = new Queue<(Round, ImmutableList<string>)>();

        queue.Enqueue((startRound, new List<string> { $"1 Start" }.ToImmutableList()));

        var calculated = new HashSet<Round>();

        var maxRequiredOreProduction = new int[] { blueprint.OreRobotOreCosts, blueprint.ClayRobotOreCosts, blueprint.ObsidianRobotOreCosts, blueprint.GeodeRobotOreCosts }.Max(); ;

        var history = ImmutableList<string>.Empty;

        var cnt = 0;

        while (queue.TryDequeue(out var currentBlob))
        {
            var (current, hist) = currentBlob;
            var minutesLeft = current.TimeLeft;


            if (minutesLeft == 0)
            {
                if (current.StoredGeode > candidate.StoredGeode)
                {
                    candidate = current;
                    history = hist;
                }
                else if (current.StoredGeode == current.StoredGeode)
                {

                    //Console.WriteLine(string.Join("\r\n", hist));
                    //Console.WriteLine($"Geodes: {produced.StoredGeode}");
                }
                continue;
            }


            var productionOre = Math.Min(current.ProductionOre, maxRequiredOreProduction);
            var maxOre = (minutesLeft * maxRequiredOreProduction) - (productionOre * (minutesLeft - 1));
            var storedOre = Math.Min(current.StoredOre + productionOre, maxOre);

            var productionClay = Math.Min(current.ProductionClay, blueprint.ObsidianRobotClayCosts);
            var maxClay = (minutesLeft * blueprint.ObsidianRobotClayCosts) - (productionClay * (minutesLeft - 1));

            var storedClay = Math.Min(current.StoredClay + productionClay, maxClay);

            var productionObsidian = Math.Min(current.ProductionObsidian, blueprint.GeodeRobotObsidianCosts);

            var maxObsidian = (minutesLeft * blueprint.GeodeRobotObsidianCosts) - (productionObsidian * (minutesLeft - 1));
            var storedObsidian = Math.Min(current.StoredObsidian + productionObsidian, maxObsidian);

            hist = hist.Add($"=== Minute {25 - minutesLeft} ===");

            var produced = current with
            {
                TimeLeft = current.TimeLeft - 1,
                StoredOre = storedOre,
                StoredClay = storedClay,
                StoredObsidian = storedObsidian,
                StoredGeode = current.StoredGeode + current.ProductionGeode,
                ProductionOre = productionOre,
                ProductionClay = productionClay,
                ProductionObsidian = productionObsidian
            };

            if (calculated.Contains(produced))
            {
                continue;
            }
            calculated.Add(produced);

            var collected = $"{produced.StoredOre},{produced.StoredClay},{produced.StoredObsidian},{produced.StoredGeode}";

            hist = hist.Add($"Collected resources: from {current.StoredOre},{current.StoredClay},{current.StoredObsidian},{current.StoredGeode} to {collected}");

            var robots = $"{current.ProductionOre},{current.ProductionClay},{current.ProductionObsidian},{current.ProductionGeode}";
            
            hist = hist.Add($"Robots: {robots}");


            if (robots == "1,4,2,1" && ( collected == "4,25,7,2" || collected == ""))
            {
                cnt++;
            }

            if (current.StoredOre >= blueprint.GeodeRobotOreCosts && current.StoredObsidian >= blueprint.GeodeRobotObsidianCosts)
            { 
                
                var geodeStep = produced with
                {
                    ProductionGeode = produced.ProductionGeode + 1,
                    StoredOre = produced.StoredOre - blueprint.GeodeRobotOreCosts,
                    StoredObsidian = produced.StoredObsidian - blueprint.GeodeRobotObsidianCosts
                };

                queue.Enqueue((geodeStep, hist.Add($"Spend {blueprint.GeodeRobotOreCosts},0,{blueprint.GeodeRobotObsidianCosts} on geode ")));
            }
            else
            {


                if (current.StoredOre >= blueprint.ObsidianRobotOreCosts && current.StoredClay >= blueprint.ObsidianRobotClayCosts)
                {
                    var obsidian = produced with
                    {
                        ProductionObsidian = produced.ProductionObsidian + 1,
                        StoredOre = produced.StoredOre - blueprint.ObsidianRobotOreCosts,
                        StoredClay = produced.StoredClay - blueprint.ObsidianRobotClayCosts
                    };
                    queue.Enqueue((obsidian, hist.Add($"Spend {blueprint.ObsidianRobotOreCosts},{blueprint.ObsidianRobotClayCosts},0 clay on obsidian")));
                }


                if (current.StoredOre >= blueprint.ClayRobotOreCosts)
                {
                    var createClayMachine = produced with
                    {
                        ProductionClay = produced.ProductionClay + 1,
                        StoredOre = produced.StoredOre - blueprint.ClayRobotOreCosts
                    };
                    queue.Enqueue((createClayMachine, hist.Add($"Spend {blueprint.ClayRobotOreCosts},0,0 on clay robot")));
                }

                if (current.StoredOre >= blueprint.OreRobotOreCosts)
                {
                    var createOreMachine = produced with
                    {
                        ProductionOre = produced.ProductionOre + 1,
                        StoredOre = produced.StoredOre - blueprint.OreRobotOreCosts
                    };
                    queue.Enqueue((createOreMachine, hist.Add($"Spend {blueprint.OreRobotOreCosts},0,0 on clay robot")));
                }


                queue.Enqueue((produced, hist.Add("Spend a minute")));
            }
        }

        return (candidate, history);
    }

    private static Blueprint Parse(Match match)
    {
        return new Blueprint(
            int.Parse(match.Groups["id"].Value),
            int.Parse(match.Groups["ore"].Value),
            int.Parse(match.Groups["clay"].Value),
            int.Parse(match.Groups["obsidianore"].Value),
            int.Parse(match.Groups["obsidianclay"].Value),
            int.Parse(match.Groups["geodeore"].Value),
            int.Parse(match.Groups["geodeobsidian"].Value)
            );
    }
}


public record Blueprint(
    int Id,
    int OreRobotOreCosts,
    int ClayRobotOreCosts,
    int ObsidianRobotOreCosts,
    int ObsidianRobotClayCosts,
    int GeodeRobotOreCosts,
    int GeodeRobotObsidianCosts
    );

public record Round
{
    public Round()
    {
    }

    public required int TimeLeft { get; init; }

    public int ProductionOre { get; init; } = 1;
    public int ProductionClay { get; init; } = 0;

    public int ProductionObsidian { get; init; } = 0;

    public int ProductionGeode { get; init; } = 0;
    public int StoredOre { get; init; } = 0;
    public int StoredClay { get; init; } = 0;

    public int StoredObsidian { get; init; } = 0;

    public int StoredGeode { get; init; } = 0;
}