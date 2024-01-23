using AoC.Utilities;
using System.Buffers;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Coordinate = AoC.Utilities.Coordinate<int>;

namespace AoC.Puzzles.Y2023.D21;

public static partial class Program
{
    private const string FTModuleName = "ft";

    private static void Main(string[] args)
    {
        List<string> inputFiles = [
            "input/example.txt",
            "input/input.txt"
           ];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var (part1, part2) = Calculate(inputs);

            Console.WriteLine($"Part 1: {part1}");

            //var part2 = CalculatePart2(inputs, 26501365);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static void Print(Dictionary<Vector3, int> grid, Brick[] bricks)
    {
        Console.WriteLine("X:");
        PrintX(grid, bricks);
        Console.WriteLine("Y:");
        PrintY(grid, bricks);
    }

    private static void PrintX(Dictionary<Vector3, int> grid, Brick[] bricks)
    {
        var sb = new StringBuilder();

        var maxZ = bricks.Max(t => t.Z2);
        var minZ = bricks.Min(t => t.Z1);

        var maxX = bricks.Max(t => t.X2);
        var minX = bricks.Min(t => t.X1);

        var maxY = bricks.Max(t => t.Y2);
        var minY = bricks.Min(t => t.Y1);

        for (int x = minX; x <= maxX; x++)
        {
            sb.Append(x);
        }
        sb.AppendLine();

        for (int z = maxZ; z >= minZ; z--)
        {
            for (int x = minX; x <= maxX; x++)
            {
                var bricksInLine = Enumerable.Range(minY, maxY - minY + 1).Select(y => new Vector3(x, y, z)).Where(p => grid.ContainsKey(p)).Select(t => grid[t]).Distinct().ToList();

                if (bricksInLine.Count == 0)
                {
                    sb.Append('.');
                }
                else if (bricksInLine.Count == 1)
                {
                    sb.Append(Convert.ToChar(bricks[bricksInLine[0]].BrickId + 'A'));
                }
                else
                {
                    sb.Append(bricksInLine.Count % 10);
                }
            }

            sb.Append($" {z}");

            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }

    private static void PrintY(Dictionary<Vector3, int> grid, Brick[] bricks)
    {
        var sb = new StringBuilder();

        var maxZ = bricks.Max(t => t.Z2);
        var minZ = bricks.Min(t => t.Z1);

        var maxX = bricks.Max(t => t.X2);
        var minX = bricks.Min(t => t.X1);

        var maxY = bricks.Max(t => t.Y2);
        var minY = bricks.Min(t => t.Y1);

        for (int y = minY; y <= maxY; y++)
        {
            sb.Append(y);
        }
        sb.AppendLine();

        for (int z = maxZ; z >= minZ; z--)
        {
            for (int y = minY; y <= maxY; y++)
            {
                var bricksInLine = Enumerable.Range(minX, maxX - minX + 1).Select(x => new Vector3(x, y, z)).Where(p => grid.ContainsKey(p)).Select(t => grid[t]).Distinct().ToList();

                if (bricksInLine.Count == 0)
                {
                    sb.Append('.');
                }
                else if (bricksInLine.Count == 1)
                {
                    sb.Append(Convert.ToChar(bricks[bricksInLine[0]].BrickId + 'A'));
                }
                else
                {
                    sb.Append(bricksInLine.Count % 10);
                }
            }
            sb.Append($" {z}");
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }

    private static (int part1, int part2) Calculate(string[] input)
    {
        var bricks = ParseInput(input);

        var grid = new Dictionary<Vector3, int>();
        for (var brick = 0; brick < bricks.Length; brick++)
        {
            foreach (var cube in bricks[brick].GetCubes())
            {
                grid.Add(cube, brick);
            }
        }
        bricks = DropBricks(grid, bricks);

        var disintegratableBricks = bricks.ToList();

        foreach (var brick in bricks)
        {
            var supporting = new HashSet<int>();

            foreach (var cube in brick.GetCubesBelow())
            {
                if (grid.TryGetValue(cube, out var brickId))
                {
                    supporting.Add(brickId);
                    bricks[brickId].BricksSupporting.Add(brick.BrickId);
                    bricks[brick.BrickId].SupportedBy.Add(brickId);
                }
            }

            if (supporting.Count == 1)
            {
                disintegratableBricks.RemoveAll(b => b.BrickId == supporting.First());
            }
        }

        var part2 = bricks.Select(brick =>
        {
            var queue = new Queue<int>();

            foreach (var supports in brick.BricksSupporting)
            {
                queue.Enqueue(supports);
            }
            var totalDisintegrated = 0;
            var fallen = new HashSet<int>() { brick.BrickId };

            while (queue.TryDequeue(out var brickId))
            {
                var supported = bricks[brickId];

                if (supported.SupportedBy.All(support => fallen.Contains(support)) && fallen.Add(brickId))
                {
                    totalDisintegrated++;

                    foreach (var supports in supported.BricksSupporting)
                    {
                        queue.Enqueue(supports);
                    }
                }
            }
            return totalDisintegrated;
        }).Sum();

        return (disintegratableBricks.Count, part2);
    }

    private static Brick[] DropBricks(Dictionary<Vector3, int> grid, Brick[] bricks)
    {
        var anyFallen = true;
        while (anyFallen)
        {
            anyFallen = false;

            foreach (var brick in bricks.Where(t => !t.IsStable))
            {
                HashSet<int> supportingBricks = [];

                if (brick.Z1 == 1)
                {
                    brick.IsStable = true;
                    continue;
                }
                var hasDropped = true;

                while (hasDropped)
                {
                    hasDropped = false;

                    if (brick.Z1 == 1)
                    {
                        brick.IsStable = true;
                        break;
                    }

                    foreach (var cube in brick.GetCubesBelow())
                    {
                        if (grid.TryGetValue(cube, out var brickId))
                        {
                            supportingBricks.Add(brickId);
                        }
                    }

                    if (supportingBricks.Count == 0)
                    {
                        foreach (var cube in brick.GetCubesBelow())
                        {
                            grid.Add(cube, brick.BrickId);
                        }
                        brick.Drop();

                        foreach (var cube in brick.GetCubesAbove())
                        {
                            grid.Remove(cube);
                        }

                        hasDropped = true;
                        anyFallen = true;
                    }
                    else
                    {
                        brick.IsStable = supportingBricks.Any(s => bricks[s].IsStable);
                    }
                }
            }
        }

        return bricks;
    }

    private static Brick[] ParseInput(string[] input)
    {
        var bricks = new List<Brick>();
        for (var l = 0; l < input.Length; l++)
        {
            var brick = Brick.Parse(l, input[l]);
            bricks.Add(brick);
        }

        return [.. bricks];
    }
}

public class Brick()
{
    public int BrickId { get; init; }
    public int X1 { get; private init; }
    public int Y1 { get; private init; }
    public int Z1 { get; private set; }
    public int X2 { get; private init; }
    public int Y2 { get; private init; }
    public int Z2 { get; private set; }

    public HashSet<int> BricksSupporting { get; init; } = [];
    public HashSet<int> SupportedBy { get; init; } = [];

    public bool IsStable { get; set; } = false;

    public static Brick Parse(int brickId, string def)
    {
        var points = def.Split('~');

        var p1 = ParsePoint(points[0]);
        var p2 = ParsePoint(points[1]);
        return new Brick
        {
            BrickId = brickId,
            X1 = Math.Min(p1.x, p2.x),
            X2 = Math.Max(p1.x, p2.x),
            Y1 = Math.Min(p1.y, p2.y),
            Y2 = Math.Max(p1.y, p2.y),
            Z1 = Math.Min(p1.z, p2.z),
            Z2 = Math.Max(p1.z, p2.z)
        };
    }

    public void Drop()
    {
        this.Z1 -= 1;
        this.Z2 -= 1;
    }

    private static (int x, int y, int z) ParsePoint(string input)
    {
        var parts = input.Split(',').Select(int.Parse).ToArray();
        return (parts[0], parts[1], parts[2]);
    }

    public IEnumerable<Vector3> GetCubes()
    {
        for (var x = X1; x <= X2; x++)
        {
            for (var y = Y1; y <= Y2; y++)
            {
                for (var z = Z1; z <= Z2; z++)
                {
                    yield return new Vector3(x, y, z);
                }
            }
        }
    }

    public IEnumerable<Vector3> GetCubesBelow()
    {
        for (var x = X1; x <= X2; x++)
        {
            for (var y = Y1; y <= Y2; y++)
            {
                yield return new Vector3(x, y, Z1 - 1);
            }
        }
    }

    public IEnumerable<Vector3> GetCubesAbove()
    {
        for (var x = X1; x <= X2; x++)
        {
            for (var y = Y1; y <= Y2; y++)
            {
                yield return new Vector3(x, y, Z2 + 1);
            }
        }
    }

    public override string ToString()
    {
        return $"{(char)((BrickId % 26) + 'A')} ({X1},{Y1},{Z1},{X2},{Y2},{Z2})";
    }
}