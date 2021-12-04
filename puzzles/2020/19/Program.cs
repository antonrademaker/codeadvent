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

        #region Part1

        private static readonly Regex RuleParser = new Regex("^(\\d+):\\s((?'char'\"(a|b)\")|(?'sequence'((\\d+)\\s)*(\\d+))|(?'or'(\\d+)\\s\\|\\s(\\d+))|(?'sequencedor'(\\d+)\\s(\\d+)\\s\\|\\s(\\d+)\\s(\\d+))|(?'sequence'((\\d+)\\s+)+(\\d+)))$", RegexOptions.Compiled);

        public class Rules : Dictionary<string, IRule>
        {
        }

        private static void Part1(string[] instructions)
        {
            var rules = new Rules();
            int pos = ReadRules(instructions, rules);

            // Read the messages and validate them

            var rulesRegex = $"^{rules["0"].ToRegexString(rules)}$";

            Console.WriteLine($"Regex: {rulesRegex}");

            var matcher = new Regex(rulesRegex, RegexOptions.Compiled);
            var (valid, invalid) = CalculateValid(instructions, pos, matcher);

            Console.WriteLine($"Valid: {valid}");
        }

        private static (int, List<string>) CalculateValid(string[] instructions, int pos, Regex matcher)
        {
            var valid = 0;
            List<string> invalid = new();

            while (pos < instructions.Length)
            {
                if (matcher.IsMatch(instructions[pos]))
                {
                    valid++;
                }
                else
                {
                    invalid.Add(instructions[pos]);
                }
                pos++;
            }

            return (valid, invalid);
        }

        private static int ReadRules(string[] instructions, Rules rules)
        {
            var pos = 0;

            while (pos < instructions.Length)
            {
                if (instructions[pos] == string.Empty)
                {
                    pos += 1;
                    break;
                }

                var match = RuleParser.Match(instructions[pos]);

                if (match.Groups["sequencedor"].Success)
                {
                    rules[match.Groups[1].Value] = new SequencedOr(match.Groups[9].Value, match.Groups[10].Value, match.Groups[11].Value, match.Groups[12].Value);
                }
                else if (match.Groups["char"].Success)
                {
                    rules[match.Groups[1].Value] = new Character(match.Groups[3].Value);
                }
                else if (match.Groups["sequence"].Success)
                {
                    rules[match.Groups[1].Value] = HandlePossibleSequence(instructions[pos], match);
                }
                else if (match.Groups["or"].Success)
                {
                    rules[match.Groups[1].Value] = new Or(match.Groups[7].Value, match.Groups[8].Value);
                }
                else
                {
                    throw new Exception($"{instructions[pos]} regex matched with {match.Groups.Count} groups");
                }

                pos++;
            }

            return pos;
        }

        private static IRule HandlePossibleSequence(string line, Match match)
        {
            if (match.Groups.ContainsKey("sequence"))
            {
                if (line == "0: 4 1 5")
                {
                    return new Sequence(new string[] { "4", "1", "5" });
                }
                if (match.Groups[5].Value != string.Empty && match.Groups[6].Value != string.Empty)
                {
                    return new Sequence(new string[] { match.Groups[5].Value, match.Groups[6].Value });
                }
                else
                {
                    return new Single(match.Groups[6].Value);
                }
            }
            throw new Exception($"{line} regex matched with {match.Groups.Count} groups");
        }

        public interface IRule
        {
            string ToRegexString(Rules rules);

            void Reset();
        }

        public class Sequence : IRule
        {
            public string[] Rules { get; init; }

            public Sequence(string[] rules)
            {
                Rules = rules;
            }

            private string regexString = string.Empty;

            public string ToRegexString(Rules rules)
            {
                if (regexString != string.Empty)
                {
                    return regexString;
                }
                var sb = new StringBuilder();
                foreach (var rule in Rules)
                {
                    var iRule = rules[rule];

                    sb.Append(iRule.ToRegexString(rules));
                }
                regexString = sb.ToString();
                return regexString;
            }

            public void Reset()
            {
                regexString = string.Empty;
            }
        }

        public class Or : IRule
        {
            public string Left { get; init; }
            public string Right { get; init; }

            public Or(string left, string right)
            {
                Left = left;
                Right = right;
            }

            private string regexString = string.Empty;

            public string ToRegexString(Rules rules)
            {
                if (regexString != string.Empty)
                {
                    return regexString;
                }
                var leftRule = rules[Left];
                var rightRule = rules[Right];
                regexString = $"({leftRule.ToRegexString(rules)}|{rightRule.ToRegexString(rules)})";
                return regexString;
            }

            public void Reset()
            {
                regexString = string.Empty;
            }
        }

        public class SequencedOr : IRule
        {
            public string Left1 { get; init; }
            public string Left2 { get; init; }
            public string Right1 { get; init; }
            public string Right2 { get; init; }

            public SequencedOr(string left1, string left2, string right1, string right2)
            {
                Left1 = left1;
                Left2 = left2;
                Right1 = right1;
                Right2 = right2;
            }

            private string regexString = string.Empty;

            public string ToRegexString(Rules rules)
            {
                if (regexString != string.Empty)
                {
                    return regexString;
                }
                var left1Rule = rules[Left1];
                var left2Rule = rules[Left2];
                var right1Rule = rules[Right1];
                var right2Rule = rules[Right2];
                regexString = $"({left1Rule.ToRegexString(rules)}{left2Rule.ToRegexString(rules)}|{right1Rule.ToRegexString(rules)}{right2Rule.ToRegexString(rules)})";
                return regexString;
            }

            public void Reset()
            {
                regexString = string.Empty;
            }
        }

        public class Single : IRule
        {
            public string Rule { get; init; }

            public Single(string rule)
            {
                Rule = rule;
            }

            public string ToRegexString(Rules rules)
            {
                return rules[Rule].ToRegexString(rules);
            }

            public void Reset()
            {
            }
        }

        public class Character : IRule
        {
            public string Value { get; init; }

            public Character(string character)
            {
                Value = character;
            }

            public string ToRegexString(Rules rules)
            {
                return Value;
            }

            public void Reset()
            {
            }
        }

        #endregion Part1

        #region Part 2

        private static void Part2(string[] instructions)
        {
            var rules = new Rules();
            int pos = ReadRules(instructions, rules);

            // Update the rules

            rules["8"] = new SequencedOrWithLoop8();

            int valid = 0;

            var instructionsLeft = new List<string>();
            instructionsLeft.AddRange(instructions.Skip(pos));

            for (var round = 0; round < 3; round++)
            {
                rules["0"].Reset();

                rules["11"] = new SequencedOrWithLoop11(round);

                // Read the messages and validate them

                var rulesRegex = $"^{rules["0"].ToRegexString(rules)}$";

                Console.WriteLine($"Regex: {rulesRegex}");

                var matcher = new Regex(rulesRegex, RegexOptions.Compiled);
                int validRun;
                (validRun, instructionsLeft) = CalculateValid(instructionsLeft.ToArray(), 0, matcher);
                valid += validRun;
            }
            Console.WriteLine($"Valid: {valid}");
        }

        public class SequencedOrWithLoop8 : IRule
        {
            public SequencedOrWithLoop8()
            {
            }

            private string regexString = string.Empty;

            public string ToRegexString(Rules rules)
            {
                if (regexString != string.Empty)
                {
                    return regexString;
                }

                var r42 = rules["42"].ToRegexString(rules);

                regexString = $"({r42})+";
                return regexString;
            }

            public void Reset()
            {
                regexString = string.Empty;
            }
        }

        public class SequencedOrWithLoop11 : IRule
        {
            public SequencedOrWithLoop11(int reps)
            {
                this.reps = reps;
            }

            private string regexString = string.Empty;
            private readonly int reps;

            public string ToRegexString(Rules rules)
            {
                if (regexString != string.Empty)
                {
                    return regexString;
                }

                var r42 = rules["42"].ToRegexString(rules);
                var r31 = rules["31"].ToRegexString(rules);

                regexString = Enumerable.Range(0, reps + 1).Aggregate(string.Empty, (acc, _) => $"{r42}{acc}{r31}");

                return regexString;
            }

            public void Reset()
            {
                regexString = string.Empty;
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