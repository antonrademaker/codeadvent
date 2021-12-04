using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _11
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = GetInputFilePath();

            var instructions = File.ReadAllLines(filePath);

            Measure(() => Part1(instructions));
            Measure(() => Part2(instructions));
        }

        private static void Part1(string[] instructions)
        {
            Console.WriteLine($"At 2020: {GetSpokenNumberAt(instructions, 2020)}");
        }

        public static int GetSpokenNumberAt(string[] instructions, int atRound)
        {
            var numbers = instructions[0].Split(',').Select(x => int.Parse(x)).ToArray();

            var countCache = new Dictionary<int, int>();

            for (var round = 0; round < numbers.Length; round++)
            {
                countCache.Add(numbers[round], round + 1);
            }

            var lastNumber = 0;
            for (var round = numbers.Length + 1; round < atRound; round++)
            {
                lastNumber = Handle(countCache, round, lastNumber);
            }
            return lastNumber;
        }

        private static int Handle(Dictionary<int, int> cache, int positionNumber, int lastNumber)
        {
            if (cache.TryGetValue(lastNumber, out var value))
            {
                cache[lastNumber] = positionNumber;
                return positionNumber - value;
            }
            else
            {
                cache[lastNumber] = positionNumber;
                return 0;
            }
        }

        private static void Part2(string[] instructions)
        {
            Console.WriteLine($"At 30000000: {GetSpokenNumberAt(instructions, 30000000)}");
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

        public static void Measure(Action action)
        {
            var sw = new Stopwatch();
            Console.WriteLine($"Start");
            sw.Start();
            action();
            sw.Stop();
            Console.WriteLine($"Ended in {sw.ElapsedMilliseconds}ms");
        }
    }
}