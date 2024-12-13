internal sealed class Program
{
    private static void Main(string[] args)
    {
        var runDebug = true;

        var files = Directory.GetFiles("input", "*.txt");

        foreach (var file in files.OrderBy(t => t).Skip(1).Take(1))
        {
            Console.WriteLine($"{file}");

            var lines = File.ReadAllLines(file);

            var walls = new Walls(0, 0, lines[0].Length - 2, lines.Length - 2, new Point(0, -1), new Point(lines.Length - 3, lines.Length - 2));

            var blizzards = Parse(lines, walls);

            var steps = CalculateShortestPath(blizzards, walls, walls.Start, walls.End);
            Console.WriteLine($"Part1: {steps}");
        }
    }

    private static int CalculateShortestPath((Dictionary<int, List<IBlizzard>> XBlizzards, Dictionary<int, List<IBlizzard>> YBlizzards) blizzards, Walls walls, Point start, Point end)
    {
        var queue = new PriorityQueue<(Point, int), int>();

        queue.Enqueue((start, 1),1);

        var directions = new Point[]
        {
            new Point(-1,0),
            new Point(1,0),
            new Point(0,-1),
            new Point(0,1)
        };

        while (queue.TryDequeue(out var queued, out var prio))
        {
            var (current, round) = queued;

            foreach (var direction in directions)
            {
                var candidate = current + direction;
                if (candidate.X < walls.Left || candidate.X > walls.Right || candidate.Y < walls.Top || candidate.Y > walls.Bottom)
                {
                    if (candidate == end)
                    {
                        return round + 1;
                    }
                    continue;
                }

                var toCalculate = blizzards.XBlizzards[candidate.Y].Concat(blizzards.YBlizzards[candidate.X]).ToList();

                //foreach(var blizzard in toCalculate)
                //{
                //    Console.WriteLine($"{round}: {blizzard}: {blizzard.CalculateRound(round)} ({candidate})");
                //}

                if (toCalculate.All(c => c.CalculateRound(round) != candidate))
                {
                    queue.Enqueue((candidate, round + 1), CalculatePrio(round + 1, candidate));
                }
            }
            if (current != start)
            {

                var toCalculate2 = blizzards.XBlizzards[current.Y].Concat(blizzards.YBlizzards[current.X]);

                if (toCalculate2.All(c => c.CalculateRound(round) != current))
                {
                    Console.WriteLine('W');
                    // We stay in position
                    queue.Enqueue((current, round + 1), CalculatePrio(round + 1, current));
                }
            }
            if (current == start && round < walls.Right)
            {
                Console.WriteLine('w');

                // stay in start
                queue.Enqueue((current, round + 1), CalculatePrio(round + 5, current));

            }
        }

        return -1;

    }

    private static int CalculatePrio(int round, Point point)
    {
        return round - point.X - point.Y;
    }

    public static (Dictionary<int, List<IBlizzard>> XBlizzards, Dictionary<int, List<IBlizzard>> YBlizzards) Parse(string[] lines, Walls walls)
    {
        var xBlizzards = Enumerable.Range(0, lines.Length - 1).ToDictionary(item => item, item => new List<IBlizzard>());
        var yBlizzards = Enumerable.Range(0, lines[0].Length - 1).ToDictionary(item => item, item => new List<IBlizzard>());

        for (var y = 0; y < lines.Length - 2; y++)
        {
            for (var x = 0; x < lines[y].Length - 2; x++)
            {
                var loc = lines[y + 1][x + 1];

                if (loc == '.')
                {
                    Console.Write('.');
                }
                else
                {
                    if (loc is '>' or '<')
                    {
                        // x

                        xBlizzards[y].Add(new XBlizzard(x, y, loc is '>' ? 1 : -1, lines[y].Length - 2));

                    }
                    else
                    {
                        yBlizzards[x].Add(new YBlizzard(x, y, loc is '>' ? 1 : -1, lines.Length - 2));

                        // y
                    }
                    Console.Write('#');
                }
            }
            Console.WriteLine();
        }

        return (xBlizzards, yBlizzards);
    }
}

public interface IBlizzard
{
    Point CalculateRound(int round);
}

public record struct XBlizzard(int X, int Y, int Direction, int Width) : IBlizzard
{
    public Point CalculateRound(int round)
    {
        var x = (X + (round * Direction)).Mod(Width);
        return new Point(x, Y);
    }
}

public record struct YBlizzard(int X, int Y, int Direction, int Height) : IBlizzard
{
    public Point CalculateRound(int round)
    {
        var y = Y + (round * Direction).Mod(Height);
        return new Point(X, y);
    }
}

public record struct Walls(int Top, int Left, int Right, int Bottom, Point Start, Point End);

public record struct Point(int X, int Y)
{
    public static Point operator +(Point a, Point b) => a with { X = a.X + b.X, Y = a.Y + b.Y };
    public static Point operator -(Point a, Point b) => a with { X = a.X - b.X, Y = a.Y - b.Y };
};

public static class IntExtensions
{
    public static int Mod(this int a, int n)
    {
        if (n == 0)
        {
            throw new ArgumentOutOfRangeException("n", "(a mod 0) is undefined.");
        }
        //puts a in the [-n+1, n-1] range using the remainder operator
        int remainder = a % n;

        //if the remainder is less than zero, add n to put it in the [0, n-1] range if n is positive
        //if the remainder is greater than zero, add n to put it in the [n-1, 0] range if n is negative
        return ((n > 0 && remainder < 0) || (n < 0 && remainder > 0)) ? remainder + n : remainder;
    }
}