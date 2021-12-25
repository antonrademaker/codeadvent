using System.Diagnostics;
using System.Text;

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
    var count = lines.Select(l => l.Count(x => x != '.')).Sum();

    var positions = Enumerable.Range(0, 2).Select(x => new Dictionary<Point, char>(count)).ToArray();

    var width = lines[0].Length;
    var height = lines.Length;

    for (var y = 0; y < lines.Length; y++)
    {
        for (var x = 0; x < lines[y].Length; x++)
        {
            switch (lines[y][x])
            {
                case '>':
                    positions[0].Add(new Point(x, y), '>');
                    break;
                case 'v':
                    positions[0].Add(new Point(x, y), 'v');
                    break;
                default:
                    break;
            }
        }
    }

    var changed = true;

    var step = 0;

    var one = 0;
    var two = 1;


    while (changed)
    {
        changed = false;

        step++;

        positions[two].Clear();

        foreach (var left in positions[one])
        {
            var nextX = new Point((left.Key.x + 1) % width, left.Key.y);

            if (left.Value == '>' && !positions[one].ContainsKey(nextX))
            {
                changed = true;
                positions[two][nextX] = left.Value;
            }
            else
            {
                positions[two][left.Key] = left.Value;
            }
        }

        positions[one].Clear();

        foreach (var down in positions[two])
        {
            var nextY = new Point(down.Key.x, (down.Key.y + 1) % height);
            if (down.Value == 'v' && !positions[two].ContainsKey(nextY))
            {
                changed = true;

                positions[one][nextY] = down.Value;
            }
            else
            {
                positions[one][down.Key] = down.Value;

            }
        }
    }
    Console.WriteLine($"Ended after: {step}");

}

void CalculatePart2(string[] lines)
{

}

public record struct Point(int x, int y)
{
    public override int GetHashCode()
    {
        return x * 997 + y;
    }
}

public record SeaCucumbers
{

}