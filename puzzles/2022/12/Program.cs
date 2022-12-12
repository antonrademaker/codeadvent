
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t).Skip(1))
{
    var fileInput = File.ReadAllLines(file);

    var lineLength = fileInput[0].Length;

    var field = new Dictionary<Point, char>();

    var start = new Point(-1, -1);
    var end = new Point(-1, -1);

    for (var y = 0; y < fileInput.Length; y++)
    {
        for (var x = 0; x < lineLength; x++)
        {
            var value = fileInput[y][x];
            var p = new Point(x, y);
            field.Add(p, value);
            if (value == 'S')
            {
                start = p;
            }
            else if (value == 'E')
            {
                end = p;
            }

        }
    }

    Console.WriteLine($"Start @ {start}, ends at :{end}");

    var part1 = CalculatePart1(field, start, end);
    Console.WriteLine($"Part 1: {part1}{Environment.NewLine}");

}

int CalculatePart1(Dictionary<Point, char> field, Point start, Point end)
{
    var prio = new PriorityQueue<Path, int>();

    prio.Enqueue(new Path('E', end, ImmutableArray<Point>.Empty.Add(end)), 1);

    var found = field.ToDictionary(p => p.Key, p => int.MaxValue);




    while (prio.TryDequeue(out var path, out var priority))
    {

        Console.WriteLine($"Dequeued: {path.Head}  {prio.Count}");
        foreach (var sp in SurroundingPoints(path.Head)
            .Where(candidate => !path.Points.Contains(candidate))
            )
        {
            if (field.TryGetValue(sp, out char value))
            {
                Console.WriteLine($"  Testing {sp} ({value} <= {path.Current})?");

                if (value == path.Current || value == path.Current - 1 || (path.Current == 'a' && value == 'S') || (path.Current == 'E' && value == 'z'))
                {

                    if (sp == start)
                    {
                        Console.WriteLine(path.ToString());
                        Console.WriteLine(string.Join(", ", path.Points));
                        return path.Points.Length;
                    }

                    if (found[sp] > path.Points.Length + 1)
                    {
                        found[sp] = path.Points.Length + 1;

                        Console.WriteLine($"    Queue: {value}@{sp}");

                        prio.Enqueue(path with { Current = value, Head = sp, Points = path.Points.Add(sp) }, -priority);

                    } else
                    {
                        Console.WriteLine($"    Skipping {found[sp]} <= {path.Points.Length + 1}");
                    }
                }
                else
                {
                    //                  Console.WriteLine($"    Not queuing: {value} <= {path.Current} @ {sp}");

                }
            }
        }

        //     Console.WriteLine($"prio queue: {prio.Count}");
    }

    return 0;
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