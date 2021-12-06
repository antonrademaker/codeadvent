using System.Collections;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

var lines = System.IO.File.ReadAllLines("input/input.txt");

var bingoNumbers = new Dictionary<int, BingoNumber>(
    Enumerable.Range(0, 100).Select(x => new KeyValuePair<int, BingoNumber>(x, new BingoNumber { Number = x }))
    );

var calledBingoNumbers = lines.First().Split(',').Select(t => int.Parse(t)).ToList();

var bingoBoards = new List<BingoBoard>();

var currentBingoCard = new BingoBoard();
var currentBingoCardLine = 0;
var currentBingoCardId = 0;

foreach (var line in lines.Skip(1))
{
    if (string.IsNullOrWhiteSpace(line))
    {
        currentBingoCard = new BingoBoard() { Id = currentBingoCardId++ };
        bingoBoards.Add(currentBingoCard);
        currentBingoCardLine = 0;
    }
    else
    {
        // Console.WriteLine(line);
        var numbers = line.Trim().Replace("  ", " ").Split(' ');
        for (var x = 0; x < numbers.Length; x++)
        {
            //   Console.WriteLine($"numbers[{x}]: {numbers[x]}");
            var number = bingoNumbers[int.Parse(numbers[x])];
            currentBingoCard.Numbers[currentBingoCardLine, x] = number;
        }
        currentBingoCardLine++;
    }
}

BingoBoard? winner = default;

var numbersCalled = 0;
var lastNumberCalled = -1;

while (winner is null && numbersCalled < calledBingoNumbers.Count)
{
    lastNumberCalled = calledBingoNumbers[numbersCalled];
    bingoNumbers[lastNumberCalled].Called = true;

    Console.WriteLine($"Called {calledBingoNumbers[numbersCalled]}");

    winner = bingoBoards.FirstOrDefault(board => board.IsRoundWinner());

    var sb = bingoBoards.Aggregate(new StringBuilder(), (sb, board) => board.Print(sb));

   // ConsoleWriter.Write(sb.ToString());

    numbersCalled++;
}

Console.WriteLine($"winner: {winner?.Id}");

var score = winner.GetScore() * lastNumberCalled;

var winners = new List<BingoBoard> { winner };

Console.WriteLine($"Score: {score}");

Console.WriteLine($"Part 2");

while (numbersCalled < calledBingoNumbers.Count)
{
    lastNumberCalled = calledBingoNumbers[numbersCalled];
    bingoNumbers[lastNumberCalled].Called = true;

    Console.WriteLine($"Called {calledBingoNumbers[numbersCalled]}");

    var roundWinners = bingoBoards.Where(board => board.IsRoundWinner()).ToList();

    winners.AddRange(roundWinners);
    Console.WriteLine($"Found {roundWinners.Count()} winners in this round: total: {winners.Count}");
    var sb = bingoBoards.Aggregate(new StringBuilder(), (sb, board) => board.Print(sb));

    //ConsoleWriter.Write(sb.ToString());

    numbersCalled++;
    if (winners.Count == bingoBoards.Count)
    {
        break;
    }
}

var lastWinner = winners.Last();

Console.WriteLine($"lastWinner: {lastWinner.Id}, lastNummer: {lastNumberCalled}, score: {lastWinner.GetScore()} = { lastWinner.GetScore() * lastNumberCalled}");

public class BingoBoard
{
    public const int Size = 5;
    public int Id { get; set; }
    public BingoNumber[,] Numbers { get; } = new BingoNumber[Size, Size];

    public bool HasWon { get; private set; }

    public bool IsRoundWinner()
    {
        if (HasWon)
        {
            Console.WriteLine($"Has already won");
            return false;
        }

        HasWon = IsWinner();

        return HasWon;
    }

