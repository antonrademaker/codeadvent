using System.Collections.Immutable;

var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t))
{
    var fileInput = File.ReadAllLines(file);

    var lineLength = fileInput[0].Length;
    var field = new Dictionary<Point, char>();
    var end = new Point(-1, -1);

    for (var y = 0; y < fileInput.Length; y++)
    {
        for (var x = 0; x < lineLength; x++)
        {
            var value = fileInput[y][x];
            var p = new Point(x, y);
            field.Add(p, value);
            if (value == 'E')
            {
                end = p;
            }
        }
    }

    var part1 = CalculatePart(field, end, lineLength, fileInput.Length, c => c == 'S');
    Console.WriteLine($"Part 1: {part1}{Environment.NewLine}");

    var part2 = CalculatePart(field, end, lineLength, fileInput.Length, c => c == 'a');
    Console.WriteLine($"Part 2: {part2}{Environment.NewLine}");
}

int CalculatePart(Dictionary<Point, char> field, Point end, int width, int height, Func<char, bool> isTarget)
{
    var prio = new PriorityQueue<Path, int>();

    prio.Enqueue(new Path('E', end, ImmutableArray<Point>.Empty.Add(end)), 1);

    var found = field.ToDictionary(p => p.Key, p => int.MaxValue);

    while (prio.TryDequeue(out var path, out var priority))
    {
        foreach (var sp in
            SurroundingPoints(path.Head)
                .Where(candidate => !path.Points.Contains(candidate))
            )
        {
            if (field.TryGetValue(sp, out char value) && ((path.Current >= 'a' && path.Current <= 'z' && value >= 'a' && value <= 'z' && value + 1 >= path.Current) || (path.Current == 'E' && value == 'z') || (path.Current == 'a' && value == 'S')))
            {
                if (isTarget(value))
                {
                    Console.WriteLine(path.ToString());
                    Console.WriteLine(string.Join(", ", path.Points));
                    Print(field, width, height, found);

                    Print2(field, width, height, path.Points);

                    return path.Points.Length;
                }

                if (found[sp] > path.Points.Length + 1)
                {
                    found[sp] = path.Points.Length + 1;
                    prio.Enqueue(path with { Current = value, Head = sp, Points = path.Points.Add(sp) }, found[sp]);
                }
            }
        }
    }

    Print(field, width, height, found);

    return 0;
}

static void Print(Dictionary<Point, char> field, int width, int height, Dictionary<Point, int> found)
{
    var st = Enumerable.Range(0, 26).Select(i => (char)(i + 'a')).ToArray();
    Console.WriteLine(st);

    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            Console.Write((found[new Point(x, y)] < int.MaxValue) ? (field[new Point(x, y)].ToString().ToUpper()) : field[new Point(x, y)]);
        }
        Console.WriteLine();
    }
}

static void Print2(Dictionary<Point, char> field, int width, int height, ImmutableArray<Point> path)
{
    var st = Enumerable.Range(0, 26).Select(i => (char)(i + 'a')).ToArray();
    Console.WriteLine(st);

    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            Console.Write(path.Contains(new Point(x, y)) ? (field[new Point(x, y)].ToString().ToUpper()) : field[new Point(x, y)]);
        }
        Console.WriteLine();
    }
}

IEnumerable<Point> SurroundingPoints(Point point)
{
    yield return point with { X = point.X - 1 };
    yield return point with { X = point.X + 1 };
    yield return point with { Y = point.Y - 1 };
    yield return point with { Y = point.Y + 1 };
}

record Path(char Current, Point Head, ImmutableArray<Point> Points)
{
    public int Length => Points.Length;
}

record struct Point(int X, int Y);