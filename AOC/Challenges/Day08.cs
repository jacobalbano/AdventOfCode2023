using AdventOfCodeScaffolding;
using AOC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Challenges;

[Challenge(8, "Haunted Wasteland")]
internal class Day08 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(2, Part1(@"
            RL

            AAA = (BBB, CCC)
            BBB = (DDD, EEE)
            CCC = (ZZZ, GGG)
            DDD = (DDD, DDD)
            EEE = (EEE, EEE)
            GGG = (GGG, GGG)
            ZZZ = (ZZZ, ZZZ)"
        ));

        Assert.AreEqual(6, Part1(@"
            LLR

            AAA = (BBB, BBB)
            BBB = (AAA, ZZZ)
            ZZZ = (ZZZ, ZZZ)"
        ));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(6L, Part2(@"
            LR

            11A = (11B, XXX)
            11B = (XXX, 11Z)
            11Z = (11B, XXX)
            22A = (22B, XXX)
            22B = (22C, 22C)
            22C = (22Z, 22Z)
            22Z = (22B, 22B)
            XXX = (XXX, XXX)"
        ));
    }

    public override object Part1(string input)
    {
        var (lr, map) = ParseInput(input);

        return FollowPath(map, lr, "AAA")
            .TakeWhile(x => x != "ZZZ")
            .Count();
    }

    public override object Part2(string input)
    {
        var (lr, map) = ParseInput(input);
        return map.Keys.Where(x => x.EndsWith('A'))
            .Select(x => FollowPath(map, lr, x))
            .Select(x => x.TakeWhile(y => !y.EndsWith('Z')).LongCount())
            .Aggregate(LCM);

        static long LCM(long a, long b) => Math.Abs(a * b) / GCD(a, b);
        static long GCD(long a, long b) => b == 0 ? a : GCD(b, a % b);
    }

    private static IEnumerable<string> FollowPath(IReadOnlyDictionary<string, Node> nodes, IEnumerable<char> instructions, string startNode)
    {
        var node = nodes[startNode];
        foreach (var dir in instructions.RepeatInfinitely())
        {
            yield return node.Name;
            node = nodes[dir switch
            {
                'R' => node.Right,
                'L' => node.Left,
                _ => throw new UnreachableCodeException()
            }];
        }
    }

    private record class Node(string Name, string Left, string Right);
    private static (string lr, IReadOnlyDictionary<string, Node> map) ParseInput(string input)
    {
        var (dirs, map) = input.ToLines()
            .Select(x => x.Trim())
            .PartitionBy(string.IsNullOrWhiteSpace)
            .Select(x => x.ToList());

        return (dirs.First(), ParseMap(map));

        static Dictionary<string, Node> ParseMap(List<string> map)
        {
            return map.Select(line => new Node(line[0..3], line[7..10], line[12..15]))
                .ToDictionary(x => x.Name);
        }
    }
}
