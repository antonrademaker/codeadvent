using AoC.Utilities;
using System.Diagnostics;

namespace AoC.Puzzles.Y2023.D17;

public static class Program
{
    private static void Main(string[] args)
    {
        string[] inputFiles = [
            "input/example.txt",
            "input/example2.txt"
            , "input/input.txt"
            ];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var part1 = CalculatePart1(inputs);

            Console.WriteLine($"Part 1: {part1}");

            var part2 = CalculatePart2(inputs);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static long CalculatePart1(string[] input)
    {
        return Calculate(input, Enqueue, 1);
    }

    private static long CalculatePart2(string[] input)
    {
        return Calculate(input, EnqueuePart2, 4);
    }

    private static long Calculate(string[] input, Action<PriorityQueue<State, int>, Coordinate, Coordinate, int, int> queueAction, int minStepsToEnd)
    {
        var (width, height, grid) = ParseInputs(input);

        var target = new Coordinate(width - 1, height - 1);

        var visited = new Dictionary<State, int>();

        Debug.Assert(grid.ContainsKey(target));

        var queue = new PriorityQueue<State, int>();

        queue.Enqueue(new State(new Coordinate(0, 0), Coordinate.OffsetRight, 1), 0);
        queue.Enqueue(new State(new Coordinate(0, 0), Coordinate.OffsetDown, 1), 0);

        while (queue.TryDequeue(out var state, out var currentHeat))
        {


            var newPosition = state.Position + state.Direction;

            if (grid.TryGetValue(newPosition, out var heat))
            {
                var newHeat = heat + currentHeat;

                if (newPosition == target && state.Straight >= minStepsToEnd)
                {
                    //Print(visited, width, height);
                    return newHeat;
                }

                if (!visited.TryGetValue(state, out var storedHeat) || currentHeat < storedHeat)
                {
                    visited[state] = currentHeat;

                    queueAction(queue, newPosition, state.Direction, newHeat, state.Straight);
                    continue;
                }

            }
        }

        return 0;
    }

    private static void Enqueue(PriorityQueue<State, int> queue, Coordinate newPosition, Coordinate direction, int newHeat, int straight)
    {
        queue.Enqueue(new State(newPosition, direction.RotateLeft, 1), newHeat);
        queue.Enqueue(new State(newPosition, direction.RotateRight, 1), newHeat);
        if (straight < 3)
        {
            queue.Enqueue(new State(newPosition, direction, straight + 1), newHeat);
        }
    }

    private static void EnqueuePart2(PriorityQueue<State, int> queue, Coordinate newPosition, Coordinate direction, int newHeat, int straight)
    {

        if (straight > 3)
        {
            queue.Enqueue(new State(newPosition, direction.RotateLeft, 1), newHeat);
            queue.Enqueue(new State(newPosition, direction.RotateRight, 1), newHeat);
        }

        if (straight < 10)
        {
            queue.Enqueue(new State(newPosition, direction, straight + 1), newHeat);
        }
    }



    private static void Print(HashSet<State> coordinates, int width, int height)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (coordinates.Any(t => t.Position == new Coordinate(x, y)))
                {
                    Console.Write("X");
                }
                else
                {
                    Console.Write(" ");

                }
            }
            Console.WriteLine();
        }
    }

    private static (int width, int height, Dictionary<Coordinate, int>) ParseInputs(string[] inputs)
    {

        var grid = new Dictionary<Coordinate, int>();

        for (var y = 0; y < inputs.Length; y++)
        {
            for (var x = 0; x < inputs[y].Length; x++)
            {
                grid[new Coordinate(x, y)] = inputs[y][x] - '0';
            }
        }
        return (inputs[0].Length, inputs.Length, grid);
    }
}

public record struct State(Coordinate Position, Coordinate Direction, int Straight);