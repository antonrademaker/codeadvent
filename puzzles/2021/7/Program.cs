
var inputs = System.IO.File.ReadAllText("input/input.txt").Split(',').Select(int.Parse).OrderBy(t => t).ToList();
var lowerBound = inputs[0];
var upperBound = inputs[^1];

var histogram = Enumerable.Range(lowerBound, upperBound - lowerBound + 1).Select(i => inputs.Count(j => j == i)).ToList();

CalculatePart1(inputs, lowerBound, upperBound, histogram);
CalculatePart2(inputs, upperBound);

static void CalculatePart1(List<int> inputs, int lowerBound, int upperBound, List<int> histogram)
{
    var left = lowerBound;
    var leftNumbers = 0;
    var right = inputs.Sum();
    var rightNumbers = inputs.Count();

    var target = inputs[0];

    var minFuel = right + 1;

    while (target <= upperBound)
    {
        var toMove = histogram[target++];
        leftNumbers += toMove;
        rightNumbers -= toMove;

        left += leftNumbers;
        right -= rightNumbers;

        if (minFuel > left + right)
        {
            minFuel = left + right;
        }
        else
        {
            break;
        }
    }

    Console.WriteLine($"Part 1; Min fuel: {minFuel}, position: {target - 1}");
}


static void CalculatePart2(List<int> inputs, int maxDistance)
{    
    var fuelUsedCrab = new int[maxDistance];
    var requiredFuel = new int[1+maxDistance];
    // Pre calculate required fuels for each position
    for (int i = 1; i < requiredFuel.Length; i++)
    {
        requiredFuel[i] = requiredFuel[i - 1] + i;
    }

    for (int i = 0; i < fuelUsedCrab.Length; i++)
    {        
        fuelUsedCrab[i] = inputs.Sum(x => requiredFuel[Math.Abs(x - i)]);
    }

    Console.WriteLine($"Part 2; Min fuel: {fuelUsedCrab.Min()}");
}