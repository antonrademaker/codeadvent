using System.Text.RegularExpressions;

Regex parser = Parser();

string[] inputFiles = { "input/example.txt", "input/input.txt" };

const int START_POSITION = 50;

foreach (string file in inputFiles)
{
    var inputs = ParseFile(file, parser);

    var answer1 = CalculateAnswer1(inputs);
    Console.WriteLine("Answer 1: {0}", answer1);

    var answer2 = CalculateAnswer2(inputs);
    Console.WriteLine("Answer 2: {0}", answer2);
}

static List<(int dir, int clicks)> ParseFile(string file, Regex parser)
{
    return [.. File.ReadAllLines(file)
        .Select(line => parser.Match(line).Groups)
        .Select(m => (dir: m[1].Value == "R" ? 1 : -1, clicks: int.Parse(m[2].Value)))];
}


static int CalculateAnswer1(List<(int dir, int clicks)> lines)
{
    var pos = START_POSITION;
    var answer = 0;

    foreach (var (dir, clicks) in lines)
    {
        pos = (pos + dir * clicks + 100) % 100;

        if (pos == 0)
        {
            answer++;
        }
    }
    return answer;
}

static int CalculateAnswer2(List<(int dir, int clicks)> lines)
{
    var pos = START_POSITION;
    var answer = 0;

    foreach (var (dir, clicks) in lines)
    {
        (pos, answer) = CalculateNextPos(pos, answer, dir, clicks);

    }
    return answer;
}

partial class Program
{
    [GeneratedRegex(@"(?<dir>[LR])(?<clicks>\d+)")]
    private static partial Regex Parser();


    public static (int pos, int answer) CalculateNextPos(int currentPos, int answer, int dir, int clicks)
    {
        int completeCycles = clicks / 100;
        answer += completeCycles;

        // Calculate the effective movement (remainder after complete cycles)
        int effectiveClicks = clicks % 100;

        // Calculate the position after the effective movement
        int nextPos = currentPos + dir * effectiveClicks;

        // Check if we cross zero during the effective movement
        if (dir > 0) // Moving right
        {
            if (currentPos > 0 && nextPos >= 100)
            {
                answer++; // Crossed from 99 to 0
            }
        }
        else // Moving left  
        {
            if (currentPos > 0 && nextPos <= 0)
            {
                answer++; // Crossed from 0 to 99
            }
        }

        // Normalize the position to 0-99 range
        nextPos = ((nextPos % 100) + 100) % 100;

        return (nextPos, answer);
    }
}
