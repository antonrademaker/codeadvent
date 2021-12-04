using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

        private static void Part1(string[] instructions)
        {
            var sum = 0L;

            foreach (var line in instructions)
            {
                var lineAnswer = Calculate(line);
                Console.WriteLine($"{line} = {lineAnswer}");
                sum += lineAnswer;
            }

            Console.WriteLine($"answer = {sum}");
        }

        private static long Calculate(string line)
        {
            var baseCalculation = new Group();

            Tokenize(line, 0, baseCalculation);

            // var (pos, answer) = Calculate(line, 0);
            var (pos, answer) = baseCalculation.Calculate(0, 0, baseCalculation.Calculateables);
            return answer;
        }

        private static int Tokenize(string line, int pos, Group parent)
        {
            while (pos < line.Length)
            {
                switch (line[pos])
                {
                    case ' ':
                        break;
                    case '(':
                        var newGroup = new Group();
                        pos = Tokenize(line, pos + 1, newGroup);
                        parent.Calculateables.Add(newGroup);
                        break;

                    case ')':
                        return pos;

                    case '+':
                        parent.Calculateables.Add(new PlusOperation());
                        break;

                    case '-':
                        parent.Calculateables.Add(new MinusOperation());
                        break;

                    case '*':
                        parent.Calculateables.Add(new MultiplyOperation());
                        break;

                    case '/':
                        parent.Calculateables.Add(new DivideOperation());
                        break;

                    case char n when (n >= '0' && n <= '9'):
                        Number? number;
                        (pos, number) = ParseNumber(line, pos);
                        parent.Calculateables.Add(number);
                        break;
                }

                pos++;
            }

            return pos;
        }

        private static (int, Number) ParseNumber(string line, int pos)
        {
            var digits = 0;

            var startPos = pos;

            while (pos < line.Length)
            {
                switch (line[pos])
                {
                    case char n when (n >= '0' && n <= '9'):
                        digits++;
                        break;

                    default:
                        return (pos - 1, new Number { Value = long.Parse(line.Substring(startPos, digits)) });
                }
                pos++;
            }
            return (pos - 1, new Number { Value = long.Parse(line.Substring(startPos, digits)) });
        }

        public interface IToken
        {
            (int pos, long answer) Calculate(long current, int pos, List<IToken> calculateables);
        }

        public class Group : IToken
        {
            public List<IToken> Calculateables { get; } = new List<IToken>();

            public (int pos, long answer) Calculate(long current, int pos, List<IToken> calculateables)
            {
                var (nextPos, rightPart) = Calculateables[0].Calculate(0, 0, Calculateables);
                while (nextPos < Calculateables.Count)
                {
                    (nextPos, rightPart) = Calculateables[nextPos].Calculate(rightPart, nextPos, Calculateables);
                }
                return (pos + 1, rightPart);
            }
        }

        public class PlusOperation : IToken
        {
            public (int pos, long answer) Calculate(long current, int pos, List<IToken> calculateables)
            {
                if (calculateables[pos + 1] is Number x)
                {
                    return (pos + 2, current + x.Value);
                }
                var (nextPos, rightPart) = calculateables[pos + 1].Calculate(0, pos + 1, calculateables);
                return (nextPos, current + rightPart);
            }
        }

        public class MinusOperation : IToken
        {
            public (int pos, long answer) Calculate(long current, int pos, List<IToken> calculateables)
            {
                if (calculateables[pos + 1] is Number x)
                {
                    return (pos + 2, current - x.Value);
                }

                var (nextPos, rightPart) = calculateables[pos + 1].Calculate(0, pos + 1, calculateables);
                return (nextPos, current - rightPart);
            }
        }

        public class MultiplyOperation : IToken
        {
            public (int pos, long answer) Calculate(long current, int pos, List<IToken> calculateables)
            {
                var (nextPos, rightPart) = calculateables[pos + 1].Calculate(0, pos + 1, calculateables);
                return (nextPos, current * rightPart);
            }
        }

        public class DivideOperation : IToken
        {
            public (int pos, long answer) Calculate(long current, int pos, List<IToken> calculateables)
            {
                var (nextPos, rightPart) = calculateables[pos + 1].Calculate(0, pos + 1, calculateables);
                return (nextPos, current / rightPart);
            }
        }

        public class Number : IToken
        {
            public long Value { get; init; }

            public (int, long) Calculate(long current, int pos, List<IToken> calculateables)
            {
                return (pos + 1, Value);
            }
        }

        #endregion Part1

        #region Part 2

        private static void Part2(string[] instructions)
        {
            var sum = 0L;

            foreach (var line in instructions)
            {
                var lineAnswer = Calculate2(line);
                Console.WriteLine($"{line} = {lineAnswer}");
                sum += lineAnswer;
            }

            Console.WriteLine($"answer = {sum}");
        }

        private static long Calculate2(string line)
        {
            var tokens = new Group();

            Tokenize(line.Replace(" ", string.Empty), 0, tokens);

            tokens = UpdateTokensWithPrecedence(tokens);

            var (_, answer) = tokens.Calculate(0, 0, tokens.Calculateables);
            return answer;
        }

        private static Group UpdateTokensWithPrecedence(Group group)
        {
            var result = new Group();
            var pos = 0;

            var updated = false;

            while (pos < group.Calculateables.Count)
            {
                if (pos + 2 < group.Calculateables.Count && group.Calculateables[pos + 1] is PlusOperation)
                {
                    var subgroup = new Group();
                    subgroup.Calculateables.Add(TokenizeWithPrecedenceIfRequired(group.Calculateables[pos]));
                    subgroup.Calculateables.Add(group.Calculateables[pos + 1]);
                    subgroup.Calculateables.Add(TokenizeWithPrecedenceIfRequired(group.Calculateables[pos + 2]));

                    result.Calculateables.Add(subgroup);
                    pos += 3;
                    updated = true;
                }
                else
                {
                    result.Calculateables.Add(TokenizeWithPrecedenceIfRequired(group.Calculateables[pos]));
                    pos++;
                }
            }

            if (updated && group.Calculateables.Count > 3)
            {
                return UpdateTokensWithPrecedence(result);
            }
            else
            {
                return result;
            }
        }

        private static IToken TokenizeWithPrecedenceIfRequired(IToken token)
        {
            if (token is Group innerGroup)
            {
                return UpdateTokensWithPrecedence(innerGroup);
            }
            else
            {
                return token;
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