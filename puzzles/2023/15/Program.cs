namespace AoC.Puzzles.Y2023.D15;

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

    private static long CalculatePart1(string input)
    {
        var parsed = ParseInputs(input);

        var sum = parsed.Select(CalculateHash).Sum();

        return sum;
    }

    private static int CalculatePart2(string input)
    {
        var parsed = ParseInputs(input);

        var operations = parsed.Select(ParsePart2);

        var boxes = Enumerable.Range(0,256).Select(t => new List<Lens>()).ToArray();

        foreach (var operation in operations)
        {
            if (operation is Lens lens)
            {
                var box = boxes[lens.Box];

                var lensToReplace = box.FirstOrDefault(l => l.Label == lens.Label);

                if (lensToReplace != default)
                {
                    var index = box.IndexOf(lensToReplace);
                    box[index] = lens;
                } else
                {
                    box.Add(lens);
                }
            }
            else if (operation is Dash dash)
            {
                var box = boxes[dash.Box];
                var lensToRemove = box.FirstOrDefault(l => l.Label == dash.Label);
                if (lensToRemove != default)
                {
                    box.Remove(lensToRemove);
                }
            }
        }
        var sum = 0;
        for(var boxIndex = 0; boxIndex < boxes.Length; boxIndex++)
        {
            for(var index = 0; index < boxes[boxIndex].Count; index++)
            {
                sum += (boxIndex + 1) * (index + 1) * boxes[boxIndex][index].FocalLength;
            }
        }

        return sum;
    }

    public static IOperation ParsePart2(string input)
    {
        if (input.Contains('='))
        {
            var split = input.Split('=');
            return new Lens(split[0], CalculateHash(split[0]), int.Parse(split[1]));
        }

        return new Dash(input[..^1], CalculateHash(input[..^1]));
    }

    private static int CalculateHash(string input)
    {
        var value = input.Aggregate(default(int), (acc, ch) => ((acc + ch) * 17) % 256);
        return value;
    }

    private static string[] ParseInputs(string inputs)
    {
        return inputs.Split(',');
    }
}

public readonly record struct Lens(string Label, int Box, int FocalLength) : IOperation;

public readonly record struct Dash(string Label, int Box) : IOperation;

public interface IOperation
{
    string Label { get; }
    int Box { get; }
}