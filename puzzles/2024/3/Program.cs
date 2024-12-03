using System.Text.RegularExpressions;

string[] inputFiles = ["input/example.txt", "input/example2.txt", "input/input.txt"];

foreach (string file in inputFiles)
{
    Console.WriteLine($"Reading: {file}");

    var input = ParseFile(file);

    var answer1 = CalculateAnswer1(input);
    Console.WriteLine($"{file}: Answer 1: {answer1}");

    var answer2 = CalculateAnswer2(input);
    Console.WriteLine($"{file}: Answer 2: {answer2}");
}

static int CalculateAnswer1(string input)
{
    var parserPart1 = Part1Parser();
    var instructions = parserPart1.Matches(input);

    return instructions.Sum(CalculateMulInstruction);
}

static int CalculateAnswer2(string input)
{
    var parserPart2 = Part2Parser();
    var instructions = parserPart2.Matches(input);

    var isMulEnabled = true;

    return instructions.Sum(instruction =>
    {
        if (instruction.Groups["do"].Success)
        {
            isMulEnabled = true;
            return 0;
        }
        if (instruction.Groups["dont"].Success)
        {
            isMulEnabled = false;
            return 0;
        }

        return isMulEnabled ? CalculateMulInstruction(instruction) : 0;
    });
}


static int CalculateMulInstruction(Match instruction)
{
    var left = int.Parse(instruction.Groups["left"].Value);
    var right = int.Parse(instruction.Groups["right"].Value);
    return left * right;
}

static string ParseFile(string file)
{
    return File.ReadAllText(file);
}

partial class Program
{
    [GeneratedRegex(@"mul\((?<left>\d+),(?<right>\d+)\)", RegexOptions.Compiled)]
    private static partial Regex Part1Parser();

    [GeneratedRegex(@"(mul\((?<left>\d+),(?<right>\d+)\)|(?<dont>don't\(\))|(?<do>do\(\)))", RegexOptions.Compiled)]
    private static partial Regex Part2Parser();
}
