namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400

Button A: X+26, Y+66
Button B: X+67, Y+21
Prize: X=12748, Y=12176

Button A: X+17, Y+86
Button B: X+84, Y+37
Prize: X=7870, Y=6450

Button A: X+69, Y+23
Button B: X+27, Y+71
Prize: X=18641, Y=10279
""";
    private const string Example2 = """
Button A: X+94, Y+34
Button B: X+22, Y+67
Prize: X=8400, Y=5400
""";

    private const string Example3 = """
Button A: X+26, Y+66
Button B: X+67, Y+21
Prize: X=12748, Y=12176
""";

    private const string Example4 = """
Button A: X+17, Y+86
Button B: X+84, Y+37
Prize: X=7870, Y=6450
""";

    private const string Example5 = """
Button A: X+69, Y+23
Button B: X+27, Y+71
Prize: X=18641, Y=10279
""";

    [Theory]
    [InlineData(Example1, 140)]
    [InlineData(Example2, 1930)]
    [InlineData(Example3, 4)]
    [InlineData(Example4, 12)]
    [InlineData(Example5, 12)]
    public void TestExample1Part1(string inputString, long expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine));

        Program.CalculateAnswer1(input).Should().Be(expected);
    }

    [Theory]
    [InlineData(Example1, 80)]
    public void TestExamplePart2(string inputString, int expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine));

        Program.CalculateAnswer2(input).Should().Be(expected);
    }
}
