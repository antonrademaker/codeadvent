using System.Numerics;
using System.Text;

using Coordinate3D = AoC.Utilities.Coordinate3D<long>;
using Coordinate2D = AoC.Utilities.Coordinate<long>;
using Microsoft.Z3;

namespace AoC.Puzzles.Y2023.D23;

public static partial class Program
{

    private static void Main(string[] args)
    {
        List<(string file, long testAreaMin, long testAreaMax)> inputFiles = [
            ("input/example.txt", 7 , 27),
            ("input/input.txt", 200000000000000, 400000000000000)
           ];

        foreach (var (file, testAreaMin, testAreaMax) in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var part1 = CalculatePart1(inputs, testAreaMin, testAreaMax);

            Console.WriteLine($"Part 1: {part1}");

            var part2 = CalculatePart2(inputs);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static long CalculatePart1(string[] inputs, long testAreaMin, long testAreaMax)
    {
        var hailstones = Parse(inputs);

        var intersections = hailstones.Select((item, index) => (item, index)).Aggregate(0, (total, nextHailstone) =>
        {
            return total + hailstones.Skip(nextHailstone.index + 1).Sum(t => t.IntersectsWithinXY(nextHailstone.item, testAreaMin, testAreaMax));
        });

        return intersections;
    }

    private static long CalculatePart2(string[] inputs)
    {
        var hailstones = Parse(inputs);

        var ctx = new Context();
        var solver = ctx.MkSolver();

        // Coordinates of the rock
        var x = ctx.MkIntConst("x");
        var y = ctx.MkIntConst("y");
        var z = ctx.MkIntConst("z");

        // Velocity of the rock
        var vx = ctx.MkIntConst("vx");
        var vy = ctx.MkIntConst("vy");
        var vz = ctx.MkIntConst("vz");

        // To find the location of the rock we need to solve the following equation
        // x = x0 + vx * t
        // y = y0 + vy * t
        // z = z0 + vz * t

        // To find the location we only need 3 hailstones positions, as we can solve the equation for each axis
        for (var i = 0; i < 3; i++)
        {
            var t = ctx.MkIntConst($"t{i}"); // t is the time it takes for the rock to reach the position of the hailstone

            var hailstone = hailstones[i];

            var px = ctx.MkInt(hailstone.Position.X);
            var py = ctx.MkInt(hailstone.Position.Y);
            var pz = ctx.MkInt(hailstone.Position.Z);

            var pvx = ctx.MkInt(hailstone.Direction.X);
            var pvy = ctx.MkInt(hailstone.Direction.Y);
            var pvz = ctx.MkInt(hailstone.Direction.Z);

            var xLeft = ctx.MkAdd(x, ctx.MkMul(t, vx)); // x0 + t * vx
            var yLeft = ctx.MkAdd(y, ctx.MkMul(t, vy)); // y0 + t * vy
            var zLeft = ctx.MkAdd(z, ctx.MkMul(t, vz)); // z0 + t * vz

            var xRight = ctx.MkAdd(px, ctx.MkMul(t, pvx)); // px + t * pvx
            var yRight = ctx.MkAdd(py, ctx.MkMul(t, pvy)); // py + t * pvy
            var zRight = ctx.MkAdd(pz, ctx.MkMul(t, pvz)); // pz + t * pvz

            solver.Add(t >= 0); // only future solutions
            solver.Add(ctx.MkEq(xLeft, xRight)); // x0 + t * vx = px + t * pvx
            solver.Add(ctx.MkEq(yLeft, yRight)); // y0 + t * vy = py + t * pvy
            solver.Add(ctx.MkEq(zLeft, zRight)); // z0 + t * vz = pz + t * pvz
        }

        solver.Check();
        var model = solver.Model;

        var rx = model.Eval(x);
        var ry = model.Eval(y);
        var rz = model.Eval(z);

        return Convert.ToInt64(rx.ToString()) + Convert.ToInt64(ry.ToString()) + Convert.ToInt64(rz.ToString());
    }

    private static Hailstone[] Parse(string[] inputs)
    {

        var hailstones = new Hailstone[inputs.Length];
        for (int i = 0; i < inputs.Length; i++)
        {
            var parts = inputs[i].Split('@', StringSplitOptions.TrimEntries);
            var position = ParseCoordinate3D(parts[0]);
            var direction = ParseCoordinate3D(parts[1]);

            hailstones[i] = new Hailstone(position, direction);
        }
        return hailstones;
    }

    private static Coordinate3D ParseCoordinate3D(string input)
    {
        var values = input.Split(',', StringSplitOptions.TrimEntries).Select(long.Parse).ToArray();
        return new Coordinate3D(values[0], values[1], values[2]);
    }
}

public record Hailstone(Coordinate3D Position, Coordinate3D Direction)
{

    public int IntersectsWithinXY(Hailstone other, long testAreaMin, long testAreaMax)
    {
        var (intersects, intersection) = IntersectsXY(other);
        if (intersects)
        {
            if (intersection.X >= testAreaMin && intersection.X <= testAreaMax &&
                intersection.Y >= testAreaMin && intersection.Y <= testAreaMax

                )
            {
                return 1;
            }
        }

        return 0;
    }

    public Hailstone AddDirection(Coordinate3D coordinate3D)
    {
        return this with
        {
            Direction = Direction + coordinate3D
        };
    }


    // https://www.geeksforgeeks.org/point-of-intersection-of-two-lines-formula/
    public (bool intersects, Coordinate2D intersection) IntersectsXY(Hailstone other)
    {
        double divisor = Direction.X * other.Direction.Y - Direction.Y * other.Direction.X;
        double t1 = (((other.Position.X - Position.X) * other.Direction.Y) - ((other.Position.Y - Position.Y) * other.Direction.X)) / divisor;
        double t2 = (((other.Position.X - Position.X) * Direction.Y) - ((other.Position.Y - Position.Y) * Direction.X)) / divisor;
        var x = Position.X + t1 * Direction.X;
        var y = Position.Y + t1 * Direction.Y;

        return (t1 >= 0 && t2 >= 0, new Coordinate2D((long)x, (long)y));
    }
}