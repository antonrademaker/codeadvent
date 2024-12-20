namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
###############
#...#...#.....#
#.#.#.#.#.###.#
#S#...#.#.#...#
#######.#.#.###
#######.#.#...#
#######.#.###.#
###..E#...#...#
###.#######.###
#...###...#...#
#.#####.#.###.#
#.#...#.#.#...#
#.#.#.#.#.#.###
#...#...#...###
###############
""";
  
    [Theory]
    [InlineData(Example1, 5, 20)]
    public void TestExamplesPart1(string inputString, int cutOff, int expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer1(input, cutOff).Should().Be(expected);
    }

    [Theory]
    [InlineData(Example1, 16)]
    public void TestExamplesPart2(string inputString, long expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer2(input).Should().Be(expected);
    }
}
