namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
r, wr, b, g, bwu, rb, gb, br

brwrr
bggr
gbbr
rrbgbr
ubwu
bwurrg
brgr
bbrgwb
""";
  
    [Theory]
    [InlineData(Example1, 6)]
    public void TestExamplesPart1(string inputString, int expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer1(input).Should().Be(expected);
    }

    [Theory]
    [InlineData(Example1, 16)]
    public void TestExamplesPart2(string inputString, long expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer2(input).Should().Be(expected);
    }
}
