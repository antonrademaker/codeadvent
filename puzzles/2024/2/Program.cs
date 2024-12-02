string[] inputFiles = { "input/example.txt", "input/input.txt" };

foreach (string file in inputFiles)
{
    Console.WriteLine("Reading: {0}", file);

    var inputs = ParseFile(file);

    var answer1 = CalculateAnswer1(inputs);

    Console.WriteLine("{0}: Answer 1: {1}", file, answer1);
    var answer2 = CalculateAnswer2(inputs);
    Console.WriteLine("{0}:Answer 2: {1}", file, answer2);
}

int CalculateAnswer1(List<int[]> inputs)
{
    return inputs.Aggregate(0, (cur, reports) => cur + (IsValid(reports) ? 1 : 0));
}

int CalculateAnswer2(List<int[]> inputs)
{
    return inputs.Aggregate(0, (cur, reports) =>
    {
        for (int i = 0; i < reports.Length; i++)
        {
            ReadOnlySpan<int> reportSelection = [.. reports[0..i], .. reports[(i + 1)..]];
            if (IsValid(reportSelection))
            {
                return cur + 1;
            }
        }

        return cur;
    });
}

bool IsValid(ReadOnlySpan<int> reports)
{
    var direction = Direction.Unknown;

    for (int i = 1; i < reports.Length; i++)
    {
        var difference = reports[i] - reports[i - 1];

        if (difference == 0 || Math.Abs(difference) > 3)
        {
            return false;
        }
        if (difference > 0)
        {
            if (direction == Direction.Descending)
            {
                return false;
            }
            direction = Direction.Ascending;
        }
        else
        {
            if (direction == Direction.Ascending)
            {
                return false;
            }
            direction = Direction.Descending;
        }

    }
    return true;
}

static List<int[]> ParseFile(string file)
{
    return File.ReadAllLines(file)
        .Select(line => line.Split(' ').Select(int.Parse).ToArray())
        .ToList();
}

public enum Direction
{
    Unknown = 0, Ascending, Descending
}