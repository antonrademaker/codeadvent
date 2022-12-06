var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t))
{
    var lines = File.ReadAllLines(file);

    foreach (var line in lines)
    {
        Console.WriteLine(line);

        var span = line.AsSpan();

        Console.WriteLine("\r\nPart 1:");
        Calculate(span, 4);
        Console.WriteLine("Part 2:");
        Calculate(span, 14);
        Console.WriteLine();
    }
}

static void Calculate(ReadOnlySpan<char> span, int required)
{
    var uniqueCount = 0;
    var len = span.Length;

    for (var pos = 0; pos < len; pos++)
    {
        var isUnique = true;

        var b = 1;

        for (; b < uniqueCount + 1 && pos - b >= 0; b++)
        {
            if (span[pos] == span[pos - b])
            {
                isUnique = false;
                break;
            }
        }

        if (isUnique)
        {
            uniqueCount++;
        }
        else
        {
            uniqueCount = b;
        }
        if (uniqueCount == required && pos >= required)
        {
            Console.WriteLine(span.ToString());

            Console.Write(new string(' ', pos + 1 - required));
            Console.WriteLine(new string('^', required));
            Console.WriteLine($"Found after {pos + 1} {span.Slice(pos - required, required)}");

            return;
        }
    }
}