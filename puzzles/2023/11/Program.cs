using System.Diagnostics;

using Point = (long x, long y);

namespace AoC.Puzzles.Y2023.D10;

public static class Program
{
    private static readonly Point Space = (-1, -1);



    private static void Main(string[] args)
    {
        string[] inputFiles = ["input/example.txt", "input/input.txt"];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var emptyRows = inputs.Select((data, row) => (data, row)).Where(t => t.data.All(c => c == '.')).Select(t => t.row).ToArray();

            var emptyColumns = Enumerable.Range(0, inputs[0].Length).Where(col => inputs.Select(row => row[col]).All(c => c == '.')).ToArray();

            var part1 = Calculate(inputs, emptyRows, emptyColumns, 1);

            Console.WriteLine($"Part 1: {part1}");

            var part2 = Calculate(inputs, emptyRows, emptyColumns, 1000000 - 1);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static long Calculate(string[] inputs, int[] emptyRows, int[] emptyColumns, int expansion)
    {
        var galaxies = inputs.SelectMany((row, y) => row.Select((ch, x) => ch == '#' ? new Point(x + emptyColumns.Count(c => c < x) * expansion, y + emptyRows.Count(r => r < y) * expansion) : Space)).Where(g => g != Space).ToArray();

        var (dist, _) = galaxies.Aggregate((dist: 0L, index: 0), (cur, galaxy) =>
        {
            var totalDistances = cur.dist;

            totalDistances += galaxies.Skip(cur.index + 1).Select(other => Math.Abs(galaxy.x - other.x) + Math.Abs(galaxy.y - other.y)).Sum();

            return (totalDistances, cur.index + 1);
        });
        return dist;
    }
}