using AoC.Utilities;
using System.Diagnostics;
using System.Text;
using Coordinate = AoC.Utilities.Coordinate<long>;

namespace AoC.Puzzles.Y2023.D18;

public static class Program
{
    private static void Main(string[] args)
    {
        string[] inputFiles = [
            "input/example.txt"
            , "input/input.txt"
            ];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var part1 = CalculatePart1(inputs);

            Console.WriteLine($"Part 1: {part1}");

            var part2 = CalculatePart2(inputs);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static long Calculate(Instruction[] instructions)
    {
        var position = new Coordinate(0, 0);
        var borders = new List<Coordinate>() { position };

        foreach (var instruction in instructions)
        {
            position += instruction.Direction * instruction.Meters;

            borders.Add(position);
        }

        return CalculateArea(borders);
    }

    private static long CalculatePart1(string[] input)
    {
        return Calculate(ParseInputs(input));
    }

    private static long CalculatePart2(string[] input)
    {
        return Calculate(ParseInputsPart2(input));
    }

    private static long CalculateArea(List<Coordinate> borders)
    {
        long area = Math.Abs(Shoelace(borders));
        long perimeter = borders.Zip(borders.Skip(1)).Sum(t => t.First.DistanceTo(t.Second));
        return area / 2 + perimeter / 2 + 1;
    }

    // https://en.wikipedia.org/wiki/Shoelace_formula
    private static long Shoelace(List<Coordinate> borders)
    {
        long area = borders.Zip(borders.Skip(1)).Select(t => t.First.X * t.Second.Y - t.Second.X * t.First.Y).Sum();
        return area;
    }

    private static Instruction[] ParseInputs(string[] inputs)
    {
        return inputs.Select(t => t.Split(' ')).Select(input => new Instruction(ToCoordinate(input[0][0]), int.Parse(input[1]))).ToArray();
    }

    private static Instruction[] ParseInputsPart2(string[] inputs)
    {
        return inputs.Select(t => t.Split(' ')).Select(t => t[^1]).Select(input => new Instruction(ToCoordinate(input[^2..^1][0]), long.Parse(input[2..^2], System.Globalization.NumberStyles.HexNumber))).ToArray();
    }

    private static Coordinate ToCoordinate(char direction)
    {
        return direction switch
        {
            'D' => Coordinate.OffsetDown,
            'R' => Coordinate.OffsetRight,
            'U' => Coordinate.OffsetUp,
            'L' => Coordinate.OffsetLeft,
            '1' => Coordinate.OffsetDown,
            '0' => Coordinate.OffsetRight,
            '3' => Coordinate.OffsetUp,
            '2' => Coordinate.OffsetLeft,
            _ => throw new Exception("Invalid direction")
        };
    }
}

public record struct Instruction(Coordinate Direction, long Meters);