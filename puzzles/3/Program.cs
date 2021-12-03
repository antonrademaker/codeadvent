using System.Collections;
using System.Linq;
using System.Text;

var lines = System.IO.File.ReadAllLines("input/input.txt");

var counts = ExtractCounts(lines);

var half = lines.Length / 2m;

Console.WriteLine($"# lines: {lines.Length}, Gamma counts: {string.Join(',', counts)} half: {half}");

var bits = lines[0].Length;

var gamma = CalculatePart1(value => value < half, counts);
var epsilon = CalculatePart1(value => value > half, counts);

Console.WriteLine($"Part 1: Power consumption: {gamma*epsilon}");

// Part 2
var oxygen = CalculatePart2((value, half) => value >= half ? '1' : '0', counts, lines.ToList());
Console.WriteLine($"oxygen : {oxygen}");
var co2 = CalculatePart2((value, half) => value < half ? '1' : '0', counts, lines.ToList());
Console.WriteLine($"co2 : {co2}");

Console.WriteLine($"Part 2: life support rating : {oxygen * co2}");


int CalculatePart1(Func<int, bool> predicate, int[] counts)
{
    var maskSb = new StringBuilder();

    for (int i = 0; i < bits; i++)
    {
        maskSb.Append(predicate(counts[i]) ? '0' : '1');
    }
    return Convert.ToInt32(maskSb.ToString(), 2);
}


int CalculatePart2(Func<int,decimal, char> predicate, int[] counts, IList<string> lines, int bitPosition = 0)
{
    var countsForSet = ExtractCounts(lines);
    var half = lines.Count / 2m;

    var mask = predicate(countsForSet[bitPosition], half);
    var candidates = lines.Where(t => t[bitPosition] == mask).ToList();

    if (candidates.Count == 1)
    {
        return Convert.ToInt32(candidates[0], 2);
    }

    return CalculatePart2(predicate, counts, candidates, bitPosition + 1);
}

static int[] ExtractCounts(IList<string> lines)
{
    var bits = lines[0].Length;
    var counts = new int[bits];
    foreach (var line in lines)
    {        
        for (int i = 0; i < bits; i++)
        {
            counts[i] += line[i] == '1' ? 1 : 0;
        }
    }

    return counts;
}