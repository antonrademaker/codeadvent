using System;
using System.IO;
using System.Linq;

namespace _5
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = GetInputFilePath();

            var lines = File.ReadLines(filePath);
            Console.WriteLine($"{lines.Count()} lines read");
            var passes = lines.Select(line => ParseBoardingpass(line)).OrderBy(pass => pass.seatId).ToList();
            Console.WriteLine($"{passes.Count()} passes parsed");

            var highestSeatId = passes.Aggregate(int.MinValue, (current, pass) => Math.Max(current, pass.seatId));
            var lowestSeatId = passes.Aggregate(int.MaxValue, (current, pass) => Math.Min(current, pass.seatId));

            Console.WriteLine($"highestSeatId : {highestSeatId}");

            for (int pos = 0; pos < highestSeatId - 1; pos++)
            {
                Console.WriteLine($"{pos}: SeatID : {passes[pos].seatId}");

                if (passes[pos].seatId != pos + lowestSeatId)
                {
                    Console.WriteLine($"Mising seat: {pos + lowestSeatId}");
                    break;
                }
            }
        }

        private static Boardingpass ParseBoardingpass(string input)
        {
            var row = Parse(input, 0, 7);
            var column = Parse(input, 7, 3);

            return new Boardingpass(row, column);
        }

        private static int Parse(string input, int posOffset, int positionsToRead)
        {
            var lbound = 0;
            var ubound = (int)Math.Pow(2, positionsToRead) - 1;
            int halvingSize;

            for (int pos = 0; pos < positionsToRead; pos++)
            {
                halvingSize = (ubound + 1 - lbound) / 2;

                switch (input[pos + posOffset])
                {
                    case 'F':
                        ubound -= halvingSize;
                        break;

                    case 'B':
                        lbound += halvingSize;
                        break;

                    case 'L':
                        ubound -= halvingSize;
                        break;

                    case 'R':
                        lbound += halvingSize;
                        break;
                }
            }

            return lbound;
        }

        public record Boardingpass(int row, int column)
        {
            public int seatId => row * 8 + column;
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