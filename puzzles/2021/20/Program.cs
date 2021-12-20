using System.Collections;
using System.Diagnostics;
using System.Text;

var inputFiles = new string[] {
    "input/example.txt"
    ,
    "input/input.txt"
};
var sw = new Stopwatch();


foreach (var exampleFile in inputFiles)
{
    Console.WriteLine(exampleFile);
    var file = File.ReadAllLines(exampleFile);

    var map = Parse(file);

    sw.Start();

    CalculatePart1(map);

    sw.Stop();
    Console.WriteLine($"Part 1 in {sw.ElapsedMilliseconds}ms");
    sw.Reset();
    sw.Start();
    CalculatePart2(map);

    sw.Stop();
    Console.WriteLine($"Part 2 in {sw.ElapsedMilliseconds}ms");

    Console.WriteLine("-- End of file--");
}

void CalculatePart1(Map map)
{
    Console.WriteLine(map.Print(5, 0));

    for (var step = 0; step < 2; step++)
    {
        map = map.Enhance();
    }

    Console.WriteLine($"Answer part 1: {map.Points.ToEnumerable<bool>().Count(t => t)}");
}

void CalculatePart2(Map map)
{
    for (var step = 0; step < 50; step++)
    {
        map = map.Enhance();
    }

    Console.WriteLine($"Answer part 2: {map.Points.ToEnumerable<bool>().Count(t => t)}");
}

Map Parse(string[] lines)
{
    var algorithm = lines[0].Select(t => t == '#').ToArray();
    var map = new Map(lines[2..], false, algorithm);

    return map;
}

public record Map
{
    public bool[,] Points { get; init; }

    public int Width { get; init; }
    public int Height { get; init; }

    public bool Fill { get; init; }

    public bool[] Algorithm { get; init; }

    public Map(string[] lines, bool fill, bool[] algorithm)
    {
        Fill = fill;
        Height = lines.Length;

        Width = lines[0].Length;

        Points = new bool[Height, Width];

        Algorithm = algorithm;

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                Points[x, y] = lines[y][x] == '#';
            }
        }
    }

    public Map Enhance()
    {
        return this with
        {
            Fill = Fill ? Algorithm[(1 << 9) - 1] : Algorithm[0],
            Width = Width + 2,
            Height = Height + 2,
            Points = Enhance(Width + 2, Height + 2)
        };
    }

    private bool[,] Enhance(int width, int height)
    {
        var result = new bool[width, height];
        
        var p = new Point(0, 0);
        for (var y = 0; y < height; y++)
        {
            p = p with { Y = y - 1 };
            for (var x = 0; x < width; x++)
            {
                p = p with { X = x - 1 };

                var newValue = new BitArray(GetNeighbours(p).Select(Get).ToArray()).BitArrayToInt();

                result[x, y] = Algorithm[newValue];
            }
        }

        return result;
    }

    public bool Get(Point p)
    {
        return Get(p.X, p.Y);
    }

    public bool Get(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return Fill;
        }
        return Points[x, y];
    }

    public static IEnumerable<Point> GetNeighbours(Point p)
    {
        // return in reversed order
        yield return p with { X = p.X + 1, Y = p.Y + 1 };
        yield return p with { Y = p.Y + 1 };
        yield return p with { X = p.X - 1, Y = p.Y + 1 };
        yield return p with { X = p.X + 1 };
        yield return p;
        yield return p with { X = p.X - 1 };
        yield return p with { X = p.X + 1, Y = p.Y - 1 };
        yield return p with { Y = p.Y - 1 };
        yield return p with { X = p.X - 1, Y = p.Y - 1 };
    }

    public string Print(int size, int? low = null)
    {
        var sb = new StringBuilder();
        for (var y = low ?? -size; y < size; y++)
        {
            for (var x = low ?? -size; x < size; x++)
            {
                sb.Append(Get(x, y) ? '#' : '.');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

public static class Helpers
{
    public static int BitArrayToInt(this BitArray ba)
    {
        var len = Math.Min(32, ba.Count);
        int n = 0;
        for (int i = 0; i < len; i++)
        {
            if (ba.Get(i))
                n |= 1 << i;
        }
        return n;
    }

    public static IEnumerable<T> ToEnumerable<T>(this Array target)
    {
        foreach (var item in target)
            yield return (T)item;
    }
}

public class Processor
{
    public readonly bool[] Algorithm;

    public Processor(string initValue)
    {
        Algorithm = initValue.Select(t => t == '#').ToArray();
    }
}

public record Point(int X, int Y);