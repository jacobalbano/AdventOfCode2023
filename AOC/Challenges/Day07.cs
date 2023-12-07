using AdventOfCodeScaffolding;
using AOC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Challenges;

[Challenge(7, "Camel Cards")]
internal class Day07 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(6440, Part1(testInput));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(5905, Part2(testInput));
    }

    public override object Part1(string input)
    {
        return Solve(input, "AKQJT98765432", useJokers: false);
    }

    public override object Part2(string input)
    {
        return Solve(input, "AKQT98765432J", useJokers: true);
    }

    private static int Solve(string input, string cards, bool useJokers)
    {
        return input.ToLines()
            .Select(x => x.Trim())
            .Select(x => (hand: x[..5], bid: int.Parse(x[6..])))
            .OrderBy(x => FindRank(x.hand, cards, useJokers))
            .Select((x, i) => x.bid * (i + 1))
            .Sum();
    }

    private static int FindRank(string hand, string cards, bool useJokers)
    {
        var (counts, ranks) = (new byte[cards.Length], new byte[5]);
        for (int i = 0; i < 5; i++)
        {
            var index = cards.IndexOf(hand[i]);
            counts[index]++;
            ranks[i] = (byte)index;
        }

        int result = FindHandType(counts, useJokers);
        for (int i = 0; i < 5; i++)
            result = result * cards.Length + cards.Length - ranks[i] + 1;

        return result;
    }

    private static int FindHandType(byte[] counts, bool useJokers)
    {
        var (first, second) = counts[..^(useJokers ? 1 : 0)]
            .OrderByDescending(x => x);
        return (first + (useJokers ? counts[^1] : 0), second) switch
        {
            (5, _) => 7, // five of a kind
            (4, _) => 6, // four of a kind
            (3, 2) => 5, // full house
            (3, _) => 4, // three of a kind
            (2, 2) => 3, // two pair
            (2, _) => 2, // one pair
            _ => 1       // high card
        };
    }

    private const string testInput = @"
        32T3K 765
        T55J5 684
        KK677 28
        KTJJT 220
        QQQJA 483";
}
