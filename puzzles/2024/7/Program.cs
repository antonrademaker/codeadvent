

string[] inputFiles = ["input/example.txt", "input/input.txt"];


foreach (string file in inputFiles)
{
    Console.WriteLine($"Reading: {file}");

    var input = ParseFile(file);

    var answer1 = CalculateAnswer1(input);
    Console.WriteLine($"{file}: Answer 1: {answer1}");

    var answer2 = CalculateAnswer2(input);
    Console.WriteLine($"{file}: Answer 2: {answer2}");
}

static int CalculateAnswer1(string[] input)
{
    var currentSum = 0;

    return currentSum;
}

static int CalculateAnswer2(string[] input)
{
    var currentSum = 0;

    return currentSum;
}

static string[] ParseFile(string file)
{
    return File.ReadAllLines(file);
}

partial class Program
{
    static HashSet<string> pages = [];
}