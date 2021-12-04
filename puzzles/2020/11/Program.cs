using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace _11
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = GetInputFilePath();

            var space = File.ReadLines(filePath)
                .Select(i => i.Select(c => Parse(c)).ToArray()).ToArray();

            var height = space.Length;
            var width = space[0].Length;

            Console.WriteLine($"Space: {space.Length}x{space[0].Length}");
            // Console.WriteLine($"Workspace: {workspaces[0].Length}x{workspaces[0][0].Length}");

            Execute(space, height, width, GenerateStepOneVisibilityMasks, StepOneDecisionTable, false);
            Execute(space, height, width, GenerateStepTwoVisibilityMasks, StepTwoDecisionTable, false);


            // Console.WriteLine($"permutations: {permutations[0]}");
        }


        static void Execute(Position[][] space, int height, int width, Func<int, int, int, int, Position[][], Mask[]> maskGenerator, Func<MaskedPlace, int, MaskedPlace> stepOneDecisionTable, bool showLog)
        {
            var workspaces = new MaskedPlace[2][];

            workspaces[0] = CreateEmptyWorkspace(space, height, width, maskGenerator);
            workspaces[1] = new MaskedPlace[workspaces[0].Length];
            if (showLog)
            {
                Print(workspaces[0], width);
            
                PrintMasks(workspaces[0], width);
            }
            ExecuteStrategy(width, workspaces, stepOneDecisionTable, showLog);
        }

        private static void ExecuteStrategy(int width, MaskedPlace[][] workspaces, Func<MaskedPlace, int, MaskedPlace> decisionTable, bool showLog)
        {
            var round = 0;
            const int maxRounds = 200;
            do
            {
                Console.WriteLine($"Round {round + 1}:");

                GenerateNextStep(workspaces[round % 2], workspaces[(round + 1) % 2], decisionTable);
                if (showLog)
                {
                    Print(workspaces[(round + 1) % 2], width);
                }
                Console.WriteLine($"End of round {round + 1}");
                round++;

                if (round > maxRounds)
                {
                    throw new Exception($"Need more than {maxRounds} rounds");
                }
            } while (!SpacesEqual(workspaces[0], workspaces[1]));

            Console.WriteLine($"Seats occupied: {workspaces[0].Count(t => t.Position == Position.OccupiedSeat)}");
        }

        private static void GenerateNextStep(MaskedPlace[] current, MaskedPlace[] next, Func<MaskedPlace, int, MaskedPlace> decisionTable)
        {
            for (var pos = 0; pos < current.Length; pos++)
            {
                var place = current[pos];

                if (place.Position != Position.Floor)
                {
                    var seatsOccupied = place.Mask.Count(m => current[m.Position].Position == Position.OccupiedSeat);
                    next[pos] = decisionTable(place, seatsOccupied);
                }
                else
                {
                    next[pos] = place;
                }
            }
        }

        private static MaskedPlace StepOneDecisionTable(MaskedPlace place, int seatsOccupied)
        {
            return seatsOccupied switch
            {
                0 => place with { Position = Position.OccupiedSeat },
                >= 4 => place with { Position = Position.EmptySeat },
                _ => place
            };
        }

        private static MaskedPlace StepTwoDecisionTable(MaskedPlace place, int seatsOccupied)
        {
            return seatsOccupied switch
            {
                0 => place with { Position = Position.OccupiedSeat },
                >= 5 => place with { Position = Position.EmptySeat },
                _ => place
            };
        }

        private static MaskedPlace[] CreateEmptyWorkspace(Position[][] positions, int height, int width, Func<int, int, int, int, Position[][], Mask[]> maskGenerator)
        {
            var result = new List<MaskedPlace>();

            result.AddRange(GenerateEmptyMaskedPlaceRow(width + 2, 0));

            for (var row = 1; row <= height; row++)
            {
                result.Add(new MaskedPlace(row, 0, EmptyMask, Position.Floor));

                for (var column = 1; column <= width; column++)
                {
                    result.Add(new MaskedPlace(row, column, maskGenerator(row, column, height + 2, width + 2, positions), positions[row - 1][column - 1]));
                }

                result.Add(new MaskedPlace(row, width + 2, EmptyMask, Position.Floor));
            }

            result.AddRange(GenerateEmptyMaskedPlaceRow(width + 2, height + 2));
            return result.ToArray();
        }

        private static readonly Mask[] EmptyMask = Array.Empty<Mask>();

        private static IEnumerable<MaskedPlace> GenerateEmptyMaskedPlaceRow(int width, int row)
        {
            for (var x = 0; x < width; x++)
            {
                yield return new MaskedPlace(row, x, EmptyMask, Position.Floor);
            }
        }

        private static Mask[] GenerateStepTwoVisibilityMasks(int row, int column, int height, int width, Position[][] positions)
        {
            return GenerateMasksVisible(row, column, height, width, positions).ToArray();
        }

        public static readonly List<Vector> visibilityVectors = new List<Vector> {
            new Vector(-1,-1),
            new Vector(-1,0),
            new Vector(-1,1),
            new Vector(0,-1),
            new Vector(0,1),
            new Vector(1,-1),
            new Vector(1,0),
            new Vector(1,1),
        };

        public record Vector(int Up, int Right);

        private static IEnumerable<Mask> GenerateMasksVisible(int row, int column, int height, int width, Position[][] positions)
        {
            var leftPosition = column - 1;
            var topPosition = row - 1;


            if (positions[topPosition][leftPosition] != Position.Floor)
            {

                foreach (var v in visibilityVectors)
                {
                    var posX = leftPosition + v.Right;
                    var posY = topPosition + v.Up;

                    while (posX >= 0 && posY >= 0 && posX < (width - 2) && posY < (height - 2))
                    {
                        if (positions[posY][posX] != Position.Floor)
                        {
                            yield return new Mask(posY + 1, posX + 1, width);
                            break;
                        }
                        else
                        {
                            posX += v.Right;
                            posY += v.Up;
                        }
                    }
                }
            }
        }

        private static Mask[] GenerateStepOneVisibilityMasks(int row, int column, int height, int width, Position[][] positions)
        {
            return GenerateMasks(row, column, height, width, positions).ToArray();
        }

        private static IEnumerable<Mask> GenerateMasks(int row, int column, int height, int width, Position[][] positions)
        {
            var leftPosition = column - 1;
            var topPosition = row - 1;

            if (positions[topPosition][leftPosition] != Position.Floor)
            {
                foreach (var v in visibilityVectors)
                {
                    var posX = leftPosition + v.Right;
                    var posY = topPosition + v.Up;

                    if (posX >= 0 && posY >= 0 && posX < (width - 2) && posY < (height - 2))
                    {
                        if (positions[posY][posX] != Position.Floor)
                        {
                            yield return new Mask(posY + 1, posX + 1, width);
                        }
                    }
                }
            }
        }

        private static int CalculateIndex(int x, int y, int width) => y * width + x;

        public static Position Parse(char p)
        {
            return p switch
            {
                '.' => Position.Floor,
                'L' => Position.EmptySeat,
                '#' => Position.OccupiedSeat,
                _ => throw new Exception($"Char not supported {p}"),
            };
        }

        public static void Print(MaskedPlace[] space, int width)
        {
            var totalWidth = width + 2;

            var sb = new StringBuilder();
            for (int pos = totalWidth; pos < space.Length - 1 - totalWidth; pos++)
            {
                if (pos % (totalWidth) > 0 && (pos % totalWidth) < totalWidth - 1)
                {
                    sb.Append(space[pos].Position switch
                    {
                        Position.Floor => '.',
                        Position.EmptySeat => 'L',
                        Position.OccupiedSeat => '#',
                        _ => throw new Exception(),
                    });
                }
                if (pos % (totalWidth) == totalWidth - 1)
                {
                    sb.AppendLine();
                }
            }

            sb.AppendLine();

            Console.WriteLine(sb.ToString());
        }

        public static void PrintMasks(MaskedPlace[] space, int width)
        {
            var totalWidth = width + 2;

            var sb = new StringBuilder();
            for (int pos = totalWidth; pos < space.Length - 1 - totalWidth; pos++)
            {
                if (pos % (totalWidth) > 0 && (pos % totalWidth) < totalWidth - 1)
                {
                    sb.AppendLine($"Masks: {pos}: Y:{space[pos].Y}, X:{space[pos].X} {space[pos].Position}");
                    if (space[pos].Mask.Any())
                    {
                        foreach (var mask in space[pos].Mask)
                        {
                            sb.AppendLine($"{mask.Y},{mask.X}, {mask.Position}");
                        }
                        sb.AppendLine();
                    }
                    else
                    {
                        sb.AppendLine("Geen mask");
                    }

                    sb.AppendLine("Next cell");
                }
            }

            sb.AppendLine();

            Console.WriteLine(sb.ToString());
        }

        public static bool SpacesEqual(Place[] spaceA, Place[] spaceB)
        {
            return spaceA.Zip(spaceB, (a, b) => a.Position == b.Position).All(p => p);
        }

        public enum Position
        {
            Floor,
            EmptySeat,
            OccupiedSeat
        }

        public record Mask
        {
            public Mask(int y, int x, int width)
            {
                X = x;
                Y = y;
                Position = CalculateIndex(x, y, width);
            }
            public int X { get; init; }
            public int Y { get; init; }
            public int Position { get; init; }
        }

        public record Place(Position Position)
        {
        }

        public record MaskedPlace(int Y, int X, Mask[] Mask, Position Position) : Place(Position)
        {
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