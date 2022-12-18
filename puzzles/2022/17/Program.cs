using System;
using System.Diagnostics;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var files = Directory.GetFiles("input", "*.txt");

        foreach (var file in files.OrderBy(t => t).Take(1))
        {
            Console.WriteLine($"==============={file}==============");

            var input = File.ReadAllText(file).Trim();

            var (part1,part2) = Calculate(input);

            Console.WriteLine($"  Part 1: {part1}");
            Console.WriteLine($"  Part 2: {part2}");

            //var part2 = CalculatePart2(input, edges);

            //Console.WriteLine($"  Part 2: {part2}");
        }
    }

    private static (long part1, long part2) Calculate(string input)
    {
        var patternIndex = 0;
        var patternLenght = input.Length;

        var left = new Point(-1, 0);
        var right = new Point(1, 0);

        var currentHeight = 0;

        var rocksFallen = 0L;

       // var currentRocks = new List<IShape>();

        var downMovement = new Point(0, -1);
        var nextRockShape = -1;

        var part1 = 0L;
        var part2 = 0L;

        var part1Index = 2022L;
        var part2Index = 22220L; // 1000000000000L;

        var tower = new HashSet<Point>();


        var cache = new Dictionary<(int rockIndex, int patternIndex), (long rocks, long height)>();

        while (part1 == 0 || part2 == 0)
        {
            var movingRock = RockGenerator(currentHeight + 4, ref nextRockShape);

            // jet movement
            while (true)
            {

                var jet = input[patternIndex] == '<' ? left : right;

                patternIndex = (patternIndex + 1) % patternLenght;

                var candidate = movingRock.Move(jet);

                if (!candidate.HitsWall && !candidate.Hits(tower))
                {
                    movingRock = candidate;
                }

                candidate = movingRock.Move(downMovement);

                if (!candidate.HitsBottom && !candidate.Hits(tower))
                {
                    movingRock = candidate;
                }
                else
                {
                    //var restingRock = new RestingRock(movingRock);

                    foreach(var p in movingRock.Points)
                    {
                        Debug.Assert(tower.Add(p));
                    }
                    //currentRocks.Add(restingRock);
                    var newHeight = movingRock.MaxHeight;
                    if (newHeight > currentHeight)
                    {
                        currentHeight = newHeight;
                    }
                    break;
                }
            }

            if (cache.TryGetValue((nextRockShape, patternIndex), out var hit) && part2 == 0)
            {
                // Console.WriteLine("Got a cache hit!!!");

                long cycle = rocksFallen - hit.rocks;
                long adds = currentHeight - hit.height;
                long remaining = 1000000000000 - rocksFallen;
                long combo = (remaining / (cycle) + 1);
                if (rocksFallen + combo * cycle >= 1000000000000)
                {
                    if (rocksFallen + combo * cycle > 1000000000000)
                    {
                        Console.WriteLine("We missed the spot");
                    }
                    else
                    {
                        part2 = currentHeight + 1 + combo * adds;
                    }
                }
            } else
            {
                cache[(nextRockShape, patternIndex)] = (rocksFallen, currentHeight + 1);
            }

            rocksFallen++;

            if (rocksFallen == part1Index)
            {
                part1 = currentHeight;
            }
        }
        return (part1, part2);
    }

    private static IShape RockGenerator(int y, ref int nextRockShape)
    {
        nextRockShape = (nextRockShape + 1) % 5;

        var startPoint = new Point(3, y);
        return nextRockShape switch
        {
            0 => new Minus(startPoint),
            1 => new Plus(startPoint),
            2 => new InvertedL(startPoint),
            3 => new Pipe(startPoint),
            _ => new Block(startPoint)
        };
    }


    private static void Print(string message, HashSet<Point> tower, int currentHeight)
    {
        var y = currentHeight;
        var x = 0;

        var sb = new StringBuilder();

        var allPoints = tower.OrderByDescending(t => t.Y).ThenBy(t => t.X).ToList();


        sb.AppendLine(message);

        foreach (var point in allPoints)
        {
            while (y > point.Y)
            {
                y--;
                if (x == 0)
                {
                    sb.AppendLine("|.......|");
                    continue;
                }
                if (x < 8)
                {
                    sb.Append(new String('.', 8 - x));
                }

                sb.Append('|');
                x = 0;
                sb.AppendLine();
            }

            if (x == 0)
            {
                sb.Append('|');
                x++;
            }
            while (point.X > x)
            {
                sb.Append('.');
                x++;
            }
            sb.Append('#');
            x++;
        }
        y--;

        if (x < 9)
        {
            sb.Append(new String('.', 8 - x));
            sb.Append("|");
        }

        sb.AppendLine();
        while (y > 0)
        {
            y--;
            sb.AppendLine("|.......|");
        }

        sb.AppendLine(new string('-', 9));
        sb.AppendLine();
        Console.WriteLine(sb.ToString());
    }


    private static void Print(string message, List<IShape> shapes)
    {
        var allPoints = shapes.SelectMany(s => s.Points).OrderByDescending(t => t.Y).ThenBy(t => t.X).ToList();

        var y = allPoints[0].Y;

        var x = 0;

        var sb = new StringBuilder();

        sb.AppendLine(message);

        foreach (var point in allPoints)
        {
            while (y > point.Y)
            {
                y--;
                if (x == 0)
                {
                    sb.AppendLine("|.......|");
                    continue;
                }
                if (x < 8)
                {
                    sb.Append(new String('.', 8 - x));
                }

                sb.Append('|');
                x = 0;
                sb.AppendLine();
            }

            if (x == 0)
            {
                sb.Append('|');
                x++;
            }
            while (point.X > x)
            {
                sb.Append('.');
                x++;
            }
            sb.Append('#');
            x++;
        }
        y--;

        if (x < 9)
        {
            sb.Append(new String('.', 8 - x));
            sb.Append("|");
        }

        sb.AppendLine();
        while (y > 0)
        {
            y--;
            sb.AppendLine("|.......|");
        }

        sb.AppendLine(new string('-', 9));
        sb.AppendLine();
        Console.WriteLine(sb.ToString());
    }
}

