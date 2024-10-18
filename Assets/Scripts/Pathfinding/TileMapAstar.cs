using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapAstar : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap placementTilemap;

    public List<Vector3Int> CalculatePath(Vector3Int start, Vector3Int end)
    {
        var gEnd = groundTilemap.GetTile(end);
        var pEnd = (placementTilemap.GetTile(end) as BaseTile);
        if (start == end)
            return new List<Vector3Int>() { start };
        if (gEnd == null || (pEnd != null && !pEnd.GroundTraversable))
            return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        Node startNode = new Node(start, null, 0, GetHCost(start, end));
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost ||
                    (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode.Position == end)
            {
                return RetracePath(currentNode);
            }

            foreach (Vector3Int neighbor in GetNeighbors(currentNode.Position))
            {
                if (!IsWalkable(neighbor) || closedSet.Contains(new Node(neighbor, null, 0, 0)))
                    continue;

                float newMovementCostToNeighbor = currentNode.GCost + GetGCost(currentNode.Position, neighbor);
                Node neighborNode = new Node(neighbor, currentNode, newMovementCostToNeighbor, GetHCost(neighbor, end));

                if (!openSet.Exists(n => n.Position == neighbor) || newMovementCostToNeighbor < neighborNode.GCost)
                {
                    neighborNode.GCost = newMovementCostToNeighbor;
                    neighborNode.HCost = GetHCost(neighbor, end);
                    neighborNode.Parent = currentNode;

                    if (!openSet.Exists(n => n.Position == neighbor))
                    {
                        openSet.Add(neighborNode);
                    }
                }
            }
        }

        return new List<Vector3Int>();
    }

    List<Vector3Int> RetracePath(Node endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    private float GetGCost(Vector3Int current, Vector3Int next)
    {
        BaseTile currentTile = groundTilemap.GetTile(current) as BaseTile;
        BaseTile nextTile = groundTilemap.GetTile(next) as BaseTile;

        float movementDirectionCost = 0;
        if (current.x == next.x || current.y == next.y)
            movementDirectionCost = 1f;
        else
            movementDirectionCost = 1.8f;

        return movementDirectionCost * nextTile.MovementCost;
    }

    float GetHCost(Vector3Int pos, Vector3Int end)
    {
        var d = end - pos;
        d = new Vector3Int(Mathf.Abs(d.x), Mathf.Abs(d.y));
        var diagonals = Mathf.Min(d.x, d.y);
        var straights = d.y + d.x - diagonals;
        return (d.x + d.y) * 0.5f;
    }

    bool IsWalkable(Vector3Int position)
    {
        BaseTile tile = groundTilemap.GetTile(position) as BaseTile;
        BaseTile tile2 = placementTilemap.GetTile(position) as BaseTile;
        if (tile == null) return false;
        if (tile2 != null && !tile2.GroundTraversable) return false;
        if (tile.TileType != TileType.Ground) return false;

        return true;
    }

    List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>
        {
            new Vector3Int(position.x, position.y + 1, 0),
            new Vector3Int(position.x, position.y - 1, 0),
            new Vector3Int(position.x + 1, position.y, 0),
            new Vector3Int(position.x + 1, position.y + 1, 0),
            new Vector3Int(position.x + 1, position.y - 1, 0),
            new Vector3Int(position.x - 1, position.y, 0),
            new Vector3Int(position.x - 1, position.y + 1, 0),
            new Vector3Int(position.x - 1, position.y - 1, 0),
        };
        return neighbors;
    }
}

public class Node
{
    public Vector3Int Position;
    public Node Parent;
    public float GCost;        
    public float HCost;        
    public float FCost => GCost + HCost;

    public Node(Vector3Int position, Node parent, float gCost, float hCost)
    {
        Position = position;
        Parent = parent;
        GCost = gCost;
        HCost = hCost;
    }
}