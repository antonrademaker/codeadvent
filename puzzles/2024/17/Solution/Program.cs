using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Solution;

public partial class Program
{
    private static readonly List<string> inputFiles = [/*"input/example1.txt","input/example2.txt",*/ "input/input.txt"];

    public static void Main(string[] _)
    {
        foreach (var file in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input = ParseFile(file);

            var startTime = Stopwatch.GetTimestamp();

            var answer1 = CalculateAnswer1(input);
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            Console.WriteLine($"{file}: Answer 1: {answer1.GetOutput()} ({elapsedTime.TotalMilliseconds}ms)");
            Console.WriteLine();

            startTime = Stopwatch.GetTimestamp();
            var answer2 = CalculateAnswer2(input);
            elapsedTime = Stopwatch.GetElapsedTime(startTime);

            Console.WriteLine($"{file}: Answer 2: {answer2} ({elapsedTime.TotalMilliseconds}ms)");
        }
    }

    private static Input ParseFile(string file)
    {
        return new Input(File.ReadAllText(file));
    }

    public static State CalculateAnswer1(Input input)
    {
        var state = input.InitialState;

        var operators = state.Program.Replace(",", string.Empty).ToCharArray().Select(t => t - '0').ToArray().AsSpan();

        state.CalculateNext();
        return state;
    }

    public static string CalculateAnswer2(Input input)
    {
        var state = input.InitialState;

        return TrySolve(state).ToString();
    }

    public static long TrySolve(State state)
    {
        var expectedOutput = state.Program.Split(',').Select(int.Parse).ToArray().AsSpan();

        var operators = state.Operators;
        var queue = new Queue<(long registerA, int digit)>();

        queue.Enqueue((0, operators.Length - 1));

        while (queue.TryDequeue(out var q))
        {
            var (registerA, digit) = q;
            for (var i = 0; i < 8; i++)
            {
                var a = (registerA << 3) + i;
                state.Reset();
                state.RegisterA = a;
                state.CalculateNext();

                if (state.Output.SequenceEqual(operators[digit..]))
                {
                    if (digit == 0)
                    {
                        return a;
                    }
                    queue.Enqueue((a, digit - 1));
                }
            }
        }
        return 0;
    }

    [GeneratedRegex(@"^Register A:\s(?<RegisterA>\d+)\s+Register B:\s(?<RegisterB>\d+)\s+Register C:\s(?<RegisterC>\d+)\s+Program:\s*(?<Program>(\d,?)*)$")]
    public static partial Regex Parser();
}

public class State
{
    public void CalculateNext()
    {
        while (InstructionPointer < Operators.Length)
        {
            var opcode = Operators[InstructionPointer];
            var operand = Operators[InstructionPointer + 1];

            InstructionPointer += 2;

            switch (opcode)
            {
                case 0:
                    RegisterA >>= (int)GetCombo(operand);
                    break;

                case 1:
                    RegisterB ^= operand;
                    break;

                case 2:
                    RegisterB = GetCombo(operand) & 7;
                    break;

                case 3:
                    if (RegisterA != 0)
                    {
                        InstructionPointer = operand;
                    }
                    break;

                case 4:
                    RegisterB ^= RegisterC;
                    break;

                case 5:
                    Output.Add((int)(GetCombo(operand) & 7));
                    break;

                case 6:
                    RegisterB = RegisterA >> (int)GetCombo(operand);
                    break;

                case 7:
                    RegisterC = RegisterA >> (int)GetCombo(operand);
                    break;

                default:
                    throw new NotImplementedException($"Opcode unknown: {opcode}");
            };
        }
    }

    public long RegisterA { get; set; }
    public long RegisterB { get; set; }
    public long RegisterC { get; set; }

    public long RegisterAOriginal { get; init; }
    public long RegisterBOriginal { get; init; }
    public long RegisterCOriginal { get; init; }

    public int GetCombo(int operand)
    {
        return operand switch
        {
            <= 3 => operand,
            4 => (int)RegisterA,
            5 => (int)RegisterB,
            6 => (int)RegisterC,
            _ => throw new NotImplementedException($"Operand {operand} not implemented")
        };
    }

    public List<int> Output { get; } = [];
    public string Program { get; init; }

    public int[] Operators { get; init; }

    public int InstructionPointer { get; set; }

    public override string ToString()
    {
        return
        $"""
Register A: {RegisterA}
Register B: {RegisterB}
Register C: {RegisterC}
Output: {GetOutput()}
"""
        ;
    }

    public string GetOutput()
    {
        return string.Join(',', Output);
    }

    public void Reset()
    {
        RegisterA = RegisterAOriginal;
        RegisterB = RegisterBOriginal;
        RegisterC = RegisterCOriginal;
        InstructionPointer = 0;
        Output.Clear();
    }
}

public readonly struct Input
{
    public State InitialState { get; }

    public Input(string input)
    {
        var parsed = Program.Parser().Match(input);

        InitialState = new State()
        {
            RegisterA = long.Parse(parsed.Groups["RegisterA"].Value),
            RegisterB = long.Parse(parsed.Groups["RegisterB"].Value),
            RegisterC = long.Parse(parsed.Groups["RegisterC"].Value),
            RegisterAOriginal = long.Parse(parsed.Groups["RegisterA"].Value),
            RegisterBOriginal = long.Parse(parsed.Groups["RegisterB"].Value),
            RegisterCOriginal = long.Parse(parsed.Groups["RegisterC"].Value),
            Program = parsed.Groups["Program"].Value,
            Operators = parsed.Groups["Program"].Value.Replace(",", string.Empty).ToCharArray().Select(t => t - '0').ToArray()
        };
    }
}