using AOC.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC.Common.Solvers;

/// <summary>
/// Implementation of pathfinding logic over a 2d boolean grid.
/// Grid cells that contain 'false' will not be passable.
/// </summary>
public class GridPathSolver : AStarSolver<GridPathSolver.Node, IAstarHeuristic<GridPathSolver.Node>>
{
    public GridPathSolver(Grid<int> grid, bool allowDiagonal)
    {
        Initialize(this.grid = ConvertGrid(grid, allowDiagonal), new NullHeuristic());
    }

    public Node GetNodeFromGridPosition(int row, int col)
    {
        return grid[row, col];
    }

    private static Grid<Node> ConvertGrid(Grid<int> grid, bool allowDiagonal)
    {
        var result = new Grid<Node>(grid.Columns, grid.Rows);
        foreach (var (row, col) in grid.Cells())
            result[row, col] = new Node { Col = col, Row = row, Value = grid[row, col] };

        foreach (var cell in grid.Cells())
        {
            var node = result[cell];
            foreach (var n in grid.SurroundingCells(cell, allowDiagonal))
            {
                var neighbor = result[n];
                node.ConnectTo(neighbor, neighbor.Value);
                neighbor.ConnectTo(node, node.Value);
            }
        }

        return result;
    }

    private class NullHeuristic : IAstarHeuristic<Node>
    {
        public float DetermineCost(Node a, Node b) => 0;
    }

    public class Node : IAstarNode<Node>
    {
        public int Row { get; init; }
        public int Col { get; init; }
        public int Value { get; init; }

        public IEnumerable<AStarNodeConnection<Node>> NeighboringNodes => neighbors.Values;

        public float GetCostForNeighbor(Node neighbor)
        {
            if (!neighbors.TryGetValue(neighbor.MakeKey(), out var connection))
                throw new Exception("Invalid neighbor!");

            return connection.Cost;
        }

        public bool IsConnectedTo(Node other)
        {
            return neighbors.ContainsKey(other.MakeKey());
        }

        public void ConnectTo(Node other, float cost)
        {
            neighbors[other.MakeKey()] = new AStarNodeConnection<Node>(other, cost);
        }

        public override string ToString()
        {
            return $"{Col} {Row}";
        }

        private int MakeKey()
        {
            return Col * 10000 + Row;
        }

        private readonly Dictionary<int, AStarNodeConnection<Node>> neighbors = new();
    }

    private readonly Grid<Node> grid;
}
