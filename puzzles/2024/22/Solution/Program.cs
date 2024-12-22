using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks.Sources;
using AoC.Utilities;
using Microsoft.Extensions.ObjectPool;
using Coordinate = AoC.Utilities.Coordinate<int>;

namespace Solution;

public partial class Program
{
    private static readonly List<string> inputFiles = ["input/example1.txt", "input/input.txt"];

    private const int MaxOptions = 19;
    private const int PowerMaxOptionsTo3 = MaxOptions * MaxOptions * MaxOptions;

    public static void Main(string[] _)
    {
        foreach (var file in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input1 = ParseFile(file);

            var startTime = Stopwatch.GetTimestamp();

            var answer1 = CalculateAnswer1(input1);
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"{file}: Answer 1: {answer1} ({elapsedTime.TotalMilliseconds}ms)");
            Console.WriteLine();

            var input2 = ParseFile(file);

            startTime = Stopwatch.GetTimestamp();
            var answer2 = CalculateAnswer2(input2);
            elapsedTime = Stopwatch.GetElapsedTime(startTime);

            Console.WriteLine($"{file}: Answer 2: {answer2} ({elapsedTime.TotalMilliseconds}ms)");
        }
    }

    private static Input ParseFile(string file)
    {
        return new Input(File.ReadAllText(file));
    }

    public static long CalculateAnswer1(Input input)
    {
        return Calculate(input, 2000).part1;
    }

    public static long CalculateAnswer2(Input input)
    {
        return Calculate(input, 2000).part2;
    }

    public static (long part1, long part2) Calculate(Input input, int steps)
    {
        ulong part1 = 0;

        DefaultDictionary<int, int> bananas = [];
        DefaultDictionary<int, int> occurred = [];

        for (var inputIndex = 0; inputIndex < input.Lines.Length; inputIndex++)
        {
            ulong secret = (ulong)input.Lines[inputIndex];
            int lastPrice = (int)(secret % 10);

            int delta;
            var sequenceIndex = 0;

            for (var iteration = 0; iteration < steps; iteration++)
            {
                secret = Prune(Mix(secret << 6, secret));
                secret = Prune(Mix(DivideBy32(secret), secret));
                secret = Prune(Mix(secret << 11, secret));

                var price = (int)(secret % 10);

                (delta, lastPrice) = (price - lastPrice, price);

                sequenceIndex = (sequenceIndex % PowerMaxOptionsTo3) * MaxOptions + (delta + 9);
                // skip first iterations and only register once per monkey
                if (iteration >= 3 && occurred[sequenceIndex] != inputIndex + 1)
                {
                    bananas[sequenceIndex] += price;
                    occurred[sequenceIndex] = inputIndex + 1;
                }
            }

            part1 += secret;
        }

        var part2 = bananas.Any() ? bananas.Values.Max() : 0;

        return ((long)part1, part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong DivideBy32(ulong value)
    {
        return value >> 5;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Mix(ulong newValue, ulong secretNumber)
    {
        return newValue ^ secretNumber;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong Prune(ulong value)
    {
        return value & 16777215;
    }
}

public readonly struct Input(string input)
{
    public readonly int[] Lines = [.. input.Split(Environment.NewLine).Select(int.Parse)];
}