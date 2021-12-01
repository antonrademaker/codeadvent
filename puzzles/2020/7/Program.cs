using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _7
{
    internal class Program
    {
        private static Regex bagContentsRegex = new Regex(@"^(\d+)\W(.+)$", RegexOptions.Compiled);
        public static Dictionary<string, Bag> bags = new();

        private static void Main(string[] args)
        {
            string filePath = GetInputFilePath();

            var input = File.ReadLines(filePath);

            foreach (var bagLine in input)
            {
                var props = bagLine.Replace(".", "").Replace(",", "").Replace("contain", "").Replace("bags", "bag").Split("bag");

                var bagColor = props[0].Trim();

                Bag bag = GetBag(bagColor);

                foreach (var contents in props.Skip(1).Select(l => l.Trim()))
                {
                    if (string.IsNullOrWhiteSpace(contents) || contents == "no other")
                    {
                        continue;
                    }

                    var match = bagContentsRegex.Match(contents.Trim());

                    var amount = int.Parse(match.Groups[1].Value);
                    var bagName = match.Groups[2].Value.Trim();

                    Bag containedBag = GetBag(bagName);
                    bag.Contents.Add(containedBag, amount);
                }
                bags[bag.Name] = bag;
                // bags.Add(bag.Name, bag);
            }

            foreach (var bag in bags)
            {
                foreach (var containedBag in bag.Value.Contents)
                {
                    bags[containedBag.Key.Name].CanBeContainedBy.Add(bag.Value);
                }
            }

            const string searchFor = "shiny gold";

            var currentBag = bags[searchFor];
            var searchBags = new List<Bag>();
            LookFor(currentBag, searchFor, searchBags);

            Console.WriteLine($"Found: {searchBags.Distinct().Count()}");

            var goldBag = bags[searchFor];

            Console.WriteLine($"{searchFor} contains: {goldBag.NumberOfBagsContained}");
        }

        private static Bag GetBag(string bagColor)
        {
            if (bags.ContainsKey(bagColor))
            {
                return bags[bagColor];
            }
            else
            {
                var newBag = new Bag(bagColor);
                bags.Add(bagColor, newBag);
                return newBag;
            }
        }

        private static void LookFor(Bag current, string searchFor, List<Bag> bags)
        {
            Console.WriteLine($"current: {current.Name}");

            foreach (var parent in current.CanBeContainedBy)
            {
                Console.WriteLine($"parent: {parent.Name}");
                bags.Add(parent);
                LookFor(parent, searchFor, bags);
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

        public class Bag
        {
            public Bag(string name)
            {
                Name = name;
            }

            public string Name { get; init; }
            public Dictionary<Bag, int> Contents { get; init; } = new();

            public List<Bag> CanBeContainedBy { get; init; } = new();

            public bool CanContainShinyGold { get; set; }

            private int? numberOfBagsContained;

            public int NumberOfBagsContained
            {
                get
                {
                    if (numberOfBagsContained.HasValue)
                    {
                        return numberOfBagsContained.Value;
                    }

                    numberOfBagsContained = Contents.Aggregate(0, (cur, kv) =>
                    {
                        return cur + (1 + kv.Key.NumberOfBagsContained) * kv.Value;
                    });
                    Console.WriteLine($"{Name} contains {numberOfBagsContained} bags ({Contents.Count} rules)");
                    return numberOfBagsContained.Value;
                }
            }
        }
    }
}