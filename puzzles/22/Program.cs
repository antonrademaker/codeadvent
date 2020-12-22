using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _11
{
    internal class Program
    {
        private static void Main()
        {
            string filePath = GetInputFilePath();

            var instructions = File.ReadAllLines(filePath);

            //  Measure(() => Part1(instructions));
            Measure(() => Part2(instructions));
        }

        private static readonly bool isDebug = false;

        #region Part 1

        private static void Part1(string[] instructions)
        {
            var decks = ParseInput(instructions);

            var round = 1;
            var winner = 0;

            while (decks.All(d => d.Any()))
            {
                foreach (var deck in decks)
                {
                    Console.WriteLine(deck.PrintToConsole());
                }
                var c1 = decks[0].Dequeue();
                var c2 = decks[1].Dequeue();
                winner = c1 > c2 ? 0 : 1;
                decks[winner].Enqueue(c1 > c2 ? c1 : c2);
                decks[winner].Enqueue(c1 > c2 ? c2 : c1);
                Console.WriteLine($"P1 plays {c1}, p2 plays {c2}");
                Console.WriteLine($"Round: {round}, Winner: {winner + 1}");
                round++;
            }

            if (decks[0].Any())
            {
                Console.WriteLine("Game won by player 1");
            }
            else
            {
                Console.WriteLine("Game won by player 2");
            }

            var winnerDeck = decks.Single(d => d.Any());
            winnerDeck.CalculatePoints(true);
        }

        private static Deck[] ParseInput(string[] instructions)
        {
            var decks = ReadDeck(instructions.Skip(1)).ToArray();

            return decks;
        }

        private static IEnumerable<Deck> ReadDeck(IEnumerable<string> input)
        {
            var player = 1;
            var deck = new Deck() { Player = player++ };
            foreach (var line in input)
            {
                if (line == string.Empty)
                {
                    yield return deck;
                    deck = new Deck() { Player = player++ };
                    continue;
                }
                if (line.StartsWith("P"))
                {
                    continue;
                }
                deck.Enqueue(int.Parse(line));
            }

            yield return deck;
        }

        public class Deck : Queue<int>
        {

            public Deck()
            {

            }

            public Deck(IEnumerable<int> items) : base(items)
            {

            }

            public int Player { get; init; }
            public string PrintToConsole()
            {
                return string.Join(", ", this);
            }

            public Deck Copy(int take)
            {
                var newDeck = new Deck(this.Take(take)) { Player = Player };

                return newDeck;
            }

            public override int GetHashCode()
            {
                return Player * 4987 * CalculateSequenceHash();
            }


            private int CalculateSequenceHash()
            {

                return this.Aggregate(4987, (acc, cur) => (acc * 113) + cur);

            }

            public int CalculatePoints(bool log = false)
            {
                var score = Enumerable.Range(0, Count).Select(p => this.ElementAt(Count - p - 1) * (p + 1)).Sum();

                if (log)
                {
                    Console.WriteLine($"Score: {score}");
                }
                return score;
            }

        }

        #endregion Part 1

        #region Part 2


        private static void Part2(string[] instructions)
        {
            var decks = ParseInput(instructions);

            var player1Winner = CalculateGame(decks[0],decks[1]);

            var winnerDeck = decks[player1Winner ? 0 : 1];
            winnerDeck.CalculatePoints(true);
        }

        private static bool CalculateGame(Deck deck1, Deck deck2)
        {
            HashSet<int> encountered = new();
            while (deck1.Any() && deck2.Any())
            {
                CalculateRound(deck1, deck2, encountered);
            }

            return deck1.Any();
        }

        private static void CalculateRound(Deck deck1, Deck deck2, HashSet<int> encountered)
        {
            var id = deck1.GetHashCode() * deck2.GetHashCode();

            var player1Wins = encountered.Contains(id);
            encountered.Add(id);

            if (player1Wins)
            {
                deck1.Clear();
                return;
            }

            var cardA = deck1.Dequeue();
            var cardB = deck2.Dequeue();
 
            if (cardA <= deck1.Count && cardB <= deck2.Count)
            {
                var deck1New = deck1.Copy(cardA);
                var deck2New = deck1.Copy(cardB);

                player1Wins = CalculateGame(deck1New, deck2New);
            }
            else
            {
                player1Wins = cardA > cardB;
            }

            if (player1Wins)
            {
                deck1.Enqueue(cardA);
                deck1.Enqueue(cardB);
            } else
            {
                deck2.Enqueue(cardB);
                deck2.Enqueue(cardA);
            }
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