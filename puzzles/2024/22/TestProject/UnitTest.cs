namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
1
10
100
2024
""";

    [Theory]
    [InlineData(Example1, 37327623)]
    public void TestExamplesPart1(string inputString, long expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer1(input).Should().Be(expected);
    }

    [Theory]
    [InlineData("123", 0, 123)]
    [InlineData("123", 1, 15887950)]
    [InlineData("123", 2, 16495136)]
    [InlineData("123", 3, 527345)]
    [InlineData("123", 4, 704524)]
    [InlineData("123", 5, 1553684)]
    [InlineData("123", 6, 12683156)]
    [InlineData("123", 7, 11100544)]
    [InlineData("123", 8, 12249484)]
    [InlineData("123", 9, 7753432)]
    [InlineData("123", 10, 5908254)]
    public void TestShortExamplesPart1(string inputString, int steps, long expected)
    {
        var input = new Input(inputString);

        Program.Calculate(input, steps).part1.Should().Be(expected);
    }

    [Fact]
    public void TestMix()
    {
        Program.Mix(42, 15).Should().Be(37);
    }


    [Fact]
    public void TestPrune()
    {
        Program.Prune(100000000).Should().Be(16113920);
    }


    private const string Example2 = """
1
2
3
2024
""";
    private const string Example3 = """
123
""";
    [Theory]
   [InlineData(Example2, 23)]
    //[InlineData(Example3, 23)]
    public void TestSolves(string inputString, int expected)
    {
        var input = new Input(inputString);

        Program.Calculate(input, 2000).part2.Should().Be(expected);
    }


}
