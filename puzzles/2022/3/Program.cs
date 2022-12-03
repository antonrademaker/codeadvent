var files = Directory.GetFiles("input", "*.txt");

static int CalculatePriority(char ch)
{
    var prio = (int)ch switch
    {
        // upper case
        < 97 => ch - 64 + 26,
        _ => ch - 96
    };
    return prio;
}

foreach (var file in files)
{
    var rucksacks = File.ReadAllLines(file);

    var part1 = rucksacks.Select(rucksack => (rucksack, size: rucksack.Length / 2))
        .Select(d => new { c1 = d.rucksack[..d.size], c2 = d.rucksack[^d.size..] })
        .Select(d => d.c1.Intersect(d.c2).Single())
        .Sum(CalculatePriority);

    Console.WriteLine($"{file}: part 1 = {part1}");

    var part2 = rucksacks.Select((rucksack, index) => (rucksack, index))
        .GroupBy(t => t.index / 3, t => t.rucksack.AsEnumerable())
        .Select(x => x.Aggregate((cur, r) => cur.Intersect(r)))
        .Select(x => x.Single())
        .Sum(CalculatePriority);

    Console.WriteLine($"{file}: part 2 = {part2}");
}