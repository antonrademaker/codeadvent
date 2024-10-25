using AoC.Utilities;

string[] inputFiles = ["input/example.txt", "input/example2.txt", "input/input.txt"];

static string GetNewLocation(string directions, Dictionary<string, (string left, string right)> nodes, string current, int steps)
{
    var instruction = directions[steps % directions.Length];

    var (left, right) = nodes[current];

    current = (instruction == 'L') ? left : right;
    return current;
}

static Dictionary<string, (string left, string right)> ParseNodes(string[] inputs)
{
    var nodes = new Dictionary<string, (string left, string right)>();

    foreach (var nodeDescription in inputs.Skip(2))
    {
        var node = nodeDescription[0..3];
        var leftNode = nodeDescription[7..10];
        var rightNode = nodeDescription[12..15];

        nodes.Add(node, (leftNode, rightNode));
    }

    return nodes;
}


foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var part1 = Calculate(inputs, "AAA", "ZZZ");

    Console.WriteLine($"Part 1: {part1[0]}");

    var part2 = NumberTheory<long>.LeastCommonMultiple(Calculate(inputs, "A", "Z"));

    Console.WriteLine($"Part 2: {part2}");
}

long[] Calculate(string[] inputs, string start, string target)
{
    var directions = inputs[0];
    Dictionary<string, (string left, string right)> nodes = ParseNodes(inputs);

    var positions = nodes.Keys.Where(t => t.EndsWith(start)).ToArray();

    var step = 0;

    var steps = new long[positions.Length];

    while (steps.Any(t => t == default))
    {
        for (var i = 0; i < positions.Length; i++)
        {
            if (positions[i].EndsWith(target) && steps[i] == default)
            {
                steps[i] = step;
            }

            positions[i] = GetNewLocation(directions, nodes, positions[i], step);
        }
        step++;
    }

    return steps;
}