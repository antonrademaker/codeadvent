using System.Collections;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using AoC.Utilities;

namespace Solution;

public partial class Program
{
    private static readonly List<string> inputFiles = [
        "input/example1.txt",
        "input/input.txt",

        ];

    public static void Main(string[] _)
    {
        foreach (var file in inputFiles)
        {
            // Console.WriteLine($"Reading: {file}");

            var input1 = ParseFile(file);

            var startTime = Stopwatch.GetTimestamp();

            var answer1 = CalculateAnswer1(input1);
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"{file}: Answer 1: {answer1} ({elapsedTime.TotalMilliseconds}ms)");

            var input2 = ParseFile(file);

            startTime = Stopwatch.GetTimestamp();
            var answer2 = CalculateAnswer2(input2);
            elapsedTime = Stopwatch.GetElapsedTime(startTime);

            Console.WriteLine($"{file}: Answer 2: {answer2} ({elapsedTime.TotalMilliseconds}ms)");
            Console.WriteLine();
        }
    }

    private static Input ParseFile(string file)
    {
        return new Input(File.ReadAllText(file));
    }

    public static long CalculateAnswer1(Input input)
    {
        var matching = 0;

        foreach (var lockObj in input.Locks)
        {
            foreach (var keyObj in input.Keys)
            {
                Console.WriteLine($"Lock: {string.Join(",", lockObj)} with key {string.Join(',', keyObj)}");
                var pos = 0;
                while (pos < lockObj.Length && lockObj[pos] + keyObj[pos] < 6)
                {
                    pos++;
                }
                Console.WriteLine($"Pos: {pos}");
                if (pos == lockObj.Length)
                {
                    matching++; ;
                }
                else
                {
                }
            }
        }

        return matching;
    }

    public static int CalculateAnswer2(Input input)
    {
        return 0;
    }
}

public readonly struct Input
{
    public readonly string[] Blocks;

    public readonly List<int[]> Keys = [];
    public readonly List<int[]> Locks = [];

    public Input(string input)
    {
        Blocks = [.. input.Split(input.Contains("\r\n") ? "\r\n\r\n" : "\n\n")];

        foreach (var block in Blocks)
        {
            var lines = block.Split(input.Contains("\r\n") ? "\r\n" : "\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (lines[0][0] == '#')
            {
                // lock
                int[] result = GetCount(lines, '#');

                Locks.Add(result);
            }
            else
            {
                int[] result = GetCount(lines, '#');

                Keys.Add(result);
            }
        }
    }

    private static int[] GetCount(string[] lines, char toCount)
    {
        var result = new int[lines[0].Length];

        for (var i = 0; i < lines[0].Length; i++)
        {
            result[i] = -1; // skip the first or last line
            for (var j = 0; j < lines.Length; j++)
            {
                if (lines[j][i] == toCount)
                {
                    result[i]++;
                }
            }
        }

        return result;
    }
}