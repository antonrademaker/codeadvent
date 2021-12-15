using System.Diagnostics;
using System.Numerics;
using System.Text;

var inputFiles = new string[] { "input/example.txt", "input/input.txt" };
var sw = new Stopwatch();

foreach (var exampleFile in inputFiles)
{
    Console.WriteLine(exampleFile);
    var file = File.ReadAllLines(exampleFile);
    sw.Start();
    CalculatePart1(file);
    sw.Stop();
    Console.WriteLine($"Part 1 in {sw.ElapsedMilliseconds}ms");
    sw.Reset();
    sw.Start();
    CalculatePart2(file);
    sw.Stop();
    Console.WriteLine($"Part 2 in {sw.ElapsedMilliseconds}ms");
    sw.Reset();
    sw.Start();
    CalculatePart3(file);
    sw.Stop();
    Console.WriteLine($"Part 3 in {sw.ElapsedMilliseconds}ms");
    Console.WriteLine("-- End of file--");
}

void CalculatePart1(string[] lines)
{
    var steps = 10;
    Calculate(lines, steps);
}

void CalculatePart2(string[] lines)
{
    var steps = 40;
    Calculate(lines, steps);
}


void CalculatePart3(string[] lines)
{
    var steps = 50000;
    Calculate(lines, steps);
}

void Calculate(string[] lines, int steps)
{
    var processor = new Processor();
    processor.Read(lines);

    for (var step = 1; step < steps + 1; step++)
    {
        processor.CalculateNextStep();
    }

    Console.WriteLine($"After step: {steps}");
    processor.Print();
}

public class Processor
{
    public string polymer = string.Empty;

    public Dictionary<Pair, BigInteger> pairs = new();

    public Dictionary<Pair, char> Rules = new();

    public void Read(string[] lines)
    {
        var readingTemplate = true;

        foreach (var line in lines.Where(t => !string.IsNullOrWhiteSpace(t)))
        {
            if (readingTemplate)
            {
                polymer = line;

                for (var i = 0; i < polymer.Length - 1; i++)
                {
                    pairs.Upsert(new Pair(polymer[i], polymer[i + 1]), new BigInteger(1));
                }

                readingTemplate = false;
            }
            else
            {
                var splitted = line.Split("->", StringSplitOptions.TrimEntries);

                Rules.Add(new Pair(splitted[0][0], splitted[0][1]), splitted[1][0]);
            }
        }
    }

    public void CalculateNextStep()
    {
        pairs = pairs.Aggregate(new Dictionary<Pair, BigInteger>(), (acc, pair) => CalculatePairs(acc, pair));
    }

    private Dictionary<Pair, BigInteger> CalculatePairs(Dictionary<Pair, BigInteger> next, KeyValuePair<Pair, BigInteger> pair)
    {
        var insert = Rules[pair.Key];

        return next.Upsert(new Pair(pair.Key.A, insert), pair.Value).Upsert(new Pair(insert, pair.Key.B), pair.Value);
    }

    public void Print()
    {
        var sb = new StringBuilder();

        var pairsToCheck = pairs.ToDictionary(entry => entry.Key, entry => entry.Value);
        // Insert the last polymer (not counted otherwise)
        pairsToCheck = pairsToCheck.Upsert(new Pair(polymer[^1], '0'), new BigInteger(1));

        var histogram = pairsToCheck.Select(pair => new { ch = pair.Key.A, count = pair.Value }).GroupBy(t => t.ch).Select(t => new { ch = t.Key, count = t.Sum(x => x.count) }).ToList();

        var min = histogram.Min(t => t.count);
        var max = histogram.Max(t => t.count);

        sb.AppendLine($"min: {min}, max: {max}: {max - min}");

        sb.AppendLine($"Total: {histogram.Sum(t => t.count)}");

        Console.WriteLine(sb.ToString());
    }

}

public static class Helpers
{

    public static Dictionary<T, BigInteger> Upsert<T>(this Dictionary<T, BigInteger> pairs, T pair, BigInteger currentValue) where T : class
    {
        if (pairs.ContainsKey(pair))
        {
            pairs[pair] += currentValue;
        }
        else
        {
            pairs[pair] = currentValue;
        }

        return pairs;
    }

    public static BigInteger Sum<T>(this IEnumerable<T> ts, Func<T, BigInteger> selector)
    {
        return ts.Aggregate(new BigInteger(0), (acc, b) => acc + selector(b));
    }
}

public record Pair(char A, char B);
