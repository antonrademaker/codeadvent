namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
#####
.####
.####
.####
.#.#.
.#...
.....

#####
##.##
.#.##
...##
...#.
...#.
.....

.....
#....
#....
#...#
#.#.#
#.###
#####

.....
.....
#.#..
###..
###.#
###.#
#####

.....
.....
.....
#....
#.#..
#.#.#
#####
""";

    [Theory]
    [InlineData(Example1, 3)]
    public void TestExamplesPart1(string inputString, long expected)
    {
        var input = new Input(inputString);

        Program.CalculateAnswer1(input).Should().Be(expected);
    }

//    private const string Example2 = """
//1
//2
//3
//2024
//""";
//    private const string Example3 = """
//123
//""";
//    [Theory]
//   [InlineData(Example2, 23)]
//    //[InlineData(Example3, 23)]
//    public void TestSolves(string inputString, int expected)
//    {
//        var input = new Input(inputString);

//        Program.Calculate(input, 2000).part2.Should().Be(expected);
//    }


}
