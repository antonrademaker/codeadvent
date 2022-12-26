using System.Diagnostics;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        var runDebug = true;

        var files = Directory.GetFiles("input", "*.txt");

        foreach (var file in files.OrderBy(t => t))
        {
            Console.WriteLine($"{file}");

            var lines = File.ReadAllLines(file);

            var (map, start, movements) = Parse(lines);

            Console.WriteLine(start.Point);
            Console.WriteLine(movements);

            var endState = Calculate(map, start, movements);

            Console.WriteLine($"Part 1:  1000 * {endState.Point.Y} + 4 * {endState.Point.X} + {endState.Direction}: {(1000 * endState.Point.Y) + (4 * endState.Point.X) + endState.Direction}");
        }
    }

    private static State Calculate(Map map, Location start, string movements)
    {
        var state = new State()
        {
            Point = start.Point,
            Direction = 0
        };

        var move = 0;

        var startOfNumber = 0;
        var numberLength = 0;

        var moves = movements.AsSpan();

        var logs = new List<string>();

        logs.Add($"Start at: {state.Point}");

        for (var i = 0; i < movements.Length; i++)
        {
            if (movements[i] == 'R' || movements[i] == 'L')
            {
                // calculate movement

                if (!int.TryParse(moves.Slice(startOfNumber, numberLength), out var steps))
                {
                    throw new Exception($"Could not parse number out of {movements.Substring(startOfNumber, numberLength)}");
                }

                CalculateMove(map, state, steps, logs);

                logs.Add($"After move {state.Point}");

                // rotate

                CalculateRotation(map, state, movements[i], logs);

                startOfNumber = i + 1;
                numberLength = 0;
            }
            else
            {
                numberLength++;
            }
        }

        // one last movement
        if (numberLength > 0)
        {
            if (!int.TryParse(moves.Slice(startOfNumber, numberLength), out var steps))
            {
                throw new Exception($"Could not parse number out of {movements.Substring(startOfNumber, numberLength)}");
            }
            CalculateMove(map, state, steps, logs);
            logs.Add($"After last move {state.Point}");
        }

        Console.WriteLine($"Locations : {string.Join("\r\n", logs)}");

        return state;
    }

    private static void CalculateMove(Map map, State state, int steps, List<string> log)
    {
        var direction = Directions[state.Direction];

        var newLocation = state.Point;

        log.Add($"Moving {steps}");

        for (var step = 0; step < steps; step++)
        {
            if (map.Locations[newLocation] is OpenTile ot)
            {
                if (ot.Part1Wraps.TryGetValue(direction, out var shortcut))
                {
                    log.Add($"Wrapping to {shortcut.Point}");

                    newLocation = shortcut.Point;
                }
                else
                {
                    var candidate = newLocation + direction;
                    if (map.Locations.TryGetValue(candidate, out var loc))
                    {
                        if (loc is OpenTile)
                        {
                            log.Add($"Moving to {candidate}");
                            newLocation = loc.Point;
                        }
                        else
                        {
                            log.Add($"Hit a wal at {candidate}");

                            // It's a wall
                            break;
                        }
                    }
                    else
                    {
                        log.Add($"Hit a wal at {candidate} (empty)");
                        break;
                    }
                }
            }
            else
            {
                log.Add($"I'm a wal? {newLocation}");
                break;
            }
        }
        state.Point = newLocation;
    }

    private static void CalculateRotation(Map map, State state, char v, List<string> log)
    {
        var oldDirection = state.Direction;
        state.Direction = ((state.Direction + (v == 'L' ? -1 : 1)) + 4) % 4;

        log.Add($"Rotating ({v}) from {oldDirection} to {state.Direction}");
    }

    private static readonly Point[] Directions = new Point[]
    {
        new Point(1,0),
        new Point(0,1),
        new Point(-1,0),
        new Point(0,-1),
    };

    private static (Map map, Location start, string movements) Parse(string[] lines)
    {
        var scanningMap = true;
        var map = new Map();
        var start = default(OpenTile);

        var height = 0;
        var width = 0;

        for (var y = 1; y <= lines.Length; y++)
        {
            var line = lines[y - 1];
            if (string.IsNullOrEmpty(line))
            {
                scanningMap = false;
                continue;
            }

            if (scanningMap)
            {
                var left = 1;

                for (var x = 1; x <= line.Length; x++)
                {
                    var col = line[x - 1];
                    if (col == ' ')
                    {
                        left++;
                    }
                    else
                    {
                        Location location = col == '.' ? new OpenTile { Point = new Point(x, y) } : new Wall { Point = new Point(x, y) };
                        map.Locations.Add(location.Point, location);
                        if (start is null && location is OpenTile ot)
                        {
                            start = ot;
                        }
                    }
                    if (line.Length > width)
                    {
                        width = line.Length;
                    }
                }

                // optimize X axis

                var leftPoint = new Point(left, y);
                var rightPoint = new Point(line.Length, y);

                if (map.Locations.TryGetValue(leftPoint, out var leftLocation) &&
                    leftLocation is OpenTile leftOpen &&
                    map.Locations.TryGetValue(rightPoint, out var rightLocation) &&
                    rightLocation is OpenTile rightOpen)
                {
                    leftOpen.Part1Wraps.Add(new Point(-1, 0), rightLocation);
                    rightOpen.Part1Wraps.Add(new Point(1, 0), leftLocation);
                }

                height++;
            }
            else
            {
                // first optimize y axis

                for (var xLoc = 1; xLoc <= width; xLoc++)
                {
                    var top = default(Location);
                    var bottom = default(Location);
                    for (var yLoc = 1; yLoc <= height; yLoc++)
                    {
                        if (top is null)
                        {
                            if (map.Locations.TryGetValue(new Point(xLoc, yLoc), out var topCandidate))
                            {
                                top = topCandidate;
                            }
                        }
                        else
                        {
                            if (map.Locations.TryGetValue(new Point(xLoc, yLoc), out var bottomCandidate))
                            {
                                bottom = bottomCandidate;
                            }
                        }
                    }

                    if (top is OpenTile topTile && bottom is OpenTile bottomTile)
                    {
                        topTile.Part1Wraps.Add(new Point(0, -1), bottomTile);
                        bottomTile.Part1Wraps.Add(new Point(0, 1), topTile);
                    }
                }
                Debug.Assert(start is { });
                return (map, start!, line);
            }
        }
        throw new Exception("Could not parse input");
    }
}

public abstract class Location
{
    public Point Point { get; init; }
}

public class OpenTile : Location
{
    public Dictionary<Point, Location> Part1Wraps { get; } = new();
}

public class Wall : Location
{
}

public class Map
{
    public readonly Dictionary<Point, Location> Locations = new();
}

public record struct Point(int X, int Y)
{
    public static Point operator +(Point a, Point b) => a with { X = a.X + b.X, Y = a.Y + b.Y };
    public static Point operator -(Point a, Point b) => a with { X = a.X - b.X, Y = a.Y - b.Y };
};

public class State
{
    public Point Point { get; set; }
    public int Direction { get; set; }
}