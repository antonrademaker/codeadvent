const int BoundaryHeight = 9;

var file = File.ReadAllLines("input/input.txt");

var rows = file.Length;
var columns = file[0].Length;

var topBottomBoundary = new int[columns + 2];
Array.Fill(topBottomBoundary, BoundaryHeight);

var field = new List<int>();

field.AddRange(topBottomBoundary);

foreach (var row in file)
{
    field.Add(BoundaryHeight);
    field.AddRange(row.Select(x => x - '0'));
    field.Add(BoundaryHeight);
}

field.AddRange(topBottomBoundary);

var span = field.ToArray();

var mask = new int[]
{
    - (columns + 2), // top
    + 1, // right
    (columns + 2), // bottom
    - 1 // left
};

var position = columns + 2;

var endPosition = (columns + 2) * (rows + 1) - 2;

var risk = 0;

var lowPoints = new List<int>();

while (position++ < endPosition)
{
    var currentHeight = span[position];

    if (currentHeight >= BoundaryHeight)
    {
        continue;
    }

    if (mask.All(p => currentHeight < span[p + position]))
    {
        lowPoints.Add(position);
        risk += currentHeight + 1;
    }
}

Console.WriteLine($"Part 1 risk: {risk}");

var basinSizes = new List<int>();

foreach (var lowPoint in lowPoints)
{
    var basin = new HashSet<int> { lowPoint };

    var queue = new Queue<int>(5);

    queue.Enqueue(lowPoint);

    while (queue.TryDequeue(out var queuePosition))
    {
        foreach (var candidate in mask.Select(m => m + queuePosition))
        {
            var value = span[candidate];
            if (value < 9 && basin.Add(candidate))
            {
                queue.Enqueue(candidate);
            }
        }
    }
    basinSizes.Add(basin.Count);
}

Console.WriteLine($"part2:{ basinSizes.OrderByDescending(t => t).Take(3).Aggregate(1, (acc, bassin) => acc * bassin)}");
