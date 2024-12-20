﻿namespace TestProject;

using Coordinate = AoC.Utilities.Coordinate<long>;
using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
###############
#.......#....E#
#.#.###.#.###.#
#.....#.#...#.#
#.###.#####.#.#
#.#.#.......#.#
#.#.#####.###.#
#...........#.#
###.#.#####.#.#
#...#.....#.#.#
#.#.#.###.#.#.#
#.....#...#.#.#
#.###.#.#.#.#.#
#S..#.....#...#
###############
""";
    private const string Example2 = """
#################
#...#...#...#..E#
#.#.#.#.#.#.#.#.#
#.#.#.#...#...#.#
#.#.#.#.###.#.#.#
#...#.#.#.....#.#
#.#.#.#.#.#####.#
#.#...#.#.#.....#
#.#.#####.#.###.#
#.#.#.......#...#
#.#.###.#####.###
#.#.#...#.....#.#
#.#.#.#####.###.#
#.#.#.........#.#
#.#.#.#########.#
#S#.............#
#################
""";

    [Theory]
    [InlineData(Example1, 7036)]
    [InlineData(Example2, 11048)]
    public void TestExamplesPart1(string inputString, long expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine));

        Program.CalculateAnswer1(input).Should().Be(expected);
    }

    private const string Example3 = """
######
##...#
##.#.#
#S.#E#
##.#.#
##...#
######
""";


    [Theory]
    [InlineData(Example1, 45)]
    [InlineData(Example2, 64)]
    [InlineData(Example3, 13)]
    public void TestExamplesPart2(string inputString, long expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine));

        Program.CalculateAnswer2(input).Should().Be(expected);
    }
}
