var scoresPart1 = new Dictionary<string, int>()
{
    { "A X", 1 + 3 },
    { "A Y", 2 + 6 },
    { "A Z", 3 + 0 },
    { "B X", 1 + 0 },
    { "B Y", 2 + 3 },
    { "B Z", 3 + 6 },
    { "C X", 1 + 6 },
    { "C Y", 2 + 0 },
    { "C Z", 3 + 3 },
};

var rounds = File.ReadAllLines("input/input.txt");

var counter = rounds.GroupBy(t => t).ToDictionary(t => t.Key, t => t.Count());

var part1 = counter.Select(t => t.Value * scoresPart1[t.Key]).Sum();

Console.WriteLine(part1);

var scoresPart2 = new Dictionary<string, int>()
{
    { "A X", 0 + 3},
    { "A Y", 3 + 1},
    { "A Z", 6 + 2},
    { "B X", 0 + 1 },
    { "B Y", 3 + 2 },
    { "B Z", 6 + 3 },
    { "C X", 0 + 2 },
    { "C Y", 3 + 3},
    { "C Z", 6 + 1 },
};

var part2 = counter.Select(t => t.Value * scoresPart2[t.Key]).Sum();

Console.WriteLine(part2);