using System;
using System.IO;
using System.Linq;

namespace _9
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = GetInputFilePath();

            var adapters = File.ReadLines(filePath).Select(i => int.Parse(i)).OrderBy(t => t).ToList();

            adapters.Add(adapters[^1] + 3);

            var builtinAdapter = adapters[^1] + 1;

            var previousJolt = 0;

            int[] differences = new int[builtinAdapter];

            for (var pos = 0; pos < adapters.Count; pos++)
            {
                var adapter = adapters[pos];
                differences[adapter] = adapter - previousJolt;

                previousJolt = adapter;
            }

            var oneJolts = differences.Count(x => x == 1);
            var threeJolts = differences.Count(x => x == 3);

            Console.WriteLine($"Answer: {oneJolts} * {threeJolts} = {oneJolts * threeJolts}");

            differences[0] = 1;

            var permutations = new long[builtinAdapter].ToList();
            permutations[^1] = 1;
            for (var pos = builtinAdapter - 4; pos >= 0; pos--)
            {
                permutations[pos] = differences[pos] > 0 ? permutations[pos + 3] + permutations[pos + 2] + permutations[pos + 1] : 0;
            }

            Console.WriteLine($"permutations: {permutations[0]}");
        }

        private static string GetInputFilePath()
        {
            string path = Directory.GetCurrentDirectory();
            var filePath = path + "\\input.txt";
            while (!File.Exists(filePath))
            {
                var dirInfo = Directory.GetParent(path);

                if (dirInfo?.Exists ?? false)
                {
                    path = dirInfo.FullName;
                    filePath = path + "\\input.txt";
                }
                else
                {
                    throw new Exception($"Path not found {path}");
                }
            }

            return filePath;
        }
    }
}