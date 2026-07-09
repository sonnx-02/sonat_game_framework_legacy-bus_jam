using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PathResult
{
    public int startVertex;
    public int[] shortestDistances;
    public int[] parents;

    public void PrintSolution()
    {
        int nVertices = shortestDistances.Length;
        Debug.Log("Vertex\t Distance\tPath");

        for (int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
        {
            List<int> path = new List<int>();
            if (vertexIndex != startVertex)
            {
                GetPath(vertexIndex, parents, ref path);
                Debug.Log($"{startVertex}->{vertexIndex} ({shortestDistances[vertexIndex]}): " +
                          string.Join("->", path.Select(x => x.ToString())));
            }
        }
    }

    public List<int> GetPath(int to, bool log = false)
    {
        int nVertices = shortestDistances.Length;
//        Debug.Log($"Path from {startVertex} to {to}");

        for (int vertexIndex = 0; vertexIndex < nVertices; vertexIndex++)
        {
            List<int> path = new List<int>();
            if (vertexIndex == to)
            {
                GetPath(vertexIndex, parents, ref path);
                if (shortestDistances[vertexIndex] > int.MaxValue / 2)
                {
                    return new List<int>() {startVertex};
                }

                return path;
            }
        }

        return null;
    }

    // Function to print shortest path
    // from source to currentVertex
    // using parents array
    private static void GetPath(int currentVertex, int[] parents, ref List<int> path)
    {
        // Base case : Source node has
        // been processed
        if (currentVertex == DijkstrasAlgorithm.NO_PARENT || currentVertex == parents[currentVertex])
        {
            return;
        }

        GetPath(parents[currentVertex], parents, ref path);
        path.Add(currentVertex);
    }
}

public class DijkstrasAlgorithm
{
    public static readonly int NO_PARENT = -1;


    // Function that implements Dijkstra's
    // single source shortest path
    // algorithm for a graph represented
    // using adjacency matrix
    // representation
    public static PathResult Dijkstra(int[,] adjacencyMatrix, int startVertex, bool logPath)
    {
        int nVertices = adjacencyMatrix.GetLength(0);

        // shortestDistances[i] will hold the
        // shortest distance from src to i
        int[] shortestDistances = new int[nVertices];

        // added[i] will true if vertex i is
        // included / in shortest path tree
        // or shortest distance from src to
        // i is finalized
        bool[] added = new bool[nVertices];

        // Initialize all distances as
        // INFINITE and added[] as false
        for (int vertexIndex = 0;
            vertexIndex < nVertices;
            vertexIndex++)
        {
            shortestDistances[vertexIndex] = int.MaxValue;
            added[vertexIndex] = false;
        }

        // Distance of source vertex from
        // itself is always 0
        shortestDistances[startVertex] = 0;

        // Parent array to store shortest
        // path tree
        int[] parents = new int[nVertices];

        // The starting vertex does not
        // have a parent
        parents[startVertex] = NO_PARENT;

        // Find shortest path for all
        // vertices
        for (int i = 1; i < nVertices; i++)
        {
            // Pick the minimum distance vertex
            // from the set of vertices not yet
            // processed. nearestVertex is
            // always equal to startNode in
            // first iteration.
            int nearestVertex = -1;
            int shortestDistance = int.MaxValue;
            for (int vertexIndex = 0;
                vertexIndex < nVertices;
                vertexIndex++)
            {
                if (!added[vertexIndex] &&
                    shortestDistances[vertexIndex] <
                    shortestDistance)
                {
                    nearestVertex = vertexIndex;
                    shortestDistance = shortestDistances[vertexIndex];
                }
            }

            // Mark the picked vertex as
            // processed
            if (nearestVertex >= 0)
            {
                added[nearestVertex] = true;

                // Update dist value of the
                // adjacent vertices of the
                // picked vertex.
                for (int vertexIndex = 0;
                    vertexIndex < nVertices;
                    vertexIndex++)
                {
                    int edgeDistance = adjacencyMatrix[nearestVertex, vertexIndex];

                    if (edgeDistance > 0
                        && ((shortestDistance + edgeDistance) <
                            shortestDistances[vertexIndex]))
                    {
                        parents[vertexIndex] = nearestVertex;
                        shortestDistances[vertexIndex] = shortestDistance +
                                                         edgeDistance;
                    }
                }
            }
        }

        PathResult result = new PathResult()
        {
            startVertex = startVertex,
            shortestDistances = shortestDistances,
            parents = parents,
        };

        if (logPath)
            result.PrintSolution();
        return result;
    }


    public static void Example()
    {
        int[,] adjacencyMatrix =
        {
            {0, 4, 0, 0, 0, 0, 0, 8, 0},
            {4, 0, 8, 0, 0, 0, 0, 11, 0},
            {0, 8, 0, 7, 0, 4, 0, 0, 2},
            {0, 0, 7, 0, 9, 14, 0, 0, 0},
            {0, 0, 0, 9, 0, 10, 0, 0, 0},
            {0, 0, 4, 0, 10, 0, 2, 0, 0},
            {0, 0, 0, 14, 0, 2, 0, 1, 6},
            {8, 11, 0, 0, 0, 0, 1, 0, 7},
            {0, 0, 2, 0, 0, 0, 6, 7, 0}
        };
        Dijkstra(adjacencyMatrix, 0, true);
    }
}