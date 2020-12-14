using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _11
{
    internal class Program
    {
        public const int Size = 36;

        private static void Main(string[] args)
        {
            string filePath = GetInputFilePath();

            var instructions = File.ReadAllLines(filePath);

            Measure(() => Part1(instructions));
            Measure(() => Part2(instructions));
        }

        private static void Part1(string[] instructions)
        {
            Dictionary<long, long> memorySpace = new();

            MaskEnum[] mask = new MaskEnum[Size];

            foreach (var line in instructions)
            {
                if (line.StartsWith("mask"))
                {
                    mask = ParseMask(line);
                }
                else if (line.StartsWith("mem"))
                {
                    (var pos, var value) = ParseMemoryLine(line);

                    var valueBits = GetBitArray(value);

                    for (var p = 0; p < Size; p++)
                    {
                        valueBits[p] = mask[Size - p - 1] switch
                        {
                            MaskEnum.Zero => false,
                            MaskEnum.One => true,
                            MaskEnum.X => valueBits[p],
                            _ => throw new Exception()
                        };
                    }

                    var v = GetLongFromBitArray(valueBits);
                    memorySpace[pos] = v;
                }
                else
                {
                    throw new Exception($"Operation not supported: {line}");
                }
            }

            var sum = memorySpace.Sum(kv => kv.Value);

            Console.WriteLine($"Sum:{sum}");
        }

        private static void Part2(string[] instructions)
        {
            Dictionary<long, long> memorySpace = new();

            MaskEnum[] mask = new MaskEnum[Size];

            foreach (var line in instructions)
            {
                if (line.StartsWith("mask"))
                {
                    mask = ParseMask(line);
                }
                else if (line.StartsWith("mem"))
                {
                    (var pos, var value) = ParseMemoryLine(line);

                    SetMemoryAddresses(pos, mask, memorySpace, value);
                }
                else
                {
                    throw new Exception($"Operation not supported: {line}");
                }
            }

            var sum = memorySpace.Sum(kv => kv.Value);

            Console.WriteLine($"Sum:{sum}");
        }

        private static (long pos, long value) ParseMemoryLine(string line)
        {
            var match = memRegex.Match(line);
            return (long.Parse(match.Groups[1].Value), long.Parse(match.Groups[2].Value));
        }

        private static BitArray GetBitArray(long value)
        {
            var valueBytes = BitConverter.GetBytes(value);

            var valueBits = new BitArray(valueBytes);

            return valueBits;
        }

        private static long GetLongFromBitArray(BitArray bitArray)
        {
            var array = new byte[8];
            bitArray.CopyTo(array, 0);
            return BitConverter.ToInt64(array, 0);
        }

        private static void SetMemoryAddresses(long pos, MaskEnum[] mask, Dictionary<long, long> memorySpace, long value)
        {
            var valueBits = GetBitArray(pos);

            SetMemoryAddresses(valueBits, mask, 0, memorySpace, value);
        }

        private static void SetMemoryAddresses(BitArray valueBits, MaskEnum[] mask, int currentPos, Dictionary<long, long> memorySpace, long value)
        {
            if (currentPos < Size)
            {
                var bitMask = mask[Size - currentPos - 1];

                switch (bitMask)
                {
                    case MaskEnum.X:
                        // Floating
                        valueBits[currentPos] = false;
                        SetMemoryAddresses(valueBits, mask, currentPos + 1, memorySpace, value);

                        valueBits[currentPos] = true;
                        SetMemoryAddresses(valueBits, mask, currentPos + 1, memorySpace, value);
                        break;

                    case MaskEnum.One:
                        valueBits[currentPos] = true;
                        SetMemoryAddresses(valueBits, mask, currentPos + 1, memorySpace, value);
                        break;

                    default:
                        SetMemoryAddresses(valueBits, mask, currentPos + 1, memorySpace, value);
                        break;
                };
            }
            else
            {
                memorySpace[GetLongFromBitArray(valueBits)] = value;
            }
        }

        private static readonly Regex memRegex = new Regex(@"^mem\[(\d+)\]\W=\W(\d+)$", RegexOptions.Compiled);

        public static MaskEnum[] ParseMask(string line)
        {
            return line.Replace("mask = ", string.Empty).Select(c => Parse(c)).ToArray();
        }

        public static MaskEnum Parse(char c)
        {
            return c switch
            {
                '0' => MaskEnum.Zero,
                '1' => MaskEnum.One,
                'X' => MaskEnum.X,
                _ => throw new ArgumentOutOfRangeException($"{c}: not supported")
            };
        }

        public enum MaskEnum
        {
            Zero,
            One,
            X
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