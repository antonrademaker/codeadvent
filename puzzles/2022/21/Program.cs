using System.Diagnostics;
using System.Text.RegularExpressions;

internal sealed class Program
{

    private static readonly Regex regex = new Regex(@"^(?<code>\w{4}):\s((?<number>\d+)|((?<left>\w{4})\s(?<operator>[\*\+\-\/])\s(?<right>\w{4})))$");

    private static void Main(string[] args)
    {
        var runDebug = true;

        var files = Directory.GetFiles("input", "*.txt");

        foreach (var file in files.OrderBy(t => t))
        {
            Console.WriteLine($"{file}");

            var lines = File.ReadAllLines(file);
            // long part1 = CalculatePart(lines, 1, 1);
            //Console.WriteLine($"Part 1: {part1}");

            //  long part2 = CalculatePart(lines, 811589153, 10);
            //Console.WriteLine($"Part 2: {part2}");

            var nodes = new Dictionary<string, Node>();


            var calculationQueue = new Queue<Node>();

            foreach (var line in lines)
            {
                var match = regex.Match(line);
                if (!match.Success)
                {
                    throw new Exception(line);
                }


                ParseNode(match, nodes, calculationQueue);
            }

            Console.WriteLine($"nodes: {nodes.Count}, queue: {calculationQueue.Count}");
            var root = CalculateRoot(calculationQueue);

            Console.WriteLine($"Part1 {root.Value}");

            Debug.Assert(root.Left != null);
            Debug.Assert(root.Right != null);



            var searchValue = root.Left.IsHumanDependent ? root.Right.Value!.Value : root.Left.Value!.Value;


            var searchTree = root.Left.IsHumanDependent ? root.Left : root.Right;

            Debug.Assert(root.Left.IsHumanDependent != root.Right.IsHumanDependent);

            while (searchTree.Code != "humn")
            {
                var isLeftKnown = searchTree.Right!.IsHumanDependent;

                var currentOperator = searchTree.Operator;

                var knownValue = (isLeftKnown ? searchTree.Left! : searchTree.Right!).Value!.Value;

                searchTree = isLeftKnown ? searchTree.Right : searchTree.Left;

                searchValue = currentOperator
                     switch
                {
                    '+' => searchValue - knownValue,
                    '-' when isLeftKnown => knownValue - searchValue,
                    '-' when !isLeftKnown => searchValue + knownValue,
                    '*' => searchValue / knownValue,
                    '/' when isLeftKnown => knownValue / searchValue,
                    '/' when !isLeftKnown => knownValue * searchValue,
                    _ => throw new Exception("Ünknown operator")
                };

                Debug.Assert(searchTree != null);
            }

            Console.WriteLine($"Human found: searchValue: {searchValue}");
        }
    }

    private static Node CalculateRoot(Queue<Node> calculationQueue)
    {
        while (calculationQueue.TryDequeue(out var node))
        {
            if (node.Code == "root" && node.Value.HasValue)
            {
                return node;
            }

            if (!node.Value.HasValue)
            {
                Debug.Assert(node.Left != null);
                Debug.Assert(node.Right != null);
                if (node.Left.Value.HasValue && node.Right.Value.HasValue)
                {

                    var leftValue = node.Left.Value.Value;
                    var rightValue = node.Right.Value.Value;

                    node.Value = node.Operator switch
                    {
                        '+' => leftValue + rightValue,
                        '*' => leftValue * rightValue,
                        '/' => leftValue / rightValue,
                        '-' => leftValue - rightValue,
                        _ => throw new Exception("Ünknown operator")
                    };

                    node.IsHumanDependent = node.Left.IsHumanDependent || node.Right.IsHumanDependent;
                }
                else
                {
                    // can't calculate this now
                    calculationQueue.Enqueue(node);
                    continue;
                }
            }

            foreach (var dependency in node.Dependencies)
            {
                calculationQueue.Enqueue(dependency);
            }
        }
        throw new Exception("Root not found");
    }

    private static void ParseNode(Match match, Dictionary<string, Node> currentNodes, Queue<Node> calculationQueue)
    {
        var code = match.Groups["code"].Value;

        if (!currentNodes.TryGetValue(code, out var node))
        {
            node = new Node { Code = code };
            currentNodes[code] = node;
        }

        if (code == "humn")
        {
            node.IsHumanDependent = true;
        }

        if (long.TryParse(match.Groups["number"].Value, out var value))
        {
            node.Value = value;
            calculationQueue.Enqueue(node);
            return;
        }

        var left = match.Groups["left"].Value;
        var right = match.Groups["right"].Value;

        if (!currentNodes.TryGetValue(left, out var leftNode))
        {
            leftNode = new Node { Code = left };
            currentNodes[left] = leftNode;
        }

        leftNode.Dependencies.Add(node);

        if (!currentNodes.TryGetValue(right, out var rightNode))
        {
            rightNode = new Node { Code = right };
            currentNodes[right] = rightNode;
        }
        rightNode.Dependencies.Add(node);

        node.Left = leftNode;
        node.Right = rightNode;
        node.Operator = match.Groups["operator"].Value[0];

    }
}


public class Node
{
    public required string Code { get; set; }
    public long? Value { get; set; }
    public bool HasValue => Value.HasValue;

    public HashSet<Node> Dependencies { get; set; } = new HashSet<Node>();

    public Node? Left { get; set; }
    public Node? Right { get; set; }

    public char? Operator { get; set; }
    public bool IsHumanDependent { get; set; }
}