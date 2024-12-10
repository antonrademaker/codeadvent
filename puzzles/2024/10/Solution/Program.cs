using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using AoC.Utilities;

using Coordinate = AoC.Utilities.Coordinate<int>;

namespace Solution;

public class Program
{
    internal const bool EnableDebug = false;

    internal const int EmptyBlock = -1;

    static readonly string[] inputFiles = ["input/example1.txt", "input/input.txt"];

    public static void Main(string[] args)
    {

        foreach (string file in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input = ParseFile(file);

            var answer1 = CalculateAnswer1(input);
            Console.WriteLine($"{file}: Answer 1: {answer1}");

            var answer2 = CalculateAnswer2(input);
            Console.WriteLine($"{file}: Answer 2: {answer2}");
        }
    }

    public static int CalculateAnswer1(Input input)
    {
        var paths = 0;

        Queue<(Coordinate loc, int height)> queue = [];

        foreach (var startLoc in input.GetStartLocations())
        {
            queue.Enqueue((startLoc, 0));

            HashSet<Coordinate> reachable = [];

            while (queue.TryDequeue(out var work))
            {
                var (location, currentHeight) = work;
                var nextHeight = currentHeight + 1;

                foreach (var dir in Coordinate.Offsets)
                {
                    var locationCandidate = location + dir;
                    var (valid, height) = input.GetHeight(locationCandidate);
                    if (!valid || height == -1)
                    {
                        // invalid or impassable 
                        continue;
                    }

                    if (currentHeight == 8 && height == 9)
                    {
                        reachable.Add(locationCandidate);
                        continue;
                    }

                    if (height == nextHeight)
                    {
                        queue.Enqueue((locationCandidate, nextHeight));
                        continue;
                    }

                }
            }

            paths += reachable.Count;
        }
        return paths;
    }

    static Input ParseFile(string file)
    {
        return new Input(File.ReadAllLines(file));
    }

    public static int CalculateAnswer2(Input input)
    {
        return 0;
    }
}


public readonly ref struct Input(string[] input)
{
    private readonly ReadOnlySpan<int> Map = string.Concat(input).Select(c => c== '.'? -1 : c - '0').ToArray().AsSpan();
    public int Width { get; } = input[0].Length;
    public int Height { get; } = input.Length;

    public (bool valid, int letter) GetHeight(Coordinate loc)
    {
        if (loc.X < 0 || loc.X >= Width || loc.Y < 0 || loc.Y >= Height)
        {
            return (false, -1);
        }
        return (true, Map[loc.Y * Width + loc.X]);
    }

    public List<Coordinate> GetStartLocations()
    {
        List<Coordinate> locs = [];

        for(var i = 0; i < Map.Length; i++)
        {
            if (Map[i] == 0)
            {
                locs.Add(new(i % Width, i / Width));
            }
        }

        return locs;
    }
}