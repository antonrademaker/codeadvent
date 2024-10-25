string[] inputFiles = ["input/example.txt", "input/input.txt"];

foreach (var file in inputFiles)
{
    Console.WriteLine(file);

    var inputs = System.IO.File.ReadAllLines(file);

    var part1 = CalculatePart1(inputs);

    Console.WriteLine($"Part 1: {part1}");

    var part2 = CalculatePart2(inputs);

    Console.WriteLine($"Part 2: {part2}");
}

long CalculatePart1(string[] inputs)
{
    var cards = inputs.Select(input => new Hand(input[..5], input[6..], false)).Order().ToArray();

    var score = cards.Select((card, index) => card.Bid * (index + 1)).Sum();

    return score;
}

long CalculatePart2(string[] inputs)
{
    var cards = inputs.Select(input => new Hand(input[..5], input[6..], true)).Order().ToArray();

    var score = cards.Select((card, index) => card.Bid * (index + 1)).Sum();

    return score;
}

readonly record struct Hand : IComparable<Hand>
{
    public readonly string CardsText { get; init; }
    public readonly CardType[] Cards { get; init; }
    public readonly int Bid { get; init; }
    public readonly HandType CalculatedHandType { get; init; }

    public Hand(string cards, string bid, bool isPart2)
    {
        CardsText = cards;
        Cards = cards.Select(c => ParseCard(c, isPart2)).ToArray();
        Bid = int.Parse(bid);

        CalculatedHandType = CalculateHandType(isPart2);
    }  

    private HandType CalculateHandType(bool isPart2)
    {
        var cardGroups = Cards.GroupBy(c => c).ToArray();
        if (cardGroups.Length == 1)
        {
            return HandType.FiveOfAKind;
        }

        var baseType = CalculateBase(cardGroups);

        if (isPart2 && cardGroups.Any(t => t.Key == CardType.Joker))
        {
            var numberOfJokers = cardGroups.First(g => g.Key == CardType.Joker).Count();

            if (numberOfJokers >= 2 && cardGroups.Length == 2)
            {
                return HandType.FiveOfAKind;
            }

            if (numberOfJokers == 3 || (numberOfJokers == 2 && cardGroups.Length == 3))
            {
                return HandType.FourOfAKind;
            }

            var max = cardGroups.Where(t => t.Key != CardType.Joker).Max(g => g.Count());

            if (numberOfJokers == 2)
            {
                return (max == 2) ? HandType.FourOfAKind : HandType.ThreeOfAKind;
            }

            if (numberOfJokers == 1)
            {
                if (baseType == HandType.TwoPair)
                {
                    return HandType.FullHouse;
                }
                return max switch
                {
                    4 => HandType.FiveOfAKind,
                    3 => HandType.FourOfAKind,
                    2 => HandType.ThreeOfAKind,
                    1 => HandType.Pair,
                    _ => throw new Exception("Unexpected")
                };
            }

        }
        return baseType;
    }

    private static HandType CalculateBase(IGrouping<CardType, CardType>[] cardGroups)
    {
        if (cardGroups.Length == 2)
        {
            if (cardGroups.Any(g => g.Count() == 4))
            {
                return HandType.FourOfAKind;
            }

            return HandType.FullHouse;
        }

        if (cardGroups.Length == 3)
        {
            if (cardGroups.Any(g => g.Count() == 3))
            {
                return HandType.ThreeOfAKind;
            }

            return HandType.TwoPair;
        }

        if (cardGroups.Length == 4)
        {
            return HandType.Pair;
        }

        return HandType.HighCard;
    }

    static CardType ParseCard(char card, bool isPart2)
    {
        return card switch
        {
            '2' => CardType.Two,
            '3' => CardType.Three,
            '4' => CardType.Four,
            '5' => CardType.Five,
            '6' => CardType.Six,
            '7' => CardType.Seven,
            '8' => CardType.Eight,
            '9' => CardType.Nine,
            'T' => CardType.Ten,
            'J' => isPart2 ? CardType.Joker : CardType.Jack,
            'Q' => CardType.Queen,
            'K' => CardType.King,
            'A' => CardType.Ace,
            _ => throw new Exception($"Unknown card: {card}")
        };
    }

    public int CompareTo(Hand other)
    {
        if (CalculatedHandType > other.CalculatedHandType)
        {
            return 1;
        }
        if (CalculatedHandType < other.CalculatedHandType)
        {
            return -1;
        }

        for(int i = 0; i < Cards.Length; i++)
        {
            if (Cards[i] > other.Cards[i])
            {
                return 1;
            }
            if (Cards[i] < other.Cards[i])
            {
                return -1;
            }
        }
        return 0;
    }
}

public enum HandType
{
    HighCard,
    Pair,
    TwoPair,
    ThreeOfAKind,    
    FullHouse,
    FourOfAKind,
    FiveOfAKind,
}

public enum CardType
{
    Joker,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}
