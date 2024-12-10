namespace TestProject;

using FluentAssertions;
using Solution;

public class UnitTest
{
    private const string Example = @"2333133121414131402";
    private const string ExampleShort = @"12345";

    [Fact]
    public void TestExamplePart1()
    {
        var input = new Input(Example);

        Program.CalculateAnswer1(input).Should().Be(1928);
    }

    [Fact]
    public void TestExamplePart2()
    {
        var input = new Input(Example);

        Program.CalculateAnswer2(input).Should().Be(2858);
    }

    [Fact]
    public void TestExampleShort()
    {
        var input = new Input(ExampleShort);

        var fs = input.GetFileSystem();

        Program.GetTextRepresentation(fs).Should().Be("0..111....22222");
    }

    //    [InlineData("AllanTaylor314", """
    //....
    //#..#
    //.^#.
    //""", 1)]
    //    [InlineData("Soultaker1", """
    //####
    //...#
    //^###
    //""", 0)]

    //    [InlineData("Soultaker2", """
    //#######
    //##...##
    //##.#.##
    //#......
    //##^####
    //""", 7)]
    //    [Theory]

    //    public void TestsPart2(string name, string inputText, int expected)
    //    {
    //        Console.WriteLine($"Running test from {name}");
    //        var input = new Input(inputText.Split(Environment.NewLine));
    //        Program.CalculateAnswer2(input).Should().Be(expected);
    //    }

}
