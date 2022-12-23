using System.Text;

var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t))
{
    Console.WriteLine($"Starting {file}");
    var lines = File.ReadAllLines(file);

    var current = Parse(lines);

    var round = 0;

    var movesNeeded = false;

    while (true)
    {
        (movesNeeded, current) = CalculateNextRound(current, round);

        round++;

        if (!movesNeeded)
        {
            Console.WriteLine($"No movements detected @ round {round}");
            break;
        }

        if (round == 10)
        {
            var empty = Print(current);

            Console.WriteLine($"Round: {round}, empty: {empty}");
        }
    }
}

static int Print(HashSet<Point> data)
{
    var sb = new StringBuilder();

    var minY = data.Min(t => t.Y);
    var maxY = data.Max(t => t.Y);
    var minX = data.Min(t => t.X);
    var maxX = data.Max(t => t.X);

    var p = new Point(0, 0);

    var empty = 0;

    for (var y = minY; y <= maxY; y++)
    {
        p = p with { Y = y };
        for (var x = minX; x <= maxX; x++)
        {
            p = p with { X = x };

            var notEmtpy = data.Contains(p);

            sb.Append(notEmtpy ? '#' : '.');

            if (!notEmtpy)
            {
                empty++;
            }
        }
        sb.AppendLine();
    }
    Console.WriteLine(sb.ToString());
    return empty;
}

static (bool movesNeeded, HashSet<Point> next) CalculateNextRound(HashSet<Point> current, int round)
{
    var next = new HashSet<Point>(current.Count);

    var counter = new Dictionary<Point, (List<Point> old, int count)>();

    var movesNeeded = false;

    foreach (var p in current)
    {
        var direction = Point.Zero;

        var proposals = p.GetNextProposedMoves(round % 4).Select(points => (points.direction, points.pointsToCheck.All(pCheck => !current.Contains(pCheck)))).Where(t => t.Item2).ToList();

        var candidate = proposals switch
        {
            { Count: 0 } => p,
            { Count: 4 } => p,
            _ => proposals.First().direction + p
        };

        if (proposals.Count < 4)
        {
            movesNeeded = true;
        }

        Add(counter, p, candidate);
    }

    foreach (var d in counter)
    {
        if (d.Value.count > 1)
        {
            // reverse

            foreach (var old in d.Value.old)
            {
                next.Add(old);
            }
        }
        else
        {
            next.Add(d.Key);
        }
    }

    return (movesNeeded, next);
}

static HashSet<Point> Parse(string[] lines)
{
    var result = new HashSet<Point>();
    for (var y = 0; y < lines.Length; y++)
    {
        for (var x = 0; x < lines[y].Length; x++)
        {
            if (lines[y][x] == '#')
            {
                result.Add(new Point(x, y));
            }
        }
    }
    return result;
}

static void Add(Dictionary<Point, (List<Point> old, int count)> counter, Point currentLocation, Point candidate)
{
    if (!counter.TryGetValue(candidate, out var data))
    {
        data = (new List<Point>() { }, 0);
    }
    data.old.Add(currentLocation);
    data.count++;

    counter[candidate] = data;
}

record struct Point(int X, int Y)
{
    public static Point operator +(Point a, Point b) => a with { X = a.X + b.X, Y = a.Y + b.Y };

    public IEnumerable<(Point direction, IEnumerable<Point> pointsToCheck)> GetNextProposedMoves(int firstDirection)
    {
        var x = X;
        var y = Y;

        for (var i = firstDirection; i < firstDirection + 4; i++)
        {
            var (direction, points) = directionGroups[i % 4];

            yield return (direction, points.Select(t => new Point(t.X + x, t.Y + y)));
        }
    }

    public static readonly Point Zero = new Point(0, 0);

    private static readonly (Point direction, Point[] points)[] directionGroups = new[]
    {
        (new Point(0,-1), new Point[] { new Point(-1,-1), new Point(0,-1), new Point(1,-1) }), // North
        (new Point(0, 1), new Point[] { new Point(-1, 1), new Point(0,1), new Point(1,1) }), // South
        (new Point(-1,0), new Point[] { new Point(-1,-1), new Point(-1,0), new Point(-1,1) }), // West
        (new Point(1, 0), new Point[] { new Point(1, -1), new Point(1,0), new Point(1,1) }), // East
    };
};