using System.Diagnostics;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using Coordinate = AoC.Utilities.Coordinate<int>;

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

        var result = 0;

        foreach (var machine in machines) {
            // a * X.A + b * X.B = X.Price
            // a * Y.A + b * Y.B = Y.Price


            // a = Y.Price - b * Y.B / Y.A


            // (Y.Price - b * Y.B / Y.A) * X.A + b * X.B = X.Price

            // X.A * Y.Price - b * Y.B + b * X.B * Y.A = X.Price * Y.A





        }



        return result;
    }



    public static long CalculateAnswer2(Input input)
    {
        return 0;
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
