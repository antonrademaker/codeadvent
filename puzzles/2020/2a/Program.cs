using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _2
{
    class Program
    {
        public static object sync = new();
        public static volatile int correct;

        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory();

            var regex = new Regex(@"(\d+)-(\d+)\W(.):\W(.+)", RegexOptions.Compiled);

            var input = File.ReadLines(path + "\\input.txt").Select(x => regex.Match(x))
                .Select(x => new Password(int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value), x.Groups[3].Value, x.Groups[4].Value))
                .ToList();

            Console.WriteLine("The size is {0}", input.Count);


            var result = Parallel.ForEach(input, (current) =>
            {

                var letterToSearch = current.letter.First();

                if (current.password[current.FirstPosition-1] == letterToSearch ^ current.password[current.LastPosition - 1] == letterToSearch)
                {
                    lock (sync)
                    {
                        correct++;
                    }
                }

            });


            Console.WriteLine($"Correct is {correct}");



        }
    }

    record Password(int FirstPosition, int LastPosition, string letter, string password) { }


}
