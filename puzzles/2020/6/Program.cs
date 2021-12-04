using System;
using System.IO;
using System.Linq;

namespace _6
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = GetInputFilePath();

            var input = File.ReadAllText(filePath);

            var groups = input.Split("\r\n\r\n");

            Console.WriteLine($"Total groups: {groups.Length}");

            var buckets = Enumerable.Range(1, 26).Select(x => (char)(x - 1 + 'a')).ToList();

            var distinctAnswerCount = 0;
            var groupAnswerCount = 0;

            foreach (var group in groups)
            {
                var groupSize = group.Count(c => c == '\n') + 1;

                var answers = buckets.ToDictionary(x => x, x => 0);

                var groupAnswers = group.Where(c => c != '\r' && c != '\n');

                foreach (var answer in groupAnswers)
                {
                    answers[answer]++;
                }

                distinctAnswerCount += groupAnswers.Distinct().Count();
                groupAnswerCount += answers.Where(kv => kv.Value == groupSize).Count();
            }

            Console.WriteLine($"Sum of anyone answer counts {distinctAnswerCount}");
            Console.WriteLine($"Sum of everyone answer counts {groupAnswerCount}");
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