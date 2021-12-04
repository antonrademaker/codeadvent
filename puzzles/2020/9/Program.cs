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

            var input = File.ReadLines(filePath).Select(i => long.Parse(i)).ToList();
            int inputSize = input.Count;
            const int preamble = 25;

            long[] buffer = new long[preamble];

            var bufferPosition = 0;

            // Load preamble
            for (int i = 0; i < preamble; i++)
            {
                buffer[i] = input[i];
            }

            var searchFor = 0L;

            foreach (var number in input.Skip(preamble))
            {
                Console.WriteLine($"Working on {number}");
                var found = buffer.Any(p1 => buffer.Any(p2 => p1 + p2 == number));
                if (!found)
                {
                    searchFor = number;
                }

                buffer[bufferPosition] = number;
                bufferPosition = (bufferPosition + 1) % preamble;
            }

            Console.WriteLine($"not found: {searchFor}");

            for (var pos = 0; pos < inputSize; pos++)
            {
                long sum = 0;
                int pos2 = pos;

                while (sum < searchFor && input[pos2] < searchFor)
                {
                    sum += input[pos2++];
                }

                if (sum == searchFor)
                {
                    var endpos = pos2 - 1;

                    var range = input.GetRange(pos, endpos - pos);

                    Console.WriteLine($"Found it: {pos} {endpos}, {range.Min()}+{range.Max()}={range.Min() + range.Max()}");
                }
            }
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