

string[] inputFiles = ["input/example.txt", "input/example2.txt", "input/example3.txt", "input/input.txt"];



foreach (string file in inputFiles)
{
    Console.WriteLine($"Reading: {file}");

    var input = ParseFile(file);

    var answer1 = CalculateAnswer1(input);
    Console.WriteLine($"{file}: Answer 1: {answer1}");

    var answer2 = CalculateAnswer2(input);
    Console.WriteLine($"{file}: Answer 2: {answer2}");
}

static int CalculateAnswer1(Input input)
{
    var wordsFound = 0;
    for (var y = 0; y < input.Height; y++)
    {
        for (var x = 0; x < input.Width; x++)
        {
            var (_, letter) = input.GetLetter(x, y);
            if (letter != Word[0])
            {
                continue;
            }
            foreach (var direction in directionsPart1)
            {
                for (var i = 1; i < 4; i++)
                {
                    var (valid, nextLetter) = input.GetLetter(x + direction.x * i, y + direction.y * i);
                    if (!valid || nextLetter != Word[i])
                    {
                        break;
                    }
                    if (i == 3)
                    {
                        wordsFound++;
                    }
                }
            }
        }
    }

    return wordsFound;
}

static int CalculateAnswer2(Input input)
{
    var xmasFound = 0;
    for (var y = 0; y < input.Height; y++)
    {
        for (var x = 0; x < input.Width; x++)
        {
            var (_, letter) = input.GetLetter(x, y);
            if (letter != WordPart2[1])
            {
                continue;
            }

            var wordsFound = 0;


            for (var dir = 0; dir < 4; dir++)
            {
                var direction = directionsPart2[dir];
                var start = directionsPart2[(dir + 2) % 4];

                var xStart = x + start.x;
                var yStart = y + start.y;

                for (var i = 0; i <= 2; i++)
                {
                    var (valid, nextLetter) = input.GetLetter(xStart + direction.x * i, yStart + direction.y * i);
                    if (!valid || nextLetter != WordPart2[i])
                    {
                        break;
                    }
                    if (i == 2)
                    {
                        wordsFound++;
                    }
                }
            }

            if (wordsFound == 2)
            {
                xmasFound++;
            }
        }
    }

    return xmasFound;
}

static Input ParseFile(string file)
{
    return new Input(File.ReadAllLines(file));
}

readonly ref struct Input(string[] letters)
{
    private readonly ReadOnlySpan<char> Letters = string.Concat(letters).AsSpan();
    public int Width { get; } = letters[0].Length;
    public int Height { get; } = letters.Length;

    public (bool valid, char letter) GetLetter(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            return (false, ' ');
        }
        return (true, Letters[y * Width + x]);

    }
}

partial class Program
{
    static readonly string Word = "XMAS";
    static readonly string WordPart2 = "MAS";
    static readonly Dictionary<string, (int x, int y)> directionsSetup = new() {
    { "NW", (-1, -1) },
    { "NE", (1, -1) },
    { "N", (0, -1) },
    { "E", (1, 0) },
    { "SE", (1, 1) },
    { "S", (0, 1) },
    { "SW", (-1, 1) },
    { "W", (-1, 0) }
};

    static readonly (int x, int y)[] directionsPart1 = [.. directionsSetup.Values];
    static readonly (int x, int y)[] directionsPart2 = [directionsSetup["NW"], directionsSetup["NE"], directionsSetup["SE"], directionsSetup["SW"]];
}