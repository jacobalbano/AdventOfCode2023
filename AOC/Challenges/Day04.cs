using AdventOfCodeScaffolding;
using AOC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Challenges;

[Challenge(4, "Scratchcards")]
internal class Day04 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(13, Part1(testInput));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(30, Part2(testInput));
    }

    public override object Part1(string input)
    {
        return input.ToLines()
            .Select(Scratchcard.Parse)
            .Select(x => (1 << x.Winners.Intersect(x.Drawn).Count()) >> 1)
            .Sum();
    }

    public override object Part2(string input)
    {
        var cards = input.ToLines()
            .Select(Scratchcard.Parse)
            .Select(x => x.Winners.Intersect(x.Drawn).Count())
            .ToArray();

        var counts = new int[cards.Length];
        Array.Fill(counts, 1);

        for (int cardId = 0; cardId < cards.Length; cardId++)
            for (int offset = 0; offset < cards[cardId]; offset++)
                counts[offset + cardId + 1] += counts[cardId];

        return counts.Sum();
    }

    private record class Scratchcard(int Id, IReadOnlyList<int> Winners, IReadOnlyList<int> Drawn)
    {
        public static Scratchcard Parse(string line)
        {
            var parser = new StringParser(line);
            int id = parser.SkipExact("Card")
                .SkipWhile(char.IsWhiteSpace)
                .ReadInt();

            parser.SkipExact(":");

            var winners = new List<int>();
            parser.SkipWhile(char.IsWhiteSpace);
            while (parser.HasMaterial)
            {
                winners.Add(parser.ReadInt());
                parser.SkipWhile(char.IsWhiteSpace);

                if (parser.Peek() == '|') break;
            }

            var drawn = new List<int>();
            parser.SkipExact("|").SkipWhile(char.IsWhiteSpace);
            while (parser.HasMaterial)
            {
                drawn.Add(parser.ReadInt());
                parser.SkipWhile(char.IsWhiteSpace);
            }

            return new(id, winners, drawn);
        }
    }

    private const string testInput = @"
Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11";
}
