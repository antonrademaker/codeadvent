
var input = System.IO.File.ReadAllText("input/example.txt");

var lanternfish = input.Split(',').Select(x => int.Parse(x)).ToList();

const int maxGeneration = 8;
const int puzzle1Days = 80;
const int puzzle2Days = 256;

var generations = new long[maxGeneration + 1];

for (var generation = 0; generation < maxGeneration; generation++)
{
    generations[generation] = lanternfish.Count(x => x == generation);
}

Console.WriteLine(string.Join(',', generations));

Calculate(generations, puzzle1Days);

Console.WriteLine($"Part 1: Total lanternfishes: {generations.Sum()}");

Calculate(generations, puzzle2Days - puzzle1Days);

Console.WriteLine($"Part 2: Total lanternfishes: {generations.Sum()}");

static void Calculate(long[] generations, int days)
{
    long toSpawn;
    for (int day = 0; day < days; day++)
    {
        toSpawn = generations[0];
        System.Buffer.BlockCopy(generations, 1, generations, 0, maxGeneration);

        generations[maxGeneration] = toSpawn;
        generations[6] += toSpawn;
    }
}