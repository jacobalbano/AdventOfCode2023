using AdventOfCodeScaffolding;
using AOC.Common;
using AOC.Common.Solvers;
using AOC.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static AOC.Common.Solvers.GridPathSolver;

namespace AOC.Challenges;

[Challenge(10, "Pipe Maze")]
internal class Day10 : ChallengeBase
{
    public override void Part1Test()
    {
        Assert.AreEqual(4, Part1(@"
            .....
            .S-7.
            .|.|.
            .L-J.
            ....."));

        Assert.AreEqual(4, Part1(@"
            -L|F7
            7S-7|
            L|7||
            -L-J|
            L|-JF"));

        Assert.AreEqual(8, Part1(@"
            ..F7.
            .FJ|.
            SJ.L7
            |F--J
            LJ..."));

        Assert.AreEqual(8, Part1(@"
            7-F7-
            .FJ|7
            SJLL7
            |F--J
            LJ.LJ"));
    }

    public override void Part2Test()
    {
        Assert.AreEqual(4, Part2(@"
            ...........
            .S-------7.
            .|F-----7|.
            .||.....||.
            .||.....||.
            .|L-7.F-J|.
            .|..|.|..|.
            .L--J.L--J.
            ..........."));

        Assert.AreEqual(8, Part2(@"
            .F----7F7F7F7F-7....
            .|F--7||||||||FJ....
            .||.FJ||||||||L7....
            FJL7L7LJLJ||LJ.L-7..
            L--J.L7...LJF7F-7L7.
            ....F-J..F7FJSL7L7L7
            ....L7.F7||L7|.L7L7|
            .....|FJLJ|FJ|F7|.LJ
            ....FJL-7.||.||||...
            ....L---J.LJ.LJLJ..."));

        Assert.AreEqual(10, Part2(@"
            FF7FSF7F7F7F7F7F---7
            L|LJ||||||||||||F--J
            FL-7LJLJ||||||LJL-77
            F--JF--7||LJLJ7F7FJ-
            L---JF-JLJ.||-FJLJJ7
            |F|F-JF---7F7-L7L|7|
            |FFJF7L7F-JF7|JL---7
            7-L-JL7||F7|L7F-7F7|
            L.L7LFJ|||||FJL7||LJ
            L7JLJL-JLJLJL--JLJ.L"));
    }

    public override object Part1(string input)
    {
        return FindPipeSections(new Grid<PipeSection>(input, PipeSection.Parse)).Count / 2;
    }

    public override object Part2(string input)
    {
        var grid = new Grid<PipeSection>(input, PipeSection.Parse);
        var sections = FindPipeSections(grid);

        var bends = new[] { SectionType.Vertical, SectionType.NorthToEast, SectionType.NorthToWest };

        int count = 0;
        for (int r = 0; r < grid.Rows; r++)
        {
            bool inside = false;
            for (int c = 0; c < grid.Columns; c++)
            {
                if (bends.Contains(grid[r, c].Type))
                    inside = !inside;
                else if (inside && grid[r, c].Type == SectionType.None)
                    count++;
            }
        }

        return count;
    }

    private List<PipeSection> FindPipeSections(Grid<PipeSection> pipes)
    {
        var last = pipes.Cells()
            .Select(x => pipes[x])
            .Where(x => x.Type == SectionType.Start)
            .Single();

        var position = pipes.SurroundingCells(last.Cell, includeDiagonals: false)
            .Select(x => pipes[x])
            .Where(x => x.CanConnectTo(last.Cell))
            .First();

        var path = new List<PipeSection> { last };
        while (position != path.First())
        {
            path.Add(position);
            (last, position) = (position, pipes[position.GetNext(last)]);
        }

        foreach (var section in pipes.Except(path))
            pipes[section.Cell] = new PipeSection(SectionType.None, section.Cell);

        var start = path.First().Cell;
        var hookups = pipes.SurroundingCells(start, false)
            .Where(x => pipes[x].CanConnectTo(start))
            .Select(x => Direction.BetweenCells(x, start))
            .ToList();

        bool north = false, south = false, east = false, west = false;
        foreach (var item in hookups)
        {
            if (item == Direction.North) north = true;
            if (item == Direction.South) south = true;
            if (item == Direction.East) east = true;
            if (item == Direction.West) west = true;
        }

        pipes[start] = new PipeSection((north, south, east, west) switch
        {
            (true, true, _, _) => SectionType.Vertical,
            (_, _, true, true) => SectionType.Horizontal,
            (true, _, true, _) => SectionType.NorthToEast,
            (true, _, _, true) => SectionType.NorthToWest,
            (_, true, _, true) => SectionType.SouthToWest,
            (_, true, true, _) => SectionType.SouthToEast,

            _ => throw new NotImplementedException(),
        }, start);

        WritePipes(pipes);
        return path;
    }

    private void WritePipes<T>(Grid<T> pipes)
    {
        Logger.LogLine("--- pipes ---");
        for (int r = 0; r < pipes.Rows; r++)
            Logger.LogLine(string.Join("", pipes.RowValues(r)));
    }

    public record class Direction(int X, int Y)
    {
        public static readonly Direction North = new(0, -1);
        public static readonly Direction South = new(0, 1);
        public static readonly Direction East = new(1, 0);
        public static readonly Direction West = new(-1, 0);

        public static Direction BetweenCells(GridCell previous, GridCell cell) => new(
            previous.X - cell.X,
            previous.Y - cell.Y
        );
    }

    private enum SectionType
    {
        None = '.',
        Start = 'S',
        Vertical = '|',
        Horizontal = '-',
        NorthToEast = 'L',
        NorthToWest = 'J',
        SouthToWest = '7',
        SouthToEast = 'F',
    }

    private record class PipeSection(SectionType Type, GridCell Cell)
    {
        public static PipeSection Parse(char c, GridCell cell)
        {
            return new PipeSection((SectionType)c, cell);
        }

        public bool CanConnectTo(GridCell cell)
        {
            if (!steps.TryGetValue(Type, out var frontier))
                return false;

            return frontier.Contains(new(cell.X - Cell.X, cell.Y - Cell.Y));
        }

        public GridCell GetNext(PipeSection previous)
        {
            /*
             * this = horizontal,   (x: 2, y: 1)
             * last = start,        (x: 1, y: 1)
             * frontier = W, E
             * 
             * we came from W  (-1, 0)
             * 
             * how to get back to W?
             *      (last.X - X, last.Y - Y)
             * 
             * take frontier which is not W
             */

            if (!steps.TryGetValue(Type, out var frontier))
                throw new NotImplementedException();

            var nextDir = frontier.First(x => x != Direction.BetweenCells(previous.Cell, Cell));
            return new GridCell(Cell.Row + nextDir.Y, Cell.Col + nextDir.X);
        }

        private static readonly Dictionary<SectionType, Direction[]> steps = new()
        {
            [SectionType.Vertical] = new[] { Direction.North, Direction.South },
            [SectionType.Horizontal] = new[] { Direction.West, Direction.East },
            [SectionType.NorthToEast] = new[] { Direction.North, Direction.East },
            [SectionType.NorthToWest] = new[] { Direction.North, Direction.West },
            [SectionType.SouthToWest] = new[] { Direction.South, Direction.West },
            [SectionType.SouthToEast] = new[] { Direction.South, Direction.East },
        };

        public override string ToString()
        {
            return Type switch
            {
                SectionType.None => " ",
                SectionType.Horizontal => "─",
                SectionType.Vertical => "│",
                SectionType.NorthToEast => "└",
                SectionType.NorthToWest => "┘",
                SectionType.SouthToEast => "┌",
                SectionType.SouthToWest => "┐",
                SectionType.Start => "S",
                _ => throw new UnreachableCodeException()
            };
        }
    }
}
