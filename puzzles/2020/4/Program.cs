using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _4
{
    internal class Program
    {
        private static Passport EmptyPassport = new Passport(null, null, null, null, null, null, null, null, true);

        private static void Main(string[] args)
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

            var inputAcc = File.ReadLines(filePath)
                .Aggregate(string.Empty, (acc, p) => p.Count() == 0 ? acc + "\r\n" : acc + " " + p);

            var input = inputAcc
                .Split("\r\n")
                .Select(line => Parse(line))
                .ToList();

            Console.WriteLine($"Number of passports: {input.Count}");

            var isValidPassport = input.Select(p => Valid(p));

            var numberOfValidPassports = isValidPassport.Count(t => t);
            Console.WriteLine($"Number of VALID passports: {numberOfValidPassports}");
        }

        private static Passport Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return EmptyPassport;
            }
            var values = input.Split(" ");

            var keyValued = new Dictionary<string, string>();

            var parseErrors = false;

            foreach (var kv in values)
            {
                var splitted = kv.Split(":");

                if (splitted.Length == 2)
                {
                    var key = splitted[0];

                    if (keyValued.ContainsKey(key))
                    {
                        parseErrors = true;
                    }
                    else
                    {
                        keyValued.Add(key, splitted[1]);
                    }
                }
            }

            return new Passport(
                keyValued.ContainsKey("byr") ? keyValued["byr"] : null,
                keyValued.ContainsKey("iyr") ? keyValued["iyr"] : null,
                keyValued.ContainsKey("eyr") ? keyValued["eyr"] : null,
                keyValued.ContainsKey("hgt") ? keyValued["hgt"] : null,
                keyValued.ContainsKey("hcl") ? keyValued["hcl"] : null,
                keyValued.ContainsKey("ecl") ? keyValued["ecl"] : null,
                keyValued.ContainsKey("pid") ? keyValued["pid"] : null,
                keyValued.ContainsKey("cid") ? keyValued["cid"] : null,
                parseErrors
            );
        }

        private static bool Valid(Passport p)
        {
            return
                !p.parseErrors &&
                IsValidBirthYear(p.byr) &&
                IsValidIssueYear(p.iyr) &&
                IsValidExpYear(p.eyr) &&
                IsValidHeight(p.hgt) &&
                IsValidColor(p.hcl) &&
                IsValidEyeColor(p.ecl) &&
                IsValidPassportId(p.pid);
        }

        private static bool IsValidBirthYear(string? input)
        {
            return ValidNumber(input, parsed => parsed >= 1920 && parsed <= 2002);
        }

        private static bool IsValidIssueYear(string? input)
        {
            return ValidNumber(input, parsed => parsed >= 2010 && parsed <= 2020);
        }

        private static bool IsValidExpYear(string? input)
        {
            return ValidNumber(input, parsed => parsed >= 2020 && parsed <= 2030);
        }

        private static bool ValidNumber(string? input, Func<int, bool> func)
        {
            if (int.TryParse(input, out var parsed))
            {
                return func(parsed);
            }
            return false;
        }

        private static bool IsValidHeight(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            if (input.EndsWith("cm"))
            {
                var length = input.Replace("cm", string.Empty);

                return ValidNumber(length, l => l >= 150 && l <= 193);
            }
            if (input.EndsWith("in"))
            {
                var length = input.Replace("in", string.Empty);

                return ValidNumber(length, l => l >= 59 && l <= 76);
            }
            return false;
        }

        private static bool IsValidColor(string? color)
        {
            if (string.IsNullOrWhiteSpace(color))
            {
                return false;
            }

            return HexRegex.IsMatch(color);
        }

        private static Regex HexRegex = new Regex(@"^#([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$", RegexOptions.Compiled);

        private static List<string> validEyeColors = new()
        {
            "amb",
            "blu",
            "brn",
            "gry",
            "grn",
            "hzl",
            "oth"
        };

        private static bool IsValidEyeColor(string? color)
        {
            if (string.IsNullOrWhiteSpace(color))
            {
                return false;
            }
            return validEyeColors.Contains(color);
        }

        private static bool IsValidPassportId(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            return ValidNumber(id, t => id.Count() == 9);
        }
    }

    public record Passport(
        string? byr,
        string? iyr,
        string? eyr,
        string? hgt,
        string? hcl,
        string? ecl,
        string? pid,
        string? cid,
        bool parseErrors
        );
}