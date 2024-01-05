using System.Diagnostics;

namespace AoC.Puzzles.Y2023.D10;

public static class Program
{
    private static readonly char[] oddEven = ['|', 'J', 'L'];



    private static void Main(string[] args)
    {
        string[] inputFiles = ["input/example.txt", "input/example_a.txt", "input/example1.txt", "input/example2.txt", "input/input.txt"];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var inputRows = inputs.SelectMany((input, y) => input.Select((c, x) => (x, y, location: new Location(c)))).ToDictionary(t => (t.x, t.y), t => t.location);

            var part1 = CalculatePart1(inputRows);

            Console.WriteLine($"Part 1: {part1}");

            var part2 = CalculatePart2(inputRows);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static long CalculatePart1(Dictionary<(int x, int y), Location> inputs)
    {
        Dictionary<(int x, int y), int> distances = CalculateLoop(inputs);

        return distances.Values.Max();
    }

    private static long CalculatePart2(Dictionary<(int x, int y), Location> inputs)
    {
        Dictionary<(int x, int y), int> distances = CalculateLoop(inputs);

        var maxY = inputs.Keys.Max(t => t.y);
        var maxX = inputs.Keys.Max(t => t.x);
        var enclosed = 0;

        for (var y = 0; y <= maxY; y++)
        {
            var crossings = 0;
            for (var x = 0; x <= maxX; x++)
            {
                var loc = inputs[(x, y)];
                if (loc.IsStart)
                {
                    var neighbors = Direction.AllDirections.Select(dir => (dir, x: x + dir.X, y: y + dir.Y)).Where(val => distances.ContainsKey((val.x, val.y))).Select(val => val.dir).ToArray();

                    var character = CharMapper.First(l => l.Value[0] == -neighbors[0] && l.Value[1] == -neighbors[1] || l.Value[1] == -neighbors[0] && l.Value[0] == -neighbors[1]);

                    loc = loc with
                    {
                        Character = character.Key
                    };
                }
                var partOfLoop = distances.ContainsKey((x, y));

                if (partOfLoop)
                {
                    if (oddEven.Contains(loc.Character))
                    {
                        crossings++;
                    }
                }
                else if (crossings % 2 == 1)
                {
                    enclosed++;
                }
            }
        }

        return enclosed;
    }

    private static Dictionary<(int x, int y), int> CalculateLoop(Dictionary<(int x, int y), Location> inputs)
    {
        var (startX, startY) = inputs.First(t => t.Value.IsStart).Key;

        var distances = new Dictionary<(int x, int y), int>
        {
            [(startX, startY)] = 0
        };

        var locations = new Queue<(int x, int y, Direction Direction, int depth)>();

        foreach (var direction in Direction.AllDirections)
        {
            locations.Enqueue((startX + direction.X, startY + direction.Y, direction, 1));
        }

        while (locations.TryDequeue(out var location))
        {
            if (inputs.TryGetValue((location.x, location.y), out var nextCandidate))
            {
                if (nextCandidate.IsStart)
                {
                    continue;
                }

                for (var i = 0; i < nextCandidate.Directions.Length; i++)
                {
                    if (nextCandidate.Directions[i] == location.Direction)
                    {
                        if (!distances.TryGetValue(((location.x, location.y)), out var dist) || dist > location.depth)
                        {
                            distances[(location.x, location.y)] = location.depth;
                        }
                        locations.Enqueue((location.x - nextCandidate.Directions[1 - i].X, location.y - nextCandidate.Directions[1 - i].Y, -nextCandidate.Directions[1 - i], location.depth + 1));
                    }
                }
            }
        }

        return distances;
    }

    public static readonly IReadOnlyDictionary<char, Direction[]> CharMapper = new Dictionary<char, Direction[]>
    {
         { '|' , [Direction.Up, Direction.Down] },
         {   '-' , [Direction.Left, Direction.Right] },
         {   'L' , [Direction.Down, Direction.Left] },
         {   'J' , [Direction.Down, Direction.Right]},
         {   '7' , [Direction.Right, Direction.Up]},
         {   'F' , [Direction.Left, Direction.Up]},
         {   '.' , [new Direction(1, 1), new Direction(1, 1)]},
         {   'S' , [new Direction(0, 0), new Direction(0, 0)]},
    };
}

readonly record struct Location
{
    public readonly char Character { get; init; }
    public readonly Direction[] Directions { get; }
    public readonly bool IsStart { get; }

    public Location(char Location)
    {
        IsStart = Location == 'S';
        Character = Location;
        Directions = Program.CharMapper[Location];
    }
}

public readonly record struct Direction(int X, int Y)
{
    public static readonly Direction Up = new(0, -1);
    public static readonly Direction Down = new(0, 1);
    public static readonly Direction Left = new(-1, 0);
    public static readonly Direction Right = new(1, 0);

    public static readonly Direction[] AllDirections = [Up, Down, Left, Right];

    public static Direction operator -(Direction direction) => new(-direction.X, -direction.Y);
}