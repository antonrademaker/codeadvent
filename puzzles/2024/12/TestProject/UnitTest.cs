namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
AAAA
BBCD
BBCC
EEEC
""";

    private const string Example2 = """
RRRRIICCFF
RRRRIICCCF
VVRRRCCFFF
VVRCCCJFFF
VVVVCJJCFE
VVIVCCJJEE
VVIIICJJEE
MIIIIIJJEE
MIIISIJEEE
MMMISSJEEE
""";

    private const string Example3 = """
A
""";

    private const string Example4 = """
AA
""";
    private const string Example5 = """
AABB
""";

    [Theory]
    [InlineData(Example1, 140)]
    [InlineData(Example2, 1930)]
    [InlineData(Example3, 4)]
    [InlineData(Example4, 12)]
    [InlineData(Example5, 24)]
    public void TestExample1Part1(string inputString, long expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine));

        Program.CalculateAnswer1(input).Should().Be(expected);
    }

    private const string Example6 = """
AAAAAA
AAABBA
AAABBA
ABBAAA
ABBAAA
AAAAAA
""";


    [Theory]
    [InlineData(Example1, 80)]
    [InlineData(Example2, 1206)]
    [InlineData(Example6, 368)]
    public void TestExamplePart2(string inputString, int expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine));

        Program.CalculateAnswer2(input).Should().Be(expected);
    }
}
