using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CombatPathfinder : MonoBehaviour
{
    [SerializeField]
    public Tilemap _walkable;
    public Tilemap _blockers;
    public Tilemap _ui;

    public List<PathPoint> CalculatePath(Vector3Int start, Vector3Int end)
    {
        if (start == end)
            return new List<PathPoint>() { new PathPoint(start, 0, 0) };
        else if (!IsTargetable(end))
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
                if (neighbor != end && !IsWalkable(neighbor) || closedSet.Any(n => n.Position == neighbor))
                    continue;
                else if (neighbor == end && !IsTargetable(neighbor))
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
        return 1;
    }

    float GetHCost(Vector3Int pos, Vector3Int end)
    {
        var d = end - pos;
        d = new Vector3Int(Mathf.Abs(d.x), Mathf.Abs(d.y));
        var diagonals = Mathf.Min(d.x, d.y);
        var straights = Mathf.Abs(d.y - d.x);
        return (diagonals * 1.6f + straights) * 0.8f;
    }

    bool IsWalkable(Vector3Int position)
    {
        return _blockers.GetTile(position) == null &&
            _walkable.GetTile(position) != null &&
            _ui.GetTile(position) != null;
    }

    bool IsTargetable(Vector3Int end)
    {
        return IsWalkable(end);
    }

    public List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        if (position.y % 2 == 0)
        {
            return new List<Vector3Int>
            {
                new Vector3Int(position.x, position.y + 1, 0),
                new Vector3Int(position.x, position.y - 1, 0),
                new Vector3Int(position.x + 1, position.y, 0),
                new Vector3Int(position.x - 1, position.y + 1, 0),
                new Vector3Int(position.x - 1, position.y - 1, 0),
                new Vector3Int(position.x - 1, position.y, 0),
            };
        }
        else
        {
            return new List<Vector3Int>
            {
                new Vector3Int(position.x, position.y + 1, 0),
                new Vector3Int(position.x, position.y - 1, 0),
                new Vector3Int(position.x + 1, position.y, 0),
                new Vector3Int(position.x + 1, position.y + 1, 0),
                new Vector3Int(position.x + 1, position.y - 1, 0),
                new Vector3Int(position.x - 1, position.y, 0),
            };
        }
    }
}