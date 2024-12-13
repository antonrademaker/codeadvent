using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using Coordinate = AoC.Utilities.Coordinate<long>;

namespace Solution;

public partial class Program
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
       var machines = input.GetMachines();
        return machines.Sum(machine => CalculatePrize(machine.A, machine.B, machine.Price));
    }

    public static long CalculatePrize(Coordinate A, Coordinate B, Coordinate price)
    {
        var top = A.X * price.Y - A.Y * price.X;
        var bottom = A.X * B.Y - B.X * A.Y;
        if (top % bottom == 0)
        {
            var b = top / bottom;
            top = price.Y - b * B.Y;
            if (top % A.Y == 0)
            {
                return 3 * top / A.Y + b;
            }
        }
        return 0;
    }


    public static long CalculateAnswer2(Input input)
    {
        var machines = input.GetMachines();
        return machines.Sum(machine => CalculatePrize(machine.A, machine.B, machine.Price + 10000000000000L));
    }

    public static readonly Regex ParseRegex = Parser();

    [GeneratedRegex(@"X([\+=])(?<X>\d+), Y([\+=])(?<Y>\d+)", RegexOptions.Compiled)]
    private static partial Regex Parser();
}

public readonly ref struct Input(string[] input)
{
    private readonly string[] StringInput = input;
    
    public List<Machine> GetMachines()
    {
        List<Machine> machines = [];

        for(int i = 0; i < StringInput.Length; i+= 4)
        {            
            machines.Add(new Machine(GetCoordinate(StringInput[i]), GetCoordinate(StringInput[i+1]), GetCoordinate(StringInput[i+2])));
        }

        return machines;
    }

    private Coordinate GetCoordinate(string input)
    {
        var coords = Program.ParseRegex.Match(input);

        return new Coordinate(int.Parse(coords.Groups["X"].Value), int.Parse(coords.Groups["Y"].Value));
    }
}

public readonly record struct Machine(Coordinate A, Coordinate B, Coordinate Price)
{
    
}
