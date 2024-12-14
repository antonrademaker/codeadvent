namespace TestProject;

using Coordinate = AoC.Utilities.Coordinate<long>;
using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
p=0,4 v=3,-3
p=6,3 v=-1,-3
p=10,3 v=-1,2
p=2,0 v=2,-1
p=0,0 v=1,3
p=3,0 v=-2,-2
p=7,6 v=-1,-3
p=3,0 v=-1,-2
p=9,3 v=2,3
p=7,3 v=-1,2
p=2,4 v=2,-3
p=9,5 v=-3,-3
""";
    private const string Example2 = """
p=2,4 v=2,-3
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
    [InlineData(Example1,11,7, 12)]
    [InlineData(Example2,11,7, 0)]
    public void TestExamplesPart1(string inputString, int width, int height, long expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine));

        Program.CalculateAnswer1(input, width, height, 100).Should().Be(expected);
    }

    [Theory]
    [InlineData(Example2, 11, 7, 0, 2,4)]
    [InlineData(Example2, 11, 7, 1, 4,1)]
    [InlineData(Example2, 11, 7, 2, 6,5)]
    [InlineData(Example2, 11, 7, 3, 8,2)]
    [InlineData(Example2, 11, 7, 4, 10,6)]
    [InlineData(Example2, 11, 7, 5, 1,3)]
    public void TestCalculatePositions(string inputString, int width, int height, int seconds, int expectedX, int expectedY)
    {
        var input = new Input(inputString.Split(Environment.NewLine));

        var positions = Program.CalculateRobotPositions(input, width, height, seconds);
        positions.Values.First().X.Should().Be(expectedX);
        positions.Values.First().Y.Should().Be(expectedY);
    }

    //[Theory]
    //[InlineData(Example1, 875318608908L)]
    //[InlineData(Example2, 0)]
    //[InlineData(Example3, 459236326669L)]
    //[InlineData(Example4, 0)]
    //[InlineData(Example5, 416082282239L)]
    //public void TestExamplesPart2(string inputString, long expected)
    //{
    //    var input = new Input(inputString.Split(Environment.NewLine));

    //    Program.CalculateAnswer2(input).Should().Be(expected);
    //}
}
