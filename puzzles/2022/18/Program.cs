using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        var files = Directory.GetFiles("input", "*.txt");

        var regex = new Regex(@"^(?<x>\d+),(?<y>\d+),(?<z>\d+)$", RegexOptions.NonBacktracking);

        foreach (var file in files.OrderBy(t => t))
        {
            Console.WriteLine($"{file}");

            var cubes = File.ReadAllLines(file).Select(l => regex.Match(l))
            .Select(m => new Cube(int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value), int.Parse(m.Groups["z"].Value)))
                .ToList();

            var (droplet, lowerBound, upperBound) = CalculateDroplet(cubes);

            var part1 = droplet.Sum(t => t.Value);

            Console.WriteLine($"  Part 1: {part1}");

            var part2 = CalculatePart2(droplet, lowerBound, upperBound);

            Console.WriteLine($"  Part 2: {part2}");
        }
    }

    private static (Dictionary<Cube, int>, Cube lowerBound, Cube upperBound) CalculateDroplet(List<Cube> cubes)
    {
        var data = new Dictionary<Cube, int>
        {
            { cubes[0], 6 }
        };

        var minX = 10;
        var minY = 10;
        var minZ = 10;

        var maxX = 0;
        var maxY = 0;
        var maxZ = 0;

        foreach (var cube in cubes.Skip(1))
        {
            var surfaces = 6;
            foreach (var side in Sides(cube))
            {
                if (data.TryGetValue(side, out var neighbour))
                {
                    surfaces--;
                    data[side] = neighbour - 1;
                }
            }
            data.Add(cube, surfaces);

            if (cube.X < minX)
            {
                minX = cube.X;
            }
            if (cube.Y < minY)
            {
                minY = cube.Y;
            }
            if (cube.Z < minZ)
            {
                minZ = cube.Z;
            }

            if (cube.X > maxX)
            {
                maxX = cube.X;
            }
            if (cube.Y > maxY)
            {
                maxY = cube.Y;
            }
            if (cube.Z > maxZ)
            {
                maxZ = cube.Z;
            }
        }

        var lowerBound = new Cube(minX - 1, minY - 1, minZ - 1);
        var upperBound = new Cube(maxX + 1, maxY + 1, maxZ + 1);

        return (data, lowerBound, upperBound);
    }

    private static long CalculatePart2(Dictionary<Cube, int> droplet, Cube lowerBound, Cube upperBound)
    {
        var start = lowerBound;

        var water = new HashSet<Cube>();

        var waterQueue = new Queue<Cube>();

        // raise water from 0,0,0
        waterQueue.Enqueue(start);
        water.Add(start);

        var surface = 0;

        while (waterQueue.TryDequeue(out var cube))
        {
            foreach (var side in Sides(cube).Where(
                c =>
                    c.X >= lowerBound.X && c.X <= upperBound.X &&
                    c.Y >= lowerBound.Y && c.Y <= upperBound.Y &&
                    c.Z >= lowerBound.Z && c.Z <= upperBound.Z
                ).Where(t => !water.Contains(t)))
            {
                if (droplet.ContainsKey(side))
                {
                    surface++;
                }
                else
                {
                    waterQueue.Enqueue(side);
                    water.Add(side);
                }
            }
        }

        return surface;
    }

    private static IEnumerable<Cube> Sides(Cube cube)
    {
        return SideVectors.Select(side => side + cube);
    }

    private static readonly Cube[] SideVectors = new Cube[]
    {
        // top
        new Cube(0,-1,0),
        // bottom
        new Cube(0,1,0),
        // left
        new Cube(-1,0,0),
        // right
        new Cube(1,0,0),
        // front
        new Cube(0,0,-1),
        // back
        new Cube(0,0,1)
    };
}

record struct Cube(int X, int Y, int Z)
{
    public static Cube operator +(Cube a, Cube b) => a with { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };
};