string[] inputFiles = ["input/example.txt","input/input.txt"];

foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var inputRows = inputs.Select(input => input.Split(' ').Select(int.Parse).ToArray());

    var part1 = CalculatePart1(inputRows);

    Console.WriteLine($"Part 1: {part1}");

    var part2 = CalculatePart2(inputRows);

    Console.WriteLine($"Part 2: {part2}");
}

long CalculatePart1(IEnumerable<int[]> inputs)
{
    var values = inputs.Select(CalculateNextValue);

    return values.Sum();
}

long CalculatePart2(IEnumerable<int[]> inputs)
{
    var values = inputs.Select(t=> t.Reverse().ToArray()).Select(CalculateNextValue);

    return values.Sum();
}

long CalculateNextValue(int[] inputs)
{
    if (inputs.All(t => t == 0))
    {
        return 0;
    }

    var next = new int[inputs.Length - 1];

    for(var index = 0; index < next.Length; index++)
    {
        next[index] = inputs[index + 1] - inputs[index];
    }

    var lastValue = CalculateNextValue(next);

    return lastValue + inputs.Last();
}