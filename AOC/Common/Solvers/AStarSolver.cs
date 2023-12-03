using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Common.Solvers;

public abstract class AStarSolver<
    TNode,
    THeuristic
>
where TNode : class, IAstarNode<TNode>
where THeuristic : IAstarHeuristic<TNode>
{
    private HashSet<TNode> Graph;
    private THeuristic Heuristic;

    protected void Initialize(IEnumerable<TNode> graph, THeuristic heuristic)
    {
        Graph = new HashSet<TNode>(graph);
        Heuristic = heuristic;
    }

    public List<TNode> Solve(TNode start, TNode goal)
    {
        if (start == null) throw new ArgumentNullException(nameof(start));
        if (goal == null) throw new ArgumentNullException(nameof(goal));

        if (!Graph.Contains(start)) throw new Exception("Start node is invalid");
        if (!Graph.Contains(goal)) throw new Exception("Goal node is invalid");

        var frontier = new PriorityQueue(Graph.Count);
        frontier.Enqueue(start, 0);

        var cameFrom = new Dictionary<TNode, TNode>();
        var costSoFar = new Dictionary<TNode, float>();
        cameFrom[start] = null;
        costSoFar[start] = 0;

        while (frontier.Count != 0)
        {
            var current = frontier.Dequeue();
            if (current == goal)
                break;

            foreach (var neighbor in current.NeighboringNodes)
            {
                var next = neighbor.Node;
                if (cameFrom.ContainsKey(next)) continue;

                var newCost = costSoFar[current] + current.GetCostForNeighbor(next);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    var priority = newCost + Heuristic.DetermineCost(next, goal);
                    frontier.Enqueue(next, priority);
                    cameFrom[next] = current;
                }
            }
        }

        var result = new List<TNode>();
        var currentNode = goal;

        do
        {
            TNode next = currentNode;
            if (!cameFrom.TryGetValue(currentNode, out next))
                break;

            result.Insert(0, currentNode);
            currentNode = next;

            if (currentNode == null)
                break;
        } while (currentNode != start);

        result.Insert(0, start);
        return result;
    }

    private class PriorityQueue
    {
        private readonly List<NodeWrapper> data;

        private class NodeWrapper : IComparable<NodeWrapper>
        {
            public TNode Node;
            public float Priority;

            public int CompareTo(NodeWrapper other)
            {
                return Priority.CompareTo(other.Priority);
            }
        }

        public PriorityQueue(int count)
        {
            data = new List<NodeWrapper>(count);
        }

        public void Enqueue(TNode item, float priority)
        {
            data.Add(new NodeWrapper { Node = item, Priority = priority });
            int ci = data.Count - 1; // child index; start at end
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                if (data[ci].CompareTo(data[pi]) >= 0)
                    break; // child item is larger than (or equal) parent so we're done

                var tmp = data[ci];
                data[ci] = data[pi];
                data[pi] = tmp;
                ci = pi;
            }
        }

        public TNode Dequeue()
        {
            // assumes pq is not empty; up to calling code
            int li = data.Count - 1; // last index (before removal)
            var frontItem = data[0];   // fetch the front
            data[0] = data[li];
            data.RemoveAt(li);

            --li; // last index (after removal)
            int pi = 0; // parent index. start at front of pq
            while (true)
            {
                int ci = pi * 2 + 1; // left child index of parent
                if (ci > li)
                    break;  // no children so done

                int rc = ci + 1;     // right child
                if (rc <= li && data[rc].CompareTo(data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    ci = rc;
                if (data[pi].CompareTo(data[ci]) <= 0)
                    break; // parent is smaller than (or equal to) smallest child so done

                // swap parent and child
                var tmp = data[pi];
                data[pi] = data[ci];
                data[ci] = tmp;
                pi = ci;
            }

            return frontItem.Node;
        }

        public TNode Peek() => data[0].Node;
        public int Count => data.Count;
    }
}

public interface IAstarNode<TNode>
    where TNode : class, IAstarNode<TNode>
{
    IEnumerable<AStarNodeConnection<TNode>> NeighboringNodes { get; }
    float GetCostForNeighbor(TNode neighbor);
    bool IsConnectedTo(TNode other);
    void ConnectTo(TNode other, float cost);
}

public interface IAstarHeuristic<TNode>
    where TNode : class, IAstarNode<TNode>
{
    float DetermineCost(TNode a, TNode b);
}

public class AStarNodeConnection<TNode>
    where TNode : class, IAstarNode<TNode>
{
    public AStarNodeConnection(TNode node, float cost)
    {
        Node = node;
        Cost = cost;
    }

    public float Cost { get; }
    public TNode Node { get; }
}
