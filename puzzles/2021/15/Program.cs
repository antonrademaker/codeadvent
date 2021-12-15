using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Collections.Generic;

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
    CalculatePart1(file);
    sw.Stop();
    Console.WriteLine($"Part 1 in {sw.ElapsedMilliseconds}ms");
    sw.Reset();
    sw.Start();
    CalculatePart2(file);
    sw.Stop();
    Console.WriteLine($"Part 2 in {sw.ElapsedMilliseconds}ms");

    Console.WriteLine("-- End of file--");
}

void CalculatePart1(string[] lines)
{
    Calculate(lines, 1);
}

void CalculatePart2(string[] lines)
{
    Calculate(lines, 5);
}

void Calculate(string[] lines, int repeat)
{
    var processor = new Processor();

    processor.Read(lines, repeat);

    var risk = processor.CalculateRiskPath();

    Console.WriteLine($"Minimal risk: {risk}");
}

public class Processor
{
    private readonly Dictionary<Point, Position> Map = new();
    private PriorityQueue<Point, int> PointsToCheck = new();

    public void Read(string[] lines, int repeat)
    {
        var origHeight = lines.Length;
        var origWidth = lines[0].Length;

        Point point = new Point(0, 0);

        for (var repeaterY = 0; repeaterY < repeat; repeaterY++)
        {
            for (var repeaterX = 0; repeaterX < repeat; repeaterX++)
            {
                for (var row = 0; row < lines.Length; row++)
                {
                    for (var col = 0; col < lines[row].Length; col++)
                    {
                        point = new Point(col + repeaterX * origWidth, row + repeaterY * origHeight);

                        var risk = lines[row][col] - '0' + repeaterY + repeaterX;
                        if (risk > 9)
                        {
                            risk -= 9;
                        }

                        Map.Add(point, new Position(risk, int.MaxValue));
                    }
                }
            }
        }

        Map[point] = Map[point] with { PathRiskLevel = 0 };

        PointsToCheck.Enqueue(point, 0);
    }

    private readonly Point[] masks = new[]
    {
        new Point( -1, 0 ),
        new Point( 1, 0 ),
        new Point( 0 , -1 ),
        new Point( 0 , 1 )
    };

    public int CalculateRiskPath()
    {
        while (PointsToCheck.TryDequeue(out var point, out var prio))
        {
            var currentLocation = Map[point];

            if (point.X == 0 && point.Y == 0)
            {
                return prio;
            }

            var newRiskPath = currentLocation.PathRiskLevel + currentLocation.RiskLevel;

            foreach (var toCheck in masks.Select(mask => point with { X = point.X + mask.X, Y = point.Y + mask.Y }))
            {                
                if (Map.TryGetValue(toCheck, out var position) && position.PathRiskLevel > newRiskPath)
                {
                    Map[toCheck] = position with { PathRiskLevel = newRiskPath };

                    PointsToCheck.Enqueue(toCheck, newRiskPath);
                }
            }
        }

        return Map[new Point(0, 0)].PathRiskLevel;
    }
}

public record Position(int RiskLevel, int PathRiskLevel);
public record Point(int X, int Y);