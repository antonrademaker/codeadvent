using System.Collections.Immutable;
using System.Text.RegularExpressions;

var files = Directory.GetFiles("input", "*.txt");

var regex = new Regex("^Valve (?<name>\\w\\w) has flow rate=(?<rate>\\d+); tunnel[s]? lead[s]? to valve[s]? (?<tunnels>((\\w\\w(, ){0,1})+))$", RegexOptions.NonBacktracking);

foreach (var file in files.OrderBy(t => t).Take(1))
{
    Console.WriteLine($"==============={file}==============");

    var input = File.ReadAllLines(file).Select(l => regex.Match(l))
    .Select(m => new Valve(m.Groups["name"].Value, int.Parse(m.Groups["rate"].Value), m.Groups["tunnels"].Value.Split(',', StringSplitOptions.TrimEntries).ToImmutableArray()))
        .ToDictionary(k => k.Name, k => k);

    var part1 = CalculatePart1(input);

    Console.WriteLine($"  Part 1: {part1}");

    //var part2 = CalculatePart2(input, file.Contains("example") ? 20 : 4000000);

    //Console.WriteLine($"  Part 2: {part2}");
}

static IList<(string source, string target, int weight, int rate)> GetEdges(Dictionary<string, Valve> valves, string source, int currentWeight, string target, HashSet<string> visited)
{
    var valve = valves[target];

    var results = new List<(string source, string target, int weight, int rate)>();

    if (valve.Rate == 0)
    {
        Console.WriteLine($"  skipping {target} (rate: {valve.Rate})");
        foreach (var nextValve in valve.Tunnels.Where(v => !visited.Contains(v)))
        {
            visited.Add(nextValve);

            results.AddRange(GetEdges(valves, source, currentWeight + 1, nextValve, visited));
        }
    }
    else
    {
        Console.WriteLine($"  target {target} (rate: {valve.Rate})");

        results.Add((source, target, currentWeight, valve.Rate));
    }
    return results;
}


static Dictionary<string, Dictionary<string, (int weight, int rate)>> CalculateEdges(Dictionary<string, Valve> valves)
{
    var edges = new Dictionary<string, Dictionary<string, (int weight, int rate)>>();

    var vQueue = new Queue<string>();
    vQueue.Enqueue("AA");

    var visited = new HashSet<string>() { "AA" };

    while (vQueue.TryDequeue(out var valveId))
    {
        var valve = valves[valveId];


        var results = new List<(string source, string target, int weight, int rate)>();

        foreach (var tunnel in valve.Tunnels)
        {
            Console.WriteLine($"Checking {valveId} => {tunnel} ");
            var calculatedEdges = GetEdges(valves, valveId, 1, tunnel, new HashSet<string>() { valveId });

            foreach (var calc in calculatedEdges)
            {
                if (!edges.TryGetValue(calc.source, out var targets))
                {
                    targets = new Dictionary<string, (int weight, int rate)>();
                }

                if (!targets.TryGetValue(calc.source, out var data) || data.weight > calc.weight)
                {
                    Console.WriteLine($"Adding {valveId} => {calc.target} ({calc.weight})");

                    targets[calc.source] = (calc.weight, calc.rate);

                    if (!visited.Contains(calc.target))
                    {
                        Console.WriteLine($"Queue {calc.target}");

                        vQueue.Enqueue(calc.target);
                        visited.Add(calc.target);
                    }
                    else
                    {
                        Console.WriteLine($"Not queueing {calc.target}");

                    }
                }

                edges[calc.source] = targets;
            }
        }
    }

    return edges;
}

static int CalculatePart1(Dictionary<string, Valve> valves)
{
    var edges = CalculateEdges(valves);

    //var startPath = new List<string>() { "AA" }.ToImmutableArray();
    //var open = new List<string>().ToImmutableArray();

    //var queue = new Queue<(ImmutableArray<string> path, ImmutableArray<string> open, int timeLeft, int value)>();

    var openableValves = valves.Values.Where(t => t.Rate > 0).Select(t => t.Name).ToImmutableHashSet();


    var best = CalculateBest(edges, openableValves);


    return best;
}

static int CalculateBest(Dictionary<string, Dictionary<string, (int weight, int rate)>> edges, ImmutableHashSet<string> openableValves, string current = "AA", int timeLeft = 30, int currentBest)
{
    if (openableValves.Count == 0)
    {
        return currentBest;
    }

    var best = currentBest;


    foreach (var edge in edges[current])
    {

    }

    return best;
}

//static long CalculatePart2(List<(Point sensor, Point beacon)> fileInput, int size)
//{
//    for (var row = 0; row < size; row++)
//    {
//        var (ranges, beacons) = CalculateRow(fileInput, row);

//        var gaps = FindGaps(ranges, beacons, size);

//        if (gaps.Any())
//        {
//            return gaps.First() * 4000000L + row;
//        }
//    }
//    return -1;
//}

public record Valve(string Name, int Rate, ImmutableArray<string> Tunnels)
{
}