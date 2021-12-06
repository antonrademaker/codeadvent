using System.Collections;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

var fileLines = System.IO.File.ReadAllLines("input/input.txt");

var parser = new Regex(@"(?'x1'\d+),(?'y1'\d+)\s->\s(?'x2'\d+),(?'y2'\d+)");

var lines = fileLines
    .Select(line => parser.Match(line))
    .Select(p => new Line(int.Parse(p.Groups["x1"].Value), int.Parse(p.Groups["y1"].Value), int.Parse(p.Groups["x2"].Value), int.Parse(p.Groups["y2"].Value)))
    .ToList();

Console.WriteLine("Part 1");
Calculate(lines.Where(l => l.x1 == l.x2 || l.y1 == l.y2));
Console.WriteLine("Part 2");
Calculate(lines);

int CalculateDirection(int i1, int i2)
{
    if (i1 == i2)
    {
        return 0;
    }
    if (i1 > i2)
    {
        return -1;
    }
    return 1;
}


void UpdateMap(Dictionary<Point, int> map, Point position)
{
    if (map.ContainsKey(position))
    {
        map[position]++;
    }
    else
    {
        map.Add(position, 1);
    }
}

void Calculate(IEnumerable<Line> lines)
{
    var map = new Dictionary<Point, int>();

    foreach (var line in lines)
    {

        var vector = new Tuple<int, int>(CalculateDirection(line.x1, line.x2), CalculateDirection(line.y1, line.y2));

        var position = new Point(line.x1, line.y1);

        UpdateMap(map, position);

        while (position.x != line.x2 || position.y != line.y2)
        {
            position = position with { x = position.x + vector.Item1, y = position.y + vector.Item2 };
            UpdateMap(map, position);
        }
    }



    var overlaps = map.Values.Where(count => count > 1).Count();

    Console.WriteLine($"Overlaps: {overlaps}");
}

public record Line(int x1, int y1, int x2, int y2)
{
}

public record Point(int x, int y);
