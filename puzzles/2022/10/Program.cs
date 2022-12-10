using System.Text;

var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t))
{
    var lines = File.ReadAllLines(file);

    var registerResults = Calculate(lines).ToList();

    var positions = new List<int> { 20, 60, 100, 140, 180, 220 };

    var locs = positions.Select(pos => new { value = registerResults[pos - 1], pos });

    var sum = locs.Select(a => a.pos * a.value).Sum();
    Console.WriteLine($"File: {file} Part 1: {sum}");

    var sb = new StringBuilder();

    for (var i = 0; i < 240; i++)
    {
        var register = registerResults[i];
        var xScreen = i % 40;

        sb.Append(register - 1 <= xScreen && xScreen <= register + 1 ? '#' : '.');

        if (xScreen == 39)
        {
            sb.AppendLine();
        }
    }
    Console.WriteLine(sb.ToString());
}

static IEnumerable<int> Calculate(string[] lines)
{
    var x = 1;
    foreach (var line in lines)
    {
        yield return x;
        if (!line.StartsWith("n"))
        {
            yield return x;
            x += int.Parse(line[5..]);
        }
    }
    yield return x;
}