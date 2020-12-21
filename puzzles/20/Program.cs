using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

        public class Tiles : Dictionary<int, Tile>
        {
        }

        public record Tile(int Id, TileBorder[] Sides, bool[,] Data)
        {
            public ImmutableArray<TileBorder> NeighbourCandidates { get; init; } = ImmutableArray.Create<TileBorder>();
            //public ImmutableArray<int> Neighbours { get; init; } = ImmutableArray.Create(0, 0, 0, 0);
          
            public int Rotations { get; init; }
            public bool IsFlipedTB { get; init; }
            public bool IsFlipedLR { get; init; }
        }

        public record TileBorder(long SideValue, Side Side, Direction Direction, int TileId);

        public enum Side
        {
            Top,
            Right,
            Bottom,
            Left
        }

        public enum Direction
        {
            LR,
            RL
        }

        private static void Part1(string[] instructions)
        {
            var tiles = new Tiles();
            var allSides = new List<TileBorder>();
            CalculateStep1(instructions, tiles, allSides);

            var corners = GetCorners(tiles);
            Console.WriteLine($"Corners: {string.Join(',', corners)}");

            if (corners.Count() == 4)
            {
                Console.WriteLine($"Answer: {corners.Aggregate(1L, (acc, cornerId) => acc * cornerId)} ");
            }
        }

        

        private static Dictionary<long, List<TileBorder>> CalculateStep1(string[] instructions, Tiles tiles, List<TileBorder> allSides)
        {
            var pos = 0;

            var positions = Enumerable.Range(0, 10).SelectMany(x => Enumerable.Range(0, 10).Select(y => (x, y))).ToArray(); ;

            while (pos < instructions.Length)
            {
                if (pos % 12 == 0)
                {
                    // New tile

                    Console.WriteLine($"pos: {pos}: {instructions[pos]}");

                    var tileId = int.Parse(instructions[pos].Replace("Tile ", string.Empty).Replace(":", String.Empty));

                    var sides = new List<TileBorder>();
                    sides.AddRange(ParseBorder(Enumerable.Range(0, 10).Select(step => instructions[pos + 1 + step][0]), Side.Left, tileId));
                    sides.AddRange(ParseBorder(Enumerable.Range(0, 10).Select(step => instructions[pos + 1 + step][9]), Side.Right, tileId));
                    sides.AddRange(ParseBorder(Enumerable.Range(0, 10).Select(step => instructions[pos + 1][step]), Side.Top, tileId));
                    sides.AddRange(ParseBorder(Enumerable.Range(0, 10).Select(step => instructions[pos + 1 + 9][step]), Side.Bottom, tileId));

                    var data = new bool[10, 10];

                    foreach (var position in positions)
                    {
                        data[position.y, position.x] = ParsePosition(instructions[position.y][position.x]);
                    }


                    allSides.AddRange(sides);
                    // Add all the reversed sides as well
                    allSides.AddRange(sides.Select(x => x with { Direction = Direction.RL, SideValue = Reverse(x.SideValue) }));
                    tiles.Add(tileId, new Tile(tileId, sides.ToArray(), data));

                    
                }
                pos += 12;
            }

            var matcher = allSides.GroupBy(tb => tb.SideValue).ToDictionary(kv => kv.Key, kv => kv.ToList());

            foreach (var sideGroup in matcher.Values.Where(t => t.Count > 1))
            {
                Console.WriteLine($"Value: {sideGroup.First().SideValue} values: {sideGroup.Count}");

                AddNeighbour(sideGroup, 0, tiles);
                AddNeighbour(sideGroup, 1, tiles);
            }

            return matcher;
        }

        private static List<int> GetCorners(Tiles tiles)
        {
            return tiles.Values.Where(tile => IsCornerCandidate(tile)).Select(t => t.Id).ToList();
        }

        private static bool IsCornerCandidate(Tile tile)
        {
            return !(tile.NeighbourCandidates.IsEmpty || tile.NeighbourCandidates.Length > 4);
        }

        private static List<int> GetBorders(Tiles tiles)
        {
            return tiles.Values.Where(tile => IsBorderCandidate(tile)).Select(t => t.Id).ToList();
        }

        private static bool IsBorderCandidate(Tile tile)
        {
            return !(tile.NeighbourCandidates.IsEmpty || tile.NeighbourCandidates.Length != 6);
        }

        private static void AddNeighbour(List<TileBorder> borders, int pos, Tiles tiles)
        {
            var tile = tiles[borders[pos].TileId];

            tiles[borders[pos].TileId] = tile with
            {
                NeighbourCandidates = tile.NeighbourCandidates.Add(borders[pos])
            };
        }

        private static IEnumerable<TileBorder> ParseBorder(IEnumerable<char> enumerable, Side side, int tileId)
        {
            var values = enumerable.Select(v => ParsePosition(v));

            var valA = new BitArray(values.ToArray());            

            yield return new TileBorder(GetLongFromBitArray(valA), side, Direction.LR, tileId);
            //yield return new TileBorder(GetLongFromBitArray(valB), side, Direction.RL, tileId);
        }

        private static long GetLongFromBitArray(BitArray bitArray)
        {
            var array = new byte[8];
            bitArray.CopyTo(array, 0);
            return BitConverter.ToInt64(array, 0);
        }

        public record LocationMask(int x, int y);

        public static long Reverse(long input)
        {
            var bits = new BitArray(BitConverter.GetBytes(input));
            return GetLongFromBitArray(Reverse(bits));
        }

        public static BitArray Reverse(BitArray array)
        {
            int len = 10;
            BitArray a = new BitArray(array);
            BitArray b = new BitArray(array);

            for (int i = 0, j = len - 1; i < len; ++i, --j)
            {
                a[i] = a[i] ^ b[j];
                b[j] = a[i] ^ b[j];
                a[i] = a[i] ^ b[j];
            }

            return a;
        }

        #endregion Part1

        #region Part 2

        private static void Part2(string[] instructions)
        {
            var tiles = new Tiles();
            var allSides = new List<TileBorder>();
            var indexedSides = CalculateStep1(instructions, tiles, allSides);

            var corners = GetCorners(tiles);
            var borders = GetBorders(tiles);

            var size = (int)Math.Sqrt(tiles.Count);

            var borderedImage = new Tile[size,size];

            var firstTile = tiles[corners[0]]; //.Select(tileId => tiles[tileId]).First(t => t.NeighbourCandidates.All(nc => nc.Side == Side.Right || nc.Side == Side.Bottom));

            // now rotate / flip to make it fit

            while (!firstTile.NeighbourCandidates.All(t => t.Side == Side.Right || t.Side == Side.Bottom))
            {
                // firstTile = RotateAndFlip(firstTile, Orientation.Right, Orientation.Bottom);

                if (!firstTile.NeighbourCandidates.Any(t => t.Side == Side.Right))
                {
                    firstTile = Rotate(firstTile);
                }
                else
                {
                    firstTile = FlipTB(firstTile);
                }
            }

            borderedImage[0,0] = firstTile;
            CalculateYTiles(tiles, indexedSides, size, borderedImage, 0);

            for (int xpos = 1; xpos < size; xpos++)
            {
                var tileLeft = borderedImage[0,xpos -1];
                var tileLeftRightSide = tileLeft.Sides.First(t => t.Side == Side.Right);

                var tileBorder = indexedSides[tileLeftRightSide.SideValue].First(t => t.TileId != tileLeft.Id);

                var tile = tiles[tileBorder.TileId];

                var reveredSideId = Reverse(tileLeftRightSide.SideValue);

                while (!tile.Sides.Any(s => s.Side == Side.Left && (s.SideValue == tileLeftRightSide.SideValue || s.SideValue == reveredSideId)))
                {
                    tile = Rotate(tile);
                }

                if (tile.Sides.First(s => s.Side == Side.Left).SideValue != tileLeftRightSide.SideValue)
                {
                    tile = FlipTB(tile);
                }
                borderedImage[0, xpos] = tile;
                CalculateYTiles(tiles, indexedSides, size, borderedImage, xpos);
            }

            for(var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    Console.Write($"{borderedImage[i,j].Id}\t");
                }
                Console.WriteLine();
            }

        }

        private static void CalculateYTiles(Tiles tiles, Dictionary<long, List<TileBorder>> indexedSides, int size, Tile[,] borderedImage, int column)
        {
            for (int ypos = 1; ypos < size; ypos++)
            {
                var tileAbove = borderedImage[ypos - 1,column];

                var tileAboveBottom = tileAbove.Sides.First(t => t.Side == Side.Bottom);

                //var top = tileAbove.NeighbourCandidates.Where(t => t.Side == Side.Bottom).ToArray();

                var tileBorder = indexedSides[tileAboveBottom.SideValue].First(t => t.TileId != tileAbove.Id);

                var tile = tiles[tileBorder.TileId];

                var reveredSideId = Reverse(tileAboveBottom.SideValue);

                while (!tile.Sides.Any(s => s.Side == Side.Top && (s.SideValue == tileAboveBottom.SideValue || s.SideValue == reveredSideId)))
                {
                    tile = Rotate(tile);
                }

                if (tile.Sides.First(s => s.Side == Side.Top).SideValue != tileAboveBottom.SideValue)
                {
                    tile = FlipLR(tile);
                }

                borderedImage[ypos,column] = tile;
            }
        }

        private static Tile FlipTB(Tile firstTile)
        {
            return firstTile with
            {
                IsFlipedTB = !firstTile.IsFlipedTB,
                Data = FlipTB(firstTile.Data, 10),
                NeighbourCandidates = FlipTB(firstTile.NeighbourCandidates),
                Sides = FlipTB(firstTile.Sides)
            };
        }
        private static Tile FlipLR(Tile firstTile)
        {
            return firstTile with
            {
                IsFlipedLR = !firstTile.IsFlipedLR,
                Data = FlipLR(firstTile.Data, 10),
                NeighbourCandidates = FlipLR(firstTile.NeighbourCandidates),
                Sides = FlipLR(firstTile.Sides)
            };
        }


        private static TileBorder[] FlipLR(TileBorder[] sides)
        {
            return sides.Select(t => t with
            {
                Side = t.Side switch
                {
                    Side.Left => Side.Right,
                    Side.Right => Side.Left,
                    _ => t.Side
                },
                SideValue = t.Side == Side.Top || t.Side == Side.Bottom ? Reverse(t.SideValue) : t.SideValue
            }).ToArray();
        }


        private static TileBorder[] FlipTB(TileBorder[] sides)
        {
            return sides.Select(t => t with
            {
                Side = t.Side switch
                {
                    Side.Top => Side.Bottom,
                    Side.Bottom => Side.Top,
                    _ => t.Side
                },
                SideValue = t.Side == Side.Left || t.Side == Side.Right ? Reverse(t.SideValue) : t.SideValue
            }).ToArray();
        }

        private static ImmutableArray<TileBorder> FlipTB(ImmutableArray<TileBorder> neighbourCandidates)
        {
            for (var i = 0; i < neighbourCandidates.Length; i++)
            {
                if (neighbourCandidates[i].Side is Side.Left or Side.Right)
                {
                    neighbourCandidates = neighbourCandidates.SetItem(i, FlipTBDirection(neighbourCandidates[i]));
                } else
                {
                    neighbourCandidates = neighbourCandidates.SetItem(i, FlipTBOrientation(neighbourCandidates[i]));
                }
            }

            return neighbourCandidates;
        }

        private static ImmutableArray<TileBorder> FlipLR(ImmutableArray<TileBorder> neighbourCandidates)
        {
            for (var i = 0; i < neighbourCandidates.Length; i++)
            {
                if (neighbourCandidates[i].Side is Side.Top or Side.Bottom)
                {
                    neighbourCandidates = neighbourCandidates.SetItem(i, FlipTBDirection(neighbourCandidates[i]));
                }
                else
                {
                    neighbourCandidates = neighbourCandidates.SetItem(i, FlipTBOrientation(neighbourCandidates[i]));
                }
            }

            return neighbourCandidates;
        }

        private static TileBorder FlipTBDirection(TileBorder tileBorder)
        {
            return tileBorder with
            {
                Direction = (Direction)((((int)tileBorder.Direction) + 1) % 2),
                SideValue = Reverse(tileBorder.SideValue)
            };
        }

        private static TileBorder FlipTBOrientation(TileBorder tileBorder)
        {
            return tileBorder with
            {
                Side = (Side)((((int)tileBorder.Side) + 2) % 4)
            };
        }

        private static bool[,] FlipTB(bool[,] data, int size)
        {
            bool[,] result = new bool[size, size];

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    result[i, j] = data[size - i - 1, j];
                }
            }

            return result;
        }

        private static bool[,] FlipLR(bool[,] data, int size)
        {
            bool[,] result = new bool[size, size];

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    result[i, j] = data[i, size - j - 1];
                }
            }

            return result;
        }

        private static Tile Rotate(Tile firstTile)
        {
            return firstTile with
            {
                Rotations = firstTile.Rotations + 1,
                Data = Rotate(firstTile.Data, 10),
                NeighbourCandidates = Rotate(firstTile.NeighbourCandidates),
                Sides = Rotate(firstTile.Sides),
            };
        }

        private static TileBorder[] Rotate(TileBorder[] sides)
        {
            return sides.Select(t => Rotate(t)).ToArray();
        }

        private static ImmutableArray<TileBorder> Rotate(ImmutableArray<TileBorder> neighbourCandidates)
        {
            for (var i = 0; i < neighbourCandidates.Length; i++) {
                neighbourCandidates = neighbourCandidates.SetItem(i, Rotate(neighbourCandidates[i]));
            }

            return neighbourCandidates;
        }

        private static TileBorder Rotate(TileBorder tileBorder)
        {
            if (tileBorder.Side == Side.Left || tileBorder.Side == Side.Right) {
                return tileBorder with
                {
                    Side = (Side)((((int)tileBorder.Side) + 1) % 4),
                    Direction = (Direction)((((int)tileBorder.Direction) + 1) % 2),
                    SideValue = Reverse(tileBorder.SideValue)
                };
            }

            return tileBorder with
            {
                Side = (Side)((((int)tileBorder.Side) + 1) % 4),
            };
        }

        private static bool[,] Rotate(bool[,] data, int size)
        {
            bool[,] result = new bool[size, size];

            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    result[i, j] = data[size - j - 1, i];
                }
            }

            return result;
        }

        private static bool ParsePosition(char v)
        {
            return v switch { '#' => true, _ => false };
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