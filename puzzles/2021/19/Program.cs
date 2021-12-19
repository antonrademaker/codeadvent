using System.Diagnostics;
using System.Text;

var inputFiles = new string[] {
    "input/example.txt"
    ,
    "input/input.txt"
};
var sw = new Stopwatch();


foreach (var exampleFile in inputFiles)
{
    Console.WriteLine(exampleFile);
    var file = File.ReadAllLines(exampleFile);
    sw.Start();

    CalculatePart1(file);

    sw.Stop();
    Console.WriteLine($"Part 1 in {sw.ElapsedMilliseconds}ms");
    sw.Reset();
    sw.Start();

    CalculatePart2(file);

    sw.Stop();
    Console.WriteLine($"Part 2 in {sw.ElapsedMilliseconds}ms");

    Console.WriteLine("-- End of file--");
}

void CalculatePart1(string[] lines)
{
    var scanners = ReadScanners(lines);

    var all = scanners.Skip(1).Select(GeneratePermutations).ToDictionary(t => t.Id, t => t);

    all.Add(scanners[0].Id, scanners[0]);

    scanners[0].IsLocated = true;

    var knownBeacons = new List<Beacon>(scanners[0].Beacons.Select(b => new Beacon(b[0], b[1], b[2])));
    var scannerLocations = new List<int[]>();

    var changed = true;

    while (changed)
    {
        changed = false;

        foreach (var scanner in all.Where(t => !t.Value.IsLocated))
        {
            if (FindMatch(knownBeacons, scanner.Value, out int[] offsets, out var beaconBositions))
            {
                changed = true;
                Console.WriteLine($"Found a match :D, scanner: {scanner.Key} offset: {string.Join(',', offsets)}");

                knownBeacons.AddRange(beaconBositions.Select(t => new Beacon(t[0] + offsets[0], t[1] + offsets[1], t[2] + offsets[2])));
                scannerLocations.Add(offsets);
                scanner.Value.IsLocated = true;
            }

        }
    }

    Console.WriteLine($"Number: {knownBeacons.Distinct().Count()} ({knownBeacons.Count})");

    var maxDistance = 0;

    for (var i = 0; i < scannerLocations.Count; i++)
    {
        for (var j = i + 1; j < scannerLocations.Count; j++)
        {
            var distance = Math.Abs(scannerLocations[i][0] - scannerLocations[j][0]) +
                Math.Abs(scannerLocations[i][1] - scannerLocations[j][1]) +
                Math.Abs(scannerLocations[i][2] - scannerLocations[j][2]);
            
            maxDistance = Math.Max(maxDistance, distance);
        }
    }

    Console.WriteLine($"Max distance: {maxDistance}");
}


bool FindMatch(List<Beacon> knownBeacons, Scanner scanner, out int[] offsets, out IEnumerable<int[]> beaconBositions)
{
    foreach (var candidate in scanner.Permutations)
    {
        var cnt = new Dictionary<string, int>();

        foreach (var candidateBeacon in candidate)
        {
            foreach (var known in knownBeacons)
            {
                var p = new int[3] { known.X - candidateBeacon[0], known.Y - candidateBeacon[1], known.Z - candidateBeacon[2] };
                var pKey = string.Join(',', p);
                var matches = 1;
                if (cnt.TryGetValue(pKey, out var value))
                {
                    matches += value;
                }
                cnt[pKey] = matches;
                if (matches >= 12)
                {
                    offsets = p;
                    beaconBositions = candidate;
                    return true;
                }
            }
        }
    }
    offsets = Array.Empty<int>();
    beaconBositions = Enumerable.Empty<int[]>();
    return false;
}

void CalculatePart2(string[] line)
{

}

List<Scanner> ReadScanners(string[] lines)
{
    var scanner = new Scanner();
    var scanners = new List<Scanner>();
    foreach (var line in lines)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }
        if (line.StartsWith("---"))
        {
            // new scanner
            var id = int.Parse(line.Replace("-", "").Replace("scanner", "").Replace(" ", ""));
            scanner = new Scanner { Id = id };
            scanners.Add(scanner);
            continue;
        }
        var location = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
        scanner.Beacons.Add(location);
    }
    return scanners;
}

Scanner GeneratePermutations(Scanner scanner)
{
    var axis = new int[] { -1, 1 };
    var permutations = new List<int> { 0, 1, 2 }.GetPermutations(3).Select(t => t.ToArray()).ToArray();

    for (var xaxis = 0; xaxis < 2; xaxis++)
    {
        for (var yaxis = 0; yaxis < 2; yaxis++)
        {
            for (var zaxis = 0; zaxis < 2; zaxis++)
            {
                for (var perm = permutations.Length - 1; perm >= 0; perm--)
                {
                    var beaconPerms = new List<int[]>();
                    foreach (var b in scanner.Beacons)
                    {
                        var permutation = new int[3];

                        permutation[permutations[perm][0]] = b[0] * axis[xaxis];
                        permutation[permutations[perm][1]] = b[1] * axis[yaxis];
                        permutation[permutations[perm][2]] = b[2] * axis[zaxis];

                        beaconPerms.Add(permutation);

                    }
                    scanner.Permutations.Add(beaconPerms);
                }
            }
        }

    }
    return scanner;
}

public class Scanner
{
    public int Id { get; init; }
    public List<int[]> Beacons { get; init; } = new();
    public List<List<int[]>> Permutations { get; init; } = new();

    public bool IsLocated { get; set; }
}

public static class Helpers
{
    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length)
    {
        if (length == 1) return list.Select(t => new T[] { t });

        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(e => !t.Contains(e)),
                (t1, t2) => t1.Concat(new T[] { t2 }));
    }

}

public record Beacon(int X, int Y, int Z);