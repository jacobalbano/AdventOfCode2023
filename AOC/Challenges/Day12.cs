using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AOC.Challenges;

[Challenge(12, "Hot Springs")]
internal class Day12 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(1L, Part1Line(Parse(@"???.### 1,1,3")));
        Assert.AreEqual(4L, Part1Line(Parse(@".??..??...?##. 1,1,3")));
        Assert.AreEqual(1L, Part1Line(Parse(@"?#?#?#?#?#?#?#? 1,3,1,6")));
        Assert.AreEqual(1L, Part1Line(Parse(@"????.#...#... 4,1,1")));
        Assert.AreEqual(4L, Part1Line(Parse(@"????.######..#####. 1,6,5")));
        Assert.AreEqual(10L, Part1Line(Parse(@"?###???????? 3,2,1")));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(1L, Part2Line(Parse(@"???.### 1,1,3")));
        Assert.AreEqual(16384L, Part2Line(Parse(@".??..??...?##. 1,1,3")));
        Assert.AreEqual(1L, Part2Line(Parse(@"?#?#?#?#?#?#?#? 1,3,1,6")));
        Assert.AreEqual(16L, Part2Line(Parse(@"????.#...#... 4,1,1")));
        Assert.AreEqual(2500L, Part2Line(Parse(@"????.######..#####. 1,6,5")));
        Assert.AreEqual(506250L, Part2Line(Parse(@"?###???????? 3,2,1")));
    }

    public override object Part1(string input)
    {
        return input.ToLines()
            .Select(Parse)
            .Sum(Part1Line);
    }

    public override object Part2(string input)
    {
        return input.ToLines()
            .Select(Parse)
            .Sum(Part2Line);
    }

    private long Part1Line((string springs, int[] numbers) line)
    {
        var countCache = new Dictionary<(string, long), long>();
        return Inner(line.springs, line.numbers);

        long Inner(string springs, int[] runs)
        {
            return countCache.Establish((springs, Key(runs)), x =>
            {
                var (rl, sl) = (runs.Length, springs.Length);
                if (rl == 0) return !springs.Contains('#') ? 1 : 0;
                if ((runs.Sum() + rl - 1) > sl) return 0;
                if (springs[0] == '.') return Inner(springs[1..], runs);

                long count = springs[0] == '?' ?
                    Inner(springs[1..], runs) :
                    0;

                var left = runs[0];
                if (!springs[..left].Contains('.') && CanContain(springs, sl, left))
                    count += Inner(springs[Math.Clamp(left + 1, 0, sl)..], runs[1..]);

                return count;
            });

            static long Key(int[] n) => n.Aggregate(1L, (a, b) => a * 10 + b);
            static bool CanContain(string springs, int sl, int left) =>
                sl <= left || sl > left && springs[left] != '#';
        }
    }

    private long Part2Line((string springs, int[] numbers) line)
    {
        var springs = string.Join("?", Enumerable.Range(0, 5)
            .Select(_ => line.springs));

        var numbers = Enumerable.Range(0, 5)
            .SelectMany(x => line.numbers)
            .ToArray();

        return Part1Line((springs, numbers));
    }

    private static (string springs, int[] numbers) Parse(string line)
    {
        var (springs, numbersRaw) = line.Split(' ');
        return (springs, numbersRaw.Split(',')
            .Select(int.Parse)
            .ToArray());
    }

    private long Part1Line_Math(string line)
    {
        var (springs, numbersRaw) = "??????????????? 3,2,6".Split(' ');
        var numbers = numbersRaw.Split(',')
            .Select(int.Parse)
            .ToArray();

        var filled = numbers.Sum();
        var empty = springs.Length - filled;
        if (empty == numbers.Length - 1)
            return 1;

        var combos = GetPermutations(
            Enumerable.Range(0, empty + numbers.Length).ToArray(),
            numbers.Length
        ).ToList();

        return combos.Count;
    }

    private static IEnumerable<T[]> GetPermutations<T>(T[] list, int length)
    {
        if (length == 1) return list.Select(t => new[] { t });
        return GetPermutations(list, length - 1)
            .SelectMany(t => list.Where(o => !t.Contains(o)), (t1, t2) => t1.Concat(new[] { t2 }).ToArray());
    }

    private int Part1Line_Tree(string line)
    {
        var (springs, numbersRaw) = line.Split(' ');
        var numbers = numbersRaw.Split(',')
            .Select(int.Parse)
            .ToArray();


        throw new NotImplementedException();
    }

    private long Part1Line_Iterative(string line)
    {
        var (springs, numbersRaw) = line.Split(' ');
        var numbers = numbersRaw.Split(',')
            .Select(int.Parse)
            .ToArray();

        return SolveInner(springs.ToCharArray(), numbers);

        static long SolveInner(char[] springs, int[] numbers)
        {
            int total = numbers.Sum();

            if (springs.Length == total + numbers.Length - 1)
                return 1;

            var possibleRanges = FindPossibleRanges(springs, numbers);

            return possibleRanges.Select(x => SolveInner(springs[x.Start..x.End], x.Numbers))
                .Sum();
        }
    }

    private record class Slice(int Start, int End, int[] Numbers);

    private static IReadOnlyList<Slice> FindPossibleRanges(Span<char> span, Span<int> numbers)
    {
        var result = new List<Slice>();
        var strings = new List<string>();

        bool doYield = false;
        int start = 0, maybe = 0, broken = 0, good = 0, skip = 0, plus = 0;
        for (int i = 0; i < span.Length; i++)
        {
            switch (span[i])
            {
                case '?': maybe++; break;
                case '#': broken++; break;
                case '.': good++; break;
            }

            if (good > 0)
            {
                doYield = true;
                int sum = 0;
                for (int x = 0; x < numbers.Length; x++)
                {
                    sum += numbers[x] + 1;
                    if (sum > i - start)
                        break;
                    skip++;
                }
            }
            else if (broken == numbers[0])
            {
                skip = 1;
                start += good;
                doYield = true;
            }

            if (doYield)
            {
                i += 1 + plus;
                result.Add(new Slice(start, i, numbers[..skip].ToArray()));
                strings.Add(new string(span[start..i]));

                numbers = numbers[skip..];
                skip = good = broken = maybe = plus = 0;
                start = i;
                doYield = false;
            }
        }

        result.Add(new(start, span.Length - 1, numbers.ToArray()));
        strings.Add(new string(span[start..]));

        return result;
    }

    private enum State { Good = '.', Bad = '#', Unknown = '?' }

    private record class Spring(State State, int GroupSize)
    {
    }
}
