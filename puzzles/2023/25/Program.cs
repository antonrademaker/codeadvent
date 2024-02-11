using System.Numerics;
using System.Text;

using Coordinate3D = AoC.Utilities.Coordinate3D<long>;
using Coordinate2D = AoC.Utilities.Coordinate<long>;
using Microsoft.Z3;
using System.Security.Cryptography.X509Certificates;

namespace AoC.Puzzles.Y2023.D23;

public static partial class Program
{

    private static void Main(string[] args)
    {
        List<string> inputFiles = [
            "input/example.txt",
            "input/input.txt"
           ];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var part1 = CalculatePart1(inputs);

            Console.WriteLine($"Part 1: {part1}");

            var part2 = CalculatePart2(inputs);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static long CalculatePart1(string[] inputs)
    {
        var (nodes, edges) = ParseGraph(inputs);

        // https://en.wikipedia.org/wiki/Karger%27s_algorithm

        return FindWithKrager(nodes, edges);
    }

    private static int FindWithKrager(HashSet<string> nodes, List<Edge> edges)
    {
        var cuts = 0;
        var sizeA = 0;
        var sizeB = 0;
        while (cuts != 3)
        {
            (cuts, sizeA, sizeB) = Contract(nodes, edges);
        }
        return sizeA * sizeB;
    }

    private static (int cuts, int minCutSize, int sizeSet2) Contract(HashSet<string> nodes, List<Edge> edges)
    {
        var nodesCopy = new HashSet<string>(nodes);
        var edgesCopy = new List<Edge>(edges);

        var contractedNodes = nodes.ToDictionary(node => node, node => new HashSet<string> { node });

        while (nodesCopy.Count > 2)
        {
            var edgeToCut = edgesCopy[Random.Shared.Next(edgesCopy.Count)];
            var nodeToContract = edgeToCut.From;
            nodesCopy.Remove(nodeToContract);
            contractedNodes[edgeToCut.To].UnionWith(contractedNodes[nodeToContract]);
            contractedNodes.Remove(nodeToContract);
            for (int i = edgesCopy.Count - 1; i >= 0; i--)
            {
                if (edgesCopy[i].Connects(nodeToContract, out var newDirection))
                {
                    if (newDirection != edgeToCut.To)
                    {
                        edgesCopy.Add(new(edgeToCut.To, newDirection));
                    }
                    edgesCopy.RemoveAt(i);
                }
            }
        }

        var sets = contractedNodes.Values.ToArray();
        var cuts = 0;
        foreach (var edge in edgesCopy)
        {
            if ((sets[0].Contains(edge.From) && sets[1].Contains(edge.To)) || (sets[0].Contains(edge.To) && sets[1].Contains(edge.From)))
            {
                cuts++;
            }
        }

        return (cuts, sets[0].Count, sets[1].Count);
    }


    private static (HashSet<string> nodes, List<Edge> edges) ParseGraph(string[] inputs)
    {
        HashSet<string> nodes = [];
        HashSet<Edge> edges = [];
        foreach (var input in inputs)
        {
            var parts = input.Split(':', StringSplitOptions.TrimEntries);
            var key = parts[0];
            var connections = parts[1].Split(" ", StringSplitOptions.TrimEntries);

            nodes.Add(key);

            foreach (var connection in connections)
            {
                nodes.Add(connection);
                edges.Add(new(key, connection));
            }
        }

        return (nodes, edges.ToList());
    }

    private static long CalculatePart2(string[] inputs)
    {
        return 0;
    }
}

public record Edge(string From, string To)
{    
    public bool Connects(string edge, out string direction)
    {
        if (edge == From)
        {
            direction = To;
            return true;
        }
        else if (edge == To)
        {
            direction = From;
            return true;
        }
        else
        {
            direction = string.Empty;
            return false;
        }
    }
}