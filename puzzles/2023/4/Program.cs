using System.Text.RegularExpressions;

Regex parser = new Regex(@"^Card\s+(?<cardId>\d+):(?<winning>(\s+\d+)+)\s*\|(?<own>(\s+\d+)+)\s*$", RegexOptions.Compiled | RegexOptions.Multiline);
string[] inputFiles = ["input/example.txt", "input/input.txt"];

foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var scratchPoints = inputs.Select(CalculateScratchPoints);

    var part1 = scratchPoints.Select(t => t.score).Sum();

    Console.WriteLine($"Part 1: {part1}");

    var part2 = CalculateTotalNumberOfCards(inputs);

    Console.WriteLine($"Part 2: {part2}");
}

(int cardId, int score, int matches) CalculateScratchPoints(string input)
{
    Console.WriteLine(input);

    var match = parser.Match(input);

    if (!match.Success)
    {
        throw new Exception($"Invalid input: {input}");
    }

    var cardId = int.Parse(match.Groups["cardId"].Value);
    var winning = match.Groups["winning"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToHashSet();
    var own = match.Groups["own"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToHashSet();

    var winningScratchcards = own.Intersect(winning);
    var matches = winningScratchcards.Count();
    var score = matches > 0 ? Enumerable.Range(1, matches - 1).Aggregate(1, (cur, next) => cur * 2) : 0;
    Console.WriteLine($"\tWinning: {string.Join(",", winningScratchcards)}, points: {score}");

    return (cardId, score, matches);
}

int CalculateTotalNumberOfCards(string[] inputs)
{
    var cards = inputs.Select(CalculateScratchPoints).ToDictionary(inp => inp.cardId, inp => inp.matches);

    var cardStack = cards.Keys.ToList();

    var totalCards = 0;

    while (cardStack.Any())
    {
        var currentCard = cardStack[0];
        cardStack.RemoveAt(0);
        totalCards++;

        var toAdd = cards[currentCard];

        var cardsToAdd = Enumerable.Range(currentCard + 1, toAdd).Reverse().ToList();

        foreach (var card in cardsToAdd)
        {
            cardStack.Insert(0, card);
        }
    }

    return totalCards;
}