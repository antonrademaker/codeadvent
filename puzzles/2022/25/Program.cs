var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t))
{
    Console.WriteLine($"Starting {file}");
    var lines = File.ReadAllLines(file);

    var parsed = Parse(lines).ToList();

    var result = parsed.First();

    foreach (var line in parsed.Skip(1))
    {
        Console.WriteLine($"\tAdding line: {Print(line)}");

        var item = line;

        for (var i = 0; i < item.Count; i++)
        {
            if (i < result.Count)
            {
                result[i] += item[i];
                Fix(result, i);
            }
            else
            {
                result.Add(item[i]);
            }
        }
        Console.WriteLine($"\t= {Print(result)}");
    }
    Console.WriteLine($"Result: {Print(result)}");
}

static void Fix(List<int> result, int position)
{
    var t = result[position];
    if (t < -2)
    {
        t += 5;
        result[position] = t;
        result[position + 1]--;

        if (position > 0)
        {
            Fix(result, position - 1);
        }
    }
    else if (t > 2)
    {
        t -= 5;
        result[position] = t;

        if (result.Count == position + 1)
        {
            result.Add(1);
        }
        else
        {
            result[position + 1]++;
            if (result[position + 1] > 2)
            {
                Fix(result, position + 1);
            }
        }
    }
}

static IEnumerable<List<int>> Parse(string[] input)
{
    return input.Select(line => line.Select(c => c switch
            {
                '=' => -2,
                '-' => -1,
                _ => c - '0'
            }).Reverse().ToList());
}

static string Print(List<int> input)
{
    var snafu = input.AsEnumerable().Reverse().Select(c => c switch
    {
        -2 => '=',
        -1 => '-',
        _ => (char)('0' + c)
    });
    return string.Join(string.Empty, snafu);
}