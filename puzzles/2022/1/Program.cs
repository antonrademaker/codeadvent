var calories = System.IO.File.ReadAllText("input/input.txt")
    .Split(Environment.NewLine + Environment.NewLine)
    .Select(s => s.Split(Environment.NewLine)).Select(lines => lines.Select(line => int.Parse(line)).Sum()).ToList();

var part1 = calories.Max();

Console.WriteLine($"Part 1: {part1}");

var part2 = calories.OrderByDescending(t => t).Take(3).Sum();

Console.WriteLine($"Part 2: {part2}");