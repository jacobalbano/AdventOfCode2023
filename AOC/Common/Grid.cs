using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Common;

public record class GridCell(int Row, int Col)
{
    public int X => Col;
    public int Y => Row;

    public static implicit operator GridCell((int row, int col) cell) => new(cell.row, cell.col);
    public static implicit operator (int row, int col)(GridCell cell) => (cell.Row, cell.Col);
}

public class Grid<T> : IEnumerable<T>
{
    public int Rows { get; }
    public int Columns { get; }

    public T this[int row, int col]
    {
        get => storage[row, col];
        set => storage[row, col] = value;
    }

    public T this[GridCell cell]
    {
        get => storage[cell.Row, cell.Col];
        set => storage[cell.Row, cell.Col] = value;
    }

    public IEnumerable<GridCell> Cells()
    {
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
                yield return (row, col);
    }

    public IEnumerable<T> RowValues(int row)
    {
        for (int col = 0; col < Columns; col++)
            yield return this[row, col];
    }

    public IEnumerable<T> ColumnValues(int col)
    {
        for (int row = 0; row < Rows; row++)
            yield return this[row, col];
    }

    public IEnumerable<T> DiagonalValues(int startRow, int startCol)
    {
        while (IsValidPosition(startRow, startCol))
            yield return this[startRow++, startCol++];
    }

    public IEnumerable<T> AntidiagonalValues(int startRow, int startCol)
    {
        while (IsValidPosition(startRow, startCol))
            yield return this[startRow--, startCol++];
    }

    public IEnumerable<GridCell> SurroundingCells(GridCell home, bool includeDiagonals)
    {
        return SurroundingCells(home.Row, home.Col, includeDiagonals);
    }

    public IEnumerable<GridCell> SurroundingCells(int homeRow, int homeCol, bool includeDiagonals)
    {
        for (int r = -1; r < 2; r++)
        {
            for (int c = -1; c < 2; c++)
            {
                if (c == 0 && r == 0)
                    continue;
                else if (!IsValidPosition(homeRow + r, homeCol + c))
                    continue;
                else if (!includeDiagonals && Math.Abs(r) == Math.Abs(c))
                    continue;

                yield return (homeRow + r, homeCol + c);
            }
        }
    }

    public Grid(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        storage = new T[Rows, Columns];
    }

    public Grid(string input, Func<char, GridCell, T> parser)
    {
        var lines = input
            .ToLines()
            .Select(x => x.Trim())
            .ToArray();

        Rows = lines.Length;
        Columns = lines[0].Length;
        storage = new T[Rows, Columns];

        foreach (var (row, col) in Cells())
            storage[row, col] = parser(lines[row][col], (row, col));
    }

    public Grid(string input, Func<char, T> parser) : this(input, (x, _) => parser(x))
    {
    }

    public IEnumerable<GridCell> FindDifferences(Grid<T> other)
    {
        foreach (var (row, col) in Cells())
            if (!this[row, col].Equals(other[row, col]))
                yield return (row, col);
    }

    public bool IsValidPosition(GridCell position) => IsValidPosition(position.Row, position.Col);

    public bool IsValidPosition(int row, int col)
    {
        return row >= 0
            && row < Rows
            && col >= 0
            && col < Columns;
    }

    public Grid<T> Clone()
    {
        var newData = new T[Rows, Columns];
        foreach (var (row, col) in Cells())
            newData[row, col] = storage[row, col];

        return new Grid<T>(newData);
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (var cell in Cells())
            yield return this[cell];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private Grid(T[,] storage)
    {
        this.storage = storage;
        Rows = storage.GetLength(0);
        Columns = storage.GetLength(1);
    }

    private readonly T[,] storage;
}

