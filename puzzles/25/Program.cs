using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        #region Part 1

        private static void Part1(string[] lines)
        {
            var publicKeys = lines.Select(t => long.Parse(t));

            var loopSizes = publicKeys.Select(t => DetermineLoopSize(7, t)).ToArray();

            foreach (var loopSize in loopSizes)
            {
                Console.WriteLine($"Loop size: {loopSize}");
            }

            var privateKeys= publicKeys.Select((publicKey, index) => CalculatePrivateKey(loopSizes[1-index], publicKey));

            foreach (var key in privateKeys)
            {
                Console.WriteLine($"Key size: {key}");
            }
        }

        private const long RemainderDivider = 20201227;

        private static long DetermineLoopSize(long subject, long publicKey)
        {
            long value = 1L;

            long loopSize = 0;

            while (value != publicKey)
            {
                value *= subject;
                value %= RemainderDivider;
                loopSize++;
            }

            return loopSize;
        }

        private static long CalculatePrivateKey(long loopSize, long subject)
        {
            long value = 1L;

            for (var i = 0; i < loopSize; i++)
            {

                value *= subject;
                value %= RemainderDivider;
            }
            return value;
        }

            #endregion Part 1

            #region Part 2

            private static void Part2(string[] lines)
            {

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