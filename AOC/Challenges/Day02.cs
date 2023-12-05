using AOC.Util;
using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Challenges;

[Challenge(2, "Cube Conundrum")]
internal class Day02 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(8, Part1(testInput));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(2286, Part2(testInput));
    }

    public override object Part1(string input)
    {
        var (r, g, b) = (12, 13, 14);
        return input.ToLines()
            .Select(x => x.Trim())
            .Select(Game.Parse)
            .Where(x => x.Pulls.All(p => p.R <= r && p.G <= g && p.B <= b))
            .Sum(x => x.Id);
    }

    public override object Part2(string input)
    {
        return input.ToLines()
            .Select(x => x.Trim())
            .Select(Game.Parse)
            .Select(x => x.Pulls.MultiMax(y => y.R, y => y.G, y => y.B))
            .Select(x => x.Aggregate((a, b) => a * b))
            .Sum();
    }

    private record class Game(int Id, IReadOnlyList<BlockSet> Pulls)
    {
        public static Game Parse(string str)
        {
            var parser = new StringParser(str);
            int id = parser.SkipUntil(' ', goPast: true).ReadInt();
            parser.SkipExact(": ");

            var pulls = new List<BlockSet>();
            while (parser.HasMaterial)
                pulls.Add(BlockSet.Parse(parser));

            return new Game(id, pulls);
        }
    }

    private record class BlockSet(int R, int G, int B)
    {
        // Should enter this method with the cursor on the number
        public static BlockSet Parse(StringParser parser)
        {
            int r = 0, g = 0, b = 0;

            parser.SkipAny(" ");
            while (parser.HasMaterial)
            {
                var num = parser.ReadInt();
                switch (parser.Skip(1).ReadChar())
                {
                    case 'r': r += num; break;
                    case 'g': g += num; break;
                    case 'b': b += num; break;
                    default: throw new Exception("Unhandled cube color");
                }

                if (!parser.SkipWhile(char.IsLetter).TryReadChar(out char c) || c == ';')
                    return new BlockSet(r, g, b);

                switch (c)
                {
                    case ',': parser.Skip(1); continue;
                    default: throw new Exception($"Invalid state (read char '{c}')");
                }
            }

            throw new UnreachableCodeException();
        }

    }

    private const string testInput = @"
            Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
            Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
            Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
            Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
            Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green";
}
