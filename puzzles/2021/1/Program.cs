Console.WriteLine($"Part 1");

var depths = System.IO.File.ReadAllLines("input/a.txt").Select(line => int.Parse(line)).ToList();

var elevation = depths[0];
var increasedCount = 0;
foreach (var depth in depths.Skip(1)) {
    if (depth > elevation) {
        Console.WriteLine("increased");
        increasedCount++;
    } else if (depth < elevation) {
        Console.WriteLine("decreased");
    }
    elevation = depth;
}

Console.WriteLine($"Increased: {increasedCount}");

Console.WriteLine($"Part 2");

var elevationSum = depths[0] + depths[1] + depths[2];
elevation = elevationSum;
Console.WriteLine($"{elevationSum} (N/A - no previous sum)");
increasedCount = 0;
var numberOfDepthsToCheck = depths.Count - 1;
for (var i = 2; i < numberOfDepthsToCheck; i++) {    
    elevationSum -= depths[i-2];
    elevationSum += depths[i+1];
    Console.Write(elevationSum);
    if (elevationSum > elevation) {
        Console.WriteLine(" (increased)");
        increasedCount++;
    } else if (elevationSum < elevation) {
        Console.WriteLine(" (decreased)");
    } else {
        Console.WriteLine(" (no change)");
    }
    elevation = elevationSum;
}

Console.WriteLine($"Increased: {increasedCount}");