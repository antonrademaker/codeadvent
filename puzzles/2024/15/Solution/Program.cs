using System.Diagnostics;
using System.Text;
using Coordinate = AoC.Utilities.Coordinate<long>;
using Direction = AoC.Utilities.Coordinate<long>;

namespace Solution;

public partial class Program
{
    private static readonly List<string> inputFiles = ["input/example1.txt", "input/example2.txt", "input/example3.txt", "input/input.txt"];

    public static void Main(string[] _)
    {
        foreach (var file in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input = ParseFile(file);

            var startTime = Stopwatch.GetTimestamp();

            var answer1 = CalculateAnswer1(input);
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"{file}: Answer 1: {answer1} ({elapsedTime.TotalMilliseconds}ms)");
            Console.WriteLine();

            startTime = Stopwatch.GetTimestamp();
            var answer2 = CalculateAnswer2(input);
            elapsedTime = Stopwatch.GetElapsedTime(startTime);

            Console.WriteLine($"{file}: Answer 2: {answer2} ({elapsedTime.TotalMilliseconds}ms)");
        }
    }

    private static Input ParseFile(string file)
    {
        return new Input(File.ReadAllLines(file));
    }

    public static long CalculateAnswer1(Input input)
    {
        var map = input.Map;

        var robot = input.Start;

        foreach (var instruction in input.Directions)
        {
            var newPosition = robot + instruction;
            if (map.TryGetValue(newPosition, out var value))
            {
                if (value.Fixed)
                {
                    continue;
                }

                // try to move box(es)

                var movable = true;

                var coordinate = newPosition;

                while (movable)
                {
                    coordinate = coordinate + instruction;

                    if (map.TryGetValue(coordinate, out var nextMovableBoxValue))
                    {
                        if (nextMovableBoxValue.Fixed)
                        {
                            movable = false;
                            break;
                        }
                    }
                    else
                    {
                        // we found a free spot

                        map[coordinate] = MapObject.Box;
                        map.Remove(newPosition);
                        robot = newPosition;
                        break;
                    }
                }
            }
            else
            {
                robot = newPosition;
            }
        }

        var gpsCoordinates = map.Where(t => t.Value == MapObject.Box).Select(t => t.Key).ToList();

        var sum = gpsCoordinates.Sum(t => 100 * t.Y + t.X);

        return sum;
    }

    public static void PrintMap(Dictionary<Coordinate, MapObject> map, Coordinate robot, int width, int height)
    {
        StringBuilder sb = PositionsToStringBuilder(map, robot, width, height);

        Console.WriteLine(sb.ToString());
    }

    private static StringBuilder PositionsToStringBuilder(Dictionary<Coordinate, MapObject> map, Coordinate robot, int width, int height)
    {
        var sb = new StringBuilder();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (map.TryGetValue(new Coordinate(x, y), out var value))
                {
                    sb.Append(value.Fixed ? 'X' : value.Value);
                }
                else if (robot.X == x && robot.Y == y)
                {
                    sb.Append('@');
                }
                else
                {
                    sb.Append('.');
                }
            }

            sb.AppendLine();
        }

        return sb;
    }

    public static long CalculateAnswer2(Input input)
    {
        var map = input.MapPart2;

        var robot = input.StartPart2;

        foreach (Direction instruction in input.Directions)
        {
            var newPosition = robot + instruction;
            if (map.TryGetValue(newPosition, out var value))
            {
                if (value.Fixed)
                {
                    continue;
                }

                // try to move box(es)
                if (instruction == Coordinate.OffsetLeft || instruction == Coordinate.OffsetRight)
                {
                    robot = TryMoveHorizontal(map, robot, instruction, newPosition);
                }
                else
                {
                    robot = TryMoveVertical(map, robot, instruction, newPosition);
                }
            }
            else
            {
                robot = newPosition;
            }
        }

        var gpsCoordinates = map.Where(t => t.Value == MapObject.BoxLeft).Select(t => t.Key).ToList();

        var sum = gpsCoordinates.Sum(t => 100 * t.Y + t.X);

        return sum;
    }

    private static Coordinate TryMoveVertical(Dictionary<Coordinate, MapObject> map, Coordinate robot, Direction instruction, Coordinate newPosition)
    {
        // first check if we can move
        if (IsVerticallyMoveable(map, robot, instruction, newPosition))
        {
            // execute the moves            
            MoveVertically(map, robot, instruction, newPosition);

            return newPosition;
        }

        // not possible to move
        return robot;
    }

    private static bool IsVerticallyMoveable(Dictionary<Coordinate, MapObject> map, Coordinate robot, Direction instruction, Coordinate newPosition)
    {
        if (!map.TryGetValue(newPosition, out var value))
        {
            return true;
        }

        if (value.Fixed)
        {
            return false;
        }

        if (value == MapObject.BoxLeft)
        {
            return IsVerticallyMoveable(map, newPosition, instruction, newPosition + instruction) && IsVerticallyMoveable(map, newPosition, instruction, newPosition + instruction + Coordinate.OffsetRight);
        }

        if (value == MapObject.BoxRight)
        {
            return IsVerticallyMoveable(map, newPosition, instruction, newPosition + instruction) && IsVerticallyMoveable(map, newPosition, instruction, newPosition + instruction + Coordinate.OffsetLeft);
        }
        return false;
    }

    private static Coordinate MoveVertically(Dictionary<Coordinate, MapObject> map, Coordinate robot, Direction instruction, Coordinate newPosition)
    {
        if (!map.TryGetValue(newPosition, out var value))
        {
            return newPosition;
        }

        if (value == MapObject.BoxLeft)
        {
            MoveVertically(map, newPosition, instruction, newPosition + instruction);
            MoveVertically(map, newPosition, instruction, newPosition + instruction + Coordinate.OffsetRight);

            map[newPosition + instruction] = map[newPosition];
            map[newPosition + instruction + Coordinate.OffsetRight] = map[newPosition + Coordinate.OffsetRight];

            map.Remove(newPosition);
            map.Remove(newPosition + Coordinate.OffsetRight);
        }

        if (value == MapObject.BoxRight)
        {
            MoveVertically(map, newPosition, instruction, newPosition + instruction + Coordinate.OffsetLeft);
            MoveVertically(map, newPosition, instruction, newPosition + instruction);

            map[newPosition + instruction + Coordinate.OffsetLeft] = map[newPosition + Coordinate.OffsetLeft];
            map[newPosition + instruction] = map[newPosition];

            map.Remove(newPosition);
            map.Remove(newPosition + Coordinate.OffsetLeft);
        }
        return newPosition;
    }

    private static Coordinate TryMoveHorizontal(Dictionary<Coordinate, MapObject> map, Coordinate robot, Direction instruction, Coordinate newPosition)
    {
        var coordinate = newPosition;

        while (true)
        {
            coordinate = coordinate + instruction;

            if (map.TryGetValue(coordinate, out var nextMovableBoxValue))
            {
                if (nextMovableBoxValue.Fixed)
                {
                    break;
                }
            }
            else
            {
                var freePosition = coordinate;

                while (freePosition != newPosition)
                {
                    map[freePosition] = map[freePosition - instruction];

                    freePosition = freePosition - instruction;
                }

                map.Remove(newPosition);

                robot = newPosition;

                break;
            }
        }

        return robot;
    }
}

