using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _11
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = GetInputFilePath();

            var instructions = File.ReadAllLines(filePath);

            var answerPart1 = Part1(instructions);

            var answerPart2 = Part2(instructions);


            //while (t < 10000000000000000)
            //{
            //    //t = busDefinitions[0].Depart;

            //    var window = t + busDefinitions[0].Id;

            //    if (busDefinitions.All(b => b.NextDepartment == t + b.Depart && b.NextDepartment <= window))
            //    {
            //        Console.WriteLine($"{round}: Found {t}");
            //        PrintDepartments(busDefinitions, t);
            //        break;
            //    }

            //    CalculateNext(busDefinitions, busses, t);

            //    t++;
            //}





        }

        private static long Part1(string[] instructions)
        {
            var arrival = long.Parse(instructions[0]);
            var busses = instructions[1].Split(",").Where(t => t != "x").Select(x => new Bus(long.Parse(x),0)).ToList();

            var nextDeparts = busses.Select(b => new Bus(b.Id, (int)Math.Ceiling((double)arrival / b.Id) * b.Id)).OrderBy(b => b.Depart);

            var nextDepart = nextDeparts.First();

            var earliest = nextDepart.Depart - arrival;

            Console.WriteLine($"Answer: {nextDepart.Id}*{earliest}={nextDepart.Id * earliest}");
            return nextDepart.Id * earliest;
        }


        private static long Part2(string[] instructions)
        {
            var bus = instructions[1].Split(',');

            long t = 0;
            long inc = long.Parse(bus[0]);

            for (var pos = 1; pos < bus.Length; pos++)
            {
                if (bus[pos] != "x")
                {
                    var tDiff = int.Parse(bus[pos]);
                    while (true)
                    {
                        t += inc;
                        if ((t + pos) % tDiff == 0)
                        {
                            inc *= tDiff;
                            break;
                        }

                    }
                }

            }
            Console.WriteLine($"Answer 2: {t}");
            return t;
        }

        public record Bus(long Id, long Depart);

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