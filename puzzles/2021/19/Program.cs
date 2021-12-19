using System.Diagnostics;
using System.Text;

var inputFiles = new string[] {
    "input/example.txt"
    ,
    "input/input.txt"
};
var sw = new Stopwatch();

Console.WriteLine($"{Helpers.SpacialTransformations.Count()}");

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

    var allToFind = scanners.Skip(1).Select(GeneratePermuations);

    var knownBeacons = new List<Vector3>(scanners[0].Beacons);
    var scannerLocations = new List<Vector3>
    {
        Vector3.Zero
    };
    var queue = new Queue<Scanner>();

    foreach (var scanner in allToFind)
    {
        queue.Enqueue(scanner);
    }

    while (queue.TryDequeue(out var scanner))
    {
        if (FindMatch(knownBeacons, scanner, out var offsets, out var beaconBositions))
        {
            knownBeacons.AddRange(beaconBositions.Select(t => t - offsets));
            scannerLocations.Add(offsets);
        }
        else
        {
            queue.Enqueue(scanner);
        }
    }


    Console.WriteLine($"Number: {knownBeacons.Distinct().Count()} ({knownBeacons.Count})");

    var maxDistance = 0;

    for (var i = 0; i < scannerLocations.Count - 1; i++)
    {
        for (var j = i + 1; j < scannerLocations.Count; j++)
        {
            var distance = Vector3.ManhattanDist(scannerLocations[i], scannerLocations[j]);
            maxDistance = Math.Max(maxDistance, distance);
        }
    }

    Console.WriteLine($"Max distance: {maxDistance}");
}


bool FindMatch(List<Vector3> knownBeacons, Scanner scanner, out Vector3 offsets, out IEnumerable<Vector3> beaconBositions)
{
    foreach (var candidate in scanner.Permutations)
    {
        var cnt = new Dictionary<Vector3, int>();

        foreach (var candidateBeacon in candidate)
        {
            foreach (var known in knownBeacons)
            {
                var offset = candidateBeacon - known;
                var matches = 1;
                if (cnt.TryGetValue(offset, out var value))
                {
                    matches += value;
                }
                if (matches >= 12)
                {
                    offsets = offset;
                    beaconBositions = candidate;
                    return true;
                }
                cnt[offset] = matches;
            }
        }
    }
    offsets = default;
    beaconBositions = Enumerable.Empty<Vector3>();
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
        scanner.Beacons.Add(Vector3.Parse(line));
    }
    return scanners;
}

Scanner GeneratePermuations(Scanner scanner)
{
    foreach (var transformation in Helpers.SpacialTransformations)
    {
        scanner.Permutations.Add(scanner.Beacons.Select(transformation).ToList());
    }
    return scanner;
}

public class Scanner
{
    public int Id { get; init; }
    public List<List<Vector3>> Permutations { get; init; } = new();
    public List<Vector3> Beacons { get; init; } = new();
}

public static class Helpers
{
    public static readonly Func<Vector3, Vector3>[] SpacialTransformations = new Func<Vector3, Vector3>[]{
        v => v,

        v => new(v.X, -v.Z, v.Y),
        v => new (v.X, -v.Y, -v.Z),
        v => new (v.X, v.Z, -v.Y),

        v => new (-v.Y, v.X, v.Z),
        v => new (v.Z, v.X, v.Y),
        v => new (v.Y, v.X, -v.Z),
        v => new (-v.Z, v.X, -v.Y),

        v => new (-v.X, -v.Y, v.Z),
        v => new (-v.X, -v.Z, -v.Y),
        v => new (-v.X, v.Y, -v.Z),
        v => new (-v.X, v.Z, v.Y),

        v => new (v.Y, -v.X, v.Z),
        v => new (v.Z, -v.X, -v.Y),
        v => new (-v.Y, -v.X, -v.Z),
        v => new (-v.Z, -v.X, v.Y),

        v => new (-v.Z, v.Y, v.X),
        v => new (v.Y, v.Z, v.X),
        v => new (v.Z, -v.Y, v.X),
        v => new (-v.Y, -v.Z, v.X),

        v => new (-v.Z, -v.Y, -v.X),
        v => new (-v.Y, v.Z, -v.X),
        v => new (v.Z, v.Y, -v.X),
        v => new (v.Y, -v.Z, -v.X)
    };

}

public record struct Vector3(int X, int Y, int Z) : IComparable<Vector3>
{
    public static Vector3 Zero { get; } = new(0, 0, 0);
    public static implicit operator (int X, int Y, int Z)(Vector3 value) => (value.X, value.Y, value.Z);
    public static implicit operator Vector3((int X, int Y, int Z) value) => new(value.X, value.Y, value.Z);
    public static Vector3 Parse(string input)
        => input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray() is { Length: 3 } v
            ? new(v[0], v[1], v[2])
            : throw new FormatException("Invalid dimension!");
    public static Vector3 operator +(Vector3 lhs, Vector3 rhs) => new(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
    public static Vector3 operator -(Vector3 lhs, Vector3 rhs) => new(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
    public static int operator *(Vector3 lhs, Vector3 rhs) => lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
    public static int ManhattanDist(Vector3 lhs, Vector3 rhs) => Math.Abs(lhs.X - rhs.X) + Math.Abs(lhs.Y - rhs.Y) + Math.Abs(lhs.Z - rhs.Z);
    public override string ToString() => $"{X},{Y},{Z}";
    public int CompareTo(Vector3 other)
    {
        var cx = X.CompareTo(other.X);
        if (cx != 0) return cx;
        var cy = Y.CompareTo(other.Y);
        if (cy != 0) return cy;
        var cz = Z.CompareTo(other.Z);
        return cz != 0 ? cz : 0;
    }
}