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

    private enum Pipe
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

    public override object Part1(string input)
    {
        return FindPipeSections(new Grid<Pipe>(input, c => (Pipe)c)).Count / 2;
    }

    private static List<(int row, int col)> FindPipeSections(Grid<Pipe> pipes)
    {
        var last = pipes.Cells()
            .Where(x => pipes[x] == Pipe.Start)
            .Single();

        var position = pipes.SurroundingCells(last, includeDiagonals: false)
            .Where(x => pipes[x] == Pipe.Horizontal || pipes[x] == Pipe.Vertical)
            .First();

        var path = new List<(int row, int col)> { last };
        while (position != path.First())
        {
            path.Add(position);
            (last, position) = (position, steps[pipes[position]]
                .Select(x => (x.r + position.row, x.c + position.col))
                .Where(x => x != last).Single());
        }

        return path;
    }

    private static readonly (int r, int c)
        north = (-1, 0),
        south = (1, 0),
        east  = (0, 1),
        west  = (0, -1);

    private static readonly Dictionary<Pipe, (int r, int c)[]> steps = new()
    {
        [Pipe.Vertical] = new[] { north, south },
        [Pipe.Horizontal] = new[] { west, east },
        [Pipe.NorthToEast] = new[] { north, east },
        [Pipe.NorthToWest] = new[] { north, west },
        [Pipe.SouthToWest] = new[] { south, west },
        [Pipe.SouthToEast] = new[] { south, east },
    };
}
