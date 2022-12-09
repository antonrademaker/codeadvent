var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t))
{
    var lines = File.ReadAllLines(file);
    Console.WriteLine($"File {file}");
    var part1 = Calculate(lines, 2);

    Console.WriteLine($"Part 1: {part1.Count}");
    var part2 = Calculate(lines, 10);
    Console.WriteLine($"Part 2: {part2.Count}");
}

static HashSet<Point> Calculate(string[] lines, int knotsCount)
{
    Point Up = new(0, 1);
    Point Right = new(1, 0);
    Point Down = new(0, -1);
    Point Left = new(-1, 0);

    var knots = Enumerable.Range(0, knotsCount).Select(t => new Point(0, 0)).ToArray();

    var visited = new HashSet<Point>() { knots[0] };

    foreach (var line in lines)
    {
        var data = line.Split(' ');

        var direction = data[0][0] switch
        {
            'U' => Up,
            'R' => Right,
            'D' => Down,
            _ => Left
        };
        var steps = int.Parse(data[1]);

        for (var i = 0; i < steps; i++)
        {
            knots[0] += direction;

            for (var knot = 1; knot < knotsCount; knot++)
            {
                if (!knots[knot].Touches(knots[knot - 1]))
                {
                    var move = new Point(knots[knot - 1].X == knots[knot].X ? 0 : (knots[knot - 1].X > knots[knot].X ? 1 : -1), knots[knot - 1].Y == knots[knot].Y ? 0 : (knots[knot - 1].Y > knots[knot].Y ? 1 : -1));

                    knots[knot] += move;
                }
            }
            visited.Add(knots[knotsCount - 1]);
        }
    }

    return visited;
}

record struct Point(int X, int Y)
{
    public static Point operator +(Point a, Point b) => a with { X = a.X + b.X, Y = a.Y + b.Y };
    public bool Touches(Point b)
    {
        return (X - 1 <= b.X && b.X <= X + 1) && (Y - 1 <= b.Y && b.Y <= Y + 1);
    }
}