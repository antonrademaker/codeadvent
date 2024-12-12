using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Coordinate = AoC.Utilities.Coordinate<int>;

namespace Solution;

public class Program
{
    private static readonly string[] inputFiles = ["input/example1.txt", "input/input.txt"];

    public static void Main(string[] args)
    {
        foreach (string file in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input = ParseFile(file);

            var startTime = Stopwatch.GetTimestamp();

            var answer1 = CalculateAnswer1(input);
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"{file}: Answer 1: {answer1} ({elapsedTime.TotalMilliseconds}ms)");

            startTime = Stopwatch.GetTimestamp();
            var answer2 = CalculateAnswer2(input);
            elapsedTime = Stopwatch.GetElapsedTime(startTime);

            Console.WriteLine($"{file}: Answer 2: {answer2} ({elapsedTime.TotalMilliseconds}ms)");
        }
    }

    static Input ParseFile(string file)
    {
        return new Input(File.ReadAllLines(file));
    }

    public static long CalculateAnswer1(Input input)
    {
        List<Region> regions = CalculateRegions(input);

        var result = regions.Sum(region => region.CoordinatesAndFences.Count * region.CoordinatesAndFences.Sum(t => t.Value));

        return result;
    }


    private static List<Region> CalculateRegions(Input input)
    {
        List<Region> regions = [];

        HashSet<Coordinate> visited = [];

        Queue<Coordinate> queue = [];

        queue.Enqueue(new Coordinate(0, 0));

        Queue<Coordinate> regionQueue = [];

        while (queue.TryDequeue(out var current))
        {
            if (!visited.Add(current))
            {
                continue;
            }
            var (valid, letter) = input.GetChar(current);
            if (!valid)
            {
                continue;
            }

            var currentRegion = new Region(letter);

            regionQueue.Enqueue(current);

            while (regionQueue.TryDequeue(out var candidate))
            {
                var fences = 0;
                if (currentRegion.CoordinatesAndFences.ContainsKey(candidate))
                {
                    continue;
                }

                foreach (var dir in Coordinate.Offsets)
                {
                    var next = candidate + dir;
                    var (nextValid, nextLetter) = input.GetChar(next);
                    if (!nextValid)
                    {
                        fences++;

                        continue;
                    }
                    if (nextLetter == letter)
                    {
                        visited.Add(candidate);
                        regionQueue.Enqueue(next);
                    }
                    else
                    {

                        queue.Enqueue(next);
                        fences++;
                    }
                }

                currentRegion.CoordinatesAndFences.Add(candidate, fences);

            }
            regions.Add(currentRegion);
        }

        return regions;
    }

    public static long CalculateAnswer2(Input input)
    {

        List<Region> regions = CalculateRegions(input);

        var result = regions.Sum(region => region.CoordinatesAndFences.Count * CalculateCorners(region));

                return result;
    }

    private static int CalculateCorners(Region region)
    {
        var corners = 0;
        var coords = region.CoordinatesAndFences.Keys;
        foreach (var coord in coords)
        {
            bool containsUp = coords.Contains(coord.Up);
            bool containsUpRight = coords.Contains(coord.Up.Right);
            bool containsRight = coords.Contains(coord.Right);
            bool containsUpLeft = coords.Contains(coord.Up.Left);
            bool containsLeft = coords.Contains(coord.Left);
            bool containsDownRight = coords.Contains(coord.Down.Right);
            bool containsDown = coords.Contains(coord.Down);
            bool containsDownLeft = coords.Contains(coord.DownLeft);

            // inward corners
            if (containsUp && containsUpRight && !containsRight)
            {
                corners++;
            }

            if (containsUp && containsUpLeft && !containsLeft)
            {
                corners++;
            }

            if (containsDown && containsDownLeft && !containsLeft)
            {
                corners++;
            }

            if (containsDown && containsDownRight && !containsRight)
            {
                corners++;
            }

            // outer corners
            if (!containsUp && !containsLeft)
            {
                corners++;
            }
            if (!containsUp && !containsRight)
            {
                corners++;
            }
            if (!containsDown && !containsLeft)
            {
                corners++;
            }
            if (!containsDown && !containsRight)
            {
                corners++;
            }
        }

        return corners;
    }
}

public class Region(char letter)
{
    public Dictionary<Coordinate, int> CoordinatesAndFences = [];

    public HashSet<Perimeter> Perimeters = [];

    public char Letter { get; } = letter;
}

public class Perimeter
{
    public Coordinate Start { get; set; }
    public Coordinate End { get; set; }
}

public readonly ref struct Input(string[] input)
{
    private readonly ReadOnlySpan<char> Map = string.Concat(input).AsSpan();
    public int Width { get; } = input[0].Length;
    public int Height { get; } = input.Length;

    public (bool valid, char letter) GetChar(Coordinate loc)
    {
        if (loc.X < 0 || loc.X >= Width || loc.Y < 0 || loc.Y >= Height)
        {
            return (false, ' ');
        }
        return (true, Map[loc.Y * Width + loc.X]);
    }
}
