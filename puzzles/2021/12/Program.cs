using System.Collections.Immutable;
using System.Text.RegularExpressions;

var exampleFiles = new string[] { "input/example.txt", "input/example2.txt", "input/example3.txt", "input/input.txt" };

foreach (var exampleFile in exampleFiles/*.Skip(2).Take(1)*/)
{
    Console.WriteLine(exampleFile);
    var file = File.ReadAllLines(exampleFile);
    CalculatePart1(file);
    CalculatePart2(file);
    Console.WriteLine("-- End of file--");

}

void CalculatePart1(string[] exampleFile)
{
    var map = new Map();

    foreach (var line in exampleFile)
    {
        var parts = line.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var left = parts[0];
        var right = parts[1];

        map.AddConnection(left, right);
    }

    var numberOfPaths = map.CalculatePaths();

    Console.WriteLine($"Found: {numberOfPaths}");


}

void CalculatePart2(string[] exampleFile)
{
    var map = new Map();

    foreach (var line in exampleFile)
    {
        var parts = line.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var left = parts[0];
        var right = parts[1];

        map.AddConnection(left, right);
    }
    var numberOfPaths = map.CalculatePaths(new Cave("Visit twice", false));
    Console.WriteLine($"Found part 2: {numberOfPaths}");
}

public class Map
{
    public HashSet<Cave> Caves { get; } = new();

    public List<Connection> Connections { get; } = new();

    public Queue<ImmutableList<Connection>> PotentialPaths { get; } = new();

    public List<ImmutableList<Connection>> EndingPaths { get; } = new();

    private Cave? start;
    public Cave Start
    {
        get
        {
            return start ??= Caves.Single(t => t.Name == "start");
        }
    }

    private Cave? end;

    public Cave End
    {
        get
        {
            return end ??= Caves.Single(t => t.Name == "end");
        }
    }

    internal void AddConnection(string left, string right)
    {
        var leftCave = GetCave(left);
        var rightCave = GetCave(right);
        
        Connections.Add(new Connection(leftCave, rightCave));
        Connections.Add(new Connection(rightCave, leftCave));
    }

    internal int CalculatePaths(Cave? edgeCase = null)
    {
        var rootList = ImmutableList.Create<Connection>();
        foreach (var startConnection in Connections.Where(t => t.EdgeA == Start).Select(t => rootList.Add(t)))
        {
            PotentialPaths.Enqueue(startConnection);
        }

        while (PotentialPaths.TryDequeue(out var potential))
        {
            var currentEdge = potential.Last().EdgeB;

            var potentialNextConnections = Connections.Where(t => t.EdgeA == currentEdge);


            foreach (var nextConnection in potentialNextConnections)
            {

                if (nextConnection.EdgeB == End)
                {
                    EndingPaths.Add(potential.Add(nextConnection));
                    continue;
                }
                if (nextConnection.EdgeB == Start)
                {
                    continue;
                }
                var candidate = potential.Add(nextConnection);

                if (nextConnection.EdgeB.IsBig)
                {
                    PotentialPaths.Enqueue(candidate);

                    continue;
                }

                var alreadyVisited = potential.Count(t => t.EdgeB == nextConnection.EdgeB);

                if (alreadyVisited == 1 && edgeCase != null && !candidate.Any(t => t.EdgeA == edgeCase))
                {
                    candidate = candidate.Add(new Connection(edgeCase, nextConnection.EdgeB));
                    alreadyVisited = 0;
                }
                    

                if (alreadyVisited == 0)
                {
                    PotentialPaths.Enqueue(candidate);
                }
            }
        }

        //foreach (var path in EndingPaths)
        //{
        //    Console.WriteLine($"{string.Join('-', path.Select(p => p.EdgeA.Name))}-{path.Last().EdgeB.Name}");
        //}

        return EndingPaths.Count;
    }



    internal Cave GetCave(string name)
    {
        var isBig = name.All(c => char.IsUpper(c));
        var cave = new Cave(name, isBig);
        if (Caves.TryGetValue(cave, out var existing))
        {
            return existing;
        }
        Caves.Add(cave);
        return cave;

    }
}

public record Connection(Cave EdgeA, Cave EdgeB);

public record Cave(string Name, bool IsBig);

//public class Cave
//{
//    public Cave(string name, bool isBig)
//    {
//        Name = name;
//        IsBig = isBig;
//    }
//    public string Name { get; set; }
//    public bool IsBig { get; set; }
//}
