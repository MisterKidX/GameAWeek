using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/Random Unit")]
public class UnitTile : Tile
{
    public int Tier = 1;
    public CreatureCount Count;
}

public enum CreatureCount
{
    Few = 4,
    Several = 9,
    Pack = 19,
    Lots = 49,
    Horde = 99,
    Throng = 249,
    Swarm = 499,
    Zounds = 999,
    Legion = 10_000
}
