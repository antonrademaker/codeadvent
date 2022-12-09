var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t))
{
    var lines = File.ReadAllLines(file);

    var width = lines[0].Length;
    var height = lines.Length;

    var data = lines.SelectMany(t => t).Select(c => c - '0').ToArray().AsSpan();



    //top->bottom
    var directions = new Direction[] {
new Direction {
    Start = width + 1,
    Step = 1,
    Steps = width - 2,
    Dir = width,
    DirSteps = height - 2
},
new Direction {
    //    Right->left

    Start = width + width - 2,
    Step = width,
    Steps = height - 2,
    Dir = -1,
    DirSteps = width - 2
},
//    Bottom->top},
new Direction
{
    Start = 1 + width * (height - 2),
    Step = 1,
    Steps = width - 2,
    Dir = -width,
    DirSteps = height - 2
},
    new Direction {
        //Left->right

        Start = width + 1,
    Step = width,
    Steps = height - 2,
    Dir = 1,
    DirSteps = width - 2
    } };

    var dataSize = data.Length;

    var viewingDistances = directions.ToDictionary(t => t, t => new int[dataSize]);

    var found = new HashSet<int>();

    foreach (var direction in directions)
    {
        Console.WriteLine($"Next direction: {direction}");

        for (var aStep = 0; aStep < direction.Steps; aStep++)
        {
            var pos = direction.Start + (aStep * direction.Step);

            var initialPos = pos + (direction.Dir * -1);

            var treeHeight = data[initialPos];

            var heightsSeen = new List<int>() { data[initialPos] };


            for (var bStep = 0; bStep < direction.DirSteps; bStep++)
            {

                if (treeHeight < data[pos])
                {
                    treeHeight = data[pos];
                    found.Add(pos);
                }

                var viewingDistance = 0;

                for (var i = heightsSeen.Count - 1; i >= 0; i--)
                {
                    if (data[pos] > heightsSeen[i])
                    {
                        viewingDistance++;
                    } else if (data[pos] >= heightsSeen[i])
                    {
                        viewingDistance++;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                viewingDistances[direction][pos] = viewingDistance;

                heightsSeen.Add(data[pos]);

                pos += direction.Dir;
            }
        }
    }
    Console.WriteLine($"Part 1: {found.Count + (width * 2) + (height - 2) * 2}");

    var scenicScores = viewingDistances.Values.Select(t => t.AsEnumerable()).Aggregate((cur, next) => cur.Zip(next, (c, n) => c * n));

    var maxScenicScore = scenicScores.Max();

    Console.WriteLine($"{data[7]}: {string.Join(',', viewingDistances.Values.Select(t => t[7]))}");
    Console.WriteLine($"{data[17]}: {string.Join(',', viewingDistances.Values.Select(t => t[17]))}");




    Console.WriteLine($"Part 2: {maxScenicScore}");

    //foreach(var pos in found)
    //{
    //    Console.WriteLine($"Found at {pos}: {data[pos]}");
    //}
}

record Direction
{
    public required int Start { get; init; }
    public required int Step { get; init; }
    public required int Steps { get; init; }
    public required int Dir { get; init; }
    public required int DirSteps { get; init; }
}