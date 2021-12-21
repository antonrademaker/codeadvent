using System.Diagnostics;
using System.Numerics;
using System.Text;

var inputFiles = new string[] {
    "input/example.txt"
    ,
    "input/input.txt"
};
var sw = new Stopwatch();


foreach (var exampleFile in inputFiles)
{
    Console.WriteLine(exampleFile);
    var file = File.ReadAllLines(exampleFile);
    sw.Start();

    CalculatePart1(file);

    sw.Stop();
    Console.WriteLine($"Part 1 in {sw.ElapsedMilliseconds}ms");
    sw.Reset();
    sw.Start();

    CalculatePart2(file);

    sw.Stop();
    Console.WriteLine($"Part 2 in {sw.ElapsedMilliseconds}ms");

    Console.WriteLine("-- End of file--");
}

void CalculatePart1(string[] lines)
{
    var dice = new Dice();

    var numberOfPlayers = lines.Length;

    var playerPositions = lines.Select(x => int.Parse(x.Split(":", StringSplitOptions.TrimEntries).Last())).ToArray();

    var playerScores = new long[numberOfPlayers];

    var currentPlayer = 1;

    do
    {
        currentPlayer = (currentPlayer + 1) % 2;
        var diceResults = dice.Throw().Take(3).ToArray();

        var diceResult = diceResults.Sum();



        playerPositions[currentPlayer] += diceResult;
        while (playerPositions[currentPlayer] > 10)
        {
            playerPositions[currentPlayer] -= 10;
        }
        playerScores[currentPlayer] += playerPositions[currentPlayer];

    } while (playerScores[currentPlayer] < 1000);

    var losingPlayer = (currentPlayer + 1) % 2;

    Console.WriteLine($"{playerScores[losingPlayer]} * {dice.Count} = {playerScores[losingPlayer] * dice.Count}");
}

void CalculatePart2(string[] lines)
{
    var playerPositions = lines.Select(x => int.Parse(x.Split(":", StringSplitOptions.TrimEntries).Last())).ToArray();

    var game = new Game(playerPositions);

    var scores = game.Play();

    var totalWins = scores.Sum();

    var maxScore = scores.Max();

    Console.WriteLine($"Max score: {maxScore}, wins: {totalWins}");

    Console.WriteLine($"Unique: {game.Cache.Values.Distinct().Count()}");
}

public class Dice
{
    private int current = 0;
    public int Count { get; private set; }
    public IEnumerable<int> Throw()
    {
        while (true)
        {
            current = (current % 100) + 1;
            Count++;
            yield return current;

        }
    }
}

public class Game
{
    public static readonly int[] Odds = new int[] { 1, 3, 6, 7, 6, 3, 1 };

    public readonly Dictionary<long, IList<long>> Cache = new();

    public int[] PlayerStartPositions { get; init; }

    public Game(int[] startPositions)
    {
        PlayerStartPositions = startPositions;
    }

    public long[] Play()
    {
        return Play(PlayerStartPositions.Select(t => new Player { Position = t, Score = 0 }).ToList(), 1,0).ToArray();
    }



    public IList<long> Play(IList<Player> players, int currentPlayer, int steps)
    {
        var newPlayers = new Player[players.Count];
        var otherPlayer = 1 - currentPlayer;

        newPlayers[otherPlayer] = players[otherPlayer];

        if (steps == 0)
        {
            newPlayers[currentPlayer] = players[currentPlayer];
        } else { 
            var nextPosition = players[currentPlayer].Position + steps;
            while (nextPosition > 10)
            {
                nextPosition -= 10;
            }

            newPlayers[currentPlayer] = players[currentPlayer] with { Position = nextPosition, Score = players[currentPlayer].Score + nextPosition };
        }


        var hash = CalculateHash(newPlayers, currentPlayer);
        if (Cache.TryGetValue(hash, out var result))
        {
            return result;
        }

        if (newPlayers.Any(p => p.Score >= 21))
        {            
            return newPlayers[0].Score >= 21 ? new long[] { 1L, 0L } : new long[] { 0L, 1L };
        }


        var scores = new List<long> { 0L, 0L }.AsEnumerable();

        var wins1 = Odds
            .Select((value, index) => Play(newPlayers, otherPlayer, index + 3).Select(x => x * value)).ToList();

        var wins = wins1.Aggregate(scores, (a, b) => a.Zip(b).Select(t => t.First + t.Second)).ToList();

        Cache.TryAdd(hash, wins);

        return wins;
    }

    private static long CalculateHash(IList<Player> players, int currentPlayer)
    {
        return ((currentPlayer + 1L) * 37) ^ players.Aggregate(1L, (acc, f) => (acc * 53) ^ f.GetHashCode());
    }
}

public struct Player
{
    public int Position { get; set; }
    public long Score { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Player player &&
               Position == player.Position &&
               Score == player.Score;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, Score);
    }
}