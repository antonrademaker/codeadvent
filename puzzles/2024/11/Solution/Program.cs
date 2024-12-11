using System.Diagnostics;

namespace Solution;

public class Program
{
    private static readonly string[] inputFiles = ["input/example1.txt", "input/input.txt"];

    static Dictionary<(long stones, int steps), long> Cache = [];

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
        return new Input(File.ReadAllText(file));
    }

    public static long CalculateAnswer1(Input input)
    {
        var steps = 25;

        return input.Map.Sum(stones => CalculateStones(stones, steps));
    }

    public static long CalculateAnswer2(Input input)
    {
        var steps = 75;

        return input.Map.Sum(stones => CalculateStones(stones, steps));
    }

    public static long CalculateStones(long stones, int steps)
    {
        if (Cache.TryGetValue((stones, steps), out var value))
        {
            return value;
        }

        var result = 1L;
        if (steps == 0)
        {
            return result;
        }
        else
        {
            if (stones == 0)
            {
                result = CalculateStones(1, steps - 1);

                Cache[(stones, steps)] = result;

                return result;
            }
            else
            {
                var length = 1 + (long)Math.Floor(Math.Log10(stones));

                if (length % 2 == 0)
                {

                    var left = stones / (long)Math.Pow(10, length / 2);
                    var right = stones % (long)Math.Pow(10, length / 2);

                    result = CalculateStones(left, steps - 1) + CalculateStones(right, steps - 1);
                }
                else
                {

                    result = CalculateStones(stones * 2024, steps - 1);
                }
            }

        }
        Cache[(stones, steps)] = result;

        return result;
    }
}

public readonly ref struct Input(string input)
{
    public readonly long[] Map = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
    public long Get(int loc)
    {
        return Map[loc];
    }
}
