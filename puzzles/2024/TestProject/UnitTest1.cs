﻿namespace TestProject;

using FluentAssertions;

public class UnitTest1
{
    [Fact]
    public void TwoAntennas()
    {
        var inputText = """
..........
..........
..........
....a.....
..........
.....a....
..........
..........
..........
..........
""";

        var input = new Input(inputText.Split(Environment.NewLine));

        Program.CalculateAnswer1(input).Should().Be(2);

    }

    [Fact]
    public void ThreeAntennas()
    {
        var inputText = """
..........
..........
..........
....a.....
........a.
.....a....
..........
..........
..........
..........
""";

        var input = new Input(inputText.Split(Environment.NewLine));

        Program.CalculateAnswer1(input).Should().Be(3);

    }



}
