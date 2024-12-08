using System.ComponentModel.DataAnnotations;
using AoC.Utilities;
using Coordinate = AoC.Utilities.Coordinate<int>;


string[] inputFiles = ["input/example.txt", "input/input.txt"];


foreach (string file in inputFiles)
{
    Console.WriteLine($"Reading: {file}");

    var input = ParseFile(file);

    var answer1 = CalculateAnswer1(input);
    Console.WriteLine($"{file}: Answer 1: {answer1}");

    var answer2 = CalculateAnswer2(input);
    Console.WriteLine($"{file}: Answer 2: {answer2}");
}



static Input ParseFile(string file)
{
    return new Input(File.ReadAllLines(file));
}

public readonly ref struct Input(string[] input)
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

    public Dictionary<char, List<Coordinate>> GetAntennas()
    {
        var antennas = new Dictionary<char, List<Coordinate>>();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var frequency = Map[y * Width + x];
                if (frequency == '.')
                {
                    continue;
                }
                if (antennas.TryGetValue(frequency, out var value))
                {
                    value.Add(new Coordinate(x, y));
                }
                else
                {
                    antennas.Add(frequency, [new Coordinate(x, y)]);
                }
            }
        }
        return antennas;
    }

    public List<(Coordinate a, Coordinate b)> GetAntennaPairs()
    {
        var antennas = GetAntennas();

        List<(Coordinate a, Coordinate b)> pairs = [];

        foreach (var (frequency, locations) in antennas)
        {
            for (var i = 0; i < locations.Count; i++)
            {
                for (var j = 0; j < locations.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    pairs.Add((locations[i], locations[j]));
                }
            }
        }

        return pairs;
    }
}

public partial class Program
{
    public static long CalculateAnswer1(Input input)
    {
        HashSet<Coordinate> antinodes = [];

        foreach (var (a, b) in input.GetAntennaPairs())
        {
            var distance = a - b;

            var antinode = a + distance;

            var (valid, _) = input.GetLetter(antinode.X, antinode.Y);

            if (valid)
            {
                antinodes.Add(antinode);
            }
        }

        return antinodes.Count;
    }

    public static long CalculateAnswer2(Input input)
    {
        HashSet<Coordinate> antinodes = [];

        foreach (var (a, b) in input.GetAntennaPairs())
        {
            var distance = a - b;
            var antinode = a;
            var valid = true;

            while (valid)
            {
                antinodes.Add(antinode);
                antinode += distance;
                (valid, _) = input.GetLetter(antinode.X, antinode.Y);
            }

        }

        return antinodes.Count;
    }


}