internal sealed class Program
{
    private static void Main(string[] args)
    {
        var runDebug = true;

        var files = Directory.GetFiles("input", "*.txt");

        foreach (var file in files.OrderBy(t => t))
        {
            Console.WriteLine($"{file}");

            var lines = File.ReadAllLines(file);
            long part1 = CalculatePart(lines, 1, 1);
            Console.WriteLine($"Part 1: {part1}");

            long part2 = CalculatePart(lines, 811589153, 10);
            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static long CalculatePart(string[] lines, long key, int numberOfMixes)
    {
        var data = GetData(lines, key);

        var result = GetDecrypted(data, numberOfMixes);

        var zeroPosition = result.FindIndex(r => r.Value == 0);

        var resultLength = result.Count;

        var part1 = new int[] { 1000, 2000, 3000 }.Sum(pos => result[(zeroPosition + pos) % resultLength].Value);
        return part1;
    }

    private static List<(int Index, long Value)> GetData(string[] lines, long key)
    {
        return lines.Select((value, index) => (index, long.Parse(value) * key)).ToList();
    }

    private static List<(int Index, long Value)> GetDecrypted(List<(int, long)> data, int numberOfMixes)
    {
        List<(int Index, long Value)> result = new(data);

        var dataLength = data.Count - 1;

        for (var i = 0; i < numberOfMixes; i++)
        {
            foreach (var (index, value) in data)
            {
                var currentIndex = result.IndexOf((index, value));
                var nextIndex = (int)((currentIndex + value) % dataLength);

                while (nextIndex < 0)
                {
                    nextIndex += dataLength;
                }

                result.RemoveAt(currentIndex);
                result.Insert(nextIndex, (index, value));
            }
        }

        return result;
    }

    private static List<(int, long)> ParseData(int[] positions)
    {
        return positions.Select(t => (t, (long)t)).ToList();
    }
}