public record struct MapObject(char Value, bool Fixed)
{
    public readonly static MapObject Wall = new('#', true);

    public readonly static MapObject Box = new('O', false);
    public readonly static MapObject BoxLeft = new('[', false);
    public readonly static MapObject BoxRight = new(']', false);

    public static MapObject Parse(char value, bool? pos = null)
    {
        return value switch
        {
            '#' => Wall,
            'O' when pos is null => Box,
            'O' when pos.Value => BoxLeft,
            'O' when !pos.Value => BoxRight,
            _ => new MapObject(value, false)
        };
    }
}

public readonly struct Input
{
    public int Height { get; }
    public int Width { get; }

    public Input(string[] input)
    {
        Width = input[0].Length;

        var parsingMap = true;

        for (var y = 0; y < input.Length; y++)
        {
            if (input[y].Length == 0)
            {
                Height = y;
                parsingMap = false;
            }
            if (parsingMap)
            {
                for (var x = 0; x < input[y].Length; x++)
                {
                    if (input[y][x] == '@')
                    {
                        Start = new Coordinate(x, y);
                        StartPart2 = new Coordinate(x * 2, y);
                    }
                    else if (input[y][x] != '.')
                    {
                        Map[new Coordinate(x, y)] = MapObject.Parse(input[y][x]);

                        MapPart2[new Coordinate(x * 2, y)] = MapObject.Parse(input[y][x], true);
                        MapPart2[new Coordinate(x * 2 + 1, y)] = MapObject.Parse(input[y][x], false);
                    }
                }
            }
            else
            {
                for (var x = 0; x < input[y].Length; x++)
                {
                    Directions.Add(Direction.Parse(input[y][x]));
                }
            }
        }
    }

    public readonly Dictionary<Coordinate, MapObject> Map = [];
    public readonly Dictionary<Coordinate, MapObject> MapPart2 = [];
    public readonly List<Direction> Directions = [];
    public readonly Coordinate Start = default;
    public readonly Coordinate StartPart2 = default;
}