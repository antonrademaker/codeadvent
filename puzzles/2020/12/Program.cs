using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _11
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = GetInputFilePath();

            var instructions = File.ReadLines(filePath)
                .Select(i => Parse(i)).ToArray();

            var ship = new Ship(0, 0, 0, 0, Direction.E);
            if (args.Length > 0)
            {
                foreach (var instruction in instructions)
                {
                    ship = ExecuteStep1(instruction, ship);
                    Print(ship);
                }
            }

            ship = new Ship(0, 0, 0, 0, Direction.E);
            Print(ship);
            var waypoint = new Waypoint(10, 1);
            foreach (var instruction in instructions)
            {
                (ship, waypoint) = ExecuteStep2(instruction, ship, waypoint);
                Print(ship);
                Print(waypoint);
            }
        }

        private static void Print(Ship ship)
        {
            Console.WriteLine($"Ship at: E:{ship.E}, N: {ship.N}, EW: {ship.EW}, NS: {ship.NS}, direction: {ship.Direction}: total travel: {Math.Abs(ship.EW) + Math.Abs(ship.NS)}");
        }

        private static void Print(Waypoint waypoint)
        {
            Console.WriteLine($"Waypoint at: E:{waypoint.E}, N: {waypoint.N}");
        }

        private static (Ship, Waypoint) ExecuteStep2(Instruction instruction, Ship ship, Waypoint waypoint)
        {
            Console.Write($"{instruction.Action}{instruction.Value}: ");
            switch (instruction.Action)
            {
                case Action.N:
                    //Action N means to move north by the given value.
                    return MoveWaypointNS(instruction.Value * 1, ship, waypoint);

                case Action.S:
                    //means to move south by the given value.
                    return MoveWaypointNS(instruction.Value * -1, ship, waypoint);

                case Action.E:
                    //means to move east by the given value.
                    return MoveWaypointEW(instruction.Value * 1, ship, waypoint);

                case Action.W:
                    //means to move west by the given value.
                    return MoveWaypointEW(instruction.Value * -1, ship, waypoint);

                case Action.L:
                    //means to turn left the given number of degrees.
                    // for now we do only straight turns
                    return RotateWaypoint(instruction.Value * -1, ship, waypoint);

                case Action.R:
                    //means to turn right the given number of degrees.
                    return RotateWaypoint(instruction.Value, ship, waypoint);

                case Action.F:
                    //means to move forward by the given value in the direction the ship is currently facing.
                    return (MoveShip(instruction.Value * -1, ship, waypoint), waypoint);

                default:
                    throw new Exception();
            }
        }

        private static Ship MoveShip(int times, Ship ship, Waypoint waypoint)
        {
            return ship with
            {
                E = ship.E + (waypoint.E * times),
                N = ship.N + (waypoint.N * times),
                EW = ship.EW + (waypoint.E * times),
                NS = ship.NS + (waypoint.N * times)
            };
        }

        private static (Ship, Waypoint) RotateWaypoint(int value, Ship ship, Waypoint waypoint)
        {
            var steps = value / 90;

            var waypointResult = waypoint;

            for (var step = 0; step < Math.Abs(steps); step++)
            {
                waypointResult = value > 0 ? RotateWaypointLeft(waypointResult) : RotateWaypointRight(waypointResult);
            }
            return (ship, waypointResult);
        }

        private static Waypoint RotateWaypointLeft(Waypoint waypoint)
        {
            return waypoint with { N = -waypoint.E, E = waypoint.N };
        }

        private static Waypoint RotateWaypointRight(Waypoint waypoint)
        {
            return waypoint with { N = waypoint.E, E = -waypoint.N };
        }

        private static (Ship, Waypoint) MoveWaypointNS(int value, Ship ship, Waypoint waypoint)
        {
            var newWaypoint = waypoint with { N = waypoint.N + value };
            return (ship, newWaypoint);
        }

        private static (Ship, Waypoint) MoveWaypointEW(int value, Ship ship, Waypoint waypoint)
        {
            var newWaypoint = waypoint with { E = waypoint.E + value };
            return (ship, newWaypoint);
        }

        private static Ship ExecuteStep1(Instruction instruction, Ship ship)
        {
            switch (instruction.Action)
            {
                case Action.N:
                    //Action N means to move north by the given value.
                    return MoveNS(instruction.Value * 1, ship);

                case Action.S:
                    //means to move south by the given value.
                    return MoveNS(instruction.Value * -1, ship);

                case Action.E:
                    //means to move east by the given value.
                    return MoveEW(instruction.Value * 1, ship);

                case Action.W:
                    //means to move west by the given value.
                    return MoveEW(instruction.Value * -1, ship);

                case Action.L:
                    //means to turn left the given number of degrees.
                    // for now we do only straight turns

                    return CalculateNewDirection(instruction, ship, 1);

                case Action.R:
                    //means to turn right the given number of degrees.
                    return CalculateNewDirection(instruction, ship, -1);

                case Action.F:
                    //means to move forward by the given value in the direction the ship is currently facing.

                    return ship.Direction switch
                    {
                        Direction.E => MoveEW(instruction.Value * 1, ship),
                        Direction.W => MoveEW(instruction.Value * -1, ship),
                        Direction.N => MoveNS(instruction.Value * 1, ship),
                        Direction.S => MoveNS(instruction.Value * -1, ship),
                        _ => throw new Exception()
                    };

                default:
                    throw new Exception();
            }
        }

        private static Ship MoveNS(int distance, Ship ship)
        {
            return ship with { N = ship.N + distance, NS = ship.NS - distance };
        }

        private static Ship MoveEW(int distance, Ship ship)
        {
            return ship with { E = ship.E + distance, EW = ship.EW - distance };
        }

        private static Ship CalculateNewDirection(Instruction instruction, Ship ship, int lr)
        {
            var steps = instruction.Value / 90;
            var nextDirection = (Direction)(((int)ship.Direction - steps * lr + 4) % 4);
            return ship with { Direction = nextDirection };
        }

        private static Regex parser = new Regex(@"^(.)(\d+)$", RegexOptions.Compiled);

        private static Instruction Parse(string input)
        {
            var match = parser.Match(input);

            return new Instruction(Enum.Parse<Action>(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
        }

        public enum Action
        {
            N,
            E,
            S,
            W,
            L,
            R,
            F
        }

        public enum Direction
        {
            N,
            E,
            S,
            W
        }

        public record Instruction(Action Action, int Value);

        public record Ship(int E, int N, int EW, int NS, Direction Direction);

        public record Waypoint(int E, int N);

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