public interface IShape
{
    public Point Point { get; }

    public int MaxHeight { get; }

    bool Hits(IShape other) => Points.Intersect(other.Points).Any();

    bool Hits(HashSet<Point> points) => Points.Intersect(points).Any();

    HashSet<Point> Points { get; }

    bool HitsLeftWall { get; } // => Points.Any(t => t.X == 0);
    bool HitsRightWall { get; } // => Points.Any(t => t.X == 8);
    bool HitsBottom { get; } //=> Points.Any(t => t.Y == 0);

    bool HitsWall => HitsLeftWall || HitsRightWall;

    IShape Move(Point move);
}

public record struct RestingRock : IShape
{
    public Point Point { get; }

    public int MaxHeight { get; }

    public HashSet<Point> Points { get; }

    public bool HitsLeftWall => throw new NotImplementedException();

    public bool HitsRightWall => throw new NotImplementedException();

    public bool HitsBottom => throw new NotImplementedException();

    public RestingRock(IShape shape)
    {
        Point = shape.Point;
        Points = shape.Points;
        MaxHeight = shape.MaxHeight;
    }

    public IShape Move(Point move)
    {
        throw new NotImplementedException();
    }
}

public record struct Minus(Point Point) : IShape
{
    static readonly HashSet<Point> points = new()
    {
        new Point(0,0), new Point(1,0), new Point(2,0), new Point(3,0)
    };

    public HashSet<Point> Points
    {
        get
        {
            var cPoint = Point;
            return points.Select(t => t + cPoint).ToHashSet();
        }
    }

    public IShape Move(Point move)
    {
        return new Minus(Point + move);
    }

    public int MaxHeight => Point.Y;

    public bool HitsLeftWall => Point.X == 0;

    public bool HitsRightWall => Point.X == 5;

    public bool HitsBottom => Point.Y == 0;
}

public record struct Plus(Point Point) : IShape
{
    static readonly HashSet<Point> points = new HashSet<Point>()
    {
        new Point(1,2),
        new Point(0,1), new Point(1,1), new Point(2,1),
        new Point(1,0)
    };

    public HashSet<Point> Points
    {
        get
        {
            var cPoint = Point;
            return points.Select(t => t + cPoint).ToHashSet();
        }
    }
    public IShape Move(Point move)
    {
        return new Plus(Point + move);
    }

    public int MaxHeight => Point.Y + 2;

    public bool HitsLeftWall => Point.X == 0;
    public bool HitsRightWall => Point.X == 6;

    public bool HitsBottom => Point.Y == 0;
}

public record struct InvertedL(Point Point) : IShape
{
    static readonly HashSet<Point> points = new HashSet<Point>()
    {
        new Point(2,2),
        new Point(2,1),
        new Point(0,0), new Point(1,0), new Point(2,0),
    };

    public HashSet<Point> Points
    {
        get
        {
            var cPoint = Point;
            return points.Select(t => t + cPoint).ToHashSet();
        }
    }
    public IShape Move(Point move)
    {
        return new InvertedL(Point + move);
    }

    public int MaxHeight => Point.Y + 2;

    public bool HitsLeftWall => Point.X == 0;

    public bool HitsRightWall => Point.X == 6;

    public bool HitsBottom => Point.Y == 0;
}

public record struct Pipe(Point Point) : IShape
{
    static readonly HashSet<Point> points = new HashSet<Point>()
    {
        new Point(0,3),
        new Point(0,2),
        new Point(0,1),
        new Point(0,0)
    };
    public HashSet<Point> Points
    {
        get
        {
            var cPoint = Point;
            return points.Select(t => t + cPoint).ToHashSet();
        }
    }
    public IShape Move(Point move)
    {
        return new Pipe(Point + move);
    }

    public int MaxHeight => Point.Y + 3;

    public bool HitsLeftWall => Point.X == 0;

    public bool HitsRightWall => Point.X == 8;

    public bool HitsBottom => Point.Y == 0;
}

public record struct Block(Point Point) : IShape
{
    static readonly HashSet<Point> points = new HashSet<Point>()
    {
        new Point(0,1), new Point(1,1),
        new Point(0,0), new Point(1,0)
    };
    public HashSet<Point> Points
    {
        get
        {
            var cPoint = Point;
            return points.Select(t => t + cPoint).ToHashSet();
        }
    }

    public IShape Move(Point move)
    {
        return new Block(Point + move);
    }
    public int MaxHeight => Point.Y + 1;

    public bool HitsLeftWall => Point.X == 0;

    public bool HitsRightWall => Point.X == 7;

    public bool HitsBottom => Point.Y == 0;
}

public record struct Point(int X, int Y)
{
    public static Point operator +(Point a, Point b) => a with { X = a.X + b.X, Y = a.Y + b.Y };
    public static Point operator -(Point a, Point b) => a with { X = a.X - b.X, Y = a.Y - b.Y };
};