using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var lines = File.ReadAllLines("input/input.txt")
    .Select(line => line.Split('|', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries))
    .Select(line => new Display(line[0].Split(' '), line[1].Split(' ')));

var part1 = 0;
var part2 = 0;



foreach (var displayLine in lines)
{
    part1 += displayLine.Count(1) + displayLine.Count(4) + displayLine.Count(7) + displayLine.Count(8);

    var digit7 = displayLine.Digits.First(digit => digit.Number == 7);

    var keep = new List<int> { 0, 3, 9 };
    foreach (var digit in displayLine.ToSolve.Where(digit => ((digit.Segments & digit7.Segments) == digit7.Segments)))
    {
        digit.Candidates = digit.Candidates.Where(n => keep.Contains(n)).ToList();
    }

    var digit3 = displayLine.ToSolve.First(t => t.Candidates.Count == 1 && t.Candidates.First() == 3);

    digit3.Number = 3;

    displayLine.ToSolve.First(t => t.Candidates.Contains(6)).Number = 6;
    displayLine.ToSolve.Where(d => d.Candidates.All(x => x == 0 || x == 9)).First(t => (t.Segments & digit3.Segments) == digit3.Segments).Number = 9;
    displayLine.ToSolve.Where(d => d.Candidates.All(x => x == 0 || x == 9)).First(t => (t.Segments & digit3.Segments) != digit3.Segments).Number = 0;

    var segmentOf5 = displayLine.Digits.Single(t => t.Number == 9).Segments ^ digit3.Segments;

    displayLine.ToSolve.First(t => (t.Segments & segmentOf5) == segmentOf5).Number = 5;

    displayLine.ToSolve.First(t => (t.Segments & segmentOf5) != segmentOf5).Number = 2;

    var display = displayLine.DisplayDigits.Aggregate(0, (acc, digit) => (acc * 10) + digit.Number!.Value);

    Console.WriteLine($"display: {display} | {string.Join(" ", displayLine.DisplayDigits.Select(t => t.Number))}");

    part2 += display;
}

Console.WriteLine($"Part 1: {part1}");
Console.WriteLine($"Part 2: {part2}");

class Display
{
    public Display(string[] digits, string[] display)
    {
        Digits = digits.Select(d => new Digit(d)).ToArray();

        DisplayDigits = display.Select(s => s.Parse()).Select(d => Digits.First(t => t.Segments == d)).ToArray();
    }

    public Digit[] Digits { get; init; }
    public Digit[] DisplayDigits { get; init; }

    public IEnumerable<Digit> ToSolve => Digits.Where(digit => !digit.Number.HasValue);

    public int Count(int? digit)
    {
        return DisplayDigits.Count(t => t.Number == digit);
    }
}

[DebuggerDisplay("Number: {number}, initial: {Initial}, Segments: {Segments}")]
class Digit
{
    public Segment Segments { get; set; }

    private int? number;
    public int? Number
    {
        get => number;
        set
        {
            if (!value.HasValue)
            {
                throw new Exception("Cannot set number to null");
            }
            Candidates.Clear();
            Candidates.Add(value.Value);
            number = value;
        }
    }

    public List<int> Candidates = new();

    public Digit(string initial)
    {
        Initial = string.Concat(initial.OrderBy(t => t));

        Segments = initial.Parse();

        switch (Initial.Length)
        {
            case 2:
                Number = 1;
                break;
            case 3:
                Number = 7;
                break;
            case 4:
                Number = 4;
                break;
            case 5:
                Candidates = new List<int> { 2, 3, 5 };
                break;
            case 6:
                Candidates = new List<int> { 0, 6, 9 };
                break;
            case 7:
                Number = 8;
                break;
            default:
                throw new Exception($"{Initial.Length} not implemented yet");
        }
        Console.WriteLine($"Init: {initial}, {Initial.Length}: Number: {Number}");
    }

    public string Initial { get; }

    public Segment Substract(Digit other)
    {

        return other.Segments ^ Segments;
    }
}


[Flags]
public enum Segment
{
    none = 0,
    a = 1,
    b = 2,
    c = 4,
    d = 8,
    e = 16,
    f = 32,
    g = 64
}

public static class SegmentExtensions
{
    public static Segment Parse(this string input)
    {
        var segment = new Segment();
        var span = input.AsSpan();

        for (var p = 0; p < span.Length; p++)
        {
            segment |= Enum.Parse<Segment>(span.Slice(p, 1));
        }
        return segment;
    }
}
