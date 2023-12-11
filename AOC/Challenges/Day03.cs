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
            .Where(cell => IsSymbol(cell, grid))
            .Select(sym => SurroundingCells(sym, grid))
            .SelectMany(cells => NeighboringRanges(cells, numberRanges))
            .Sum(range => RangeToInt(grid, range));
    }

    public override object Part2(string input)
    {
        var grid = new Grid<char>(input, c => c);
        var numberRanges = FindNumberRanges(grid);
        return grid.Cells()
            .Where(cell => grid[cell] == '*')
            .Select(sym => NeighboringRanges(grid.SurroundingCells(sym, true).ToList(), numberRanges).ToList())
            .Where(ranges => ranges.Count == 2)
            .Sum(ranges => RangeToInt(grid, ranges[0]) * RangeToInt(grid, ranges[^1]));
    }

    private static int RangeToInt(Grid<char> grid, IReadOnlyList<GridCell> range)
    {
        return (int) range.Select(cell => char.GetNumericValue(grid[cell]))
            .Aggregate((a, b) => a * 10 + b);
    }

    private static List<List<GridCell>> FindNumberRanges(Grid<char> grid)
    {
        return Enumerable.Range(0, grid.Rows)
            .Select(row => Enumerable.Range(0, grid.Columns).Select(col => (row, col)).ToList())
            .SelectMany(rowCells => rowCells.PartitionBy(cell => !char.IsNumber(grid[cell]))
                .Select(cell => cell.Select(x => new GridCell(x.row, x.col)).ToList())
                .Where(cells => cells.Any())
                .ToList())
            .ToList();
    }

    private static IEnumerable<List<GridCell>> NeighboringRanges(List<GridCell> cells, List<List<GridCell>> numberRanges)
    {
        return numberRanges.Where(range => range.Intersect(cells).Any());
    }

    private static List<GridCell> SurroundingCells(GridCell sym, Grid<char> grid)
    {
        return grid.SurroundingCells(sym, true).ToList();
    }

    private static bool IsSymbol(GridCell cell, Grid<char> grid)
    {
        return grid[cell] != '.' && !char.IsNumber(grid[cell]);
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
