using System.Text.RegularExpressions;

var fileLines = File.ReadAllLines("input/input.txt");
var parser = new Regex(@"(?'x1'\d+),(?'y1'\d+)\s->\s(?'x2'\d+),(?'y2'\d+)");
var lines = fileLines
    .Select(line => parser.Match(line))
    .Select(p => new Line(int.Parse(p.Groups["x1"].Value), int.Parse(p.Groups["y1"].Value), int.Parse(p.Groups["x2"].Value), int.Parse(p.Groups["y2"].Value)))
    .ToList();

Console.WriteLine("Part 1: {0}", Calculate(lines.Where(l => l.X1 == l.X2 || l.Y1 == l.Y2)));
Console.WriteLine("Part 2: {0}", Calculate(lines));

int Calculate(IEnumerable<Line> lines)
{
    var map = new Dictionary<Point, int>();
    foreach (var line in lines)
    {
        var vector = new { X = CalculateDirection(line.X1, line.X2), Y = CalculateDirection(line.Y1, line.Y2) };
        var position = new Point(line.X1, line.Y1);
        UpdateMap(map, position);
        while (position.X != line.X2 || position.Y != line.Y2)
        {
            position = position with { X = position.X + vector.X, Y = position.Y + vector.Y };
            UpdateMap(map, position);
        }
    }
    return map.Values.Where(count => count > 1).Count();
}
void UpdateMap(Dictionary<Point, int> map, Point position) {
    if (map.ContainsKey(position)) {
        map[position]++;
    }
    else {
        map.Add(position, 1);
    }
}
int CalculateDirection(int i1, int i2) => i2.CompareTo(i1);
public record Line(int X1, int Y1, int X2, int Y2);
public record Point(int X, int Y);