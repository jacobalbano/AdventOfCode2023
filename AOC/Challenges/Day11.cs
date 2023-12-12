using AdventOfCodeScaffolding;
using AOC.Common;
using AOC.Common.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Challenges;

[Challenge(11, "Cosmic Expansion")]
internal class Day11 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(374L, Solve(testInput, 2));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(1030L, Solve(testInput, 10));
        Assert.AreEqual(8410L, Solve(testInput, 100));
    }

    public override object Part1(string input)
    {
        return Solve(input, 2);
    }

    public override object Part2(string input)
    {
        return Solve(input, 1000000);
    }

    private static long Solve(string input, int expandBy)
    {
        var map = new Grid<bool>(input, x => x == '#');
        var galaxies = map.Cells()
            .Where(x => map[x])
            .ToList();

        var emptyCols = Enumerable.Range(0, map.Columns)
            .Where(x => !map.ColumnValues(x).Any(x => x))
            .ToList();

        var emptyRows = Enumerable.Range(0, map.Rows)
            .Where(x => !map.RowValues(x).Any(x => x))
            .ToList();

        return Enumerable.Range(0, galaxies.Count - 1)
            .SelectMany(x => Enumerable.Range(x + 1, galaxies.Count - x - 1)
            .Select(y => (A: galaxies[x], B: galaxies[y])))
            .Sum(x => ManhattanWithExpansion(x.A, x.B));
        
        long ManhattanWithExpansion(GridCell a, GridCell b)
        {
            var (top, bottom) = a.Y < b.Y ? (a, b) : (b, a);
            var (left, right) = a.X < b.X ? (a, b) : (b, a);
            return Math.Abs(left.X - right.X) + Math.Abs(top.Y - bottom.Y)
                + emptyCols.Count(x => x > left.X && x < right.X) * (expandBy - 1)
                + emptyRows.Count(y => y > top.Y && y < bottom.Y) * (expandBy - 1);
        }
    }

    private const string testInput = @"
        ...#......
        .......#..
        #.........
        ..........
        ......#...
        .#........
        .........#
        ..........
        .......#..
        #...#.....";
}
