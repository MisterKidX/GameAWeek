using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapAstar : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap placementTilemap;
    public Tilemap metadataTilemap;

    public List<PathPoint> CalculatePath(Vector3Int start, Vector3Int end)
    {
        if (start == end)
            return new List<PathPoint>() { new PathPoint(start, 0, 0) };
        else if (!IsTargetable(end))
            return null;

        List<Node> openSet = new List<Node>();
        Dictionary<Vector3Int, float> closedSet = new();

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
            if (!closedSet.ContainsKey(currentNode.Position))
                closedSet.Add(currentNode.Position, currentNode.FCost);
            else if (currentNode.FCost < closedSet[currentNode.Position])
                closedSet[currentNode.Position] = currentNode.FCost;

            if (currentNode.Position == end)
            {
                return RetracePath(currentNode);
            }

            foreach (Vector3Int neighbor in GetNeighbors(currentNode.Position))
            {
                if (neighbor != end && !IsWalkable(neighbor, end))
                    continue;
                if (closedSet.ContainsKey(neighbor) && closedSet[neighbor] < currentNode.FCost)
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

        return new List<PathPoint>();
    }

    List<PathPoint> RetracePath(Node endNode)
    {
        List<PathPoint> path = new();
        Node currentNode = endNode;

        while (currentNode != null)
        {
            path.Add(new PathPoint(currentNode.Position, currentNode.GCost,
                currentNode.Parent == null ? currentNode.GCost : currentNode.GCost - currentNode.Parent.GCost));
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
            movementDirectionCost = 1.6f;

        return movementDirectionCost * nextTile.MovementCost;
    }

    float GetHCost(Vector3Int pos, Vector3Int end)
    {
        var d = end - pos;
        d = new Vector3Int(Mathf.Abs(d.x), Mathf.Abs(d.y));
        var diagonals = Mathf.Min(d.x, d.y);
        var straights = Mathf.Abs(d.y - d.x);
        return (diagonals * 1.6f + straights) * 0.8f;
    }

    bool IsWalkable(Vector3Int position, Vector3Int end)
    {
        BaseTile groundTile = groundTilemap.GetTile(position) as BaseTile;
        BaseTile placementTile = placementTilemap.GetTile(position) as BaseTile;
        BaseTile hiddenBlocker = metadataTilemap.GetTile(position) as BaseTile;
        var threatTile = metadataTilemap.GetTile(position) as ThreatTile;
        bool nearEnd = false;

        if (threatTile != null)
            nearEnd = GetNeighbors(position).Any(pos => pos == end);

        if (groundTile == null) return false;
        if (groundTile.TileType != TileType.Ground) return false;
        if (placementTile != null && !placementTile.GroundTraversable) return false;
        if (hiddenBlocker != null && !hiddenBlocker.GroundTraversable) return false;
        if (threatTile != null && !nearEnd)
            return false;

        return true;
    }

    bool IsTargetable(Vector3Int end)
    {
        BaseTile groundTile = groundTilemap.GetTile(end) as BaseTile;
        BaseTile placementTile = placementTilemap.GetTile(end) as BaseTile;
        BaseTile aquirableTile = placementTilemap.GetTile(end) as AquirableTile;
        BaseTile hiddenBlocker = metadataTilemap.GetTile(end) as BaseTile;
        var threatTile = metadataTilemap.GetTile(end) as ThreatTile;

        if (groundTile == null) return false;
        if (groundTile.TileType != TileType.Ground) return false;
        if (placementTile != null && !placementTile.GroundTraversable && aquirableTile == null) return false;
        if (hiddenBlocker != null && !hiddenBlocker.GroundTraversable)
            return false;

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

public struct PathPoint
{
    public Vector3Int Position;
    public float AccumulatedMoveCost;
    public float MovemventCost;

    public PathPoint(Vector3Int position, float accumulatedMovementCost, float movementCost)
    {
        Position = position;
        AccumulatedMoveCost = accumulatedMovementCost;
        MovemventCost = movementCost;
    }
}