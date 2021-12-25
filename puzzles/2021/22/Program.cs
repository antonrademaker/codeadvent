using System.Diagnostics;

var inputFiles = new string[] {
    "input/example.small.txt",
    "input/example.txt",
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
    var instructions = lines.Select(line =>
    {
        var parts = line.Split(' ');
        var turnOn = parts[0] == "on";
        var positions = parts[1].Split(',').Select(s => s[2..].Split("..").Select(int.Parse).ToArray()).ToArray();

        var startPoint = new Vector3(positions[0][0], positions[1][0], positions[2][0]);
        var endPoint = new Vector3(positions[0][1], positions[1][1], positions[2][1]);
        return (turnOn, startPoint, endPoint);
    });

    var grid = new HashSet<Vector3>();

    foreach (var (turnOn, start, end) in instructions)
    {
        if (
            (start.X < -50 && end.X < -50) ||
            (start.Y < -50 && end.Y < -50) ||
            (start.Z < -50 && end.Z < -50) ||
            (start.X > 50 && end.X > 50) ||
            (start.Y > 50 && end.Y > 50) ||
            (start.Z > 50 && end.Z > 50)
            )
        {
            continue;
        }

        var xBoundaryLow = Math.Max(-50, start.X);
        var xBoundaryHigh = Math.Min(100, end.X - start.X + 1);
        foreach (var x in Enumerable.Range(xBoundaryLow, xBoundaryHigh))
        {
            var yBoundaryLow = Math.Max(-50, start.Y);
            var yBoundaryHigh = Math.Min(100, end.Y - start.Y + 1);
            foreach (var y in Enumerable.Range(yBoundaryLow, yBoundaryHigh))
            {
                var zBoundaryLow = Math.Max(-50, start.Z);
                var zBoundaryHigh = Math.Min(100, end.Z - start.Z + 1);
                foreach (var z in Enumerable.Range(zBoundaryLow, zBoundaryHigh))
                {
                    if (turnOn)
                    {
                        grid.Add(new Vector3(x, y, z));
                    }
                    else
                    {
                        grid.Remove(new Vector3(x, y, z));
                    }
                }
            }
        }
    }

    Console.WriteLine($"size: {grid.Count}");
}


void CalculatePart2(string[] lines)
{
    var instructions = lines.Select(line =>
    {
        var parts = line.Split(' ');
        var turnOn = parts[0] == "on";
        var positions = parts[1].Split(',').Select(s => s[2..].Split("..").Select(int.Parse).ToArray()).ToArray();

        var cuboid = new Cuboid(turnOn, positions[0][0], positions[1][0], positions[2][0], positions[0][1], positions[1][1], positions[2][1]);

        return cuboid;
    });

    var cuboids = new List<Cuboid>();

    foreach (var newCuboid in instructions)
    {
        var newCuboids = cuboids.Where(t => newCuboid.Intersects(t)).Select(existingCuboid => newCuboid.Intersect(existingCuboid, !existingCuboid.IsTurnedOn)).ToList();
        
        if (newCuboid.IsTurnedOn)
        {
            cuboids.Add(newCuboid);
        }

        cuboids.AddRange(newCuboids);
    }
    var volume = cuboids.Sum(c => c.Volume() * (c.IsTurnedOn ? 1 :-1));
    Console.WriteLine($"Cuboids: {cuboids.Count}, volume {volume}");    
}

public record struct Vector3(int X, int Y, int Z);

public record struct Cuboid(bool IsTurnedOn, int X1, int Y1, int Z1, int X2, int Y2, int Z2)
{
    public bool Intersects(Cuboid other)
    {
        return !(X1 > other.X2 || X2 < other.X1 || Y1 > other.Y2 || Y2 < other.Y1 || Z1 > other.Z2 || Z2 < other.Z1);
    }
    public Cuboid Intersect(Cuboid other, bool isTurnedOn)
    {
        return new Cuboid(isTurnedOn,
            Math.Max(X1, other.X1),
            Math.Max(Y1, other.Y1),
            Math.Max(Z1, other.Z1),
            Math.Min(X2, other.X2),
            Math.Min(Y2, other.Y2),
            Math.Min(Z2, other.Z2));
    }

    public long Volume()
    {
        return 1L * (X2 - X1 + 1) * (Y2 - Y1 + 1) * (Z2 - Z1 + 1);
    }
}