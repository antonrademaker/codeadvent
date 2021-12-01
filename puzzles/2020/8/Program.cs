using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _8
{
    class Program
    {

        private static Regex instructionRegex = new Regex(@"^(nop|acc|jmp)\W((\+|\-)\d+)$", RegexOptions.Compiled);


        static void Main(string[] args)
        {
            string filePath = GetInputFilePath();

            var input = File.ReadLines(filePath).ToList();

            var instructions = input.Select(l => instructionRegex.Match(l))
                .Select(
                 instruction =>
                new Instruction(Enum.Parse<Operation>(instruction.Groups[1].Value), int.Parse(instruction.Groups[2].Value)))
                .ToArray();

            bool executedOk;
            int acc;


            var isExecuted = Enumerable.Range(0, instructions.Count()).Select(i => false).ToArray();

            (executedOk,acc) = Execute(instructions, isExecuted,true);

            Console.WriteLine($"Acc: {acc} success:{executedOk}");

        }

        private static Instruction[] GenerateWithNop(int location, Instruction[] instructions)
        {
            var clone = (Instruction[]) instructions.Clone();
            clone[location] = new Instruction(Operation.nop, 0);
            return clone;
        }

        private static Instruction[] GenerateWithJmp(int location, Instruction[] instructions)
        {
            var clone = (Instruction[])instructions.Clone();
            var argument = instructions[location].Argument;
            clone[location] = new Instruction(Operation.jmp, argument);
            return clone;
        }

        public record Instruction(Operation Operation, int Argument);

        private static (bool,int) Execute(Instruction[] instructions, bool[] isExecuted, bool allowFixing, int currentInstruction = 0, int acc = 0)
        {
            while (currentInstruction < instructions.Length)
            {
                if (isExecuted[currentInstruction])
                {
                    return (false, acc);
                }
                
                var instruction = instructions[currentInstruction];

                Console.WriteLine($"Operation: {instruction.Operation} {instruction.Argument} ({allowFixing})");
                switch (instruction.Operation)
                {
                    case Operation.nop:

                        if (allowFixing)
                        {
                            var candidate = GenerateWithJmp(currentInstruction, instructions);
                            var canidateExecuted = (bool[])isExecuted.Clone();

                            var (success, canidatedAcc) = Execute(candidate, canidateExecuted, false, currentInstruction, acc);

                            if (success)
                            {
                                Console.WriteLine($"Instruction {currentInstruction} was corrupted (nop => jmp)");
                                return (success, canidatedAcc);
                            }
                        }
                        isExecuted[currentInstruction] = true;
                        currentInstruction++;
                        break;
                    case Operation.acc:
                        acc += instruction.Argument;
                        isExecuted[currentInstruction] = true;
                        currentInstruction++;
                        break;

                    case Operation.jmp:

                        if (allowFixing)
                        {
                            var candidate = GenerateWithNop(currentInstruction, instructions);
                            var canidateExecuted = (bool[])isExecuted.Clone();

                            var (success, canidatedAcc) = Execute(candidate, canidateExecuted, false, currentInstruction, acc);

                            if (success)
                            {
                                Console.WriteLine($"Instruction {currentInstruction} was corrupted (jmp => nop)");

                                return (success, canidatedAcc);
                            }
                        }
                        isExecuted[currentInstruction] = true;
                        currentInstruction += instruction.Argument;

                        break;
                }
                
            }

            return (true,acc);
        }

        public enum Operation
        {
            nop,
            acc,
            jmp
        }

        private static string GetInputFilePath()
        {
            string path = Directory.GetCurrentDirectory();
            var filePath = path + "\\input.txt";
            while (!File.Exists(filePath))
            {
                var dirInfo = Directory.GetParent(path);

                if (dirInfo?.Exists ?? false)
                {
                    path = dirInfo.FullName;
                    filePath = path + "\\input.txt";
                }
                else
                {
                    throw new Exception($"Path not found {path}");
                }
            }

            return filePath;
        }
    }
}
