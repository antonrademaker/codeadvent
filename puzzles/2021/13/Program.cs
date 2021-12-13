
using System.Diagnostics;
using System.Text;

var inputFiles = new string[] { "input/example.txt", "input/input.txt", "input/large.txt", "input/xlarge.txt" };
var sw = new Stopwatch();

foreach (var exampleFile in inputFiles/*.Skip(2).Take(1)*/)
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
    var paper = new Paper();
    paper.Read(lines);
    paper.Fold();
    Console.WriteLine($"Part 1: Visible dots: {paper.Positions.Count()}");
}

void CalculatePart2(string[] lines)
{
    var paper = new Paper();
    paper.Read(lines);
    while (paper.Fold()) ;
    paper.Print();

    Console.WriteLine($"Visible dots: {paper.Positions.Count()}");
}

public class Paper
{
    public readonly HashSet<Point> Positions = new();

    private readonly Queue<string> FoldsToApply = new();

    public void Read(string[] lines)
    {
        var readingPositions = true;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                readingPositions = false;
                continue;
            }

            if (readingPositions)
            {
                var pos = line.Split(',').Select(long.Parse).ToArray();
                Positions.Add(new Point(pos[0], pos[1]));
            }
            else
            {
                FoldsToApply.Enqueue(line);
            }
        }
    }

    public bool Fold()
    {
        if (FoldsToApply.TryDequeue(out var fold)) {

            var foldInstruction = fold.Split(' ')[2].Split('=');

            var axis = foldInstruction[0];
            var value = long.Parse(foldInstruction[1]);

            Console.WriteLine($"Fold along {axis} {value}");

            Func<Point, bool> test = axis == "y" ? (Point p) => p.Y > value : (Point p) => p.X > value;
            Func<Point, Point> flipper = axis == "y" ? (Point p) => p with { Y = value * 2 - p.Y } : (Point p) => p with { X = value * 2 - p.X };

            var pointsToFlip = Positions.Where(x => test(x)).ToList();

            foreach(var point in pointsToFlip)
            {
                Positions.Remove(point);
                Positions.Add(flipper(point));
            }
            return true;
        }
        return false;
    }

    public void Print()
    {
        var sb = new StringBuilder();

        var columns = Positions.Select(p => p.X).Max();
        var rows = Positions.Select(p => p.Y).Max();

        for (var y = 0; y <= rows; y++)
        {
            for(var x = 0; x <= columns; x++)
            {
                if (Positions.Contains(new Point(x,y))) {
                    sb.Append('#');
                } else
                {
                    sb.Append('.');
                }
            }
            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());

    }
}

public record Point(long X, long Y);