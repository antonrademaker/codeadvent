// symbols = *@-+#%=/$&

using System.Diagnostics;
using System.Text.RegularExpressions;

string[] inputFiles = ["input/example.txt", "input/input.txt"];

foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var width = inputs[0].Length;
    var height = inputs.Length;

    var board = new Dictionary<(int x, int y), Number>();

    var numbers = new List<Number>();
    var gears = new List<int>();

    var symbolLocations = new List<(int x, int y)>();

    for (int row = 0; row < height; row++)
    {
        Number? currentNumber = null;

        for (int col = 0; col < width; col++)
        {
            var current = inputs[row][col];

            if (char.IsDigit(current))
            {
                if (currentNumber == null)
                {
                    currentNumber = new Number(current);
                    numbers.Add(currentNumber);
                }
                else
                {
                    currentNumber.Add(current);
                }
                board.Add((col, row), currentNumber);
            }
            else
            {
                if (currentNumber != null)
                {
                    currentNumber = null;
                }
                if (current != '.')
                {
                    symbolLocations.Add((col, row));
                }
            }
        }
    }

    foreach (var symbolLocation in symbolLocations)
    {
        var (col, row) = symbolLocation;

        var firstGear = -1;

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (board.TryGetValue((x + col, y + row), out var number))
                {
                    if (!number.IsPartNumber)
                    {
                        number.IsPartNumber = true;

                        if (inputs[row][col] == '*')
                        {
                            if (firstGear == -1)
                            {
                                firstGear = number.GetNumber();
                            } else
                            {
                                gears.Add(firstGear* number.GetNumber());
                            }
                        }

                    }
                }
            }
        }
    }

    var partNumbers = numbers.Where(t => t.IsPartNumber).ToList();

    var part1 = partNumbers.Sum(t => t.GetNumber());

    Console.WriteLine($"Part 1: {part1}");

    var part2 = gears.Sum();
    Console.WriteLine($"Part 2: {part2}");
}

class Number(char start)
{
    public List<char> Chars { get; private set; } = [start];

    public void Add(char c)
    {
        Chars.Add(c);
    }

    public bool IsPartNumber { get; set; }

    public int GetNumber()
    {
        return int.Parse(Chars.ToArray());
    }
}
