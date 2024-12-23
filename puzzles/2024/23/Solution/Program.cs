using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using AoC.Utilities;

namespace Solution;

public partial class Program
{
    private static readonly List<string> inputFiles = ["input/example1.txt", "input/input.txt", "input/aoc-2024-day-23-challenge-1.txt", "input/aoc-2024-day-23-challenge-2.txt", "input/aoc-2024-day-23-challenge-3.txt"];

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
        HashSet<string> currentSets3 = [];

        foreach (var computerWithT in input.ComputersWithT)
        {
            List<(string, string)> currentSets2 = [];

            foreach (var connection in input.Computers[computerWithT].Connections)
            {
                currentSets2.Add((computerWithT, connection));
            }

            foreach (var (a, b) in currentSets2)
            {
                foreach (var connection in input.Computers[b].Connections.Where(c => c != a))
                {
                    if (input.Computers[connection].Connections.Contains(a))
                    {
                        currentSets3.Add(ToKey(a, b, connection));
                    }
                }
            }
        }

        return currentSets3.Count;
    }

    public static string CalculateAnswer2(Input input)
    {
        HashSet<string> biggestSet = [];

        var byConnections = input.Computers.Values.ToArray();

        for (var current = 0; current < byConnections.Length; current++)
        {
            var computer = byConnections[current];

            var candidateSet = new HashSet<string>(computer.Connections)
            {
                computer.Name
            };

            foreach (var connection in computer.Connections)
            {
                if (candidateSet.Contains(connection))
                {
                    candidateSet.IntersectWith(input.Computers[connection].Connections);
                    candidateSet.Add(connection);
                }
            }
            if (candidateSet.Count > biggestSet.Count)
            {
                biggestSet = candidateSet;
            }
        }

        var ans = string.Join(',', biggestSet.ToArray().OrderBy(x => x));
                
        Console.WriteLine(Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(ans))).ToLowerInvariant());

        return ans;
    }

    public static string ToKey(string a, string b, string c)
    {
        string[] l = [a, b, c];
        var ordered = l.OrderBy(x => x).ToArray();

        return string.Join(',', ordered);
    }
}

public class Computer(string name)
{
    public string Name { get; } = name;

    public HashSet<string> Connections { get; } = [];
}

public readonly struct Input
{
    public readonly string[] Lines;

    public readonly DefaultFactoryDictionary<string, Computer> Computers = new DefaultFactoryDictionary<string, Computer>(name => new Computer(name));

    public readonly HashSet<string> ComputersWithT = [];

    public Input(string input)
    {
        Lines = [.. input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)];

        foreach (var line in Lines)
        {
            var parts = line.Split("-");
            var a = parts[0];
            var b = parts[1];
            Computers[a].Connections.Add(b);
            Computers[b].Connections.Add(a);

            if (parts[0][0] == 't')
            {
                ComputersWithT.Add(parts[0]);
            }
            if (parts[1][0] == 't')
            {
                ComputersWithT.Add(parts[1]);
            }
        }
    }
}