    public bool IsWinner()
    {
        for (var row = 0; row < Size; row++)
        {
            int column = 0;

            // check for winning row
            bool winner = true;
            while (winner && column < Size)
            {
                winner &= (Numbers[row, column].Called);
                column++;
            }

            if (winner)
            {
                return true;
            }
        }
        for (var column = 0; column < Size; column++)
        {
            // check for winning column
            bool winner = true;
            var row = 0;
            while (winner && row < Size)
            {
                winner &= (Numbers[row, column].Called);

                row++;
            }

            if (winner)
            {
                return true;
            }
        }

        Console.WriteLine($"{Id}: Not winning yet");

        return false;
    }
    public int GetScore() {
        var sum = 0;
        for (var row = 0; row < Size; row++)
        {
            for (var column = 0; column < Size; column++)
            {
                sum += Numbers[row, column].Called ? 0 : Numbers[row, column].Number;
            }
        }
        Console.WriteLine($"sum: {sum}");
        return sum;
    }

    public StringBuilder Print(StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();
        for (var x = 0; x < Size; x++)
        {

            for (var y = 0; y < Size; y++)
            {
                sb.Append(Numbers[x, y].Called ? "{FC=Green}" : "{FC=Grey}");
                sb.Append($"{Numbers[x, y].Number,3}{{/FC}}");

            }
            sb.AppendLine();

        }
        sb.AppendLine();
        return sb;
    }
}
public class BingoNumber
{
    public int Number { get; init; }
    public bool Called { get; set; }
}

public class ConsoleWriter
{

    public static void Write(string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            return;
        }

        var color_match = Regex.Match(msg, @"{[FB]C=[a-z]+}|{\/[FB]C}", RegexOptions.IgnoreCase);
        if (color_match.Success)
        {
            var initial_background_color = Console.BackgroundColor;
            var initial_foreground_color = Console.ForegroundColor;
            var background_color_history = new List<ConsoleColor>();
            var foreground_color_history = new List<ConsoleColor>();

            var current_index = 0;

            while (color_match.Success)
            {
                if ((color_match.Index - current_index) > 0)
                {
                    Console.Write(msg.Substring(current_index, color_match.Index - current_index));
                }

                if (color_match.Value.StartsWith("{BC=", StringComparison.OrdinalIgnoreCase)) // set background color
                {
                    var background_color_name = GetColorNameFromMatch(color_match);
                    Console.BackgroundColor = GetParsedColorAndAddToHistory(background_color_name, background_color_history, initial_background_color);
                }
                else if (color_match.Value.Equals("{/BC}", StringComparison.OrdinalIgnoreCase)) // revert background color
                {
                    Console.BackgroundColor = GetLastColorAndRemoveFromHistory(background_color_history, initial_background_color);
                }
                else if (color_match.Value.StartsWith("{FC=", StringComparison.OrdinalIgnoreCase)) // set foreground color
                {
                    var foreground_color_name = GetColorNameFromMatch(color_match);
                    Console.ForegroundColor = GetParsedColorAndAddToHistory(foreground_color_name, foreground_color_history, initial_foreground_color);
                }
                else if (color_match.Value.Equals("{/FC}", StringComparison.OrdinalIgnoreCase)) // revert foreground color
                {
                    Console.ForegroundColor = GetLastColorAndRemoveFromHistory(foreground_color_history, initial_foreground_color);
                }

                current_index = color_match.Index + color_match.Length;
                color_match = color_match.NextMatch();
            }

            Console.Write(msg.Substring(current_index));

            Console.BackgroundColor = initial_background_color;
            Console.ForegroundColor = initial_foreground_color;
        }
        else
        {
            Console.Write(msg);
        }
    }

    public static void WriteLine(string msg)
    {
        Write(msg);
        Console.WriteLine();
    }

    private static string GetColorNameFromMatch(Match match)
    {
        return match.Value.Substring(4, match.Value.IndexOf("}") - 4);
    }

    private static ConsoleColor GetParsedColorAndAddToHistory(string colorName, List<ConsoleColor> colorHistory, ConsoleColor defaultColor)
    {
        var new_color = Enum.TryParse<ConsoleColor>(colorName, true, out var parsed_color) ? parsed_color : defaultColor;
        colorHistory.Add(new_color);

        return new_color;
    }

    private static ConsoleColor GetLastColorAndRemoveFromHistory(List<ConsoleColor> colorHistory, ConsoleColor defaultColor)
    {
        if (colorHistory.Any())
        {
            colorHistory.RemoveAt(colorHistory.Count - 1);
        }

        return colorHistory.Any() ? colorHistory.Last() : defaultColor;
    }
}