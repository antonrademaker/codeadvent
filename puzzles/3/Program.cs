using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _3
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory();

            var input = File.ReadLines(path + "\\input.txt")
                .Select(line => line.Select(c => new Point(c == '.' ? GroundType.Empty : GroundType.Tree)).ToList())
                .ToList();

            var world = new World(input).Optimize();

            var foundTrees = new List<int>();

            var stepFunctions = new List<StepFunction>
            {
                new StepFunction(1,1),
                new StepFunction(3,1),
                new StepFunction(5,1),
                new StepFunction(7,1),
                new StepFunction(1,2),
            };

            foreach (var stepFunction in stepFunctions)
            {

                var trees = CountTrees(world, stepFunction);
                foundTrees.Add(trees);
                Console.WriteLine($"Found: {trees }");

            }

            var result = foundTrees.Aggregate(1L, (acc, x) => acc * x);

            Console.WriteLine($"Found: {result }" );
            
        }

        public record StepFunction(int stepsRight, int stepsDown);

        private static int CountTrees(OptimizedWorld world, StepFunction stepFunction)
        {
            var currentX = 0;
            var currentY = 0;

            var worldHeight = world.GridHeight;

            var trees = 0;

            while (currentY < worldHeight)
            {
                if (world.Get(currentX, currentY).GroundType == GroundType.Tree)
                {
                    trees++;
                }

                currentX += stepFunction.stepsRight;
                currentY += stepFunction.stepsDown;
            }

            return trees;
        }
    }
    public enum GroundType
    {
        Empty,
        Tree
    }

    public record Point(GroundType GroundType);

    public class World
    {      

        List<List<Point>> grid = new();

        public World()
        {

        }

        public World(List<List<Point>> points)
        {
            Add(points);
        }

        public World Add(List<Point> row)
        {            
            grid.Add(row);
            return this;
        }

        public World Add(List<List<Point>> rows)
        {
            grid.AddRange(rows);
            return this;
        }

        public OptimizedWorld Optimize()
        {
            return new OptimizedWorld(grid);
        }
    }

    public class OptimizedWorld
    {
        public OptimizedWorld(List<List<Point>> grid)
        {
            Grid = grid;
            GridWidth = Grid[0].Count;
            GridHeight = Grid.Count;
        }

        public List<List<Point>> Grid { get; }
        public int GridWidth { get; }
        public int GridHeight { get;  }

        public Point Get(int x, int y)
        {
            var stepsToRight = x % GridWidth;
            return Grid[y][stepsToRight];
        }
    }
}
