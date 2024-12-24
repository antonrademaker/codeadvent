using System.Collections;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using AoC.Utilities;

namespace Solution;

public partial class Program
{
    private static readonly List<string> inputFiles = [
        "input/example1.txt",
        "input/example2.txt",
        "input/input.txt"

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
        var queue = new Queue<string>();

        foreach (var wire in input.WireValues)
        {
            queue.Enqueue(wire.Key);
        }

        while (queue.TryDequeue(out var wire))
        {
            var wireValue = input.WireValues[wire];

            foreach (var operation in input.Operators[wire])
            {
                if (operation.Push(wireValue, out var output, out var outputWire))
                {
                    input.WireValues[outputWire] = output;
                    queue.Enqueue(outputWire);
                }
            }
        }

        return GetZWireValue(input);
    }

    private static long GetZWireValue(Input input)
    {
        return ToLong(input.WireValues.Where(t => t.Key.StartsWith('z')).OrderBy(x => x.Key).Select(t => t.Value).ToArray());
    }

    private static long GetXWireValue(Input input)
    {
        return ToLong(input.WireValues.Where(t => t.Key.StartsWith('x')).OrderBy(x => x.Key).Select(t => t.Value).ToArray());
    }

    private static long GetYWireValue(Input input)
    {
        return ToLong(input.WireValues.Where(t => t.Key.StartsWith('x')).OrderBy(x => x.Key).Select(t => t.Value).ToArray());
    }

    private static long ToLong(bool[] x)
    {
        var zWiresArray = new BitArray(x);

        var array = new byte[64];
        zWiresArray.CopyTo(array, 0);

        return BitConverter.ToInt64(array, 0);
    }

    public static int CalculateAnswer2(Input input)
    {
        return 0;
    }
}

public abstract class Operation(string outputWire)
{
    protected readonly string outputWire = outputWire;

    public abstract bool Push(bool input, out bool output, out string outputWire);
}

public class AndOperation(string output) : Operation(output)
{
    private readonly bool[] inputs = new bool[2];
    private int inputCount;

    public override bool Push(bool input, out bool output, out string outputWire)
    {
        inputs[inputCount++] = input;
        if (inputCount == 2)
        {
            output = inputs[0] && inputs[1];
            outputWire = this.outputWire;
            return true;
        }
        output = false;
        outputWire = string.Empty;
        return false;
    }
}

public class OrOperation(string output) : Operation(output)
{
    private readonly bool[] inputs = new bool[2];
    private int inputCount;

    public override bool Push(bool input, out bool output, out string outputWire)
    {
        inputs[inputCount++] = input;
        if (inputCount == 2)
        {
            output = inputs[0] || inputs[1]; // can optimize this
            outputWire = this.outputWire;
            return true;
        }
        output = false;
        outputWire = string.Empty;

        return false;
    }
}

public class XorOperation(string output) : Operation(output)
{
    private readonly bool[] inputs = new bool[2];
    private int inputCount;

    public override bool Push(bool input, out bool output, out string outputWire)
    {
        inputs[inputCount++] = input;
        if (inputCount == 2)
        {
            output = inputs[0] ^ inputs[1];
            outputWire = this.outputWire;
            return true;
        }
        output = false;
        outputWire = string.Empty;

        return false;
    }
}






public readonly struct Input
{
    public readonly string[] Lines;

    public readonly DefaultFactoryDictionary<string, List<Operation>> Operators = new(x => []);

    public readonly Dictionary<string, bool> WireValues = [];

    public readonly string LastZWire;
       

    public Input(string input)
    {
        Lines = [.. input.Split(input.Contains("\r\n") ? "\r\n" : "\n")];

        var parsingWires = true;

        LastZWire = string.Empty;

        foreach (var line in Lines)
        {
            if (line == string.Empty)
            {
                parsingWires = false;
                continue;
            }

            if (parsingWires)
            {
                var split = line.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                WireValues[split[0]] = split[1] == "1";

                LastZWire = split[0];
            }
            else
            {
                var split = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                Operation operation = split[1] switch
                {
                    "AND" => new AndOperation(split[4]),
                    "OR" => new OrOperation(split[4]),
                    "XOR" => new XorOperation(split[4]),
                    _ => throw new InvalidOperationException()
                };
                Operators[split[0]].Add(operation);

                Operators[split[2]].Add(operation);

            }
        }
    }
}