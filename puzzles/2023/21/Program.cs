using AoC.Utilities;
using System.Buffers;
using System.Text;
using Coordinate = AoC.Utilities.Coordinate<int>;

namespace AoC.Puzzles.Y2023.D21;

public static partial class Program
{
    private const string FTModuleName = "ft";

    private static void Main(string[] args)
    {
        List<(string fileName, int stepsLeft)> inputFiles = [
            ("input/example.txt", 6),
            ("input/input.txt",64)
            //, 
            ];

        foreach (var (file, stepsLeft) in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var part1 = CalculatePart1(inputs, stepsLeft);

            Console.WriteLine($"Part 1: {part1}");

            //var part2 = CalculatePart2(inputs);

            //Console.WriteLine($"Part 2: {part2}");
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
                    if (rocks.Contains(neighbor))
                    {
                        continue;
                    }
                    iterations[next].Add(neighbor);
                }
            }
            iterations[current].Clear();
        }

        return iterations[stepsLeft % 2].Count;
    }

    private static bool IsRock(this HashSet<Coordinate> rocks, Coordinate coordinate, int width, int height)
    {
        return rocks.Contains(new Coordinate(coordinate.X % width, coordinate.Y % height));
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
