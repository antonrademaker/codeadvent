using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        private static readonly Regex fieldRulesRegex = new Regex(@"^([a-z\s]+)\:\W(\d+)-(\d+)(\sor\s(\d+)-(\d+))?$", RegexOptions.Compiled);

        private static void Part1(string[] instructions)
        {
            var world = Parse(instructions);

            var invalidValues = new List<int>();

            Parallel.ForEach(world.OtherTickets, ticket =>
            {
                IsValidTicket(ticket, world.Fields, invalidValues);
            });

            Console.WriteLine($"Invalid values: {PrintIEnumerable(invalidValues)}");
            Console.WriteLine($"Invalid sum: {invalidValues.Sum()}");
        }

        private static string PrintIEnumerable(IEnumerable<int> input)
        {
            return input.Aggregate(new StringBuilder(), (sb, v) => { sb.Append(v); sb.Append(','); return sb; }).ToString();
        }

        private static void Part2(string[] instructions)
        {
            var world = Parse(instructions);

            var invalidValues = new List<int>();

            var validatedWorld = world with
            {
                OtherTickets = world.OtherTickets.Where(ticket => IsValidTicket(ticket, world.Fields, invalidValues)).ToArray()
            };

            var candidates = Enumerable.Range(0, validatedWorld.Fields.Length).ToArray();

            var candidateWorld = validatedWorld with
            {
                Fields = validatedWorld.Fields.Select(field => FillInitialCandidates(field, candidates)).ToArray()
            };
            var round = 0;
            while (!candidateWorld.Fields.All(field => field.Found))
            {
                var fieldsFound = new List<int>();

                var fields = candidateWorld.Fields.Select(
                        field => UpdateCandidates(field, candidateWorld.OtherTickets, fieldsFound)
                        ).ToArray();

                foreach (var fieldPositionFound in fieldsFound)
                {
                    fields = fields.Select(field => RemoveCandidates(field, fieldPositionFound)).ToArray();
                }

                candidateWorld = candidateWorld with
                {
                    Fields = fields
                };
                round++;
                Console.WriteLine($"Round: {round,5:#}:");
                Print(fields);

                if (round > 100)
                {
                    throw new Exception("Round to big");
                }
            }

            Console.WriteLine($"------ Found -------");
            Print(candidateWorld.Fields);

            Console.WriteLine($"------ Own ticket -------");

            var multiplied = candidateWorld.Fields.Where(t => t.Name.StartsWith("departure")).Select(field =>
            {
                var ownTicketValue = candidateWorld.OwnTicket.Values[field.Position ?? throw new Exception("This should not happen")];
                Console.WriteLine($"{field.Position}:{field.Name}= {ownTicketValue}");
                return ownTicketValue;
            }).Aggregate(1L, (acc, value) => acc * value);

            Console.WriteLine($"Answer: {multiplied}");
        }

        private static void Print(Field[] fields)
        {
            for (var pos = 0; pos < fields.Length; pos++)
            {
                Console.WriteLine($"{pos}: {fields[pos].Name,-10} found: {fields[pos].Found}@{fields[pos].Position}\t|| {PrintIEnumerable(fields[pos].Candidates)}");
            }
        }

        private static Field RemoveCandidates(Field field, int fieldPositionFound)
        {
            if (field.Found)
            {
                return field;
            }

            return field with
            {
                Candidates = field.Candidates.Where(t => t != fieldPositionFound).ToArray()
            };
        }

        private static Field UpdateCandidates(Field field, Ticket[] tickets, List<int> fieldsFound)
        {
            if (field.Found)
            {
                return field;
            }
            var candidates = new List<int>();

            foreach (var index in field.Candidates)
            {
                var values = tickets.Select(t => t.Values[index]);

                if (values.All(value => field.Rules.Any(rule => rule.Lower <= value && value <= rule.Upper)))
                {
                    candidates.Add(index);
                }
            }

            if (candidates.Count == 1)
            {
                var position = candidates.First();
                var newField = field with
                {
                    Found = true,
                    Position = position,
                    Candidates = candidates.ToArray()
                };

                fieldsFound.Add(position);

                return newField;
            }

            return field with { Candidates = candidates.ToArray() };
        }

        private static Field FillInitialCandidates(Field field, int[] candidates)
        {
            return field with { Candidates = candidates };
        }

        private static World Parse(string[] instructions)
        {
            var fields = new List<Field>();
            var linePos = 0;
            while (linePos < instructions.Length)
            {
                var match = fieldRulesRegex.Match(instructions[linePos]);
                if (!match.Success)
                {
                    linePos += 2;
                    break;
                }
                var validators = new List<Validator>
                {
                    new Validator (int.Parse(match.Groups[2].Value),int.Parse(match.Groups[3].Value)),
                    new Validator (int.Parse(match.Groups[5].Value),int.Parse(match.Groups[6].Value)),
                };

                fields.Add(new Field(match.Groups[1].Value) { Rules = validators });

                linePos++;
            }
            var ownTicket = ParseTicket(instructions[linePos]);
            linePos += 3;

            var otherTickets = new List<Ticket>();
            while (linePos < instructions.Length)
            {
                otherTickets.Add(ParseTicket(instructions[linePos]));
                linePos++;
            }

            return new World(fields.ToArray(), ownTicket, otherTickets.ToArray());
        }

        private static bool IsValidTicket(Ticket ticket, Field[] fields, List<int> invalidValues)
        {
            var ticketHasValidValues = true;
            foreach (var value in ticket.Values)
            {
                if (!fields.Any(field => field.Rules.Any(rule => rule.Lower <= value && value <= rule.Upper)))
                {
                    invalidValues.Add(value);
                    ticketHasValidValues = false;
                }
            }

            return ticketHasValidValues;
        }

        private static Ticket ParseTicket(string ticketLine)
        {
            var values = ticketLine.Split(",").Select(x => int.Parse(x)).ToArray();

            return new Ticket(values);
        }

        public record World(Field[] Fields, Ticket OwnTicket, Ticket[] OtherTickets);

        public record Ticket(int[] Values)
        {
            public bool? IsValid { get; init; }
            public int[] InvalidValues { get; init; } = Array.Empty<int>();
        }

        public record Field(string Name)
        {
            public List<Validator> Rules { get; init; } = new();
            public int[] Candidates { get; init; } = Array.Empty<int>();
            public bool Found { get; init; }
            public int? Position { get; init; }
        }

        public record Validator(int Lower, int Upper);

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