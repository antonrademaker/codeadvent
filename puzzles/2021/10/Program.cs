var lines = File.ReadAllLines("input/input.txt");

var part1Scores = new Dictionary<char, int>()
{
    {  ')', 3 },
    {  ']', 57 },
    {  '}', 1197 },
    {  '>', 25137 },
};

var part2Scores = new Dictionary<char, int>()
{
    {  ')', 1 },
    {  ']', 2 },
    {  '}', 3 },
    {  '>', 4 },
};

var pairs = new Dictionary<char, char>()
{
    { '(', ')' },
    { '[', ']' },
    { '{', '}' },
    { '<', '>' }
};

var part1Score = 0L;
var part2Score = new List<long>();

foreach (var line in lines)
{
    var stack = new Stack<char>();
    var illegalFound = false;

    foreach (var c in line)
    {
        var score = 0L;
        switch (c)
        {
            case '(':
            case '[':
            case '{':
            case '<':
                stack.Push(pairs[c]);
                break;
            case ')':
            case ']':
            case '}':
            case '>':
                if (stack.TryPop(out var expected) && expected != c)
                {
                    score = part1Scores[c];
                    part1Score += score;
                    illegalFound = true;
                }
                break;
            default:
                throw new Exception($"Encounterd {c}, but that's not supported");
        }
        if (illegalFound)
        {
            break;
        }
        part1Score += score;


    }

    if (!illegalFound && stack.Any())
    {
        var score2 = 0L;
        while (stack.TryPop(out char next))
        {
            score2 = (score2 * 5) + part2Scores[next];
        }
        part2Score.Add(score2);
    }
}

Console.WriteLine($"Part 1: {part1Score}");
Console.WriteLine($"Part 2: {part2Score.OrderBy(x => x).ToArray()[(part2Score.Count ) / 2]}");