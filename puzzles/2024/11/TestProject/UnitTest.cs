namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = "125 17";
    

    [Theory]
    [InlineData(Example1, 55312)]    
    public void TestExamplePart1(string inputString, long expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer1(input).Should().Be(expected);
    }

    //[Theory]
    ////[InlineData(Example1Part2, 3)]
    ////[InlineData(Example2Part2, 13)]
    ////[InlineData(Example3Part2, 227)]
    //[InlineData(Example1, 81)]
    //public void TestExamplePart2(string inputString, int expected)
    //{
    //    var input = new Input(inputString);

    //    Program.CalculateAnswer2(input).Should().Be(expected);
    //}
}
