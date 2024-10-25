using AoC.Utilities;
using System.Buffers;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Coordinate = AoC.Utilities.Coordinate<int>;

namespace AoC.Puzzles.Y2023.D21;

public static partial class Program
{
    private const string FTModuleName = "ft";

    private static void Main(string[] args)
    {
        List<(string fileName, int stepsLeft)> inputFiles1 = [
            ("input/example.txt", 6),
            ("input/input.txt",64)
            ];

        List<string> inputFiles2 = [
            ("input/input.txt")
           ];

        foreach (var (file, stepsLeft) in inputFiles1)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var part1 = CalculatePart1(inputs, stepsLeft);

            Console.WriteLine($"Part 1: {part1}");
        }

        foreach (var file in inputFiles2)
        {
            var inputs = System.IO.File.ReadAllLines(file);

            var part2 = CalculatePart2(inputs, 26501365);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static void Print(HashSet<Coordinate> rocks, Dictionary<Coordinate, int> gardens, string[] inputs)
    {
        var sb = new StringBuilder();
        for (int y = 0; y < inputs.Length; y++)
        {
            for (int x = 0; x < inputs[0].Length; x++)
            {
                var coord = new Coordinate(x, y);

                if (rocks.Contains(coord))
                {
                    sb.Append('#');
                }
                else if (gardens.ContainsKey(coord))
                {
                    sb.Append('O');
                }
                else
                {
                    sb.Append('.');
                }
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb.ToString());
    }

    private static long CalculatePart1(string[] input, int stepsLeft)
    {
        var (rocks, start) = ParseInput(input);
        return Reachable(stepsLeft, rocks, start, input.Length);
    }

    private static long Reachable(int stepsLeft, HashSet<Coordinate> rocks, Coordinate start, int size)
    {
        HashSet<Coordinate>[] iterations = [new(), new()];

        iterations[0].Add(start);

        for (int i = 0; i < stepsLeft; i++)
        {
            var current = i % 2;
            var next = (i + 1) % 2;

            foreach (var coord in iterations[current])
            {
                foreach (var neighbor in coord.Neighbors)
                {
                    if (rocks.Contains(neighbor) || neighbor.X < 0 || neighbor.X >= size || neighbor.Y < 0 || neighbor.Y >= size)
                    {
                        continue;
                    }
                    iterations[next].Add(neighbor);
                }
            }
        }

        return iterations[stepsLeft % 2].Count;
    }

    private static long CalculatePart2(string[] input, int steps)
    {
        // https://www.youtube.com/watch?v=9UOMZSL0JTg      

        var (rocks, start) = ParseInput(input);

        var size = input.Length;

        Debug.Assert(start.X == start.Y && start.X == size / 2);
        Debug.Assert(steps % size == size / 2);

        var gridWidth = steps / size - 1;

        long odd1 = gridWidth / 2 * 2 + 1;
        long even1 = ((gridWidth + 1) / 2) * 2;

        long odd = odd1 * odd1;
        long even = even1 * even1;

        long oddPoints = Reachable(size * 2 + 1, rocks, start, size);
        long evenPoints = Reachable(size * 2, rocks, start, size);

        var stepsLeft = size - 1;

        long cornerTop = Reachable(stepsLeft, rocks, new Coordinate(start.X, size - 1), size);
        long cornerRight = Reachable(stepsLeft, rocks, new Coordinate(0, start.Y), size);
        long cornerBottom = Reachable(stepsLeft, rocks, new Coordinate(start.X, 0), size);
        long cornerLeft = Reachable(stepsLeft, rocks, new Coordinate(size - 1, start.Y), size);

        var stepsSmall = size / 2 - 1;

        long smallTopRight = Reachable(stepsSmall, rocks, new Coordinate(0, size - 1), size);
        long smallTopLeft = Reachable(stepsSmall, rocks, new Coordinate(size - 1, size - 1), size);
        long smallBottomRight = Reachable(stepsSmall, rocks, new Coordinate(0, 0), size);
        long smallBottomLeft = Reachable(stepsSmall, rocks, new Coordinate(size - 1, 0), size);

        var stepsLarge = size * 3 / 2 - 1;

        long largeTopRight = Reachable(stepsLarge, rocks, new Coordinate(0, size - 1), size);
        long largeTopLeft = Reachable(stepsLarge, rocks, new Coordinate(size - 1, size - 1), size);
        long largeBottomRight = Reachable(stepsLarge, rocks, new Coordinate(0, 0), size);
        long largeBottomLeft = Reachable(stepsLarge, rocks, new Coordinate(size - 1, 0), size);

        return odd * oddPoints +
            even * evenPoints +
            cornerTop + cornerRight + cornerBottom + cornerLeft +
            (gridWidth + 1) * (smallTopRight + smallTopLeft + smallBottomRight + smallBottomLeft) +
            gridWidth * (largeTopRight + largeTopLeft + largeBottomRight + largeBottomLeft);
    }


    private static (HashSet<Coordinate> rocks, Coordinate start) ParseInput(string[] inputs)
    {
        var rocks = new HashSet<Coordinate>();

        var start = new Coordinate();

        for (int y = 0; y < inputs.Length; y++)
        {
            for (int x = 0; x < inputs[0].Length; x++)
            {
                switch (inputs[y][x])
                {
                    case '#':
                        rocks.Add(new Coordinate(x, y));
                        break;
                    case 'S':
                        start = new Coordinate(x, y);
                        break;
                }
            }
        }
        return (rocks, start);
    }
}
