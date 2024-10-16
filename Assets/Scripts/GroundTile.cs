using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="Tiles/Ground")]
public class GroundTile : Tile
{
    public TileType TileType = TileType.Ground;
    public int MovementCost = 1;
    public bool Traversable = true;
}
