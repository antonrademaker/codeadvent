using AoC.Utilities;
using System.Runtime.InteropServices;
using System.Timers;
using CoordinateDirection = (AoC.Utilities.Coordinate Position, AoC.Utilities.Coordinate Direction);

namespace AoC.Puzzles.Y2023.D16;

public static class Program
{
    private static void Main(string[] args)
    {
        string[] inputFiles = [
            "input/example.txt"
            , "input/input.txt"
            ];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var part1 = CalculatePart1(inputs);

            Console.WriteLine($"Part 1: {part1}");

            var part2 = CalculatePart2(inputs);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static long CalculatePart1(string[] input)
    {
        var (width, height, parsed) = ParseInputs(input);

        var currentPosition = Coordinate.OffsetLeft;

        var currentDirection = Coordinate.OffsetRight;

        var energized = CalculateBeam(parsed, width, height, currentPosition, currentDirection);
        return energized.Count;
    }

    private static long CalculatePart2(string[] input)
    {
        var (width, height, parsed) = ParseInputs(input);

        var beams = new List<CoordinateDirection>();

        beams.AddRange(Enumerable.Range(0, width).Select(x => new CoordinateDirection(new Coordinate(x,-1), Coordinate.OffsetDown)));
        beams.AddRange(Enumerable.Range(0, width).Select(x => new CoordinateDirection(new Coordinate(x,height), Coordinate.OffsetUp)));
        beams.AddRange(Enumerable.Range(0, height).Select(y => new CoordinateDirection(new Coordinate(-1,y), Coordinate.OffsetRight)));
        beams.AddRange(Enumerable.Range(0, height).Select(y => new CoordinateDirection(new Coordinate(width,y), Coordinate.OffsetLeft)));

        return beams.Max(beam => CalculateBeam(parsed, width, height, beam.Position, beam.Direction).Count());
    }

    private static HashSet<Coordinate> CalculateBeam(Dictionary<Coordinate, char> grid, int width, int height, Coordinate currentPosition, Coordinate currentDirection)
    {
       // var energized = new HashSet<Coordinate>();
        var energizedDirections = new HashSet<(Coordinate position,Coordinate direction)>();
        
        var queue = new Queue<(Coordinate position, Coordinate direction)>();
        queue.Enqueue((currentPosition, currentDirection));

        while (queue.TryDequeue(out (Coordinate position, Coordinate direction) state))
        {
            var (oldPosition,direction) = state;
            var position = oldPosition + direction;

            if (position.X < 0 || position.X >= width || position.Y < 0 || position.Y >= height)
            {
                continue;
            }

            if (!energizedDirections.Add((position, direction)))
            {
                // already visited
                continue;
            }

            switch (grid[position])
            {
                case '.':
                    queue.Enqueue((position, direction));
                    break;

                case '/':
                    queue.Enqueue((position, CalculateMirror(direction, false)));
                    break;
                case '\\':
                    queue.Enqueue((position, CalculateMirror(direction, true)));
                    break;
                case '|':
                    if (direction == Coordinate.OffsetLeft || direction == Coordinate.OffsetRight)
                    {
                        queue.Enqueue((position, Coordinate.OffsetUp));
                        queue.Enqueue((position, Coordinate.OffsetDown));
                    } else
                    {
                        queue.Enqueue((position, direction));
                    }
                    break;

                case '-':
                    if (direction == Coordinate.OffsetDown || direction == Coordinate.OffsetUp)
                    {
                        queue.Enqueue((position, Coordinate.OffsetLeft));
                        queue.Enqueue((position, Coordinate.OffsetRight));                       
                    }
                    else
                    {
                        queue.Enqueue((position, direction));
                    }
                    break;


                default: throw new Exception("Invalid input");
            };
        }

        return energizedDirections.Select(t => t.position).ToHashSet();
    }

    private static void Print(HashSet<Coordinate> coordinates, int width, int height)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (coordinates.Contains(new Coordinate(x, y)))
                {
                    Console.Write('.');

                }
                else
                {
                    Console.Write(' ');
                }
            }
            Console.WriteLine();
        }
    }

    private static Coordinate CalculateSplitter(Coordinate direction, bool invert)
    {
        if (direction == Coordinate.OffsetLeft)
        {
            return invert ? Coordinate.OffsetUp : Coordinate.OffsetDown;
        }
        else if (direction == Coordinate.OffsetRight)
        {
            return invert ? Coordinate.OffsetDown : Coordinate.OffsetUp;
        }
        else if (direction == Coordinate.OffsetUp)
        {
            return invert ? Coordinate.OffsetLeft : Coordinate.OffsetRight;
        }
        else if (direction == Coordinate.OffsetDown)
        {
            return invert ? Coordinate.OffsetRight : Coordinate.OffsetLeft;
        }
        else
        {
            throw new Exception("Invalid direction");
        }
    }

    private static Coordinate CalculateMirror(Coordinate direction, bool invert)
    {

        if (direction == Coordinate.OffsetLeft)
        {
            return invert ? Coordinate.OffsetUp : Coordinate.OffsetDown;
        }
        else if (direction == Coordinate.OffsetRight)
        {
            return invert ? Coordinate.OffsetDown : Coordinate.OffsetUp;
        }
        else if (direction == Coordinate.OffsetUp)
        {
            return invert ? Coordinate.OffsetLeft : Coordinate.OffsetRight;
        }
        else if (direction == Coordinate.OffsetDown)
        {
            return invert ? Coordinate.OffsetRight : Coordinate.OffsetLeft;
        }
        else
        {
            throw new Exception("Invalid direction");
        }


    }

    private static (int width, int height, Dictionary<Coordinate, char>) ParseInputs(string[] inputs)
    {

        var grid = new Dictionary<Coordinate, char>();

        for (var y = 0; y < inputs.Length; y++)
        {
            for (var x = 0; x < inputs[y].Length; x++)
            {
                grid[new Coordinate(x, y)] = inputs[y][x];
            }
        }
        return (inputs[0].Length, inputs.Length, grid);
    }
}