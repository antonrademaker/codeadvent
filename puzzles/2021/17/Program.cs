using System.Collections;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

var inputFiles = new string[] {
    "input/example.txt",
    "input/input.txt"
};
var sw = new Stopwatch();


foreach (var exampleFile in inputFiles)
{
    Console.WriteLine(exampleFile);
    var file = File.ReadAllLines(exampleFile);
    sw.Start();
    foreach (var line in file)
    {
        CalculatePart1(line);
    }
    sw.Stop();
    Console.WriteLine($"Part 1 in {sw.ElapsedMilliseconds}ms");
    sw.Reset();
    sw.Start();
    foreach (var line in file)
    {
        CalculatePart2(line);
    }

    sw.Stop();
    Console.WriteLine($"Part 2 in {sw.ElapsedMilliseconds}ms");

    Console.WriteLine("-- End of file--");
}

void CalculatePart1(string line)
{
    var ocean = new Ocean(line);

    var (maxHeight, _) = ocean.Calculate();
    Console.WriteLine($"Max height: {maxHeight}");
}

void CalculatePart2(string line)
{
    var ocean = new Ocean(line);

    var (_, numberOfPaths) = ocean.Calculate();

    Console.WriteLine($"numberOfPaths: {numberOfPaths}");

}

public class Ocean
{
    private static Regex Parser = new Regex(@"^(.)+:\sx=(?<x1>-?\d+)\.\.(?<x2>-?\d+),\sy=(?<y1>-?\d+)\.\.(?<y2>-?\d+)");

    private int X1 { get; init; }
    private int X2 { get; init; }
    private int Y1 { get; init; }
    private int Y2 { get; init; }


    public Ocean(string line)
    {
        var match = Parser.Match(line);

        X1 = int.Parse(match.Groups["x1"].Value);
        X2 = int.Parse(match.Groups["x2"].Value);
        Y1 = int.Parse(match.Groups["y1"].Value);
        Y2 = int.Parse(match.Groups["y2"].Value);
    }

    public (int, int) Calculate()
    {
        var maxXVelocity = CalculateMaxXVelocity();
        var maxYVelocity = CalculateMaxYVelocity();

        var paths = new List<Path>();
        var maxHeights = new List<int>();

        for (var xv = CalculateMinXVelocity(); xv <= maxXVelocity; xv++)
        {
            for (var yv = CalculateMinYVelocity(); yv <= maxYVelocity; yv++)
            {
                var initPath = new Path(xv, yv);

                if (Hits(initPath, out var maxHeight))
                {
                    maxHeights.Add(maxHeight);
                    paths.Add(initPath);
                }
            }
        }

        return (maxHeights.Any() ? maxHeights.Max() : 0, paths.Count);
    }

    public bool Hits(Path vector, out int maxHeight)
    {
        var position = new Point(0, 0);
        maxHeight = 0;
        while (position.x < X2 && position.y > Y1)
        {
            position = position with { x = position.x + vector.XVelocity, y = position.y + vector.YVelocity };
            maxHeight = Math.Max(maxHeight, position.y);

            if (IsInTargetArea(position))
            {
                return true;
            }

            vector = vector with
            {
                XVelocity = Math.Max(vector.XVelocity - 1, 0),
                YVelocity = vector.YVelocity - 1
            };
        }

        return false;
    }

    private int CalculateMinXVelocity()
    {
        var v = 1;
        while (v <= X1 && (v * (v + 1)) / 2 < X1)
        {
            v++;
        }
        return v;
    }

    private int CalculateMaxXVelocity()
    {
        return X2;
    }

    private int CalculateMinYVelocity()
    {
        return Y1;
    }

    private int CalculateMaxYVelocity()
    {
        return Math.Abs(Y1);
    }

    private bool IsInTargetArea(Point p)
    {
        return X1 <= p.x && p.x <= X2 && Y1 <= p.y && p.y <= Y2;
    }

}

public record struct Path(int XVelocity, int YVelocity);

public record struct Point(int x, int y);