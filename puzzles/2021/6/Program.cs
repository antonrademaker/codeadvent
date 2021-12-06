
var input = System.IO.File.ReadAllText("input/input.txt");

var lanternfish = input.Split(',').Select(x => int.Parse(x)).ToList();

var generations = Enumerable.Range(0,9).Select(_ => 0L).ToList();

var maxGeneration = 8;

for(var generation = 0; generation < maxGeneration; generation++) { 
    generations[generation] = lanternfish.Count(x => x == generation);
}

Console.WriteLine(string.Join(',', generations));

Calculate(generations, 80);

Console.WriteLine($"Part 1: Total lanternfishes: {generations.Sum()}");

Calculate(generations, 256-80);

Console.WriteLine($"Part 2: Total lanternfishes: {generations.Sum()}");

static void Calculate(List<long> generations, int days)
{
    for (int day = 0; day < days; day++)
    {
        var toSpawn = generations[0];
        generations.RemoveAt(0);
        generations.Add(toSpawn);
        generations[6] += toSpawn;
    }
}