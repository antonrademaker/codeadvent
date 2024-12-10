namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 =
"""
89010123
78121874
87430965
96549874
45678903
32019012
01329801
10456732
""";
    private const string Example2 =
"""
...0...
...1...
...2...
6543456
7.....7
8.....8
9.....9
""";
    private const string Example3 =
"""
..90..9
...1.98
...2..7
6543456
765.987
876....
987....
""";
    private const string Example4 =
"""
10..9..
2...8..
3...7..
4567654
...8..3
...9..2
.....01
""";

    [Theory]
    [InlineData(Example1, 36)]
    [InlineData(Example2, 2)]
    [InlineData(Example3, 4)]
    [InlineData(Example4, 3)]
    public void TestExamplePart1(string inputString, int expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine));

        Program.CalculateAnswer1(input).Should().Be(expected);
    }

    private const string Example1Part2 =
"""
.....0.
..4321.
..5..2.
..6543.
..7..4.
..8765.
..9....
""";

    private const string Example2Part2 =
"""
..90..9
...1.98
...2..7
6543456
765.987
876....
987....
""";
    
    private const string Example3Part2 =
"""
012345
123456
234567
345678
4.6789
56789.
""";


    [Theory]
    [InlineData(Example1Part2, 3)]
    [InlineData(Example2Part2, 13)]
    [InlineData(Example3Part2, 227)]
    [InlineData(Example1, 81)]
    public void TestExamplePart2(string inputString, int expected)
    {
        var input = new Input(inputString.Split(Environment.NewLine));

        Program.CalculateAnswer2(input).Should().Be(expected);
    }
}
