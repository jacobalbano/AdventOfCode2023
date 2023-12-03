using AdventOfCodeScaffolding;
using AOC.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Challenges;

[Challenge(3, "Gear Ratios")]
internal class Day03 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(4361, Part1(sample));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(467835, Part2(sample));
    }

    public override object Part1(string input)
    {
        var grid = new Grid<char>(input, c => c);
        var numberRanges = FindNumberRanges(grid);

        return grid.Cells()
            .Where(cell => grid[cell] != '.' && !char.IsNumber(grid[cell]))
            .Select(sym => grid.SurroundingCells(sym, true).ToList())
            .SelectMany(cell => numberRanges
                .Where(range => range.Intersect(cell).Any()))
            .Distinct()
            .Select(range => RangeToInt(grid, range))
            .Sum();
    }

    public override object Part2(string input)
    {
        var grid = new Grid<char>(input, c => c);
        var numberRanges = FindNumberRanges(grid);
        return grid.Cells()
            .Where(cell => grid[cell] == '*')
            .Select(sym => numberRanges
                .Where(range => range.Intersect(grid.SurroundingCells(sym, true)).Any()).ToList())
                .Where(ranges => ranges.Count == 2)
                .Select(ranges => (a: ranges[0], b: ranges[^1]))
            .Select(ratio => RangeToInt(grid, ratio.a) * RangeToInt(grid, ratio.b))
            .Sum();
    }

    private static int RangeToInt(Grid<char> grid, IReadOnlyList<(int row, int col)> range)
    {
        return (int) range.Select(cell => char.GetNumericValue(grid[cell]))
            .Aggregate((a, b) => a * 10 + b);
    }

    private static List<List<(int row, int col)>> FindNumberRanges(Grid<char> grid)
    {
        return Enumerable.Range(0, grid.Rows)
            .Select(row => Enumerable.Range(0, grid.Columns).Select(col => (row, col)).ToList())
            .SelectMany(rowCells => rowCells.PartitionBy(cell => !char.IsNumber(grid[cell]))
                .Select(cell => cell.ToList())
                .Where(cells => cells.Any())
                .ToList())
            .ToList();
    }

    private const string sample = @"
        467..114..
        ...*......
        ..35..633.
        ......#...
        617*......
        .....+.58.
        ..592.....
        ......755.
        ...$.*....
        .664.598..";
}
