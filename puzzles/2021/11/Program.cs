const bool EnableLogging = false;

var file = File.ReadAllLines("input/input.txt");

var rows = file.Length;
var columns = file[0].Length;

var size = columns * rows;

var grid = new Grid(columns);

var p = 0;

foreach (var row in file)
{
    foreach (var octopus in row.Select(x => x - '0'))
    {
        grid.Locations[p++] = octopus;
    }
}

if (EnableLogging)
{
    Console.WriteLine("Before any steps:");
    grid.Print();
}
var all = false;

int step = 0;

int totalFlashingAfter100 = 0;


while (!all)
{

    
    all = grid.CalculateNextStep();

    if (EnableLogging)
    {
        Console.WriteLine($"After step: {step + 1}");
        grid.Print();
    }

    step++;
    if (step == 100)
    {
        
        totalFlashingAfter100 = grid.TotalFlashing;
    }
}

Console.WriteLine($"Part 1: {grid.TotalFlashing}");
Console.WriteLine($"Part 2: {step}");

public class Grid
{
    const int Flashing = 10;


    public Grid(int columns)
    {
        PositionMask = new[]
        {
             -(columns + 1), // top left
             -1, // left
            
            columns - 1, // bottom left
            
             -(columns), // top
             columns, // bottom
            -(columns - 1), // top right
             
            +1, // right
             columns + 1 // bottom right
         };

        Size = columns * columns;
        Width = columns;

        Locations = new int[Size];
    }

    public readonly int[] PositionMask;

    public int Size { get; }
    public int Width { get; }

    public int[] Locations { get; }

    public int TotalFlashing { get; private set; }

    public bool CalculateNextStep()
    {
        var flashing = 0;
        for (var i = Size - 1; i >= 0; i--)
        {
            IncrementAndCheckFlashing(i);
        }

        for (var i = Size - 1; i >= 0; i--)
        {
            if (Locations[i] == Flashing)
            {
                flashing++;
                Locations[i] = 0;
            }
        }

        TotalFlashing += flashing;

        return flashing == 100;
    }

    public void IncrementAndCheckFlashing(int location)
    {
        if (Locations[location] < Flashing)
        {
            Locations[location]++;

            if (Locations[location] == Flashing)
            {
                var rowPos = location % Width;

                foreach (var mask in PositionMask.Skip(rowPos == 0 ? 3 : 0).Take(rowPos == Width - 1 ? 5 : Width))
                {
                    var p = mask + location;


                    if (p >= 0 && p < Size && Locations[p] < Flashing)
                    {
                        IncrementAndCheckFlashing(p);
                    }
                }
            }
        }
    }

    public void Print()
    {
        for (int i = 0; i < Locations.Length; i++)
        {
            Console.Write(Locations[i]);
            if (i % Width == Width - 1)
            {
                Console.WriteLine();
            }
        }
        Console.WriteLine();

    }
}



