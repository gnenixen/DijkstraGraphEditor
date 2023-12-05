using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CDijkstra {
    // Simple comparer, wherer [0] is connection weight and [1] is node id
    private class CDistanceComparer : IComparer<int[]> {
        public int Compare(int[] x, int []y) {
            if (x[0] == y[0]) 
                return x[1] - y[1];
            
            return x[0] - y[0];
        }
    }

    public List<int> Search(CGraph<Vector2> graph, int from, int to) {
        if (from == to)
            return null;

        /*
            In process of graph edit some nodes can be removed, and we get OutOfBounds error when try (for example)
            access 7 node when only 5 is active. To prevent errors all access to nodes by real id must be processed
            by slotToId[id].
        */
        Dictionary<int, int> slotsToId = new();
        List<int> activeNodes = graph.GetActiveNodes();
        for (int i = 0; i < activeNodes.Count; i++) {
            slotsToId.Add(activeNodes[i], i);
        }

        int NodeIdToNodeIndex(int id) => slotsToId[id];

        /*
            One list of pair (node, weigth) per connection, where index is source node
                connections[SourceNode] => List<(node, weight)> 'all connections accessed'
        */
        List<(int node, float weight)>[] connections = new List<(int node, float weight)>[graph.NodesCount];
        for (int i = 0; i < graph.NodesCount; i++) {
            connections[i] = graph.GetAllConnectionsWithNode(i);
        }

        /*
            Use SortedSet as PriorityQueue to reduce complexity of Dijkstra alghoritm
            from O(N^2) to O(C * log(N)) where C - connections, N - nodes
        */
        SortedSet<int[]> queue = new(new CDistanceComparer());

        // Handle result minimum distances from 'from' node to any other
        int[] distance = new int[graph.NodesCount];

        // Handle all paths to build 'from->to' path after main stage processed
        List<int>[] paths = new List<int>[graph.NodesCount];

        for (int i = 0; i < graph.NodesCount; i++) {
            distance[i] = int.MaxValue;
            paths[i] = new List<int>();
        }

        // 1) Main stage - Dijkstra alghoritm

        queue.Add(new int[] {0, from});
        distance[from] = 0;

        while (queue.Count > 0) {
            int[] minDistanceNode = queue.Min;
            queue.Remove(minDistanceNode);
            int u = NodeIdToNodeIndex(minDistanceNode[1]);

            foreach ((int rawNodeId, float weight) in connections[u]) {
                var node = NodeIdToNodeIndex(rawNodeId);

                if (distance[node] > distance[u] + weight) {
                    distance[node] = distance[u] + (int)weight;
                    paths[node].Add(u);
                    queue.Add(new int[] {distance[node], node});
                }
            }
        }

        // 2) Path build stage - restor path from nodes visit history

        List<int> result = new();

        int n = to;
        result.Add(to);

        while (n != from) {
            /*
                From and To nodes is in two different graphs (even while they are in single CGraph, they are not connected properly),
                pathfinding is impossible, just return null and print error
            */
            if (paths[NodeIdToNodeIndex(n)].Count == 0) {
                Debug.LogError("Canno't find path from '" + from + "' to '" + to + "' nodes, most likely reason is they are not connected properly.");
                return null;
            }

            var nextNode = paths[NodeIdToNodeIndex(n)].Last();

            result.Add(nextNode);
            n = nextNode;
        }

        result.Reverse();

        return result;
    }
}
