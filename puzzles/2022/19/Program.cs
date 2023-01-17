using System.Collections.Immutable;
using System.Text.RegularExpressions;

internal class Program
{
    private static readonly Regex regex = new Regex(@"^Blueprint (?<id>\d+):\s+Each ore robot costs (?<ore>\d+) ore.\s+Each clay robot costs (?<clay>\d+) ore.\s+Each obsidian robot costs (?<obsidianore>\d+) ore and (?<obsidianclay>\d+) clay.\s+Each geode robot costs (?<geodeore>\d+) ore and (?<geodeobsidian>\d+) obsidian.", RegexOptions.Multiline | RegexOptions.NonBacktracking);
    private const int MaxTimePart1 = 24;
    private const int MaxTimePart2 = 32;

    private static void Main(string[] args)
    {
        var files = Directory.GetFiles("input", "*.txt");

        foreach (var file in files.OrderBy(t => t).Take(1))
        {
            Console.WriteLine($"{file}");

            var fileContent = File.ReadAllText(file);

            var blueprints = regex.Matches(fileContent).Cast<Match>().Select(Parse);

            //  Console.WriteLine(string.Join("\r\n", blueprints));
            Part1(blueprints);
            Part2(blueprints);
        }
    }

    private static void Part1(IEnumerable<Blueprint> blueprints)
    {
        var scores = blueprints.ToDictionary(t => t, t => 0L);

        Parallel.ForEach(blueprints, blueprint =>
        {            
            scores[blueprint] = CalculateBlueprint(blueprint, MaxTimePart1).StoredGeode;
        });

        var part1 = scores.Aggregate(0L, (current, scoredBlueprint) => current + scoredBlueprint.Key.Id * scoredBlueprint.Value);

        Console.WriteLine($"Part 1: {part1}");
    }

    private static void Part2(IEnumerable<Blueprint> blueprints)
    {
        var scores = blueprints.ToDictionary(t => t, t => 0L);

        Parallel.ForEach(blueprints.Take(3), blueprint =>
        {
            scores[blueprint] = CalculateBlueprint(blueprint, MaxTimePart2).StoredGeode;
        });

        var part1 = scores.Aggregate(1L, (current, scoredBlueprint) => current * scoredBlueprint.Value);

        Console.WriteLine($"Part 2: {part1}");
    }

    private static Round CalculateBlueprint(Blueprint blueprint, int time)
    {
        var startRound = new Round() { TimeLeft = time };
        var candidate = startRound;

        var queue = new Queue<Round>();

        queue.Enqueue(startRound);

        var calculated = new HashSet<Round>();

        var maxRequiredOreProduction = new int[] { blueprint.OreRobotOreCosts, blueprint.ClayRobotOreCosts, blueprint.ObsidianRobotOreCosts, blueprint.GeodeRobotOreCosts }.Max();


        while (queue.TryDequeue(out var currentBlob))
        {
            var current = currentBlob;
            var minutesLeft = current.TimeLeft;

            if (minutesLeft == 0)
            {
                if (current.StoredGeode > candidate.StoredGeode)
                {
                    Console.WriteLine($"Found {current.StoredGeode}");
                    candidate = current;
                }
                else if (current.StoredGeode == candidate.StoredGeode)
                {
                    //Console.WriteLine(string.Join("\r\n", hist));
                    //Console.WriteLine($"Geodes: {produced.StoredGeode}");
                }
                continue;
            }

            var productionOre = current.ProductionRateOre;
            var storedOre = Math.Min(current.StoredOre + productionOre, maxRequiredOreProduction * current.TimeLeft);

            var productionClay = current.ProductionRateClay;
            var storedClay = current.StoredClay + productionClay;

            var productionObsidian = current.ProductionRateObsidian;
            var storedObsidian = current.StoredObsidian + productionObsidian;

            var storedGeode = current.StoredGeode + current.ProductionGeode;

            var produced = current with
            {
                TimeLeft = current.TimeLeft - 1,
                StoredOre = storedOre,
                StoredClay = storedClay,
                StoredObsidian = storedObsidian,
                StoredGeode = storedGeode,
                ProductionRateOre = productionOre,
                ProductionRateClay = productionClay,
                ProductionRateObsidian = productionObsidian
            };

            if (calculated.Contains(produced))
            {
                continue;
            }

            calculated.Add(produced);

            queue.Enqueue(produced);

            if (current.StoredOre >= blueprint.GeodeRobotOreCosts && current.StoredObsidian >= blueprint.GeodeRobotObsidianCosts)
            {
                var createGeodeMachine = produced with
                {
                    ProductionGeode = produced.ProductionGeode + 1,
                    StoredOre = produced.StoredOre - blueprint.GeodeRobotOreCosts,
                    StoredObsidian = produced.StoredObsidian - blueprint.GeodeRobotObsidianCosts
                };

                queue.Enqueue(createGeodeMachine);
            }

            if (current.StoredOre >= blueprint.ObsidianRobotOreCosts && current.StoredClay >= blueprint.ObsidianRobotClayCosts && produced.ProductionRateObsidian < blueprint.GeodeRobotObsidianCosts)
            {
                var createObsidianMachine = produced with
                {
                    ProductionRateObsidian = produced.ProductionRateObsidian + 1,
                    StoredOre = produced.StoredOre - blueprint.ObsidianRobotOreCosts,
                    StoredClay = produced.StoredClay - blueprint.ObsidianRobotClayCosts
                };
                queue.Enqueue(createObsidianMachine);
            }

            if (current.StoredOre >= blueprint.ClayRobotOreCosts && current.ProductionRateClay < blueprint.ObsidianRobotClayCosts)
            {
                var createClayMachine = produced with
                {
                    ProductionRateClay = produced.ProductionRateClay + 1,
                    StoredOre = produced.StoredOre - blueprint.ClayRobotOreCosts
                };
                queue.Enqueue(createClayMachine);
            }

            if (current.StoredOre >= blueprint.OreRobotOreCosts && current.ProductionRateOre < maxRequiredOreProduction)
            {
                var createOreMachine = produced with
                {
                    ProductionRateOre = produced.ProductionRateOre + 1,
                    StoredOre = produced.StoredOre - blueprint.OreRobotOreCosts
                };
                queue.Enqueue(createOreMachine);
            }

        }

        return candidate;
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

public sealed record Blueprint(
    int Id,
    int OreRobotOreCosts,
    int ClayRobotOreCosts,
    int ObsidianRobotOreCosts,
    int ObsidianRobotClayCosts,
    int GeodeRobotOreCosts,
    int GeodeRobotObsidianCosts
    );

public sealed record Round
{
    public Round()
    {
    }

    public required int TimeLeft { get; init; }

    public int ProductionRateOre { get; init; } = 1;
    public int ProductionRateClay { get; init; } = 0;

    public int ProductionRateObsidian { get; init; } = 0;

    public int ProductionGeode { get; init; } = 0;
    public int StoredOre { get; init; } = 0;
    public int StoredClay { get; init; } = 0;

    public int StoredObsidian { get; init; } = 0;

    public int StoredGeode { get; init; } = 0;

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(TimeLeft);
        hash.Add(ProductionRateOre);
        hash.Add(ProductionRateClay);
        hash.Add(ProductionRateObsidian);
        hash.Add(ProductionGeode);
        hash.Add(StoredOre);
        hash.Add(StoredClay);
        hash.Add(StoredObsidian);
        hash.Add(StoredGeode);
        return hash.ToHashCode();
    }
}