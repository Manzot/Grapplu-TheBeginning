using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarPathfinding 
{
    public Node[,] Nodes { get; private set; }
    int width;
    int height;

    public AStarPathfinding(bool[,] map)
    {
        width = map.GetLength(0);
        height = map.GetLength(1);
        Nodes = new Node[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Nodes[i, j] = new Node(new Vector2Int(i, j), map[i, j]);
            }
        }
    }
    public List<Node> FindPath(Vector2Int startingNode, Vector2Int targetNode)
    {
        List<Node> path = new List<Node>();
        if(RecursivePathSearch(Nodes[startingNode.x, startingNode.y], Nodes[targetNode.x, targetNode.y]))
        {
            Node toAdd = Nodes[targetNode.x, targetNode.y];
            while (toAdd.parent != null)
            {
                path.Add(toAdd);
                toAdd = toAdd.parent;
            }
            path.Add(Nodes[startingNode.x, startingNode.y]);
            path.Reverse();
        }
        return path;
    }

    bool RecursivePathSearch(Node from, Node target)
    {
        from.state = Node.State.Closed;
        List<Node> candidateNextNodes = GetValidNeighbourTiles(from, target);
        candidateNextNodes = candidateNextNodes.OrderBy(node => node.F).ToList();
        foreach (Node nextNode in candidateNextNodes)
        {
            if (nextNode == target)
                return true;
            else if (RecursivePathSearch(nextNode, target))
                return true;
        }
        return false;
    }

    List<Node> GetValidNeighbourTiles(Node relativeTo, Node target)
    {
        List<Node> toRet = new List<Node>();
        List<Vector2Int> allSorroundingVectors = ReturnAllNeighbours(relativeTo.position);
        for (int i = 0; i < allSorroundingVectors.Count; i++)
        {
            int x = allSorroundingVectors[i].x;
            int y = allSorroundingVectors[i].y;
            if (x < 0 || x >= width || y < 0 || y >= height)
                continue;
            if(!(Nodes[x, y].isWalkable))
                continue;
            if (Nodes[x, y].state == Node.State.Closed)
                continue;
            if(Nodes[x,y].state == Node.State.Untested)
            {
                Nodes[x, y].parent = relativeTo;
                Nodes[x, y].state = Node.State.Open;
                Nodes[x, y].G = MovementCost(relativeTo, Nodes[x, y]);
                Nodes[x, y].H = Heuristoc(Nodes[x,y], target);
                toRet.Add(Nodes[x, y]);
            }
            else if (Nodes[x, y].state == Node.State.Open)
            {
                float simpleCost = MovementCost(relativeTo, Nodes[x, y]);
                float candidateGvalue = simpleCost + relativeTo.G;
                if(candidateGvalue < Nodes[x, y].G)
                {
                    Nodes[x, y].G = candidateGvalue;
                    Nodes[x, y].parent = relativeTo;
                }
            }
        }
        return toRet;
    }

    List<Vector2Int> ReturnAllNeighbours(Vector2Int relativeTo)
    {
        List<Vector2Int> toRet = new List<Vector2Int>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                toRet.Add(new Vector2Int(x, y) + relativeTo);
            }
        }
        //toRet.Add(new Vector2Int(-1, 0) + relativeTo);
        //toRet.Add(new Vector2Int(1, 0) + relativeTo);
        //toRet.Add(new Vector2Int(0, 1) + relativeTo);
        //toRet.Add(new Vector2Int(0, -1) + relativeTo);

        return toRet;
    }

    float Heuristoc(Node thisNode, Node targetNode)
    {
        return Vector2Int.Distance(thisNode.position, targetNode.position);
    }
    float MovementCost(Node from, Node to)
    {
        return Vector2Int.Distance(from.position, to.position);
    }
}
