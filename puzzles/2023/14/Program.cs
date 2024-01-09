using State = (bool IsRound, bool IsMovable);

namespace AoC.Puzzles.Y2023.D12;

public static class Program
{
    private static void Main(string[] args)
    {
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
    }

    private static int CalculatePart1(string[] inputs)
    {
        var parsed = ParseInputs(inputs);

        var width = inputs[0].Length;
        var height = inputs.Length;

        var grid = Tilt(parsed, height, width, Direction.North);
        return CalculateLoad(grid, height, width);
    }

    private static int CalculatePart2(string[] inputs)
    {
        var grid = ParseInputs(inputs);

        var width = inputs[0].Length;
        var height = inputs.Length;

        var startOfCycle = 0;

        var total = 1000000000;

        var cache = new Dictionary<string, int>();

        var states = new List<Dictionary<Direction, (bool IsRound, bool IsMovable)>>();

        for (var steps = 0; steps < total; steps++)
        {
            foreach (var direction in Direction.AllDirections)
            {
                grid = Tilt(grid, height, width, direction);
            }

            var key = CalculateKey(grid);

            if (cache.TryGetValue(key, out var firstDetection))
            {
                startOfCycle = firstDetection;
                break;
            }

            cache.Add(key, steps);
            states.Add(grid.ToDictionary(t => t.Key, t => t.Value));
        }

        var index = startOfCycle + ((total - startOfCycle) % (states.Count - startOfCycle)) - 1;
        return CalculateLoad(states[index], height, width);
    }

    private static string CalculateKey(Dictionary<Direction, (bool IsRound, bool IsMovable)> grid)
    {
        return string.Join(',', grid.Where(t => t.Value.IsRound).OrderBy(k => k.Key.Y).ThenBy(k => k.Key.X).Select(p => $"{p.Key.X}-{p.Key.Y}"));
    }

    private static int CalculateLoad(Dictionary<Direction, State> grid, int height, int width)
    {
        return grid.Where(t => t.Value.IsRound).Sum(loc => height - loc.Key.Y);
    }

    private static Dictionary<Direction, State> Tilt(Dictionary<Direction, State> currentGrid, int height, int width, Direction dir)
    {
        var grid = true;

        foreach (var point in currentGrid.Where(t => t.Value.IsRound).Select(t => t.Key))
        {
            currentGrid[point] = (true, true);
        }

        while (grid)
        {
            grid = false;

            var moveable = currentGrid.Where(t => t.Value.IsRound).Select(t => t.Key).ToList();

            foreach (var point in moveable)
            {
                var nextTile = point + dir;

                if (currentGrid.ContainsKey(nextTile) || dir.X != 0 && (nextTile.X < 0 || nextTile.X >= width) || (dir.Y != 0 && (nextTile.Y < 0 || nextTile.Y >= height)))
                {
                    currentGrid[point] = (currentGrid[point].IsRound, false);
                    continue;
                }

                grid = true;

                currentGrid[nextTile] = currentGrid[point];
                currentGrid.Remove(point);
            }
        }

        return currentGrid;
    }

    private static Dictionary<Direction, State> ParseInputs(string[] inputs)
    {
        var grid = new Dictionary<Direction, State>();

        for (var y = 0; y < inputs.Length; y++)
        {
            for (var x = 0; x < inputs[y].Length; x++)
            {
                if (inputs[y][x] != '.')
                {
                    grid.Add(new Direction(x, y), new State(inputs[y][x] == 'O', inputs[y][x] == 'O'));
                }
            }
        }

        return grid;
    }
}

public readonly record struct Direction(int X, int Y)
{
    public static readonly Direction North = new(0, -1);
    public static readonly Direction South = new(0, 1);
    public static readonly Direction West = new(-1, 0);
    public static readonly Direction East = new(1, 0);

    public static readonly Direction[] AllDirections = [North, West, South, East];

    public static Direction operator -(Direction direction1, Direction direction2) => new(direction1.X - direction2.X, direction1.Y - direction2.Y);
    public static Direction operator +(Direction direction1, Direction direction2) => new(direction1.X + direction2.X, direction1.Y + direction2.Y);
}