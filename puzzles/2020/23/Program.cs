using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace _11
{
    internal class Program
    {
        private static void Main()
        {
            string filePath = GetInputFilePath();

            var instructions = File.ReadAllLines(filePath);

            foreach (var line in instructions)
            {
                Measure(() => Part1(line));
            }
            foreach (var line in instructions)
            {
                Measure(() => Part2(line));
            }
        }

        #region Part 1

        private static void Part1(string initial)
        {
            var (cup, total, lookup) = ParseInput(initial);

            cup = Calculate(cup, lookup, total, 100, true);

            var startCup = cup;

            while (startCup.Label != 1)
            {
                startCup = startCup.Next;
            }
            startCup = startCup.Next;

            do
            {
                Console.Write(startCup.Label);
                startCup = startCup.Next;
            } while (startCup.Label != 1);
            Console.WriteLine();

        }

        private static Cup Calculate(Cup cup, Cup[] lookup, int total, int rounds, bool showLog)
        {
            

            for (var round = 1; round <= rounds; round++)
            {
                if (showLog)
                {
                    Console.WriteLine($"-- move {round} --");
                    Console.WriteLine($"cups:{Print(cup, cup)}");
                } else if (round % 10000 == 0)
                {
                    Console.Write($"\rMove {round}");
                }

                var (pickup, pickupEnd) = Pickup(cup, 3);
                if (showLog)
                {
                    Console.WriteLine($"Pick up:{Print(pickup)}");
                }

                var destinationLabel = cup.Label - 1;
                if (destinationLabel < 1)
                {
                    destinationLabel = total;
                }

                while (pickup.Contains(destinationLabel))
                {
                    destinationLabel--;
                    if (destinationLabel < 1)
                    {
                        if (showLog)
                        {
                            Console.WriteLine($"total: {total}");
                        }
                        destinationLabel = total;
                    }
                }
                if (showLog)
                {
                    Console.WriteLine($"destination: {destinationLabel}");
                }

                var destinationCup = lookup[destinationLabel];

                

                Insert(destinationCup, pickup, pickupEnd);

                cup = cup.Next;
                if (showLog)
                {
                    Console.WriteLine();
                }
            }
            if (showLog)
            {
                Console.WriteLine($"Final :{Print(cup, cup)}");
            }
            return cup;
        }

        private static void Insert(Cup destination, Cup toInsert, Cup lastInsert)
        {
            var afterPickup = destination.Next;
            destination.Next = toInsert;
            toInsert.Previous = destination;
            lastInsert.Next = afterPickup;
            afterPickup.Previous = afterPickup;
        }

        private static (Cup, Cup) Pickup(Cup cup, int count)
        {
            var firstToPick = cup.Next;

            var lastToPick = cup.Next;

            for (int p = 1; p < count; p++)
            {
                lastToPick = lastToPick.Next;
            }

            cup.Next = lastToPick.Next;
            lastToPick.Next = firstToPick;
            firstToPick.Previous = lastToPick;

            return (firstToPick, lastToPick);

        }

        public static string Print(Cup cup, Cup? current = null)
        {
            var sb = new StringBuilder();

            var c = cup;

            do
            {
                if (current == c)
                {
                    sb.Append($" ({current.Label})");
                }
                else
                {
                    sb.Append($" {c.Label}");
                }

                c = c.Next;
            } while (c != cup);

            return sb.ToString();
        }

        public static (Cup current, int total, Cup[] lookup) ParseInput(string input, int requestedTotal = 0)
        {
            var labels = input.Select(t => int.Parse(t.ToString())).ToArray();

            var total = Math.Max(requestedTotal, labels.Length);
            var lookup = new Cup[total + 1];


            var current = new Cup(labels[0]);
            lookup[current.Label] = current;

            var first = current;

            var emptyCup = new Cup(-1);

            for (var i = 1; i < labels.Length; i++)
            {
                var newCup = new Cup(labels[i]) { Previous = current };
                lookup[newCup.Label] = newCup;
                current.Next = newCup;
                current = newCup;
            }

            for (var i = labels.Length + 1; i <= requestedTotal; i++)
            {
                var newCup = new Cup(i) { Previous = current };
                lookup[i] = newCup;
                current.Next = newCup;
                current = newCup;
            }

            current.Next = first;
            first.Previous = current;

            return (first, total, lookup);
        }
        [DebuggerDisplay("{Label}")]
        public class Cup : IEnumerable<Cup>
        {

            private static readonly Cup EmptyCup = new Cup(-1);
            public Cup(int label)
            {
                Label = label;
            }

            public Cup Next { get; set; } = EmptyCup;
            public Cup Previous { get; set; } = EmptyCup;
            public int Label { get; }

            public bool Contains(int label)
            {
                if (Label == label)
                {
                    return true;
                }

                var current = Next;

                while (current != this)
                {
                    if (current.Label == label)
                    {
                        return true;
                    }
                    current = current.Next;
                }
                return false;
            }

            private IEnumerable<Cup> GetAllCups()
            {
                var current = this;

                do
                {
                    yield return current;
                    current = current.Next;
                } while (current != this);
            }

            public IEnumerator<Cup> GetEnumerator()
            {
                return GetAllCups().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion Part 1

        #region Part 2


        private static void Part2(string initial)
        {
            var (cup, total, lookup) = ParseInput(initial, 1000000);
            cup = Calculate(cup, lookup, total, 10000000, false);

            var answerCup = lookup[1].Next;

            var answer = 1L;
            Console.WriteLine();

            for (var i = 0; i < 2; i++)
            {
                Console.WriteLine($"Label: {answerCup.Label}");
                answer *= answerCup.Label;
                answerCup = answerCup.Next;
            }
            Console.WriteLine($"Answer: {answer}");


            Console.WriteLine();
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