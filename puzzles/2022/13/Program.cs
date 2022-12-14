var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t))
{
    var fileInput = File.ReadAllLines(file);

    var parsed = fileInput.Where(t => !string.IsNullOrWhiteSpace(t)).Select(line => TryParse(line.AsSpan(), out var _)).ToList();

    Console.WriteLine($"File: {file}");

    Console.WriteLine($"Part 1: {Part1(parsed)}");
    Console.WriteLine($"Part 2: {Part2(parsed)}");
}

int Part1(List<IPackage> parsed)
{
    var sum = 0;
    for (var index = 0; index < parsed.Count; index += 2)
    {
        var pairIndex = ((index / 2) + 1);

        if (parsed[index].CompareTo(parsed[index + 1]) < 0)
        {
            sum += pairIndex;
        }
    }

    return sum;
}

int Part2(List<IPackage> parsed)
{
    var firstDivider = TryParse("[[2]]".AsSpan(), out var _);
    var secondDivider = TryParse("[[6]]".AsSpan(), out var _);

    var list = new List<IPackage>()
    {
         firstDivider,
         secondDivider
    };

    list.AddRange(parsed);

    var ordered = list.Order().ToList();

    var first = ordered.IndexOf(firstDivider) + 1;
    var second = ordered.IndexOf(secondDivider) + 1;

    return first * second;
}

IPackage TryParse(ReadOnlySpan<char> input, out int nextPosition)
{
    if (input[0] == '[')
    {
        var packages = new List<IPackage>();

        var start = 1;
        if (input[start] == ']')
        {
            nextPosition = start + 1;
            // empty array
            return new PacketArray();
        }
        while (true)
        {
            packages.Add(TryParse(input[start..], out var last));
            start += last;
            if (input[start] == ']')
            {
                nextPosition = start + 1;
                return new PacketArray(packages);
            }
            if (input[start] == ',')
            {
                start++;
            }
            else
            {
                throw new Exception($"Could not parse: (start {start}) {input[start]}");
            }
        }
    }
    else
    {
        var start = 0;
        var current = 0;

        while (input[current] >= '0' && input[current] <= '9')
        {
            current++;
        }
        nextPosition = current;
        return new Packet(int.Parse(input[start..current]));
    }
}

internal interface IPackage : IComparable<IPackage>
{
}

record struct Packet(int Value) : IPackage
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public int CompareTo(IPackage? other)
    {
        if (other is null)
        {
            return -1;
        }
        if (other is Packet otherPacket)
        {
            return Value.CompareTo(otherPacket.Value);
        }

        if (other is PacketArray otherArray)
        {
            return new PacketArray(this).CompareTo(otherArray);
        }

        return -1;
    }
}

internal class PacketArray : IPackage
{
    public PacketArray(IEnumerable<IPackage> packages)
    {
        Packages = packages.ToArray();
    }

    public PacketArray(IPackage packet)
    {
        Packages = new[] { packet };
    }

    public PacketArray()
    {
        Packages = Array.Empty<IPackage>();
    }

    public IPackage[] Packages { get; init; }

    public override string ToString()
    {
        return $"[{string.Join(",", Packages.Select(t => t.ToString()))}]";
    }

    public int CompareTo(IPackage? other)
    {
        if (other is null)
        {
            return -1;
        }
        if (other is Packet)
        {
            return CompareTo(new PacketArray(other));
        }

        if (other is PacketArray otherArray)
        {
            for (var i = 0; i < Packages.Length; i++)
            {
                if (i >= otherArray.Packages.Length)
                {
                    return 1;
                }
                var elementCompare = Packages[i].CompareTo(otherArray.Packages[i]);
                if (elementCompare != 0)
                {
                    return elementCompare;
                }
            }

            if (Packages.Length < otherArray.Packages.Length)
            {
                return -1;
            }
        }

        return 0;
    }
}