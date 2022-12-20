using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var runDebug = false;

        var files = Directory.GetFiles("input", "*.txt");

        foreach (var file in files.OrderBy(t => t))
        {
            Console.WriteLine($"{file}");

            var lines = File.ReadAllLines(file);

            var positions = lines.Select(t => int.Parse(t)).ToArray();
            Data[] data = ParseData(positions);

            Console.WriteLine($"{data.Length}");
            var dataLength = data.Length;
            if (runDebug)
            {
                Print(data[0], dataLength);
            }


            var half = dataLength / 2;

            Data pointZero = data[0];

            for (var i = 0; i < data.Length; i++)
            {
                var movePositions = data[i].Value % dataLength;

                if (Math.Abs(movePositions) > half)
                {
                    // switch direction

                    movePositions = (dataLength - movePositions) % dataLength;
                }

                var cur = data[i];

                Debug.Assert(cur != null);

                if (movePositions != 0)
                {
                    cur.Before.After = cur.After;
                    cur.After.Before = cur.Before;

                    if (movePositions > 0)
                    {
                        for (var counter = 0; counter < movePositions; counter++)
                        {
                            cur = cur.After;
                        }

                        cur.MixAfter(data[i]);

                    }
                    else if (movePositions < 0)
                    {
                        for (var counter = 0; counter > movePositions; counter--)
                        {
                            cur = cur.Before;
                        }
                        cur.MixBefore(data[i]);
                    }
                }
                else
                {
                    pointZero = data[i];
                }

                if (runDebug)
                {
                    Print(data[0], dataLength);
                }
            }
            // var mixedData = new Data[data.Length];

            var curPointer = pointZero;

            var searchLocations = new HashSet<int> { 1000 % dataLength, 2000 % dataLength, 3000 % dataLength };

            var c = 0;
            var sum = 0;
            while (c < dataLength)
            {
                if (searchLocations.Contains(c))
                {
                    searchLocations.Remove(c);
                    sum += curPointer.Value;
                }

                curPointer = curPointer.After;

                c++;
            }

            Console.WriteLine($"Sum: {sum}");
            //if (runDebug)
            //{
            //    break;
            //}
        }
    }

    private static Data[] ParseData(int[] positions)
    {
        var data = new Data[positions.Length];

        data[0] = new Data(positions[0]);

        for (var i = 1; i < positions.Length; i++)
        {
            data[i] = new Data(positions[i], data[i - 1]);
        }

        data[^1].After = data[0];
        data[0].Before = data[^1];
        return data;
    }

    private static void Print(Data data, int dataLength)
    {
        var str = string.Join(',', ToValues(data, dataLength));

        Console.WriteLine(str);
    }

    private static IEnumerable<int> ToValues(Data data, int dataLength)
    {
        var current = data;

        var cnt = 0;

        do
        {
            yield return current.Value;
            current = current.After;
        } while (current != data && cnt++ < dataLength);
    }
}

[DebuggerDisplay("Val: {Value}, Before.Val: {Before.Value}, After.Val: {After.Value}")]
internal sealed class Data
{
    public Data(int value)
    {
        Value = value;
        Before = this;
        After = this;
    }

    public Data(int value, Data before)
    {
        Value = value;
        Before = before;
        before.InitializeAfter(this);
    }

    public int Value { get; }

    public Data Before { get; set; }
    public Data After { get; set; }

    private void InitializeAfter(Data data)
    {
        After = data;
    }

    public void MixAfter(Data data)
    {
        After.Before = data;

        data.After = After;
        data.Before = this;

        After = data;
    }

    public void MixBefore(Data data)
    {
        Before.After = data;
        data.Before = Before;
        data.After = this;
        Before = data;
    }
}