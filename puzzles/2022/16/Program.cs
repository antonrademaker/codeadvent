using System.Collections.Immutable;
using System.Text.RegularExpressions;
using System.Linq;

var files = Directory.GetFiles("input", "*.txt");

var regex = new Regex("^Valve (?<name>\\w\\w) has flow rate=(?<rate>\\d+); tunnel[s]? lead[s]? to valve[s]? (?<tunnels>((\\w\\w(, ){0,1})+))$", RegexOptions.NonBacktracking);

foreach (var file in files.OrderBy(t => t))
{
    Console.WriteLine($"==============={file}==============");

    var input = File.ReadAllLines(file).Select(l => regex.Match(l))
    .Select(m => new Valve(m.Groups["name"].Value, int.Parse(m.Groups["rate"].Value), m.Groups["tunnels"].Value.Split(',', StringSplitOptions.TrimEntries).ToImmutableArray()))
        .ToDictionary(k => k.Name, k => k);

    var edges = CalculateEdges(input);

    var part1 = CalculatePart1(input, edges);

    Console.WriteLine($"  Part 1: {part1}");

    var part2 = CalculatePart2(input, edges);

    Console.WriteLine($"  Part 2: {part2}");
}

static IList<(string source, string target, int weight, int rate)> GetEdges(Dictionary<string, Valve> valves, string source, int currentWeight, string target, HashSet<string> visited)
{
    var valve = valves[target];

    var results = new List<(string source, string target, int weight, int rate)>();
    foreach (var nextValve in valve.Tunnels.Where(v => !visited.Contains(v)))
    {
        visited.Add(nextValve);

        results.AddRange(GetEdges(valves, source, currentWeight + 1, nextValve, visited));
        if (valve.Rate > 0)
        {
            results.Add((source, target, currentWeight, valve.Rate));
        }
    }
    return results;
}

static Dictionary<string, (int weight, int rate)> CalculateDistances(Dictionary<string, Valve> valves, string start)
{
    var distances = new Dictionary<string, (int weight, int rate)>();

    var vQueue = new PriorityQueue<string, int>();
    vQueue.Enqueue(start, 0);

    var visited = new HashSet<string>();

    while (vQueue.TryDequeue(out var valve, out var prio))
    {
        var valveData = valves[valve];
        if (valveData.Rate > 0 && (!distances.TryGetValue(valve, out var data) || data.weight > prio))
        {
            distances[valve] = (prio, valveData.Rate);
        }

        foreach (var tunnel in valveData.Tunnels)
        {
            if (!visited.Contains(tunnel))
            {
                vQueue.Enqueue(tunnel, prio + 1);
                visited.Add(tunnel);
            }
        }
    }

    return distances;
}

static Dictionary<string, Dictionary<string, (int weight, int rate)>> CalculateEdges(Dictionary<string, Valve> valves)
{
    return valves.Keys.Select(valve => (valve, distances: CalculateDistances(valves, valve))).ToDictionary(k => k.valve, k => k.distances);
}

static int CalculatePart1(Dictionary<string, Valve> valves, Dictionary<string, Dictionary<string, (int weight, int rate)>> edges)
{
    var openableValves = valves.Values.Where(t => t.Rate > 0).Select(t => t.Name).ToImmutableHashSet();

    var best = CalculateBest(edges, openableValves);

    return best;
}

static int CalculatePart2(Dictionary<string, Valve> valves, Dictionary<string, Dictionary<string, (int weight, int rate)>> edges)
{
    var openableValves = valves.Values.Where(t => t.Rate > 0).Select(t => t.Name).ToImmutableHashSet();

    var best = CalculateBestWithElephant(edges, openableValves);

    return best;
}

static int CalculateBest(Dictionary<string, Dictionary<string, (int weight, int rate)>> edges, ImmutableHashSet<string> openableValves, string current = "AA", int timeLeft = 30, int currentPressure = 0)
{
    if (openableValves.Count == 0 || timeLeft < 0)
    {
        return currentPressure;
    }

    var best = currentPressure;
    foreach (var edge in edges[current].Where(edge => openableValves.Contains(edge.Key)))
    {
        var newTimeLeft = timeLeft - edge.Value.weight - 1;
        var candidateToOpen = CalculateBest(edges, openableValves.Remove(edge.Key), edge.Key, newTimeLeft, currentPressure + (edge.Value.rate * newTimeLeft));
        if (candidateToOpen > best)
        {
            best = candidateToOpen;
        }
    }

    return best;
}


static int CalculateBestWithElephant(Dictionary<string, Dictionary<string, (int weight, int rate)>> edges, ImmutableHashSet<string> openableValves, string current1 = "AA", string current2 = "AA", int timeLeft1 = 26, int timeLeft2 = 26, int currentPressure = 0)
{
    if (openableValves.Count == 0 || timeLeft1 <= 0 || timeLeft2 <= 0)
    {
        return currentPressure;
    }

    var best = currentPressure;
    foreach (var newCurrent1 in openableValves)
    {
        var edge = edges[current1][newCurrent1];

        var newTimeLeft1 = timeLeft1 - edge.weight - 1;

        if (newTimeLeft1 > 0)
        {
            var openableValvesNext1 = openableValves.Remove(newCurrent1);

            var newPressure1 = currentPressure + (edge.rate * newTimeLeft1);

            foreach (var edgeCode2 in openableValvesNext1)
            {
                var (weight, rate) = edges[current2][edgeCode2];
                var newTimeLeft2 = timeLeft2 - weight - 1;
                if (newTimeLeft2 > 0)
                {
                    var openableValvesNext2 = openableValvesNext1.Remove(edgeCode2);
                    var newPressure2 = newPressure1 + (rate * newTimeLeft2);

                    var candidateToOpen = CalculateBestWithElephant(edges, openableValvesNext2, newCurrent1, edgeCode2, newTimeLeft1, newTimeLeft2, newPressure2);
                    if (candidateToOpen > best)
                    {
                        best = candidateToOpen;
                    }
                }
            }

            var bestAlone = CalculateBestWithElephant(edges, openableValvesNext1, newCurrent1, current2, newTimeLeft1, timeLeft2, newPressure1);
            if (bestAlone > best)
            {
                best = bestAlone;
            }
        }
    }
    return best;
}

public record Valve(string Name, int Rate, ImmutableArray<string> Tunnels)
{
}