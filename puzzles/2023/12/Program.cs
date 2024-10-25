namespace AoC.Puzzles.Y2023.D12;

public static class Program
{
    private static void Main(string[] args)
    {
        string[] inputFiles = ["input/example.txt", "input/input.txt"];

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

    private static long CalculatePart1(string[] inputs)
    {
        var parsed = ParseInputs(inputs);
        var cache = new Dictionary<string, long>();

        return parsed.Sum(state => Calculate(state.Pattern, state.Groups, cache));
    }

    private static long CalculatePart2(string[] inputs)
    {
        var parsed = ParseInputs(inputs).Select(input => new Input(string.Join('?', Enumerable.Repeat(input.Pattern,5)), Enumerable.Repeat(input.Groups,5).SelectMany(x => x).ToArray()));
        var cache = new Dictionary<string, long>();
        return parsed.Sum(state => Calculate(state.Pattern, state.Groups, cache));
    }

    private static Input[] ParseInputs(string[] inputs)
    {
        return inputs.Select(i => i.Split(' ')).Select(x => new Input(x[0], x[1].Split(',').Select(int.Parse).ToArray())).ToArray();
    }

    private static long Calculate(string pattern, int[] groups, Dictionary<string, long> cache, bool noGroupsLeft = false)
    {
        var key = $"{pattern}{string.Join(',',groups)} {noGroupsLeft}";
        if (cache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        if (pattern == string.Empty)
        {
            return (groups.Length != 0 ? 0L : 1L);
        }

        if (groups.Length == 0)
        {
            return pattern.Any(spring => spring == '#') ? 0L : 1L;
        }

        if (pattern[0] == '.')
        {
            return Calculate(pattern[1..], groups, cache);
        }

        if (noGroupsLeft)
        {
            return (pattern[0] == '#') ? 0L : Calculate(pattern[1..], groups, cache);
        }

        long counter = default;

        if (pattern.Length >= groups[0] && pattern[..groups[0]].All(spring => spring != '.'))
        {
            // Consume the group
            counter += Calculate(pattern[groups[0]..], groups[1..], cache, true);
        }
        if (pattern[0] == '?')
        {
            // Possibility
            counter += Calculate(pattern[1..], groups, cache);
        }

        cache[key] = counter;
        return counter;
    }
}

public record struct Input(string Pattern, int[] Groups);