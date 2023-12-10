using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Challenges;

[Challenge(9, "Mirage Maintenance")]
internal class Day09 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(114L, Part1(sampleInput));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(2L, Part2(sampleInput));
    }

    public override object Part1(string input)
    {
        return input.ToLines()
            .Select(x => x.SplitSpaces())
            .Select(x => x.Select(long.Parse).ToArray())
            .Select(x => MakeTriangle(x)
            .Select(x => x.Last())
            .Aggregate((a, b) => a + b))
            .Sum();
    }

    public override object Part2(string input)
    {
        return input.ToLines()
            .Select(x => x.SplitSpaces())
            .Select(x => x.Select(long.Parse).ToArray())
            .Select(x => MakeTriangle(x)
            .Select(x => -x.First())
            .Reverse()
            .Aggregate((a, b) => b - a))
            .Sum();
    }

    private static IEnumerable<long[]> MakeTriangle(long[] history)
    {
        return EnumerableExtensions.Iterate(history, s => s.WindowBy(2)
            .Select(x => x[1] - x[0]).ToArray())
            .TakeUntil(x => x.All(y => y == 0));
    }

    private const string sampleInput = @"
        0 3 6 9 12 15
        1 3 6 10 15 21
        10 13 16 21 30 45";
}
