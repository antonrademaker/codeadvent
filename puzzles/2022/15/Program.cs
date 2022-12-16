using System.Text.RegularExpressions;

var files = Directory.GetFiles("input", "*.txt");

var regex = new Regex("^Sensor at x=(?<sensorX>-?\\d+), y=(?<sensorY>\\-?\\d+): closest beacon is at x=(?<beaconX>-?\\d+), y=(?<beaconY>-?\\d+)$", RegexOptions.NonBacktracking);

foreach (var file in files.OrderBy(t => t))
{
    Console.WriteLine($"{file}");

    var input = File.ReadAllLines(file).Select(l => regex.Matches(l).Cast<Match>()
    .Select(m => (sensor: new Point(int.Parse(m.Groups["sensorX"].Value), int.Parse(m.Groups["sensorY"].Value)),
    beacon: new Point(int.Parse(m.Groups["beaconX"].Value), int.Parse(m.Groups["beaconY"].Value)))).FirstOrDefault())
        .ToList();

    var part1 = CalculatePart1(input, file.Contains("example") ? 10 : 2000000);

    Console.WriteLine($"  Part 1: {part1}");

    var part2 = CalculatePart2(input, file.Contains("example") ? 20 : 4000000);

    Console.WriteLine($"  Part 2: {part2}");
}

static int CalculatePart1(List<(Point sensor, Point beacon)> fileInput, int rowToCalculate)
{
    var (ranges, beacons) = CalculateRow(fileInput, rowToCalculate);
    return ranges.Sum(t => t.Length) - beacons.Count;
}

static long CalculatePart2(List<(Point sensor, Point beacon)> fileInput, int size)
{
    for (var row = 0; row < size; row++)
    {
        var (ranges, beacons) = CalculateRow(fileInput, row);

        var gaps = FindGaps(ranges, beacons, size);

        if (gaps.Any())
        {
            return gaps.First() * 4000000L + row;
        }
    }
    return -1;
}

static List<int> FindGaps(List<RowPart> ranges, HashSet<int> beacons, int size)
{
    List<int> results = new();
    var rIndex = 0;

    while (ranges[rIndex].End < 0 && rIndex < ranges.Count)
    {
        // skip all smaller ones
        rIndex++;
    }

    var x = ranges[rIndex].End + 1;
    rIndex++;

    while (x <= size && rIndex < ranges.Count)
    {
        if (ranges[rIndex].Start != x)
        {
            if (!beacons.Contains(x))
            {
                results.Add(x);
            }
            x++;
        }
        else
        {
            x = ranges[rIndex].End;
        }
    }

    return results;
}

static (List<RowPart> ranges, HashSet<int> beacons) CalculateRow(List<(Point sensor, Point beacon)> input, int rowToCalculate)
{
    var hitsInRow = new HashSet<RowPart>();
    var beacons = new HashSet<int>();

    foreach (var (sensor, beacon) in input)
    {
        if (beacon.Y == rowToCalculate)
        {
            beacons.Add(beacon.X);
        }

        var distance = ManhattanDistance(sensor, beacon);

        var toHitRowDistance = Math.Abs(sensor.Y - rowToCalculate);

        if (toHitRowDistance < distance)
        {
            //hitsInRow.Add(new Point(sensor.X, rowToCalculate));
            var halfSize = distance - toHitRowDistance;

            hitsInRow.Add(new RowPart(sensor.X - halfSize, sensor.X + halfSize));
        }
    }

    // now reduce the ranges

    var ranges = hitsInRow.OrderBy(t => t.Start).ThenBy(t => t.End).ToList();

    var simpleRanges = new List<RowPart>
    {
        ranges[0]
    };

    for (var i = 1; i < ranges.Count; i++)
    {
        if (simpleRanges[^1].Start <= ranges[i].Start && simpleRanges[^1].End >= ranges[i].End)
        {
            continue;
        }
        else
        {
            if (simpleRanges[^1].End >= ranges[i].Start && simpleRanges[^1].End < ranges[i].End)
            {
                simpleRanges[^1] = simpleRanges[^1] with { End = ranges[i].End };
            }
            else
            {
                simpleRanges.Add(ranges[i]);
            }
        }
    }

    return (simpleRanges, beacons);
}

static int ManhattanDistance(Point a, Point b)
{
    return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}

record struct RowPart(int Start, int End)
{
    public int Length => End - Start + 1;
}

record struct Point(int X, int Y)
{
    public static Point operator +(Point a, Point b) => a with { X = a.X + b.X, Y = a.Y + b.Y };
};