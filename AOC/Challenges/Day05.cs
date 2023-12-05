using AdventOfCodeScaffolding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AOC.Challenges;

[Challenge(5, "If You Give A Seed A Fertilizer")]
internal class Day05 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(35L, Part1(testInput));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(46L, Part2(testInput));
    }

    public override object Part1(string input)
    {
        var sections = input.ToLines()
            .Select(x => x.Trim())
            .PartitionBy(string.IsNullOrWhiteSpace)
            .Select(x => x.ToArray())
            .ToArray();

        var pipeline = new Pipeline(sections.Skip(1)
            .Select(Map.Parse)
            .ToArray());

        return sections.First()
            .SelectMany(x => x["seeds:".Length..].Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(long.Parse)
            .Select(pipeline.Transform)
            .Min();
    }


    public override object Part2(string input)
    {
        var sections = input.ToLines()
            .Select(x => x.Trim())
            .PartitionBy(string.IsNullOrWhiteSpace)
            .Select(x => x.ToArray())
            .ToArray();

        var pipeline = new Pipeline(sections.Skip(1)
            .Select(Map.Parse)
            .ToArray());

        return sections.First()
            .SelectMany(x => x["seeds:".Length..].Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(long.Parse)
            .ChunkBy(2)
            .SelectMany(x => RangeLong(x[0], x[1]))
            .AsParallel()
            .WithDegreeOfParallelism(100)
            .Select(pipeline.Transform)
            .Min();
    }

    private static IEnumerable<long> RangeLong(long start, long length)
    {
        for (long i = 0; i < length; ++i)
            yield return start + i;
    }

    private record class Pipeline(IReadOnlyList<Map> Maps)
    {
        public long Transform(long seed)
        {
            foreach (var map in Maps)
                seed = map.FindDest(seed);

            return seed;
        }
    }

    private record class Map(IReadOnlyList<Range> Ranges)
    {
        public static Map Parse(string[] x)
        {
            return new Map(x.Skip(1)
               .Select(Range.Parse)
               .OrderBy(x => x.Source)
               .ToArray());
        }

        public long FindDest(long source)
        {
            return Ranges.Where(x => source >= x.Source && source <= x.SourceEnd)
                .Select(x => x.Dest + (source - x.Source))
                .FirstOrDefault(source);
        }
    }

    [DebuggerDisplay("src: {Source}...{SourceEnd}, dest: {Dest}...{DestEnd}")]
    private record class Range(long Dest, long Source, long Length)
    {
        public long DestEnd => Dest + Length - 1;
        public long SourceEnd => Source + Length - 1;

        public static Range Parse(string line)
        {
            var (dest, source, length) = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .ToArray();

            return new Range(dest, source, length);
        }
    }

    private const string testInput = @"
        seeds: 79 14 55 13

        seed-to-soil map:
        50 98 2
        52 50 48

        soil-to-fertilizer map:
        0 15 37
        37 52 2
        39 0 15

        fertilizer-to-water map:
        49 53 8
        0 11 42
        42 0 7
        57 7 4

        water-to-light map:
        88 18 7
        18 25 70

        light-to-temperature map:
        45 77 23
        81 45 19
        68 64 13

        temperature-to-humidity map:
        0 69 1
        1 0 69

        humidity-to-location map:
        60 56 37
        56 93 4";
}
