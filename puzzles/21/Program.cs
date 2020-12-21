using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _11
{
    internal class Program
    {
        private static void Main()
        {
            string filePath = GetInputFilePath();

            var instructions = File.ReadAllLines(filePath);

            Measure(() => Part1(instructions));
            Measure(() => Part2(instructions));
        }


        private static (Dictionary<string, int> allergenCount, Dictionary<string, HashSet<string>>) ParseInput(string[] input)
        {
            Dictionary<string, int> ingredientCount = new();
            Dictionary<string, HashSet<string>> potentialAllergen = new();

            foreach (var row in input)
            {
                var split = row.Split(" (contains ");
                var ingredients = split[0].Split(" ");
                var allergens = split[1].Replace(")", string.Empty).Split(", ");

                foreach (var ingredient in ingredients)
                {
                    if (ingredientCount.ContainsKey(ingredient))
                    {
                        ingredientCount[ingredient] += 1;
                    }
                    else
                    {
                        ingredientCount[ingredient] = 1;
                    }
                }

                foreach (var allergen in allergens)
                {
                    if (potentialAllergen.ContainsKey(allergen))
                    {
                        potentialAllergen[allergen].IntersectWith(ingredients);
                    }
                    else
                    {
                        potentialAllergen[allergen] = new(ingredients);
                    }
                }
            }

            return (ingredientCount, potentialAllergen);
        }


        #region Part1
        private static void Part1(string[] instructions)
        {
            var (ingredientsCount, potentialAllergen) = ParseInput(instructions);

            var allergens = potentialAllergen.Values.SelectMany(x => x).ToHashSet();
            var answer = ingredientsCount
                .Where(kvp => !allergens.Contains(kvp.Key))
                .Sum(kvp => kvp.Value);

            Console.WriteLine($"Answer: {answer}");
        }


        #endregion Part1

        #region Part 2
        private static void Part2(string[] instructions)
        {
            var (_, potentialAllergen) = ParseInput(instructions);

            while (potentialAllergen.Values.Any(x => x.Count != 1))
            {
                foreach (var allergen in potentialAllergen.Where(kvp => kvp.Value.Count == 1))
                {
                    foreach (var (key, value) in potentialAllergen.Where(kvp => kvp.Key != allergen.Key))
                    {
                        potentialAllergen[key] = value.Where(x => x != allergen.Value.Single()).ToHashSet();
                    }
                }
            }

            Console.WriteLine($"Answer 2: {string.Join(",", potentialAllergen.OrderBy(x => x.Key).Select(x => x.Value.Single()))}");
        }
        #endregion Part 2

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