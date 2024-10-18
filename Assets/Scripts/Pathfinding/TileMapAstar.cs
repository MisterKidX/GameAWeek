using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapAstar : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap placementTilemap;

    private List<Vector3Int> path = new List<Vector3Int>();

    public Vector3Int startPos;
    public Vector3Int endPos;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 0;
            var worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
            var pos = groundTilemap.WorldToCell(worldPoint);
            var tile = groundTilemap.GetTile(pos);

            if (groundTilemap != null)
                startPos = pos;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 0;
            var worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
            var pos = groundTilemap.WorldToCell(worldPoint);
            var tile = groundTilemap.GetTile(pos);

            if (groundTilemap != null)
                endPos = pos;
        }

        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            FindPathAndDraw();
        }
    }

    public void FindPathAndDraw()
    {
        path = FindPath();
    }

    public List<Vector3Int> FindPath()
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        Node startNode = new Node(startPos, null, 0, GetHCost(startPos, endPos));
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

            if (currentNode.Position == endPos)
            {
                return RetracePath(currentNode);
            }

            foreach (Vector3Int neighbor in GetNeighbors(currentNode.Position))
            {
                if (!IsWalkable(neighbor) || closedSet.Contains(new Node(neighbor, null, 0, 0)))
                    continue;

                float newMovementCostToNeighbor = currentNode.GCost + GetGCost(currentNode.Position, neighbor);
                Node neighborNode = new Node(neighbor, currentNode, newMovementCostToNeighbor, GetHCost(neighbor, endPos));

                if (!openSet.Exists(n => n.Position == neighbor) || newMovementCostToNeighbor < neighborNode.GCost)
                {
                    neighborNode.GCost = newMovementCostToNeighbor;
                    neighborNode.HCost = GetHCost(neighbor, endPos);
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
            movementDirectionCost += 1f;
        else
            movementDirectionCost += 1.5f;

        return movementDirectionCost * nextTile.MovementCost;
    }

    float GetHCost(Vector3Int pos, Vector3Int end)
    {
        var d = end - pos;
        d = new Vector3Int(Mathf.Abs(d.x), Mathf.Abs(d.y));
        var diagonals = Mathf.Min(d.x, d.y);
        var straights = d.y + d.x - diagonals;
        return (diagonals * 1.5f + straights) * 1.05f;
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


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (path != null && path.Count > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(groundTilemap.CellToWorld(path[i]) + new Vector3(0.5f, 0.5f, 0),
                                groundTilemap.CellToWorld(path[i + 1]) + new Vector3(0.5f, 0.5f, 0));
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundTilemap.CellToWorld(startPos) + new Vector3(0.5f, 0.5f, 0), 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(groundTilemap.CellToWorld(endPos) + new Vector3(0.5f, 0.5f, 0), 0.2f);
        }
    }
#endif
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