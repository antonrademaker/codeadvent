var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files)
{
    var lines = File.ReadAllLines(file);

    var pairs = lines.Select(text => text.Split(',').Select(r => r.Split('-').Select(int.Parse)).Select(r => (start: r.Min(), end: r.Max())))
        .Select(p => (first: p.First(), second: p.Last()))
        .ToList();

    var part1 = pairs.Where(p => (p.first.start <= p.second.start && p.first.end >= p.second.end) || (p.second.start <= p.first.start && p.second.end >= p.first.end));

    Console.WriteLine($"{file}: part 1 = {part1.Count()}");

    var part2 = pairs.Count(p => Math.Max(p.first.start, p.second.start) <= Math.Min(p.first.end, p.second.end));
    Console.WriteLine($"{file}: part 2 = {part2}");
}