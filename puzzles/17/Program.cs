using System;
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

            Measure(() => Part1(instructions));
            Measure(() => Part2(instructions));
        }
        #region Part1
        private static void Part1(string[] instructions)
        {
            var states = Enumerable.Range(0, 2).Select(x => new Dictionary<string, Pocket>()).ToArray();

            var cubeDimensions = ReadInitialState(instructions, states[0]);
            Console.WriteLine($"Round: 0");

            PrintCube(states[0], cubeDimensions);
            for (var round = 0; round < 6; round++)
            {
                Console.WriteLine($"Round: {round}");
                cubeDimensions = CalculateNextRound(states[round % 2], states[(round + 1) % 2], cubeDimensions);
            }

            var active = states[0].Values.Count(p => p.IsActive);

            Console.WriteLine(active);
        }

        private static readonly CubeDirections[] cubeDirections = Enumerable.Range(-1, 3).SelectMany(x =>
            Enumerable.Range(-1, 3).SelectMany(y => Enumerable.Range(-1, 3).Select(z => new CubeDirections(x, y, z))
        )).Where(directions => directions.X != 0 || directions.Y != 0 || directions.Z != 0).ToArray();

        private static CubeDimensions CalculateNextRound(Dictionary<string, Pocket> current, Dictionary<string, Pocket> next, CubeDimensions cubeDimensions)
        {
            var nextCube = cubeDimensions with
            {
                topLeftFront = cubeDimensions.topLeftFront with
                {
                    X = cubeDimensions.topLeftFront.X - 1,
                    Y = cubeDimensions.topLeftFront.Y - 1,
                    Z = cubeDimensions.topLeftFront.Z - 1,
                },
                bottomRightBack = cubeDimensions.bottomRightBack with
                {
                    X = cubeDimensions.bottomRightBack.X + 1,
                    Y = cubeDimensions.bottomRightBack.Y + 1,
                    Z = cubeDimensions.bottomRightBack.Z + 1,
                }
            };

            var lowerX = cubeDimensions.topLeftFront.X;
            var lowerY = cubeDimensions.topLeftFront.Y;
            var lowerZ = cubeDimensions.topLeftFront.Z;

            var upperX = cubeDimensions.bottomRightBack.X;
            var upperY = cubeDimensions.bottomRightBack.Y;
            var upperZ = cubeDimensions.bottomRightBack.Z;

            for (var x = nextCube.topLeftFront.X; x <= nextCube.bottomRightBack.X; x++)
            {
                for (var y = nextCube.topLeftFront.Y; y <= nextCube.bottomRightBack.Y; y++)
                {
                    for (var z = nextCube.topLeftFront.Z; z <= nextCube.bottomRightBack.Z; z++)
                    {

                        var key = GetLocation(x, y, z);

                        var pocket = current.ContainsKey(key) ? current[key] : new Pocket(x, y, z, false);

                        if (pocket.Neighbours == null)
                        {
                            pocket.Neighbours = cubeDirections
                                .Select(direction =>
                                new Location(pocket.X + direction.X, pocket.Y + direction.Y, pocket.Z + direction.Z)
                                ).ToArray();
                        }

                        var activeRecords = pocket.Neighbours.Select(t => current.ContainsKey(t.Key) ? current[t.Key] : null).Count(x => x?.IsActive ?? false);

                        var nextPocket = pocket with
                        {
                            IsActive = (pocket.IsActive && (activeRecords is 2 or 3)) || (!pocket.IsActive && (activeRecords is 3))
                        };

                        next[key] = nextPocket;
                        
                        if (nextPocket.IsActive)
                        {
                            lowerX = Math.Min(lowerX, x);
                            lowerY = Math.Min(lowerY, y);
                            lowerZ = Math.Min(lowerZ, z);
                            upperX = Math.Max(upperX, x);
                            upperY = Math.Max(upperY, y);
                            upperZ = Math.Max(upperZ, z);
                        }
                    }
                }
            }

            var calculatedCube = new CubeDimensions(new Location(lowerX, lowerY, lowerZ), new Location(upperX, upperY, upperZ));
           
            PrintCube(next, calculatedCube);

            return calculatedCube;
        }

        private static void PrintCube(Dictionary<string, Pocket> next, CubeDimensions nextCube)
        {
            var sb = new StringBuilder();

            for (int z = nextCube.topLeftFront.Z; z <= nextCube.bottomRightBack.Z; z++)
            {
                sb.AppendLine($"z={z}");
                for (int y = nextCube.topLeftFront.Y; y <= nextCube.bottomRightBack.Y; y++)
                {
                    for (int x = nextCube.topLeftFront.X; x <= nextCube.bottomRightBack.X; x++)
                    {
                        if (next.TryGetValue(GetLocation(x, y, z), out var value))
                        {
                            sb.Append(value.IsActive ? '#' : '.');
                        }
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }

        public static CubeDimensions ReadInitialState(string[] input, Dictionary<string, Pocket> state)
        {
            var z = 0;
            for (var y = 0; y < input.Length; y++)
            {
                for (var x = 0; x < input[y].Length; x++)
                {
                    state[GetLocation(x, y, z)] = input[y][x] switch
                    {
                        '#' => new Pocket(x, y, z, true),
                        _ => new Pocket(x, y, z, false)
                    };
                }
            }
            return new CubeDimensions(new Location(0, 0, 0), new Location(input[0].Length - 1, input.Length - 1, 0));
        }

        public record CubeDimensions(Location topLeftFront, Location bottomRightBack);

        public record Location(int X, int Y, int Z)
        {
            public string Key => GetLocation(X, Y, Z);
        }

        public record Pocket(int X, int Y, int Z, bool IsActive) : Location(X, Y, Z)
        {
            public Location[]? Neighbours = null;
        }
        public record CubeDirections(int X, int Y, int Z);

        public static string GetLocation(int x, int y, int z)
        {
            return $"{x},{y},{z}";
        }
        #endregion

        #region Part 2



        private static void Part2(string[] instructions)
        {
            var states = Enumerable.Range(0, 2).Select(x => new Dictionary<string, Pocket2>()).ToArray();

            var cubeDimensions = ReadInitialState2(instructions, states[0]);
            Console.WriteLine($"Round: 0");

            PrintCube2(states[0], cubeDimensions);
            for (var round = 0; round < 7; round++)
            {
                Console.WriteLine($"Round: {round}");
                cubeDimensions = CalculateNextRound2(states[round % 2], states[(round + 1) % 2], cubeDimensions);
            }

            var active = states[0].Values.Count(p => p.IsActive);

            Console.WriteLine(active);
        }

        private static readonly CubeDirections2[] cubeDirections2 = Enumerable.Range(-1, 3).SelectMany(w => Enumerable.Range(-1, 3).SelectMany(x =>
            Enumerable.Range(-1, 3).SelectMany(y => Enumerable.Range(-1, 3).Select(z => new CubeDirections2(x, y, z, w)))
        )).Where(directions => directions.X != 0 || directions.Y != 0 || directions.Z != 0 || directions.W != 0).ToArray();

        private static CubeDimensions2 CalculateNextRound2(Dictionary<string, Pocket2> current, Dictionary<string, Pocket2> next, CubeDimensions2 cubeDimensions)
        {
            var nextCube = cubeDimensions with
            {
                topLeftFront = cubeDimensions.topLeftFront with
                {
                    X = cubeDimensions.topLeftFront.X - 1,
                    Y = cubeDimensions.topLeftFront.Y - 1,
                    Z = cubeDimensions.topLeftFront.Z - 1,
                    W = cubeDimensions.topLeftFront.W - 1,
                },
                bottomRightBack = cubeDimensions.bottomRightBack with
                {
                    X = cubeDimensions.bottomRightBack.X + 1,
                    Y = cubeDimensions.bottomRightBack.Y + 1,
                    Z = cubeDimensions.bottomRightBack.Z + 1,
                    W = cubeDimensions.bottomRightBack.W + 1,
                }
            };

            var lowerX = cubeDimensions.topLeftFront.X;
            var lowerY = cubeDimensions.topLeftFront.Y;
            var lowerZ = cubeDimensions.topLeftFront.Z;
            var lowerW = cubeDimensions.topLeftFront.W;

            var upperX = cubeDimensions.bottomRightBack.X;
            var upperY = cubeDimensions.bottomRightBack.Y;
            var upperZ = cubeDimensions.bottomRightBack.Z;
            var upperW = cubeDimensions.bottomRightBack.W;
            for (var w = nextCube.topLeftFront.W; w <= nextCube.bottomRightBack.W; w++)
            {
                for (var x = nextCube.topLeftFront.X; x <= nextCube.bottomRightBack.X; x++)
                {
                    for (var y = nextCube.topLeftFront.Y; y <= nextCube.bottomRightBack.Y; y++)
                    {
                        for (var z = nextCube.topLeftFront.Z; z <= nextCube.bottomRightBack.Z; z++)
                        {

                            var key = GetLocation2(x, y, z,w);

                            var pocket = current.ContainsKey(key) ? current[key] : new Pocket2(x, y, z, w, false);

                            if (pocket.Neighbours == null)
                            {
                                pocket.Neighbours = cubeDirections2
                                    .Select(direction =>
                                    new Location2(pocket.X + direction.X, pocket.Y + direction.Y, pocket.Z + direction.Z, pocket.W + direction.W)
                                    ).ToArray();
                            }

                            var activeRecords = pocket.Neighbours.Select(t => current.ContainsKey(t.Key) ? current[t.Key] : null).Count(x => x?.IsActive ?? false);

                            var nextPocket = pocket with
                            {
                                IsActive = (pocket.IsActive && (activeRecords is 2 or 3)) || (!pocket.IsActive && (activeRecords is 3))
                            };

                            next[key] = nextPocket;
                            
                            if (nextPocket.IsActive)
                            {
                                lowerX = Math.Min(lowerX, x);
                                lowerY = Math.Min(lowerY, y);
                                lowerZ = Math.Min(lowerZ, z);
                                lowerW = Math.Min(lowerW, w);
                                upperX = Math.Max(upperX, x);
                                upperY = Math.Max(upperY, y);
                                upperZ = Math.Max(upperZ, z);
                                upperW = Math.Max(upperW, w);
                            }
                        }
                    }
                }
            }

            var calculatedCube = new CubeDimensions2(new Location2(lowerX, lowerY, lowerZ, lowerW), new Location2(upperX, upperY, upperZ, upperW));
            
            PrintCube2(next, calculatedCube);

            return calculatedCube;
        }

        private static void PrintCube2(Dictionary<string, Pocket2> next, CubeDimensions2 nextCube)
        {
            var sb = new StringBuilder();
            for (int w = nextCube.topLeftFront.W; w <= nextCube.bottomRightBack.W; w++)
            {
                for (int z = nextCube.topLeftFront.Z; z <= nextCube.bottomRightBack.Z; z++)
                {
                    sb.AppendLine($"z={z}, w={w}");
                    for (int y = nextCube.topLeftFront.Y; y <= nextCube.bottomRightBack.Y; y++)
                    {
                        for (int x = nextCube.topLeftFront.X; x <= nextCube.bottomRightBack.X; x++)
                        {
                            if (next.TryGetValue(GetLocation2(x, y, z, w), out var value))
                            {
                                sb.Append(value.IsActive ? '#' : '.');
                            }
                        }
                        sb.AppendLine();
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }

        public static CubeDimensions2 ReadInitialState2(string[] input, Dictionary<string, Pocket2> state)
        {
            var z = 0;
            var w = 0;
            for (var y = 0; y < input.Length; y++)
            {
                for (var x = 0; x < input[y].Length; x++)
                {
                    state[GetLocation2(x, y, z, w)] = input[y][x] switch
                    {
                        '#' => new Pocket2(x, y, z, z, true),
                        _ => new Pocket2(x, y, z, z, false)
                    };
                }
            }
            return new CubeDimensions2(new Location2(0, 0, 0, 0), new Location2(input[0].Length - 1, input.Length - 1, 0, 0));
        }

        public record CubeDimensions2(Location2 topLeftFront, Location2 bottomRightBack);

        public record Location2(int X, int Y, int Z, int W)
        {
            public string Key => GetLocation2(X, Y, Z, W);
        }

        public record Pocket2(int X, int Y, int Z, int W, bool IsActive) : Location2(X, Y, Z, W)
        {
            public Location2[]? Neighbours = null;
        }
        public record CubeDirections2(int X, int Y, int Z, int W);

        public static string GetLocation2(int x, int y, int z, int w)
        {
            return $"{x},{y},{z},{w}";
        }

        #endregion

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