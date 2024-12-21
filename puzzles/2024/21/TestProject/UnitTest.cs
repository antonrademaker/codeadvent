namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
029A
980A
179A
456A
379A
""";

    [Theory]
    [InlineData(Example1, 126384)]
    public void TestExamplesPart1(string inputString, int expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer1(input).Should().Be(expected);
    }

    private const string ExampleSolve1 = "029A";
    private const string ExampleSolve2 = "980A";
    private const string ExampleSolve3 = "179A";
    private const string ExampleSolve4 = "456A";
    private const string ExampleSolve5 = "379A";

    [Theory]
    [InlineData(ExampleSolve1, 68)]
    [InlineData(ExampleSolve2, 60)]
    [InlineData(ExampleSolve3, 68)]
    [InlineData(ExampleSolve4, 64)]
    [InlineData(ExampleSolve5, 64)]
    public void TestSolves(string inputString, int expected)
    {
        
        Program.Shortest(inputString, 0 , 2).Should().Be(expected);
    }


}
