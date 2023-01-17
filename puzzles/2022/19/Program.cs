using System.Text.RegularExpressions;

internal class Program
{
    private static readonly Regex regex = new Regex(@"^Blueprint (?<id>\d+):\s+Each ore robot costs (?<ore>\d+) ore.\s+Each clay robot costs (?<clay>\d+) ore.\s+Each obsidian robot costs (?<obsidianore>\d+) ore and (?<obsidianclay>\d+) clay.\s+Each geode robot costs (?<geodeore>\d+) ore and (?<geodeobsidian>\d+) obsidian.", RegexOptions.Multiline | RegexOptions.NonBacktracking);
    private const int MaxTimePart1 = 24;
    private const int MaxTimePart2 = 32;

    private static void Main(string[] args)
    {
        var files = Directory.GetFiles("input", "*.txt");

        foreach (var file in files.OrderBy(t => t).Skip(1))
        {
            Console.WriteLine($"{file}");

            var fileContent = File.ReadAllText(file);

            var blueprints = regex.Matches(fileContent).Cast<Match>().Select(Parse).ToList();

            Part1(blueprints);
            Part2(blueprints);
        }
    }

    private static void Part1(IEnumerable<Blueprint> blueprints)
    {
        var scores = blueprints.AsParallel().Select(blueprint => (blueprintId: blueprint.Id, storedGeode: CalculateBlueprint(blueprint, MaxTimePart1).StoredGeode));

        var part1 = scores.Aggregate(0L, (current, scoredBlueprint) => current + scoredBlueprint.blueprintId * scoredBlueprint.storedGeode);

        Console.WriteLine($"Part 1: {part1}");
    }

    private static void Part2(IEnumerable<Blueprint> blueprints)
    {
        var toCalculate = blueprints.Take(3).AsParallel().Select(blueprint => CalculateBlueprint(blueprint, MaxTimePart2).StoredGeode);

        var part2 = toCalculate.Aggregate((current, scoredBlueprint) => current * scoredBlueprint);

        Console.WriteLine($"Part 2: {part2}");
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
                    candidate = current;
                }
                continue;
            }

            var maxRequiredOre = (maxRequiredOreProduction * minutesLeft) - (current.ProductionRateOre * (minutesLeft - 1));

            var productionOre = current.ProductionRateOre;
            var storedOre = Math.Min(current.StoredOre, maxRequiredOre);

            var productionClay = current.ProductionRateClay;
            var storedClay = Math.Min(current.StoredClay, current.TimeLeft * blueprint.ObsidianRobotClayCosts - (current.TimeLeft - 1) * current.ProductionRateObsidian);

            var productionObsidian = current.ProductionRateObsidian;
            var storedObsidian = Math.Min(current.StoredObsidian, current.TimeLeft * blueprint.GeodeRobotObsidianCosts - (current.TimeLeft - 1) * current.ProductionRateObsidian);

            var produced = current with
            {
                StoredOre = storedOre + productionOre,
                StoredClay = storedClay + productionClay,
                StoredObsidian = storedObsidian + productionObsidian,
                StoredGeode = current.StoredGeode + current.ProductionRateGeode,
                ProductionRateOre = productionOre,
                ProductionRateClay = productionClay,
                ProductionRateObsidian = productionObsidian
            };

            if (calculated.Add(produced))
            {
                produced = produced with { TimeLeft = minutesLeft - 1 };
                queue.Enqueue(produced);

                if (current.StoredOre >= blueprint.GeodeRobotOreCosts && current.StoredObsidian >= blueprint.GeodeRobotObsidianCosts)
                {
                    var createGeodeMachine = produced with
                    {
                        ProductionRateGeode = produced.ProductionRateGeode + 1,
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

    public int ProductionRateGeode { get; init; } = 0;
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
        hash.Add(ProductionRateGeode);
        hash.Add(StoredOre);
        hash.Add(StoredClay);
        hash.Add(StoredObsidian);
        hash.Add(StoredGeode);
        return hash.ToHashCode();
    }
}