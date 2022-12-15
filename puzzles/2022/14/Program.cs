using System.Text;

var files = Directory.GetFiles("input", "*.txt");

static Point? GetCandidate(Dictionary<Point, char> field, Point[] sandDirections, Point current)
{
    for (var d = 0; d < sandDirections.Length; d++)
    {
        var candidate = current + sandDirections[d];

        if (!field.ContainsKey(candidate))
        {
            return candidate;
        }
    }

    return null;
}

foreach (var file in files.OrderBy(t => t))
{
    var fileInput = File.ReadAllLines(file);

    var field = Parse(fileInput);

    Console.WriteLine($"File: {file}, rock: {field.Count}");

    var part1 = CalculatePart(field);

    Console.WriteLine($"Part 1: {part1}");



    var field2Lines = new List<string>();
    field2Lines.AddRange(fileInput);

    var bottom = field.Keys.Max(t => t.Y) + 2;
    field2Lines.Add($"{500 - bottom},{bottom} -> {500 + bottom},{bottom}");

    var part2 = CalculatePart(Parse(field2Lines));

    Console.WriteLine($"Part 2: {part2}");
}

Dictionary<Point, char> Parse(IEnumerable<string> input)
{
    var field = new Dictionary<Point, char>();

    foreach (var line in input)
    {
        var points = line.Split(" -> ").Select(point => point.Split(',')).Select(point => new Point(int.Parse(point[0]), int.Parse(point[1]))).ToArray();

        var current = points[0];

        field[current] = '#';

        foreach (var nextPoint in points.Skip(1))
        {
            var direction = new Point(current.X == nextPoint.X ? 0 : (current.X < nextPoint.X ? 1 : -1),
                current.Y == nextPoint.Y ? 0 : (current.Y < nextPoint.Y ? 1 : -1)
                );

            while (current != nextPoint)
            {
                current += direction;
                field[current] = '#';
            }
        }
    }

    return field;
}

static void Print(Dictionary<Point, char> field)
{
    var left = field.Keys.Min(t => t.X);
    var right = field.Keys.Max(t => t.X);

    var top = 0;
    var bottom = field.Keys.Max(t => t.Y);
    var sb = new StringBuilder();

    for (var y = top; y <= bottom; y++)
    {
        for (var x = left; x <= right; x++)
        {
            if (field.TryGetValue(new Point(x, y), out char value))
            {
                sb.Append(value);
            }
            else
            {
                sb.Append('.');
            }
        }
        sb.AppendLine();
    }

    Console.WriteLine(sb.ToString());
}

static int CalculatePart(Dictionary<Point, char> field)
{
    var bottom = field.Keys.Max(t => t.Y);
    var source = new Point(500, 0);
    var current = source;
    var sandPath = new Stack<Point>();

    var sandDirections = new Point[] { new Point(0, 1), new Point(-1, 1), new Point(1, 1) };

    var totalSand = 0;

    while (true)
    {
        var candidate = GetCandidate(field, sandDirections, current);
        if (candidate is Point p)
        {
            if (p.Y > bottom)
            {
                Print(field);
                return totalSand;
            }
            current = p;
            sandPath.Push(current);
        }
        else
        {
            if (!field.ContainsKey(current))
            {
                field.Add(current, 'o');
                totalSand++;
            }

            if (!sandPath.TryPop(out var newCurrent))
            {
                if (current == source)
                {
                    Print(field);
                    return totalSand;
                }

                current = source;

            }
            else
            {
                current = newCurrent;
            }
        }
    }
}

record struct Point(int X, int Y)
{
    public static Point operator +(Point a, Point b) => a with { X = a.X + b.X, Y = a.Y + b.Y };
};