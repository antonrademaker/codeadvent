using System.Diagnostics;
using System.Linq;

namespace AoC.Puzzles.Y2023.D12;

public static class Program
{

    private static void Main(string[] args)
    {
        string[] inputFiles = ["input/example.txt", "input/input.txt"];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllText(file);

            var part1 = CalculatePart1(inputs);

            Console.WriteLine($"Part 1: {part1}");

            var part2 = CalculatePart2(inputs);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static int CalculatePart1(string inputs)
    {
        var parsed = ParseInputs(inputs);

        return parsed.Sum(input => CalculateReflection(input));
    }

    private static int CalculatePart2(string inputs)
    {
        var parsed = ParseInputs(inputs);

        return parsed.Sum(CalculateSmudge);
    }

    private static string[][] ParseInputs(string inputs)
    {
        return inputs.Split(Environment.NewLine + Environment.NewLine).Select(t => t.Split(Environment.NewLine).ToArray()).ToArray();
    }

    private static int CalculateSmudge(string[] input)
    {
        var reflection = CalculateReflection(input);

        var width = input[0].Length;

        for(var line = 0; line < input.Length; line++)
        {
            var oldLine = input[line];

            for (var pos = 0; pos < width; pos++)
            {
                var newLine = input[line].ToCharArray();

                newLine[pos] = oldLine[pos] == '.' ? '#' : '.';

                input[line] = new string(newLine);

                var newReflection = CalculateReflection(input, reflection);
                if (newReflection > 0)
                {
                    return newReflection;
                }
                input[line] = oldLine;
            }
        }

        Console.WriteLine("Unable to find smudge in:");

        Console.WriteLine(string.Join(Environment.NewLine, input));

        Console.WriteLine($"Old reflection: {reflection}");

        Console.WriteLine();

        return -1;
    }

    private static int CalculateReflection(string[] input, int oldReflection = 0)
    {
        var reflection = ScanForReflection(input, oldReflection / 100);

        if (reflection > 0 && oldReflection != reflection * 100)
        {
            return reflection * 100;
        }
        var colsData = new string[input[0].Length];
        for (int i = 0; i < input[0].Length; i++)
        {
            colsData[i] = string.Concat(input.Select(row => row[i]));
        }        

        reflection = ScanForReflection(colsData, oldReflection);

        if (reflection > 0 && oldReflection != reflection)
        {
            return reflection;
        }
        return 0;
    }

    private static int ScanForReflection(string[] input, int oldReflection)
    {
        Debug.Assert(input.Length > 1);
        for (var x = 0; x < input.Length - 1; x++)
        {
            if (input[x] == input[x + 1])
            {
                var maxLines = Math.Min(x, input.Length - x - 2);

                var isReflection = Enumerable.Range(1, maxLines).All(m => input[x - m] == input[x + 1 + m]);

                if (isReflection && oldReflection != x + 1)
                {
                    return x + 1;
                }
            }
        }

        return -1;
    }
}

