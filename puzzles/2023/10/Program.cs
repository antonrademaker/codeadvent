using System.Diagnostics;

string[] inputFiles = ["input/example.txt", "input/example1.txt","input/input.txt"];

foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var inputRows = inputs.SelectMany((input, y) => input.Select((c, x) => (x, y, location: new Location(c)))).ToDictionary(t => (t.x, t.y), t => t.location);

    var part1 = CalculatePart1(inputRows);

    Console.WriteLine($"Part 1: {part1}");

    //var part2 = CalculatePart2(inputRows);

    //Console.WriteLine($"Part 2: {part2}");
}

long CalculatePart1(Dictionary<(int x, int y), Location> inputs)
{
    var startX = 0;
    var startY = 0;

    foreach (var loc in inputs.Keys)
    {
        if (inputs[loc].IsStart)
        {
            startX = loc.x;
            startY = loc.y;

        }
    }

    var distances = new Dictionary<(int x, int y), int>();

    var locations = new Queue<(int x, int y, Direction Direction, int depth)>();

    AddLocationToQueue(locations, startX, startY, -1, 0);
    AddLocationToQueue(locations, startX, startY, 1, 0);
    AddLocationToQueue(locations, startX, startY, 0, -1);
    AddLocationToQueue(locations, startX, startY, 0, 1);

    while (locations.TryDequeue(out var location))
    {
        if (inputs.TryGetValue((location.x, location.y), out var nextCandidate)) {

            Console.Write(nextCandidate.Character);

            if (nextCandidate.IsStart)
            {                
                continue;
            }

            if (!distances.TryGetValue(((location.x, location.y)), out var dist) || dist > location.depth)
            {
                distances[(location.x, location.y)] = location.depth;
            }

            if (nextCandidate.First == location.Direction)
            {
                locations.Enqueue((location.x - nextCandidate.Second.X, location.y - nextCandidate.Second.Y, new Direction(-nextCandidate.Second.X, -nextCandidate.Second.Y), location.depth + 1));
            }
            else if (nextCandidate.Second == location.Direction)
            {
                locations.Enqueue((location.x - nextCandidate.First.X, location.y - nextCandidate.First.Y, new Direction(-nextCandidate.First.X, -nextCandidate.First.Y), location.depth + 1));
            }
        }
    }

    return distances.Values.Max();
}

void AddLocationToQueue(Queue<(int x, int y, Direction dir, int depth)> queue, int startX, int startY, int xDiff, int yDiff)
{
    queue.Enqueue((startX + xDiff, startY + yDiff, new Direction(xDiff, yDiff), 1));
}

readonly record struct Location
{
    public readonly char Character { get; }
    public readonly Direction First { get; }
    public readonly Direction Second { get; }
    public readonly bool IsStart { get; }

    public Location(char Location)
    {
        Character = Location;
        (First, Second) = Location switch
        {
            '|' => (new Direction(0, -1), new Direction(0, 1)),
            '-' => (new Direction(-1, 0), new Direction(1, 0)),
            'L' => (new Direction(0, 1), new Direction(-1, 0)),
            'J' => (new Direction(0, 1), new Direction(1, 0)),
            '7' => (new Direction(1, 0), new Direction(0, -1)),
            'F' => (new Direction(-1, 0), new Direction(0, -1)),
            '.' => (new Direction(1, 1), new Direction(1, 1)),
            'S' => (new Direction(0, 0), new Direction(0, 0)),
            _ => throw new ArgumentException("Invalid location", nameof(Location))
        };

        IsStart = First == Second && First == new Direction(0, 0);
    }
}

readonly record struct Direction(int X, int Y);