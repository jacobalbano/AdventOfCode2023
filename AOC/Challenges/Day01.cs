using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AOC.Challenges;

[Challenge(1, "Trebuchet?!")]
internal class Day01 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(142, Part1(@"
                1abc2
                pqr3stu8vwx
                a1b2c3d4e5f
                treb7uchet"));
    }


    public override void Part2Test()
    {
        // line 2 is modified to include an overlap between the two results
        Assert.AreEqual(281, Part2(@"
                two1nine
                eighthree
                abcone2threexyz
                xtwone3four
                4nineeightseven2
                zoneight234
                7pqrstsixteen"));
    }

    public override object Part1(string input)
    {
        return input.ToLines()
            .Select(x => x.Where(char.IsNumber).ToList())
            .Select(x => $"{x[0]}{x[^1]}")
            .Select(int.Parse)
            .Sum();
    }

    public override object Part2(string input)
    {
        return input.ToLines()
            .Select(x => numbers.OverlappingMatches(x).ToList())
            .Select(x => $"{parse(x[0])}{parse(x[^1])}")
            .Select(int.Parse)
            .Sum();

        static int parse(string value) => int.TryParse(value, out var result)
            ? result
            : parts.IndexOf(value);
    }

    private static readonly List<string> parts = new() { "\\d", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
    private static readonly Regex numbers = new(string.Join('|', parts), RegexOptions.Compiled);
}
