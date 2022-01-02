using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

var inputFiles = new string[] {
//    "input/example.txt",
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
    var map = new MapPart1();

    var roomLine1 = lines[2].Split('#', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();
    var roomLine2 = lines[3].Split('#', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();

    var positions = ImmutableDictionary<int, Letter>.Empty;
    var startPosition = 11;

    for (var x = 0; x < roomLine2.Length; x++)
    {
        positions = positions.Add(startPosition++, LetterFactory(roomLine1[x]));
        positions = positions.Add(startPosition++, LetterFactory(roomLine2[x]));
    }

    for (var y = 18; y > 10; y--)
    {

        if (map.Edges[y].Accepts == positions[y].Char && (y % 2 == 0 || positions[y + 1].IsFinished))
        {
            var cur = positions[y];
            positions = positions.Remove(y).Add(y, cur with { IsFinished = true });
        }

    }

    positions.Print();
    long currentLow = CalculateLowestEnergy(map, positions);
    Console.WriteLine($"Lowest: {currentLow}");
}

void CalculatePart2(string[] lines)
{
    var map = new MapPart2();

    var extraLines = new string[] { "#D#C#B#A#", "#D#B#A#C#" };

    var roomLine1 = lines[2].Split('#', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();
    var roomLine2 = extraLines[0].Split('#', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();
    var roomLine3 = extraLines[1].Split('#', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();
    var roomLine4 = lines[3].Split('#', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => s[0]).ToArray();

    var positions = ImmutableDictionary<int, Letter>.Empty;
    var startPosition = 11;

    for (var x = 0; x < roomLine2.Length; x++)
    {
        positions = positions.Add(startPosition++, LetterFactory(roomLine1[x]));
        positions = positions.Add(startPosition++, LetterFactory(roomLine2[x]));
        positions = positions.Add(startPosition++, LetterFactory(roomLine3[x]));
        positions = positions.Add(startPosition++, LetterFactory(roomLine4[x]));
    }
    // mark all positions as finished if possible
    for (var y = 26; y > 10; y--)
    {
        if (map.Edges[y].Accepts == positions[y].Char && ((y - 11) % 4 == 3 || positions[y + 1].IsFinished))
        {
            var cur = positions[y];
            positions = positions.Remove(y).Add(y, cur with { IsFinished = true });
        }
    }

    positions.Print();
    long currentLow = CalculateLowestEnergy(map, positions);
    Console.WriteLine($"Lowest: {currentLow}");
}


long CalculateLowestEnergy(Map map, ImmutableDictionary<int, Letter> positions)
{
    var queue = new PriorityQueue<(ImmutableDictionary<int, Letter>, long), int>();

    queue.Enqueue((positions, 0L), 0);

    var currentLow = long.MaxValue;

    var calculated = new HashSet<long>();

    while (queue.TryDequeue(out var s, out var good))
    {
        (var state, var usedEnergy) = s;
        var newStates = map.CalculateNewStates(state, usedEnergy);

        foreach (var newState in newStates)
        {
            if (newState.newState.All(t => t.Value.IsFinished))
            {
                currentLow = Math.Min(currentLow, usedEnergy + newState.energy);
            }
            else
            {
                var newGood = newState.newState.Count(t => t.Value.IsFinished);
                if (calculated.Add(CalculatePositionHash(newState.newState, usedEnergy + newState.energy)))
                {
                    queue.Enqueue((newState.newState, usedEnergy + newState.energy), 20 - newGood);
                }
            }
        }
    }

    return currentLow;
}

long CalculatePositionHash(ImmutableDictionary<int, Letter> positions, long energy)
{
    var pHash = positions.Aggregate(1L, (acc, cur) => acc + (cur.Key * 997 * cur.Value.Char.GetHashCode()));

    return pHash * (991 * energy);
}


Letter LetterFactory(char letter)
{
    return letter switch
    {
        'A' => new Letter('A', 1),
        'B' => new Letter('B', 10),
        'C' => new Letter('C', 100),
        _ => new Letter('D', 1000),
    };
}

public record Letter(char Char, int Weight)
{
    public bool HasMoved { get; init; }
    public bool IsFinished { get; init; }
}

public class MapPart1 : Map
{
    public MapPart1()
    {
        ColSize = 2;
        var cantStop = new List<int> { 2, 4, 6, 8 };

        var position = 11;


        Edges.AddRange(
            Enumerable.Range(0, position).Select(i => new Edge
            {
                Id = i,
                IsTopRow = true,
                CantStop = cantStop.Contains(i)
            }));


        foreach (var c in "ABCD")
        {
            Edges.AddRange(Enumerable.Range(position, 2).Select(i => new Edge
            {
                Id = i,
                Accepts = c
            }));

            position += 2;
        }
        Segments.Add(Edges[0], new List<Segment> {
            new Segment(Edges[1])
        });
        for (var x = 1; x < 10; x++)
        {

            Segments.Add(Edges[x], new List<Segment> {
                new Segment(Edges[x+1]),
                new Segment(Edges[x-1])
            });

        }

        Segments.Add(Edges[10], new List<Segment> {
            new Segment(Edges[9])
        });

        var baseRow = 2;

        var colStart = 11;

        var colSize = 2;

        for (int x = 0; x < 4; x++)
        {

            Segments.Add(Edges[colStart], new List<Segment> {
                new Segment(Edges[colStart+1]),
                new Segment(Edges[baseRow])}
            );


            Segments.Add(Edges[colStart + 1], new List<Segment> { new Segment(Edges[colStart]) });

            Segments[Edges[baseRow]].Add(new Segment(Edges[colStart]));

            colStart += colSize;
            baseRow += colSize;
        }
    }
}


public class MapPart2 : Map
{
    public MapPart2()
    {

        ColSize = 4;

        var cantStop = new List<int> { 2, 4, 6, 8 };

        var position = 11;


        Edges.AddRange(
            Enumerable.Range(0, position).Select(i => new Edge
            {
                Id = i,
                IsTopRow = true,
                CantStop = cantStop.Contains(i)
            }));


        foreach (var c in "ABCD")
        {
            Edges.AddRange(Enumerable.Range(position, 4).Select(i => new Edge
            {
                Id = i,
                Accepts = c
            }));

            position += 4;
        }
        Segments.Add(Edges[0], new List<Segment> {
            new Segment(Edges[1])
        });

        for (var x = 1; x < 10; x++)
        {

            Segments.Add(Edges[x], new List<Segment> {
                new Segment(Edges[x+1]),
                new Segment(Edges[x-1])
            });

        }

        Segments.Add(Edges[10], new List<Segment> {
            new Segment(Edges[9])
        });

        var baseRow = 2;

        var colStart = 11;

        var colSize = 4;

        for (int x = 0; x < 4; x++)
        {
            Segments[Edges[baseRow]].Add(new Segment(Edges[colStart]));

            Segments.Add(Edges[colStart], new List<Segment> {
                new Segment(Edges[baseRow])}
            );

            for (var col = 1; col < 4; col++)
            {

                Segments.Add(Edges[colStart + col], new List<Segment> {
                    new Segment(Edges[colStart + col - 1])
                });

                Segments[Edges[colStart + col - 1]].Add(new Segment(Edges[colStart + col]));

            }


            colStart += colSize;
            baseRow += 2;
        }
    }
}

public class Map
{
    public int ColSize = 0;


    public List<Edge> Edges { get; set; } = new();

    public Dictionary<Edge, List<Segment>> Segments { get; set; } = new();


    public IEnumerable<(ImmutableDictionary<int, Letter> newState, long energy)> CalculateNewStates(ImmutableDictionary<int, Letter> state, long currentEnergy)
    {
        foreach (var (position, letter) in state.Where(t => !t.Value.IsFinished).OrderByDescending(t => t.Value.Weight))
        {
            foreach (var newPosition in GetReachablePositions(state, position, letter))
            {
                // move the letter
                var newState = state.Remove(position).Add(newPosition.newPosition, newPosition.result);

                yield return (newState, newPosition.newEnergy);

            }
        }
    }

    private IEnumerable<(int newPosition, long newEnergy, Letter result)> GetReachablePositions(ImmutableDictionary<int, Letter> state, int position, Letter letter)
    {
        var dict = new Dictionary<int, long>();

        var positionsVisited = ImmutableHashSet<int>.Empty;

        foreach (var (newPosition, newEnergy) in ReachablePositions(positionsVisited, dict, state, position, position, letter, false))
        {
            if (!dict.ContainsKey(newPosition) || dict[newPosition] > newEnergy) { }
            {
                dict[newPosition] = newEnergy;
            }
        }

        var resultLetter = letter;

        if (!resultLetter.HasMoved)
        {
            resultLetter = resultLetter with
            {
                HasMoved = true
            };
        }
        else
        {
            resultLetter = resultLetter with
            {
                IsFinished = true
            };
        }

        return dict.Select(t => (t.Key, t.Value, resultLetter));
    }

    private IEnumerable<(int newPosition, long newEnergy)> ReachablePositions(ImmutableHashSet<int> positionsVisited, Dictionary<int, long> dict, ImmutableDictionary<int, Letter> state, int oldPosition, int position, Letter letter, bool switched)
    {
        var currentPosition = Edges[position];

        foreach (var reachable in Segments[currentPosition])
        {
            var newPosition = reachable.Other.Id;

            if (state.ContainsKey(newPosition))
            {
                continue;
            }

            if (positionsVisited.Contains(newPosition))
            {
                continue;
            }

            var path = positionsVisited.Add(newPosition);

            var hasSwitched = switched;

            if (!switched && currentPosition.IsTopRow != Edges[newPosition].IsTopRow)
            {
                hasSwitched = true;
            }

            var newEnergy = letter.Weight;

            if (AllowedMove(state, oldPosition, newPosition, letter, hasSwitched))
            {
                yield return (reachable.Other.Id, letter.Weight);
            }

            foreach (var steps in ReachablePositions(path, dict, state, oldPosition, newPosition, letter, hasSwitched))
            {
                var energy = letter.Weight + steps.newEnergy;
                yield return (steps.newPosition, energy);
            }
        }
    }

    private bool AllowedMove(ImmutableDictionary<int, Letter> state, int position, int newPosition, Letter letter, bool hasSwitched)
    {
        if (!hasSwitched)
        {
            return false;
        }

        if (Edges[newPosition].CantStop)
        {
            return false;
        }

        if (!letter.HasMoved)
        {
            if (!Edges[newPosition].IsTopRow)
            {
                return false;
            }
        }
        else
        {
            if (Edges[newPosition].IsTopRow)
            {
                return false;
            }
            if (Edges[newPosition].Accepts != letter.Char)
            {
                return false;
            }
        }

        if (newPosition >= 11)
        {
            if ((newPosition - 11) % ColSize != (ColSize - 1))
            {
                if (!state.ContainsKey(newPosition + 1) || !state[newPosition + 1].IsFinished)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

public struct Edge
{
    public int Id { get; init; }

    public bool IsTopRow { get; init; }

    public char? Accepts { get; init; }

    public bool CantStop { get; init; }
}

public record Segment(Edge Other);

public static class Helpers
{

    public static void Print(this ImmutableDictionary<int, Letter> positions)
    {
        var sb = new StringBuilder();
        sb.AppendLine(new String('#', 13));

        sb.Append('#');
        for (var i = 0; i < 11; i++)
        {
            sb.Append(positions.TryGetValue(i, out var letter) ? letter.Char : '.');
        }
        sb.Append("#");
        sb.AppendLine();

        var colAdder = positions.Count == 8 ? 2 : 4;

        var first = 11;

        for (var row = 0; row < colAdder; row++)
        {
            if (row == 0)
            {
                sb.Append("###");
            }
            else
            {
                sb.Append("  #");
            }

            for (var col = 0; col < 4; col++)
            {
                sb.Append(positions.TryGetValue((col * colAdder) + first + row, out var letter) ? letter.Char : '.');
                sb.Append("#");

            }
            if (row == 0)
            {
                sb.Append("##");
            }
            sb.AppendLine();
        }

        sb.AppendLine($"  {new String('#', 9)}");

        Console.WriteLine(sb.ToString());
    }
}