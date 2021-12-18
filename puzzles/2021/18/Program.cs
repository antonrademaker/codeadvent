using System.Diagnostics;
using System.Text;

var inputFiles = new string[] {
    "input/example.txt",
    "input/input.txt"
};
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

    Console.WriteLine("-- End of file--");
}

void CalculatePart1(string[] line)
{
    var p = Calculator.Parse(line[0]);
    for (var index = 1; index < line.Length; index++)
    {
        p = Calculator.Add(p, Calculator.Parse(line[index]));
    }
    Console.WriteLine(Calculator.Print(p));
    Console.WriteLine($"Magnitude: {Calculator.CalculateMagnitude(p)}");
}

void CalculatePart2(string[] line)
{
    var parsed = line.Select(Calculator.Parse).ToArray();

    var max = 0L;

    for (var i = 0; i < line.Length; i++)
    {
        for (var j = i + 1; j < line.Length; j++)
        {
            var mag = Calculator.CalculateMagnitude(Calculator.Add(parsed[i], parsed[j]));
            if (mag > max)
            {
                max = mag;
            }
        }
    }

    Console.WriteLine($"Max mag: {max}");
}

public sealed record Pair
{
    public Pair(int number, int depth)
    {
        Number = number;
        Depth = depth;
    }

    public Pair(Pair left, Pair right, int depth)
    {
        ValueLeft = left;
        ValueRight = right;
        Depth = depth;
    }

    public Pair? ValueLeft { get; init; }
    public Pair? ValueRight { get; init; }
    public int? Number { get; init; }

    public int Depth { get; init; }

    public bool Equals(Pair? other) => other != null &&
        ((ValueLeft == null && other.ValueLeft == null) || (ValueLeft != null && other.ValueLeft != null && ValueLeft.Equals(other.ValueLeft))) &&
        ((ValueRight == null && other.ValueRight == null) || (ValueRight != null && other.ValueRight != null && ValueRight.Equals(other.ValueRight))) &&
        Number == other.Number;

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public static class Calculator
{
    public static Pair Add(Pair pair1, Pair pair2)
    {
        return Reduce(new Pair(IncreaseDepth(pair1), IncreaseDepth(pair2), 0));
    }

    public static int CalculateMagnitude(Pair pair)
    {
        if (pair.Number.HasValue)
        {
            return pair.Number.Value;
        }

        return CalculateMagnitude(pair.ValueLeft!) * 3 + CalculateMagnitude(pair.ValueRight!) * 2;
    }

    public static Pair Reduce(Pair pair)
    {
        var hasChanges = true;

        while (hasChanges)
        {
            hasChanges = HasExploded(pair, out pair);
            if (!hasChanges)
            {
                hasChanges = HasSplitted(pair, out pair);
            }
        }
        return pair;
    }

    public static Pair IncreaseDepth(Pair pair)
    {
        if (pair.Number.HasValue)
        {
            return pair with
            {
                Depth = pair.Depth + 1
            };
        }
        else
        {
            return pair with
            {
                ValueLeft = IncreaseDepth(pair.ValueLeft!),
                ValueRight = IncreaseDepth(pair.ValueRight!),
                Depth = pair.Depth + 1
            };
        }
    }

    public static Pair Parse(string input)
    {
        var position = 0;
        return Parse(input.AsSpan(), ref position, 0);
    }

    private static Pair Parse(ReadOnlySpan<char> input, ref int currentPosition, int depth)
    {
        if (char.IsDigit(input[currentPosition]))
        {
            // parse number
            var start = currentPosition;

            while (currentPosition < input.Length && char.IsDigit(input[currentPosition]))
            {
                currentPosition++;
            }
            
            var numberValue = int.Parse(input[start..currentPosition]);

            return new Pair(numberValue, depth);
        }

        // consume [
        currentPosition++;

        var left = Parse(input, ref currentPosition, depth + 1);
        // consume ,
        currentPosition++;


        var right = Parse(input, ref currentPosition, depth + 1);
        // consume ]
        currentPosition++;

        return new Pair(left, right, depth);
    }

    public static string Print(Pair input)
    {
        var sb = new StringBuilder();
        Print(input, sb);
        return sb.ToString();
    }
    public static void Print(Pair input, StringBuilder sb)
    {
        if (input.Number.HasValue)
        {
            sb.Append(input.Number);
        }
        else
        {
            sb.Append('[');
            Print(input.ValueLeft!, sb);
            sb.Append(',');
            Print(input.ValueRight!, sb);
            sb.Append(']');
        }
    }

    public static bool HasExploded(Pair input, out Pair output)
    {
        var (updated, _, _, hasExploded) = Explode(input);
        output = updated;
        return hasExploded;
    }

    public static (Pair updated, int left, int right, bool hasChanged) Explode(Pair input)
    {
        if (input.Number.HasValue)
        {
            return (input, 0, 0, false);
        }
        else
        {
            if (input.Depth == 4)
            {
                // explode 
                return (new Pair(0, 4), input.ValueLeft!.Number!.Value, input.ValueRight!.Number!.Value, true);
            }
            else
            {
                var (u1, l1, r1, c1) = Explode(input.ValueLeft!);

                if (c1)
                {
                    if (r1 > 0)
                    {
                        var newRight = AddLeft(input.ValueRight!, r1);
                        return (input with
                        {
                            ValueLeft = u1,
                            ValueRight = newRight,
                        }, l1, 0, true);
                    }
                    return (input with
                    {
                        ValueLeft = u1,
                    }, l1, r1, true);

                }
                else
                {
                    var (u2, l2, r2, c2) = Explode(input.ValueRight!);

                    if (c2)
                    {
                        if (l2 > 0)
                        {
                            return (input with
                            {
                                ValueLeft = AddRight(input.ValueLeft!, l2),
                                ValueRight = u2,
                            }, 0, r2, true);
                        }
                        return (input with
                        {
                            ValueRight = u2,
                        }, l2, r2, true);
                    }
                    return (input, 0, 0, false);
                }
            }
        }
    }

    public static Pair AddLeft(Pair pair, int number)
    {
        if (pair.ValueLeft != null)
        {
            var newLeft = AddLeft(pair.ValueLeft, number);
            return pair with
            {
                ValueLeft = newLeft,
            };
        }
        else
        {
            return pair with { Number = pair.Number + number };
        }
    }

    public static Pair AddRight(Pair pair, int number)
    {
        if (pair.ValueRight != null)
        {
            var newRight = AddRight(pair.ValueRight, number);
            return pair with
            {
                ValueRight = newRight
            };
        }
        else
        {
            return pair with { Number = pair.Number + number };
        }
    }

    public static bool HasSplitted(Pair input, out Pair output)
    {
        var (updated, hasSplitted) = Split(input);
        output = updated;
        return hasSplitted;
    }

    public static (Pair updated, bool splitted) Split(Pair input)
    {
        if (input.Number.HasValue)
        {
            if (input.Number.Value >= 10)
            {
                var half = input.Number.Value / 2d;
                return (new Pair(
                    new Pair((int)Math.Floor(half), input.Depth + 1),
                    new Pair((int)Math.Ceiling(half), input.Depth + 1),
                    input.Depth
                    ), true);
            }
            return (input, false);
        }

        var (leftPair, leftUpdated) = Split(input.ValueLeft!);

        if (leftUpdated)
        {
            return (input with { ValueLeft = leftPair }, true);
        }

        var (rightPair, rightUpdated) = Split(input.ValueRight!);

        if (rightUpdated)
        {
            return (input with { ValueRight = rightPair }, true);
        }

        return (input, false);
    }
}