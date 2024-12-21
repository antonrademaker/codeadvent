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
    [InlineData(Example1, 10, 10)]
    public void TestExamplesPart1(string inputString, int cutoff, int expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer1(input, cutoff).Should().Be(expected);
    }

    [Theory]
    [InlineData(Example1, 50, 278)]
    public void TestExamplesPart2(string inputString, int cutoff, int expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer2(input, cutoff).Should().Be(expected);
    }
}
