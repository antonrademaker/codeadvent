namespace TestProject;

using FluentAssertions;
using Solution;
using System.Text;

public class UnitTest
{
    private const string Example1 = """
Register A: 729
Register B: 0
Register C: 0

Program: 0,1,5,4,3,0
""";
    private const string OperatorExample1 = """
Register A: 0
Register B: 0
Register C: 9

Program: 2,6
""";

    private const string OperatorExample1Expected =
        """
        Register A: 0
        Register B: 1
        Register C: 9
        Output: 
        """;

    private const string OperatorExample2 = """
Register A: 10
Register B: 0
Register C: 0

Program: 5,0,5,1,5,4
""";

    private const string OperatorExample2Expected =
"""
Register A: 10
Register B: 0
Register C: 0
Output: 0,1,2
""";


    private const string OperatorExample3 = """
Register A: 2024
Register B: 0
Register C: 0

Program: 0,1,5,4,3,0
""";

    private const string OperatorExample3Expected =
"""
Register A: 0
Register B: 0
Register C: 0
Output: 4,2,5,6,7,7,7,7,3,1,0
""";

    private const string OperatorExample4 = """
Register A: 0
Register B: 29
Register C: 0

Program: 1,7
""";

    private const string OperatorExample4Expected =
"""
Register A: 0
Register B: 26
Register C: 0
Output: 
""";
    private const string OperatorExample5 = """
Register A: 0
Register B: 2024
Register C: 43690

Program: 4,0
""";

    private const string OperatorExample5Expected =
"""
Register A: 0
Register B: 44354
Register C: 43690
Output: 
""";

    private const string OperatorExample6 = """
Register A: 1
Register B: 2
Register C: 3

Program: 
""";

    private const string OperatorExample6Expected =
"""
Register A: 1
Register B: 2
Register C: 3
Output: 
""";

    [Theory]
    [InlineData(Example1, "4,6,3,5,6,3,5,2,1,0")]

    public void TestExamplesPart1(string inputString, string expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer1(input).GetOutput().Should().Be(expected);
    }

    [Theory]
    [InlineData(OperatorExample1, OperatorExample1Expected)]
    [InlineData(OperatorExample2, OperatorExample2Expected)]
    [InlineData(OperatorExample3, OperatorExample3Expected)]
    [InlineData(OperatorExample4, OperatorExample4Expected)]
    [InlineData(OperatorExample5, OperatorExample5Expected)]
    [InlineData(OperatorExample6, OperatorExample6Expected)]

    public void TestOperatorExamplesPart1(string inputString, string expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer1(input).ToString().Should().Be(expected);
    }

    private const string Example3 = """
Register A: 2024
Register B: 0
Register C: 0

Program: 0,3,5,4,3,0
""";


    [Theory]
    [InlineData(Example3, "117440")]
    public void TestExamplesPart2(string inputString, string expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer2(input).Should().Be(expected);
    }
}
