using UnityEngine.Tilemaps;

public abstract class BaseTile : Tile
{
    public TileType TileType = TileType.Ground;
    public int MovementCost = 1;
    public bool GroundTraversable = true;
    public bool FlightTraversable = true;
}
