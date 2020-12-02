using System;
using System.IO;
using System.Linq;

namespace _1
{
    class Program
    {
        const int lookup = 2020;
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory();


            var input = File.ReadLines(path + "\\input.txt").Select(x => int.Parse(x)).OrderBy(x => x).ToList();
            Console.WriteLine("The size is {0}", input.Count);

            var lowerIndex = 0;
            var upperIndex = input.Count - 1;

            var lowest = input[lowerIndex];
            var biggest = input[upperIndex];

            Console.WriteLine($"Lowest: {lowest}, biggest: {biggest}");

            while (lowerIndex < upperIndex)
            {

                lowest = input[lowerIndex];

                var upIndex = upperIndex;
                while (upIndex > lowerIndex)
                {
                    biggest = input[upIndex];

                    Console.WriteLine($"Lowest: {lowest}, biggest: {biggest}, sum: {lowest + biggest}");

                    if (lowest + biggest == lookup)
                    {
                        break;
                    }
                    upIndex--;
                }
                if (lowest + biggest == lookup)
                {
                    break;
                }
                lowerIndex++;
            }
            Console.WriteLine($"Found: {lowest} {biggest}, answer: {lowest*biggest}");
        }
    }
}
