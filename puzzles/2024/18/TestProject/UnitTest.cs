namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
5,4
4,2
4,5
3,0
2,1
6,3
2,4
1,5
0,6
3,3
2,6
5,1
1,2
5,5
2,5
6,5
1,4
0,4
6,4
1,1
6,1
1,0
0,5
1,6
2,0
""";
  
    [Theory]
    [InlineData(Example1, 22)]
    public void TestExamplesPart1(string inputString, long expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine), 6, 6, 12);

        Program.CalculateAnswer1(input).Should().Be(expected);
    }
   
    [Theory]
    [InlineData(Example1, "6,1")]
    public void TestExamplesPart2(string inputString, string expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine), 6,6,int.MaxValue);

        Program.CalculateAnswer2(input).Should().Be(expected);
    }
}
