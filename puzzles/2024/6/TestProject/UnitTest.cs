namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    [Fact]
    public void TestExamplePart1()
    {
        var inputText = """
....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...
""";

        var input = new Input(inputText.Split(Environment.NewLine));

        Program.CalculateAnswer1(input).Should().Be(41);
    }

    [Fact]
    public void TestExamplePart2()
    {
        var inputText = """
....#.....
.........#
..........
..#.......
.......#..
..........
.#..^.....
........#.
#.........
......#...
""";

        var input = new Input(inputText.Split(Environment.NewLine));

        Program.CalculateAnswer2(input).Should().Be(6);
    }

    [InlineData("AllanTaylor314", """
....
#..#
.^#.
""", 1)]
    [InlineData("Soultaker1", """
####
...#
^###
""", 0)]

    [InlineData("Soultaker2", """
#######
##...##
##.#.##
#......
##^####
""", 7)]
    [Theory]

    public void TestsPart2(string name, string inputText, int expected)
    {
        Console.WriteLine($"Running test from {name}");
        var input = new Input(inputText.Split(Environment.NewLine));
        Program.CalculateAnswer2(input).Should().Be(expected);
    }

}
