var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t))
{
    var fileInput = File.ReadAllLines(file);

    var field = Parse(fileInput);
    
    Console.WriteLine($"File: {file}, rock: {field.Count}");

//    Console.WriteLine($"Part 1: {Part1(parsed)}");
//    Console.WriteLine($"Part 2: {Part2(parsed)}");
}

Dictionary<Point, char> Parse(string[] input) {
    var field = new Dictionary<Point, char>();


    foreach (var line in input)
    {
        var points = line.Split(" -> ").Select(point => point.Split(',')).Select(point => new Point(int.Parse(point[0]), int.Parse(point[1]))).ToArray();

        var current = points[0];

        field[current] = '#';

        foreach(var nextPoint in points.Skip(1))
        {
            var direction = new Point(current.X == nextPoint.X ? 0 : (current.X < nextPoint.X ? 1 : -1),
                current.Y == nextPoint.Y ? 0 : (current.Y < nextPoint.Y ? 1 : -1)
                );

            while (current != nextPoint)
            {
                current += direction;
                field[current] = '#';

            }
        }

    }

    return field;
}

record struct Point(int X, int Y) {
    public static Point operator +(Point a, Point b) => a with { X = a.X + b.X, Y = a.Y + b.Y };
};