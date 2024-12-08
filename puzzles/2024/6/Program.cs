﻿using System.Diagnostics;
using AoC.Utilities;

string[] inputFiles = ["input/example.txt", "input/example2.txt", "input/input.txt"/**/];

foreach (string file in inputFiles)
{
    Console.WriteLine($"Reading: {file}");

    var input = ParseFile(file);

    var answer1 = CalculateAnswer1(input);
    Console.WriteLine($"{file}: Answer 1: {answer1}");

    var answer2 = CalculateAnswer2(input);
    Console.WriteLine($"{file}: Answer 2: {answer2}");
}

static int CalculateAnswer1(Input input)
{
    var (locations, _) = GetGuardPath(input, input.GetStart(), Coordinate<int>.OffsetUp);

    // PrintLocations(input, locations.Select(t => t.coord).ToList(), new Coordinate<int>(-1, -1));

    return locations.Select(t => t.coord).Distinct().Count();
}

static (List<(Coordinate<int> coord, Coordinate<int> direction)> locations, int loops) GetGuardPath(Input input,
    Coordinate<int> guardPosition,
    Coordinate<int> guardDirection,
    Coordinate<int>? extraObstruction = null)
{
    List<(Coordinate<int> coord, Coordinate<int> direction)> locations = [];
    HashSet<string> guardLocations = [];

    var loops = 0;

    locations.Add((guardPosition, guardDirection));

    guardLocations.Add(GetKey(guardPosition, guardDirection));

    Debug.Assert(!extraObstruction.HasValue || extraObstruction != input.GetStart());

    while (true)
    {
        var nextPosition = guardPosition + guardDirection;

        var (valid, position) = input.GetLetter(nextPosition.X, nextPosition.Y);
        if (!valid)
        {
            // outside of the map
            break;
        }

        if ((position == '.' || position == '^') && extraObstruction != nextPosition)
        {
            guardPosition = nextPosition;
            locations.Add((guardPosition, guardDirection));
        }
        else
        {
            guardDirection = guardDirection.RotateRight;

            locations.Add((guardPosition, guardDirection));

            if (!guardLocations.Add(GetKey(guardPosition, guardDirection)))
            {
                //      PrintLocations(input, locations.Select(t => t.coord).ToList(), extraObstruction);
                loops++;
                break;
            }
        }
    }

    return (locations, loops);
}

static string GetKey(Coordinate<int> guardPosition, Coordinate<int> guardDirection)
{
    return $"{guardPosition.X},{guardPosition.Y}|{guardDirection.X},{guardDirection.Y}";
}

static int CalculateAnswer2(Input input)
{
    var (locations, _) = GetGuardPath(input, input.GetStart(), Coordinate<int>.OffsetUp);

    HashSet<Coordinate<int>> loopLocations = [];
    HashSet<Coordinate<int>> visited = [];

    foreach (var (coord, direction) in locations.GroupBy(t => t.coord).Select(t => (t.Key, t.Last().direction)))
    {
        visited.Add(coord);

        var nextGuardPosition = coord + direction;
        var (valid, nextGuardLetter) = input.GetLetter(nextGuardPosition.X, nextGuardPosition.Y);

        if (valid && nextGuardLetter == '.' && !visited.Contains(nextGuardPosition))
        {
            // check if we can find a loop
            var nextDirection = direction.RotateRight;
            var (newLocations, loops) = GetGuardPath(input, coord, nextDirection, nextGuardPosition);
            if (loops > 0)
            {
                // Console.WriteLine($"Found loop at {nextGuardPosition}");
                loopLocations.Add(nextGuardPosition);
            }
        }
    }
    return loopLocations.Count;
}

static void PrintLocations(Input input, List<Coordinate<int>> locations, Coordinate<int>? extraObstruction)
{
    for (var y = 0; y < input.Height; y++)
    {
        for (var x = 0; x < input.Width; x++)
        {
            var (valid, letter) = input.GetLetter(x, y);

            var coord = new Coordinate<int>(x, y);

            if (locations.Contains(coord))
            {
                if (letter == '^')
                {
                    Console.Write("^");
                    continue;
                }
                Console.Write("X");
                continue;
            }
            if (extraObstruction == coord)
            {
                Console.Write("E");
                continue;
            }
            Console.Write(letter);
        }
        Console.WriteLine();
    }
}

static Input ParseFile(string file)
{
    return new Input(File.ReadAllLines(file));
}

readonly ref struct Input(string[] input)
{
    private readonly ReadOnlySpan<char> Map = string.Concat(input).AsSpan();
    public int Width { get; } = input[0].Length;
    public int Height { get; } = input.Length;

    public (bool valid, char letter) GetLetter(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return (false, ' ');
        }
        return (true, Map[y * Width + x]);
    }

    public Coordinate<int> GetStart()
    {
        var location = Map.IndexOf('^');

        return new(location % Width, location / Width);
    }
}

internal partial class Program
{
}