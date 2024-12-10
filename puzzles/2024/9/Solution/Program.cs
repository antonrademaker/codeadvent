using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using AoC.Utilities;

namespace Solution;

public class Program
{
    internal const bool EnableDebug = false;

    internal const int EmptyBlock = -1;

    static readonly string[] inputFiles = ["input/example.txt", "input/example1.txt", "input/input.txt"];

    public static void Main(string[] args)
    {

        foreach (string file in inputFiles)
        {
            Console.WriteLine($"Reading: {file}");

            var input = ParseFile(file);

            var answer1 = CalculateAnswer1(input);
            Console.WriteLine($"{file}: Answer 1: {answer1}");

            var answer2 = CalculateAnswer2(input);
            Console.WriteLine($"{file}: Answer 2: {answer2}");
        }
    }

    public static long CalculateAnswer1(Input input)
    {
        var fs = input.GetFileSystem();
        var disk = GetBlockRepresentation(fs);

        var nextBlockToMoveIndex = disk.Length - 1;

        for (var index = 0; index < disk.Length; index++)
        {
            if (disk[index] == EmptyBlock)
            {

                (disk[index], disk[nextBlockToMoveIndex]) = (disk[nextBlockToMoveIndex], EmptyBlock);

                while (nextBlockToMoveIndex > index && disk[--nextBlockToMoveIndex] == EmptyBlock)
                {

                }
            }
            if (nextBlockToMoveIndex <= index)
            {
                break;
            }
        }

        return CalculateChecksum(disk);
    }

    private static long CalculateChecksum(int[] disk)
    {
        return disk.Select((fileId, index) => (fileId, index)).Aggregate(0L, (acc, data) => data.fileId != EmptyBlock ? acc + data.index * data.fileId : acc);
    }

    private static long CalculateChecksum(List<Block> disk)
    {
        var sum = 0L;

        var index = 0;

        foreach (var block in disk)
        {
            if (block.FileId != EmptyBlock)
            {
                for(var i = 0; i < block.Size; i++)
                {
                    sum += block.FileId * (index + i);
                }
                
            }

            index += block.Size;

        }
        return sum;
        
    }

    public static int[] GetBlockRepresentation(List<Block> fs)
    {
        var newSize = fs.Sum(t => t.Size);

        var result = new int[newSize];

        var index = 0;

        foreach (var block in fs)
        {
            for (var i = 0; i < block.Size; i++)
            {
                result[index++] = block.FileId;
            }
        }
        return result;
    }

    public static string GetTextRepresentation(int[] blocks)
    {
        var sb = new StringBuilder();

        for (var index = 0; index < blocks.Length; index++)
        {
            var ch = blocks[index] == EmptyBlock ? '.' : Convert.ToChar(blocks[index] + '0');
            sb.Append(ch);
        }
        return sb.ToString();
    }

    public static string GetTextRepresentation(List<Block> fs)
    {
        var sb = new StringBuilder();

        foreach (var block in fs)
        {

            if (block.FileId > 9)
            {
                break;
            }

            var ch = block.FileId == EmptyBlock ? '.' : Convert.ToChar(block.FileId + '0');

            sb.Append(new string(ch, block.Size));
        }

        return sb.ToString();
    }

    static Input ParseFile(string file)
    {
        return new Input(File.ReadAllText(file));
    }

    public static long CalculateAnswer2(Input input)
    {
        var disk = input.GetFileSystem();

        var fileIds = disk.Select(t => t.FileId).Where(t => t != EmptyBlock).OrderByDescending(x => x).ToList();

        foreach (var fileId in fileIds)
        {
            var blockCandidate = disk.First(block => block.FileId == fileId);
            var blockCandidateIndex = disk.IndexOf(blockCandidate);

            var space = disk.FirstOrDefault(block => block.FileId == EmptyBlock && block.Size >= blockCandidate.Size);

            if (space == default)
            {
                continue;
            }

            var spaceIndex = disk.IndexOf(space);

            if (spaceIndex > blockCandidateIndex)
            {
                continue;
            }

            if (space.Size > blockCandidate.Size)
            {
                // split the block first
                // insert empty block after

                if (disk[spaceIndex + 1].FileId == EmptyBlock)
                {
                    disk[spaceIndex + 1].Size += space.Size - blockCandidate.Size;
                }
                else
                {
                    disk.Insert(spaceIndex + 1, new Block(space.Size - blockCandidate.Size, EmptyBlock));

                    blockCandidateIndex = disk.IndexOf(blockCandidate);
                    spaceIndex = disk.IndexOf(space);
                }

                space.Size = blockCandidate.Size;
                space.FileId = fileId;
            } else
            {
                space.Size = blockCandidate.Size;
                space.FileId = fileId;
            }

            blockCandidate.FileId = EmptyBlock;
        }
        return CalculateChecksum(disk);
        
    }
}
public record Input(string InputString)
{

    public readonly int[] DiskMap = InputString.Select(t => t - '0').ToArray();

    public int Length { get; } = InputString.Length;

    public List<Block> GetFileSystem()
    {
        return DiskMap.Select((t, i) => new Block(t, (i % 2 == 0) ? i / 2 : Program.EmptyBlock)).ToList();
    }

}

public class Block(int size, int fileId)
{
    public int Size { get; set; } = size;
    public int FileId { get; set; } = fileId;
}
