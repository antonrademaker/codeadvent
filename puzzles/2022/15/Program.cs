using System.Text.RegularExpressions;

var files = Directory.GetFiles("input", "*.txt");

var regex = new Regex("^Sensor at x=(?<sensorX>-?\\d+), y=(?<sensorY>\\-?\\d+): closest beacon is at x=(?<beaconX>-?\\d+), y=(?<beaconY>-?\\d+)$", RegexOptions.NonBacktracking);

foreach (var file in files.OrderBy(t => t))
{
    var fileInput = File.ReadAllLines(file).Select(l => regex.Matches(l).Cast<Match>()
    .Select(m => (sensor: new Point(int.Parse(m.Groups["sensorX"].Value), int.Parse(m.Groups["sensorY"].Value)),
    beacon: new Point(int.Parse(m.Groups["beaconX"].Value), int.Parse(m.Groups["beaconY"].Value)))).FirstOrDefault())
        .ToList();

    var rowToCalculate = file.Contains("example") ? 10 : 2000000;

    Console.WriteLine($"Part 1: {Calculate(fileInput, rowToCalculate)}");
}

static int Calculate(List<(Point sensor, Point beacon)> fileInput, int rowToCalculate)
{
    var hitsInRow = new HashSet<Point>();
    var beacons = new HashSet<Point>();

    foreach (var (sensor, beacon) in fileInput)
    {
        if (beacon.Y == rowToCalculate)
        {
            beacons.Add(beacon);
        }

        var distance = ManhattanDistance(sensor, beacon);

        var toHitRowDistance = Math.Abs(sensor.Y - rowToCalculate);

        if (toHitRowDistance < distance)
        {
            hitsInRow.Add(new Point(sensor.X, rowToCalculate));
            var steps = distance - toHitRowDistance;
            for (var i = 1; i <= steps; i++)
            {
                hitsInRow.Add(new Point(sensor.X - i, rowToCalculate));
                hitsInRow.Add(new Point(sensor.X + i, rowToCalculate));
            }
        }
    }

    var min = hitsInRow.Min(t => t.X);
    var max = hitsInRow.Max(t => t.X);

    Console.WriteLine($"Min: {min}-{max}: hits: {hitsInRow.Count}: beacons: {beacons.Count}");

    return hitsInRow.Count - beacons.Count;
}

static int ManhattanDistance(Point a, Point b)
{
    return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}
record struct Point(int X, int Y)
{
    public static Point operator +(Point a, Point b) => a with { X = a.X + b.X, Y = a.Y + b.Y };
};