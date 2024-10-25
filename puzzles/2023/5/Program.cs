using System.Text.RegularExpressions;
using System.Diagnostics;

object lck = new object();

string[] inputFiles = ["input/example.txt", "input/input.txt"];

foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var location = CalculatePart1(inputs);

    var part1 = location;

    Console.WriteLine($"Part 1: {part1}");

    var part2 = CalculatePart2(inputs);

    Console.WriteLine($"Part 2: {part2}");
}

long CalculatePart1(string[] inputs)
{
    var (seeds, mappings, maps) = PreCalculate(inputs);

    var locations = new List<long>(seeds.Count);

    foreach (var seed in seeds)
    {
        locations.Add(CalculateLocation(mappings, maps, seed));
    }

    return locations.Min();
}

long CalculatePart2(string[] inputs)
{
    var (seeds, mappings, maps) = PreCalculate(inputs);

    var location = long.MaxValue;

    var seedRanges = new List<(long start, long length)>();

    for (var index = 0; index < seeds.Count / 2; index++)
    {
        var start = seeds[index * 2];
        var length = seeds[index * 2 + 1];

        seedRanges.Add((start, length));
    }

    Parallel.ForEach(seedRanges, (seedRange, _) =>
    {
        var loc = CalculateRange(mappings, maps, seedRange.start, seedRange.length);
        lock (lck)
        {
            Console.WriteLine($"Finished {seedRange}: {loc}");
            if (loc < location)
            {
                location = loc;
            }
        }
    });

    return location;
}

static long CalculateRange(Dictionary<string, List<Mapping>> mappings, List<string> maps, long start, long length)
{
    var location = long.MaxValue;

    for (var seedOffset = 0; seedOffset < length; seedOffset++)
    {
        var seed = start + seedOffset;

        var result = CalculateLocation(mappings, maps, seed);
        
        if (result < location)
        {
            location = result;
        }
    }
    return location;
}
static (List<long> seeds, Dictionary<string, List<Mapping>> mappings, List<string> maps) PreCalculate(string[] inputs)
{
    Regex mappingParser = new Regex(@"^(?<mapSource>.*)\-to\-(?<mapDestination>.*)\smap:$", RegexOptions.Compiled | RegexOptions.Multiline);

    var seeds = inputs[0][7..].Split(' ').Select(long.Parse).ToList();
    var current = 1;

    var sourceCategory = string.Empty; var destinationCategory = string.Empty;

    var mappings = new Dictionary<string, List<Mapping>>();
    var maps = new List<string>() { "seed" };
    while (current < inputs.Length)
    {
        if (inputs[current] == string.Empty)
        {
            current++;

            var group = mappingParser.Match(inputs[current]);

            sourceCategory = group.Groups["mapSource"].Value;
            destinationCategory = group.Groups["mapDestination"].Value;

            maps.Add(destinationCategory);

            mappings.Add(sourceCategory, []);
        }
        else
        {
            var map = inputs[current].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

            mappings[sourceCategory].Add(new Mapping(map[0], map[1], map[2]));
        }
        current++;
    }

    return (seeds, mappings, maps);
}

static long CalculateLocation(Dictionary<string, List<Mapping>> mappings, List<string> maps, long seed)
{
    var currentValue = seed;

    foreach (var map in maps)
    {
        if (mappings.TryGetValue(map, out var categoryMaps))
        {
            var mapping = categoryMaps.FirstOrDefault(s => s.Contains(currentValue));
            if (mapping.RangeLength > 0)
            {
                currentValue = currentValue - mapping.Source + mapping.Destination;
            }
        }
        else
        {
            return currentValue;
        }
    }
    return -1;
}

record struct Mapping(long Destination, long Source, long RangeLength)
{
    public readonly bool Contains(long value) => value - Source < RangeLength && value - Source >= 0;
}