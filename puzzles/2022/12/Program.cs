﻿
using System.Collections.Immutable;

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

    var part1 = CalculatePart1(field, start, end, lineLength, fileInput.Length);
    Console.WriteLine($"Part 1: {part1}{Environment.NewLine}");

}

int CalculatePart1(Dictionary<Point, char> field, Point start, Point end, int width, int height)
{
    var prio = new PriorityQueue<Path, int>();

    prio.Enqueue(new Path('S', start, ImmutableArray<Point>.Empty.Add(start)), 1);

    var found = field.ToDictionary(p => p.Key, p => int.MaxValue);




    while (prio.TryDequeue(out var path, out var priority))
    {

        //Console.WriteLine($"Dequeued: {path.Head} {prio.Count} {field[path.Head]}");
        foreach (var sp in SurroundingPoints(path.Head)
            .Where(candidate => !path.Points.Contains(candidate))
            )
        {

            if (field.TryGetValue(sp, out char value))
            {
                // Console.WriteLine($"  Testing {sp} ({value} <= {(char)(path.Current + 1)})? {value <= path.Current + 1}");

                if ((value >= 'a' && value <= path.Current + 1) || (path.Current == 'S' && value == 'a') || (path.Current == 'z' && value == 'E'))
                {

                    if (sp == end)
                    {
                        Console.WriteLine(path.ToString());
                        Console.WriteLine(string.Join(", ", path.Points));
                        Print(field, width, height, found);

                        Print2(field, width, height, path.Points);


                        return path.Points.Length;
                    }

                    if (found[sp] > path.Points.Length + 1)
                    {
                        //       Console.WriteLine($"    Queue: {value}@{sp}-- {found[sp]} > {path.Points.Length + 1}");

                        found[sp] = path.Points.Length + 1;


                        prio.Enqueue(path with { Current = value, Head = sp, Points = path.Points.Add(sp) }, found[sp]);

                    }
                    else
                    {
                        //      Console.WriteLine($"    Skipping {found[sp]} <= {path.Points.Length + 1}");
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