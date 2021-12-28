using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Diagnostics;

var inputFiles = new string[] {
    "input/input.txt"
};
var sw = new Stopwatch();

foreach (var exampleFile in inputFiles)
{
    Console.WriteLine(exampleFile);
    var file = File.ReadAllLines(exampleFile);

    var blocks = ParseBlocks(file).ToList();
    var calc = new Calculator();

    sw.Start();

    CalculatePart1(calc, blocks);

    sw.Stop();
    Console.WriteLine($"Part 1 in {sw.ElapsedMilliseconds}ms");
    sw.Reset();
    sw.Start();

    CalculatePart2(calc, blocks);

    sw.Stop();
    Console.WriteLine($"Part 2 in {sw.ElapsedMilliseconds}ms");

    Console.WriteLine("-- End of file--");
}



void CalculatePart1(Calculator calc, List<Func<State,State>> blocks)
{
    var result = calc.CalculateFirst(0, blocks, 0, true);

    var max = result!.Aggregate(0L, (acc, t) => acc * 10 + t);
    Console.WriteLine($"Max: {max}");
}

void CalculatePart2(Calculator calc, List<Func<State, State>> blocks)
{
    var result = calc.CalculateFirst(0, blocks, 0, false);
    var min = result!.Aggregate(0L, (acc, t) => acc * 10 + t);

    Console.WriteLine($"Min: {min}");
}

IEnumerable<Func<State, State>> ParseBlocks(string[] lines)
{
    var options = ScriptOptions.Default.AddReferences(typeof(State).Assembly).WithOptimizationLevel(Microsoft.CodeAnalysis.OptimizationLevel.Release);

    var blocks = new List<Func<State, State>>();

    var csharpLines = new List<string>
    {
        "state => {"
    };

    var block = 0;

    foreach (var line in lines)
    {
        if (line.StartsWith("inp") && block++ > 0)
        {
            csharpLines.Add("return state;");
            csharpLines.Add("}");

            yield return CSharpScript.EvaluateAsync<Func<State, State>>(string.Join(string.Empty, csharpLines), options).Result;

            csharpLines.Clear();
            csharpLines.Add("state => {");
        }

        if (!line.StartsWith("inp"))
        {
            csharpLines.Add(RewriteLine(line));
        }
    }

    csharpLines.Add("return state;");
    csharpLines.Add("}");

    yield return CSharpScript.EvaluateAsync<Func<State, State>>(string.Join(string.Empty, csharpLines), options).Result;
}

string RewriteLine(string line)
{
    if (line.StartsWith("inp"))
    {
        var args = line.Split(' ');

        return $"{ParseArgument(args[1])} = state.ReadFromMomad();";
    }
    else if (line.StartsWith("add"))
    {
        var args = line.Split(' ');

        return $"{ParseArgument(args[1])} = {ParseArgument(args[1])} + {ParseArgument(args[2])};";

    }
    else if (line.StartsWith("mul"))
    {
        var args = line.Split(' ');

        return $"{ParseArgument(args[1])} = {ParseArgument(args[1])} * {ParseArgument(args[2])};";

    }
    else if (line.StartsWith("div"))
    {
        var args = line.Split(' ');

        var right = args[2];

        return $"{ParseArgument(args[1])} = (int) System.Math.Round(1m * {ParseArgument(args[1])} / {ParseArgument(args[2])},System.MidpointRounding.ToZero);";
    }
    else if (line.StartsWith("mod"))
    {
        var args = line.Split(' ');

        return $"{ParseArgument(args[1])} = {ParseArgument(args[1])} % {ParseArgument(args[2])};";

    }
    else if (line.StartsWith("eql"))
    {
        var args = line.Split(' ');

        return $"{ParseArgument(args[1])} = {ParseArgument(args[1])} == {ParseArgument(args[2])} ? 1 : 0;";
    }
    else
    {
        return $"// could not parse: {line}";
    }
}

string ParseArgument(string arg)
{
    if (char.IsNumber(arg[0]) || arg[0] == '-')
    {
        return arg;
    }
    else
    {
        return $"state.{arg.ToUpperInvariant()}";
    }
}

public class State
{
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int Z { get; set; }
}


public class Calculator
{
    HashSet<(int depth, int w, int z)> cache = new();

    public Stack<int>? CalculateFirst(int depth, List<Func<State, State>> funcs, int z, bool searchHighest)
    {
        if (depth > 13)
        {
            return null;
        }

        for (int t = 9; t > 0; t--)
        {
            var w = searchHighest ? t : 10 - t;

            var state = new State { W = w, Z = z };
            if (cache.Add((depth, w, z)))
            {

                var nextState = funcs[depth](state);

                if (nextState.Z > 26 * 26 * 26 * 26)
                {
                    return null;
                }

                if (depth == 13 && nextState.Z == 0)
                {
                    var stack = new Stack<int>(14);
                    stack.Push(w);
                    return stack;
                }
                var search = CalculateFirst(depth + 1, funcs, nextState.Z, searchHighest);

                if (search != null)
                {
                    search.Push(w);
                    return search;
                }
            }

        }

        return null;
    }
}
