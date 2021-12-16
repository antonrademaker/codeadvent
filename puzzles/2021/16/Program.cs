using System.Collections;
using System.Diagnostics;
using System.Text;

var inputFiles = new string[] {
    "input/example.txt",
    "input/input.txt"
};
var sw = new Stopwatch();

foreach (var exampleFile in inputFiles)
{
    Console.WriteLine(exampleFile);
    var file = File.ReadAllLines(exampleFile);
    sw.Start();
    foreach (var line in file)
    {
        CalculatePart1(line);
    }
    sw.Stop();
    Console.WriteLine($"Part 1 in {sw.ElapsedMilliseconds}ms");
    sw.Reset();
    sw.Start();
    foreach (var line in file)
    {
        CalculatePart2(line);
    }

    sw.Stop();
    Console.WriteLine($"Part 2 in {sw.ElapsedMilliseconds}ms");

    Console.WriteLine("-- End of file--");
}

void CalculatePart1(string line)
{
    var reader = new InputReader(line);
    // reader.Print();
    var mainPacket = PacketFactory.ReadPacket(reader);
    Console.WriteLine($"Sum of versions: {mainPacket.GetVersionSum()}");
}

void CalculatePart2(string line)
{
    var reader = new InputReader(line);
    //reader.Print();
    var mainPacket = PacketFactory.ReadPacket(reader);
    Console.WriteLine($"Result: {mainPacket.GetResult()}");
}

public static class PacketFactory
{
    public static readonly Dictionary<long, Func<Packet[], long>> Operators = new()
    {
        { 0, x => x.Sum(t => t.GetResult()) },
        { 1, x => x.Aggregate(1L, (acc, t) => acc * t.GetResult()) },
        { 2, x => x.Min(t => t.GetResult()) },
        { 3, x => x.Max(t => t.GetResult()) },
        { 5, x => x[0].GetResult() > x[1].GetResult() ? 1L : 0L },
        { 6, x => x[0].GetResult() < x[1].GetResult() ? 1L : 0L },
        { 7, x => x[0].GetResult() == x[1].GetResult() ? 1L : 0L },
    };

    public static Packet ReadPacket(InputReader reader)
    {
        var version = reader.ReadVersion();
        var typeId = reader.ReadTypeId();

        switch (typeId)
        {
            case 4L:
                // literal
                var literal = reader.ReadLiteral();

                return new LiteralPacket
                {
                    Version = version,
                    TypeId = typeId,
                    Literal = literal

                };
            default:
                // Operator
                return ReadOperator(reader, version, typeId);

        }
    }

    private static OperatorPacket ReadOperator(InputReader reader, long version, long typeId)
    {
        var lengthTypeID = reader.ReadBool();
        var subPackets = new List<Packet>();

        if (lengthTypeID)
        {
            var numberOfPacketsToRead = reader.ReadNumberOfSubPackets();

            for (var i = 0; i < numberOfPacketsToRead; i++)
            {
                subPackets.Add(ReadPacket(reader));
            }
        }
        else
        {
            var numberOfBitsToRead = reader.SubPacketsSize();
            var currentPosition = reader.Position;
            while (reader.Position < currentPosition + numberOfBitsToRead)
            {
                subPackets.Add(ReadPacket(reader));
            }
        }


        return new OperatorPacket
        {
            Version = version,
            TypeId = typeId,
            SubPackets = subPackets.ToArray(),
            Operator = Operators[typeId]
        };

    }
}

public abstract class Packet
{
    public long Version { get; init; }
    public long TypeId { get; init; }

    public abstract long GetVersionSum();
    public abstract long GetResult();
}

public class LiteralPacket : Packet
{
    public long Literal { get; init; }

    public override long GetVersionSum()
    {
        return Version;
    }

    public override long GetResult()
    {
        return Literal;
    }
}

public class OperatorPacket : Packet
{
    public Packet[] SubPackets { get; init; } = Array.Empty<Packet>();

    public Func<Packet[], long> Operator { get; init; } = subs => 0L;

    public override long GetVersionSum()
    {
        var version = Version + SubPackets.Sum(x => x.GetVersionSum());
        return version;
    }

    public override long GetResult()
    {
        return Operator(SubPackets);
    }
}

public class InputReader
{
    private readonly IReadOnlyDictionary<char, bool[]> MapHex = new Dictionary<char, bool[]>()
    {
        { '0', new bool[] { false, false, false, false } },
        { '1', new bool[] { false, false, false, true } },
        { '2', new bool[] { false, false, true, false } },
        { '3', new bool[] { false, false, true, true } },
        { '4', new bool[] { false, true, false, false } },
        { '5', new bool[] { false, true, false, true } },
        { '6', new bool[] { false, true, true, false } },
        { '7', new bool[] { false, true, true, true } },
        { '8', new bool[] { true, false, false, false } },
        { '9', new bool[] { true, false, false, true } },
        { 'A', new bool[] { true, false, true, false } },
        { 'B', new bool[] { true, false, true, true } },
        { 'C', new bool[] { true, true, false, false } },
        { 'D', new bool[] { true, true, false, true } },
        { 'E', new bool[] { true, true, true, false } },
        { 'F', new bool[] { true, true, true, true } }
    };

    private readonly string originalInput;

    private readonly IEnumerable<bool> inputStream;

    public int Position { get; private set; }

    public InputReader(string input)
    {
        originalInput = input;

        var inputEnumerator = input.SelectMany(x => MapHex[x]).GetEnumerator();

        inputStream = GetLive(inputEnumerator);
    }

    private IEnumerable<bool> GetLive(IEnumerator<bool> iterator)
    {
        while (iterator.MoveNext())
        {
            Position++;
            yield return iterator.Current;
        }
    }

    public long ReadVersion()
    {
        return ParseNumber(inputStream.Take(3));
    }
    public long ReadTypeId()
    {
        return ParseNumber(inputStream.Take(3));
    }
    public bool ReadBool()
    {
        return inputStream.Take(1).First();
    }

    public long ReadNumberOfSubPackets()
    {
        return ParseNumber(inputStream.Take(11));
    }

    public long SubPacketsSize()
    {
        return ParseNumber(inputStream.Take(15));
    }

    private static long ParseNumber(IEnumerable<bool> input)
    {
        var array = input.Reverse().ToArray();
        var arr = new BitArray(array);

        return GetIntFromBitArray(arr);
    }

    public long ReadLiteral()
    {
        var block = inputStream.Take(5).ToArray();

        var bits = new List<IEnumerable<bool>>();

        while (block[0])
        {
            bits.Add(block.Skip(1).Take(4));
            block = inputStream.Take(5).ToArray();
        }

        bits.Add(block.Skip(1).Take(4));

        var literal = ParseNumber(bits.SelectMany(t => t));
        return literal;
    }

    private static long GetIntFromBitArray(BitArray bitArray)
    {
        var array = new int[2];
        bitArray.CopyTo(array, 0);
        return (uint)array[0] + ((long)(uint)array[1] << 32);
    }

    public void Print()
    {
        var sb = new StringBuilder();
        foreach (var x in originalInput.SelectMany(x => MapHex[x]))
        {
            sb.Append(x ? '1' : '0');
        }
        Console.WriteLine(sb.ToString());
    }
}