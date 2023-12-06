using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace AOC.Challenges;

[Challenge(6, "Wait For It")]
internal class Day06 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(288L, Part1(testInput));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(71503L, Part2(testInput));
    }

    public override object Part1(string input)
    {
        return input.ToLines()
            .Select(x => x.SplitSpaces().Skip(1).Select(long.Parse))
            .Lag()
            .SelectMany(x => x.Item1.Zip(x.Item2))
            .Select(r => CountSolutions(r.First, r.Second))
            .Aggregate((a, b) => a * b);
    }

    public override object Part2(string input)
    {
        var (time, dist) = input.ToLines()
            .Select(x => x.SplitSpaces())
            .Select(x => string.Join("", x.Skip(1)))
            .Select(long.Parse);

        return CountSolutions(time, dist);
    }

    private static long CountSolutions(long recordTime, long recordDist)
    {
        return EnumerableExtensions.RangeL(0, recordTime)
            .Select(hold => (recordTime - hold) * hold)
            .Count(dist => dist > recordDist);
    }

    private const string testInput = @"
            Time:      7  15   30
            Distance:  9  40  200";
}
