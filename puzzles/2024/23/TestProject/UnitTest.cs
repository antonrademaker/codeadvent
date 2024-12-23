namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example1 = """
kh-tc
qp-kh
de-cg
ka-co
yn-aq
qp-ub
cg-tb
vc-aq
tb-ka
wh-tc
yn-cg
kh-ub
ta-co
de-co
tc-td
tb-wq
wh-td
ta-ka
td-qp
aq-cg
wq-ub
ub-vc
de-ta
wq-aq
wq-vc
wh-yn
ka-de
kh-ta
co-tc
wh-qp
tb-vc
td-yn
""";

    [Theory]
    [InlineData(Example1, 7)]
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
