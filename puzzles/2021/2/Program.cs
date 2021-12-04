
var ops = File.ReadAllLines("inputs/input.txt").Select(line => {
    var splitted = line.Split(' ');
    var operation = splitted[0];  
    var value = int.Parse(splitted[1]);
    return (operation, value);
}).ToList();

long depth = 0;
long position = 0;

foreach (var line in ops)
{
    if (line.operation == "forward")
    {
        position += line.value;
    } else if (line.operation == "down")
    {
        depth += line.value;
    } else
    {
        depth -= line.value;
    }
}

Console.WriteLine($"Depth: {depth}, y position: {position}, multiply: {depth * position}");


depth = 0;
position = 0;
long aim = 0;

foreach (var line in ops)
{
    if (line.operation == "forward")
    {
        position += line.value;
        depth += aim * line.value;
    }
    else if (line.operation == "down")
    {
        aim += line.value;
    }
    else
    {
        aim -= line.value;
    }
}

Console.WriteLine($"Depth: {depth}, y position: {position}, multiply: {depth * position}");
