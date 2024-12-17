using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Solution;

public partial class Program
{
    private static readonly List<string> inputFiles = [/*"input/example1.txt",*/ "input/input.txt"];

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

        while (state.InstructionPointer < operators.Length)
        {
            var opcode = operators[state.InstructionPointer];
            var operand = operators[state.InstructionPointer + 1];

            state.InstructionPointer += 2;

            switch (opcode)
            {
                case 0:
                    state.Adv(operand);
                    break;

                case 1:
                    state.Bxl(operand);
                    break;

                case 2:
                    state.Bst(operand);
                    break;

                case 3:
                    state.Jnz(operand);
                    break;

                case 4:
                    state.Bxc(operand);
                    break;

                case 5:
                    state.Out(operand);
                    break;

                case 6:
                    state.Bdv(operand);
                    break;

                case 7:
                    state.Cdv(operand);
                    break;

                default:
                    throw new NotImplementedException($"Opcode unknown: {opcode}");
            };
        }

        return state;
    }

    public static string CalculateAnswer2(Input input)
    {
        return "";
    }

    [GeneratedRegex(@"^Register A:\s(?<RegisterA>\d+)\s+Register B:\s(?<RegisterB>\d+)\s+Register C:\s(?<RegisterC>\d+)\s+Program:\s*(?<Program>(\d,?)*)$")]
    public static partial Regex Parser();
}

public class State
{
    public void Cdv(int operand)
    {
        RegisterC = RegisterA / (int)Math.Pow(2.0, GetCombo(operand));
    }

    public void Bdv(int operand)
    {
        RegisterB = RegisterA / (int)Math.Pow(2.0, GetCombo(operand));
    }

    public void Out(int operand)
    {
        Output.Add((int)(GetCombo(operand) % 8));
    }

    public void Bxc(int operand)
    {
        RegisterB ^= RegisterC;
    }

    public void Jnz(int operand)
    {
        if (RegisterA != 0)
        {
            InstructionPointer = operand;
        }
    }

    public void Bst(int operand)
    {
        RegisterB = GetCombo(operand) % 8;
    }

    public void Bxl(int operand)
    {
        RegisterB ^= operand;
    }

    public void Adv(int operand)
    {
        RegisterA /= (int)Math.Pow(2.0, GetCombo(operand));
    }

    public int RegisterA { get; set; }
    public int RegisterB { get; set; }
    public int RegisterC { get; set; }

    public int GetCombo(int operand)
    {
        return operand switch
        {
            0 or 1 or 2 or 3 => operand,
            4 => RegisterA,
            5 => RegisterB,
            6 => RegisterC,

            _ => throw new NotImplementedException($"Operand {operand} not implemented")
        };
    }

    public List<int> Output { get; } = [];
    public string Program { get; init; }

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
}

public readonly struct Input
{
    public State InitialState { get; }

    public Input(string input)
    {
        var parsed = Program.Parser().Match(input);

        InitialState = new State()
        {
            RegisterA = int.Parse(parsed.Groups["RegisterA"].Value),
            RegisterB = int.Parse(parsed.Groups["RegisterB"].Value),
            RegisterC = int.Parse(parsed.Groups["RegisterC"].Value),

            Program = parsed.Groups["Program"].Value
        };
    }
}