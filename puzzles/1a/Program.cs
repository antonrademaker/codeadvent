using System;
using System.Collections.Generic;
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


            var firstIndex = 0;

            var lowerIndex = 1;
            var upperIndex = input.Count - 1;

            var firstValue = -1;
            var secondValue = -1;
            var thirdValue = -1;


            while (secondValue < 0)
            {
                firstValue = input[lowerIndex];

                (secondValue, thirdValue) = Find(input, lookup-firstValue,  lowerIndex++, upperIndex);
            }

            int lowest = 0;
            int biggest = 0;

            Console.WriteLine($"Found: {firstValue} {secondValue} {thirdValue}, answer: {firstValue * secondValue * thirdValue}");
        }

        public static (int,int) Find(List<int> input, int lookUpValue, int lowerIndex, int upperIndex)
        {

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

                    if (lowest + biggest == lookUpValue)
                    {
                        return (lowest, biggest);
                    }
                    upIndex--;
                }
                lowerIndex++;
            }

            return (-1, -1);
        }
    }
}
