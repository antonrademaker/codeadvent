using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Day18;

public class CalculatorTests
{
    [Fact]
    public void Add_Two()
    {
        var p1 = new Pair(
            new Pair(1, 1),
            new Pair(2, 1),
            0);

        // [[3,4],5]
        var p2 = new Pair(
            new Pair(
                new Pair(3, 2),
                new Pair(4, 2),
                1
            ),
            new Pair(5, 1),
            0
            );

        var result = Calculator.Add(p1, p2);

        var expected = new Pair(
            new Pair(
                new Pair(1, 2),
                new Pair(2, 2),
                1
            ),
            new Pair(
                new Pair(
                    new Pair(3, 3),
                    new Pair(4, 3),
                    1
                ),
                new Pair(5, 2),
                    1
                ),
            0
            );

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_Single()
    {
        var input = "[1,2]";

        var expected = new Pair(
            new Pair(1, 1),
            new Pair(2, 1),
            0);

        var result = Calculator.Parse(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_Complex()
    {
        var input = "[[3,4],5]";

        var expected = new Pair(
             new Pair(
                 new Pair(3, 2),
                 new Pair(4, 2),
                 1
             ),
             new Pair(5, 1),
             0
             );

        var result = Calculator.Parse(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Parse_Number()
    {
        var input = "12";

        var expected = new Pair(12, 0);
        var result = Calculator.Parse(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Print_Number()
    {
        var input = new Pair(12, 0);
        var sb = new StringBuilder();
        Calculator.Print(input, sb);
        var result = sb.ToString();

        Assert.Equal("12", result);
    }


    [Fact]
    public void Print_Single()
    {
        var input = new Pair(
        new Pair(1, 1),
        new Pair(2, 1),
        0);
        var sb = new StringBuilder();
        Calculator.Print(input, sb);
        var result = sb.ToString();

        Assert.Equal("[1,2]", result);
    }

    [Fact]
    public void Print_Complex()
    {

        var input = new Pair(
             new Pair(
                 new Pair(3, 2),
                 new Pair(4, 2),
                 1
             ),
             new Pair(5, 1),
             0
             );


        var sb = new StringBuilder();
        Calculator.Print(input, sb);
        var result = sb.ToString();

        Assert.Equal("[[3,4],5]", result);
    }

    public static TheoryData<string, string, int, int, bool> ExplodeData =>
    new TheoryData<string, string, int, int, bool>
        {
        {"[1,2]","[1,2]", 0,0, false },
        {"[[[[[9,8],1],2],3],4]","[[[[0,9],2],3],4]", 9,0, true },
        {"[7,[6,[5,[4,[3,2]]]]]","[7,[6,[5,[7,0]]]]", 0,2, true },
        {"[[6,[5,[4,[3,2]]]],1]","[[6,[5,[7,0]]],3]", 0,0, true},
        {"[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]","[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", 0,0, true },
        {"[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]","[[3,[2,[8,0]]],[9,[5,[7,0]]]]", 0,2, true },
        };

    [Theory]
    [MemberData(nameof(ExplodeData))]
    public void Explode_Simple(string inp, string expected, int leftExpected, int rightExpected, bool updateExpected)
    {
        var input = Calculator.Parse(inp);

        var (result, left, right, updated) = Calculator.Explode(input);

        var output = Calculator.Print(result);

        Assert.Equal(expected, output);
        Assert.Equal(leftExpected, left);
        Assert.Equal(rightExpected, right);
        Assert.Equal(updateExpected, updated);
    }
    [Fact]
    public void Explode_Simple2()
    {
        var input =
             new Pair(
                 new Pair(9, 5),
                 new Pair(8, 5),
                 4
             );

        var (result, left, right, updated) = Calculator.Explode(input);

        Assert.True(updated);
        Assert.Equal(9, left);
        Assert.Equal(8, right);

        var expected = new Pair(0, 4);

        Assert.Equal(expected, result);

    }

    [Fact]
    public void Explode_Simple3()
    {
        var input =
            new Pair(
             new Pair(
                 new Pair(9, 5),
                 new Pair(8, 5),
                 4
             ),
             new Pair(2, 4),
             3)
             ;

        var (result, left, right, updated) = Calculator.Explode(input);

        Assert.True(updated);
        Assert.Equal(9, left);
        Assert.Equal(0, right);

        var expected = new Pair(new Pair(0, 4), new Pair(10, 4), 3);

        Assert.Equal(expected, result);

    }


    public static TheoryData<string[], string> AddsData =>
    new TheoryData<string[], string>
        {
        { new [] { "[1,1]",
"[2,2]",
"[3,3]",
"[4,4]"
        }, "[[[[1,1],[2,2]],[3,3]],[4,4]]" },
        {
        new []
        {
"[1,1]",
"[2,2]",
"[3,3]",
"[4,4]",
"[5,5]"
        }, "[[[[3,0],[5,3]],[4,4]],[5,5]]"
        }, {
        new []
        {
"[1,1]",
"[2,2]",
"[3,3]",
"[4,4]",
"[5,5]",
"[6,6]"
        }, "[[[[5,0],[7,4]],[5,5]],[6,6]]"
        },
        { new [] { "[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]", "[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]" }, "[[[[4,0],[5,4]],[[7,7],[6,0]]],[[8,[7,7]],[[7,9],[5,0]]]]" },

{ new [] { "[[[[4,0],[5,4]],[[7,7],[6,0]]],[[8,[7,7]],[[7,9],[5,0]]]]", "[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]" }, "[[[[6,7],[6,7]],[[7,7],[0,7]]],[[[8,7],[7,7]],[[8,8],[8,0]]]]" },

{ new [] { "[[[[6,7],[6,7]],[[7,7],[0,7]]],[[[8,7],[7,7]],[[8,8],[8,0]]]]", "[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]" }, "[[[[7,0],[7,7]],[[7,7],[7,8]]],[[[7,7],[8,8]],[[7,7],[8,7]]]]" },

{ new [] { "[[[[7,0],[7,7]],[[7,7],[7,8]]],[[[7,7],[8,8]],[[7,7],[8,7]]]]", "[7,[5,[[3,8],[1,4]]]]" }, "[[[[7,7],[7,8]],[[9,5],[8,7]]],[[[6,8],[0,8]],[[9,9],[9,0]]]]" },

{ new [] { "[[[[7,7],[7,8]],[[9,5],[8,7]]],[[[6,8],[0,8]],[[9,9],[9,0]]]]", "[[2,[2,2]],[8,[8,1]]]" }, "[[[[6,6],[6,6]],[[6,0],[6,7]]],[[[7,7],[8,9]],[8,[8,1]]]]" },

{ new [] { "[[[[6,6],[6,6]],[[6,0],[6,7]]],[[[7,7],[8,9]],[8,[8,1]]]]", "[2,9]" }, "[[[[6,6],[7,7]],[[0,7],[7,7]]],[[[5,5],[5,6]],9]]" },

{ new [] { "[[[[6,6],[7,7]],[[0,7],[7,7]]],[[[5,5],[5,6]],9]]", "[1,[[[9,3],9],[[9,0],[0,7]]]]" }, "[[[[7,8],[6,7]],[[6,8],[0,8]]],[[[7,7],[5,0]],[[5,5],[5,6]]]]" },

{ new [] { "[[[[7,8],[6,7]],[[6,8],[0,8]]],[[[7,7],[5,0]],[[5,5],[5,6]]]]", "[[[5,[7,4]],7],1]" }, "[[[[7,7],[7,7]],[[8,7],[8,7]]],[[[7,0],[7,7]],9]]" },

{ new [] { "[[[[7,7],[7,7]],[[8,7],[8,7]]],[[[7,0],[7,7]],9]]", "[[[[4,2],2],6],[8,7]]" }, "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]" },

        {
            new [] {
"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]",
"[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]",
"[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]",
"[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]",
"[7,[5,[[3,8],[1,4]]]]",
"[[2,[2,2]],[8,[8,1]]]",
"[2,9]",
"[1,[[[9,3],9],[[9,0],[0,7]]]]",
"[[[5,[7,4]],7],1]",
"[[[[4,2],2],6],[8,7]]"
        }, "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]"
        }
        };

    [Theory]
    [MemberData(nameof(AddsData))]
    public void Adds(string[] pairs, string expected)
    {
        var result = Calculator.Parse(pairs[0]);
        for (var index = 1; index < pairs.Length; index++)
        {
            result = Calculator.Add(result, Calculator.Parse(pairs[index]));
        }


        Assert.Equal(expected, Calculator.Print(result));
    }



    public static TheoryData<string, long> MagnitudeData =>
    new TheoryData<string, long>
    {
        { "[[1,2],[[3,4],5]]", 143L },
        { "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", 1384L },
{ "[[[[1,1],[2,2]],[3,3]],[4,4]]", 445L },
{ "[[[[3,0],[5,3]],[4,4]],[5,5]]", 791L },
{ "[[[[5,0],[7,4]],[5,5]],[6,6]]", 1137L },
{ "[[[[8,7],[7,7]],[[8,6],[7,7]]],[[[0,7],[6,6]],[8,7]]]", 3488L }
    };



    [Theory]
    [MemberData(nameof(MagnitudeData))]
    public void Magnitude(string input, long expected)
    {
        var parsed = Calculator.Parse(input);
        var calculated = Calculator.CalculateMagnitude(parsed);
        Assert.Equal(expected, calculated);
    }

}




