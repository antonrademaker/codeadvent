var files = Directory.GetFiles("input", "*.txt");

foreach (var file in files.OrderBy(t => t))
{
    var lines = File.ReadAllLines(file);

    var root = new DirectoryNode(null, "/");

    var current = root;

    foreach (var line in lines.Skip(1))
    {
        if (line.StartsWith("$"))
        {
            // change directory
            if (line == "$ cd /")
            {
                current = root;
            }
            else if (line == "$ cd ..")
            {
                current = current.Parent ?? throw new Exception("Parent should not be null");
            }
            else if (line.StartsWith("$ cd "))
            {
                current = (DirectoryNode)current.Nodes[line.Replace("$ cd ", string.Empty)];
            }
        }
        else
        {
            // output

            if (line.StartsWith("dir"))
            {
                current.AddDirectory(line.Replace("dir ", string.Empty));
            }
            else
            {
                var data = line.Split(' ');

                current.AddFile(data[1], long.Parse(data[0]));
            }

        }

    }

    var part1Nodes = root.Find(dn => dn.Size < 100000);
    Console.WriteLine($"{file} Part 1: {part1Nodes.Sum(t => t.Size)}");

    var totalDiskSize = 70000000L;

    var totalFileSize = root.Size;

    var unused = totalDiskSize - totalFileSize;

    var requiredSize = 30000000;

    var toClean = requiredSize - unused;

    var part2Nodes = root.Find(dn => dn.Size >= toClean).OrderBy(dn => dn.Size);

    Console.WriteLine($"{file} Part 2: {part2Nodes.First().Size}");

}

abstract class Node
{
    public Node(DirectoryNode? parent, string name)
    {
        Parent = parent;
        Name = name;
    }

    public DirectoryNode? Parent { get; init; }
    public string Name { get; init; }

    public abstract long Size { get; }
}

class FileNode : Node
{
    public FileNode(DirectoryNode parent, string name, long size) : base(parent, name)
    {
        Size = size;
    }
    public override long Size { get; }
}

class DirectoryNode : Node
{

    public DirectoryNode(DirectoryNode? parent, string name) : base(parent, name) { }

    public Dictionary<string, Node> Nodes { get; } = new();


    public override long Size => Nodes.Values.Sum(t => t.Size);

    public void AddFile(string name, long size)
    {
        Nodes.Add(name, new FileNode(this, name, size));
    }

    public void AddDirectory(string name)
    {
        Nodes.Add(name, new DirectoryNode(this, name));
    }

    public IEnumerable<DirectoryNode> Find(Func<DirectoryNode, bool> func)
    {
        foreach (var node in Nodes.Values)
        {
            if (node is DirectoryNode dn)
            {
                foreach (var node2 in dn.Find(func))
                {
                    yield return node2;
                }
            }
        }

        if (func(this))
        {
            yield return this;
        }
    }
}