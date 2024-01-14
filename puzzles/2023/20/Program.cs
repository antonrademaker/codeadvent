namespace AoC.Puzzles.Y2023.D19;

public static partial class Program
{
    private const string FTModuleName = "ft";

    private static void Main(string[] args)
    {
        string[] inputFiles = [
            "input/example.txt",
             "input/example2.txt"
            , "input/input.txt"
            ];

        foreach (var file in inputFiles)
        {
            Console.WriteLine(file);

            var inputs = System.IO.File.ReadAllLines(file);

            var (part1, _) = Calculate(inputs, false);

            Console.WriteLine($"Part 1: {part1}");

            var (_, part2) = Calculate(inputs, true);

            Console.WriteLine($"Part 2: {part2}");
        }
    }

    private static (long part1, long part2) Calculate(string[] input, bool isPart2)
    {
        var modules = ParseInput(input);

        if (isPart2 && !modules.ContainsKey("rx"))
        {
            return (0, 0);
        }

        var buttonPulse = new Pulse("button", "broadcaster", false);

        var queue = new Queue<Pulse>();

        var highPulses = 0L;
        var lowPulses = 0L;

        var stepsToRx = 0L;
        var presses = 0;


        // only one modules has output to rx (ft)
        // find all modules that are input to that single module
        var cycles = modules.Values.Where(t => t.Outputs.Contains(FTModuleName)).ToDictionary(m => m.Name, m => 0L);

        while (isPart2 || presses < 1000)
        {
            presses++;

            queue.Enqueue(buttonPulse);
            lowPulses++;

            while (queue.TryDequeue(out var pulse))
            {
                var target = modules[pulse.Target];
                var newPulses = target.HandlePulse(pulse);

                highPulses += newPulses.Count(p => p.IsHigh);
                lowPulses += newPulses.Count(p => !p.IsHigh);

                foreach (var newPulse in newPulses)
                {
                    if (isPart2 && newPulse.Target == FTModuleName && newPulse.IsHigh && cycles.TryGetValue(newPulse.Source, out var cycle) && cycle == 0) 
                    {
                        cycles[newPulse.Source] = presses;
                        if (cycles.Values.All(cycle => cycle != 0))
                        {
                            return (0, cycles.Values.Aggregate(1L, (cur, next) => cur * next));
                        }
                        
                    }
                    queue.Enqueue(newPulse);
                }
            }
        }
        Console.WriteLine($"low: {lowPulses} * high: {highPulses} = {lowPulses * highPulses}");
        return (part1: lowPulses * highPulses, part2: stepsToRx);
    }

    private static Dictionary<string, IModule> ParseInput(string[] inputs)
    {
        var modules = new Dictionary<string, IModule>();

        foreach (var input in inputs)
        {
            var declaration = input.Split(" -> ");
            var definition = declaration[0];
            var outputs = declaration[1];

            IModule module = input[0] switch
            {
                '%' => new FlipFlop(definition[1..], outputs),
                '&' => new Conjunction(definition[1..], outputs),
                _ => new Broadcaster(definition, outputs)
            };

            modules.Add(module.Name, module);
        }

        foreach (var (name, module) in modules.ToDictionary())
        {
            foreach (var output in module.Outputs)
            {
                if (modules.TryGetValue(output, out var mod))
                {
                    mod.AddInput(module);
                }
                else
                {
                    modules.Add(output, new Output(output));
                }
            }
        }

        return modules;
    }
}

public interface IModule
{
    public string Name { get; init; }
    public string[] Outputs { get; init; }

    public Pulse[] HandlePulse(Pulse input);

    public void AddInput(IModule module) { }
}

public class Broadcaster(string name, string outputs) : IModule
{
    public string Name { get; init; } = name;
    public string[] Outputs { get; init; } = outputs.Split(',', StringSplitOptions.TrimEntries);

    public Pulse[] HandlePulse(Pulse input)
    {
        return Outputs.Select(t => new Pulse(Name, t, input.IsHigh)).ToArray();
    }
}

public class FlipFlop(string name, string outputs) : IModule
{
    public string Name { get; init; } = name;

    public string[] Outputs { get; init; } = outputs.Split(',', StringSplitOptions.TrimEntries);

    public bool TurnedOn { get; private set; }

    public Pulse[] HandlePulse(Pulse input)
    {
        if (!input.IsHigh)
        {
            TurnedOn = !TurnedOn;

            return Outputs.Select(t => new Pulse(Name, t, TurnedOn)).ToArray();
        }

        return [];
    }
}

public class Output(string name) : IModule
{
    public string Name { get; init; } = name;

    public string[] Outputs { get; init; } = [];

    public Pulse[] HandlePulse(Pulse input)
    {
        return [];
    }
}

public class Conjunction(string name, string outputs) : IModule
{
    public string Name { get; init; } = name;

    public string[] Outputs { get; init; } = outputs.Split(',', StringSplitOptions.TrimEntries);

    private Dictionary<string, bool> Inputs { get; init; } = [];

    public void AddInput(IModule module)
    {
        Inputs.Add(module.Name, false);
    }

    public Pulse[] HandlePulse(Pulse input)
    {
        Inputs[input.Source] = input.IsHigh;

        var all = Inputs.Values.All(t => t);

        return Outputs.Select(o => new Pulse(Name, o, !all)).ToArray();
    }
}

public record struct Pulse(string Source, string Target, bool IsHigh)
{
    public override string ToString()
    {
        return $"{Source} -{(IsHigh ? "high" : "low")}-> {Target}";
    }
}