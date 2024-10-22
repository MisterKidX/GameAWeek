using UnityEngine;

[CreateAssetMenu(fileName = "Resource Pickup", menuName = "Tiles/Resource Pickup Tile")]
public class ResourcePickupTile : AquirableTile
{
    public int Amount;
    public ResourceModel ResourceModel;

    public override void Aquire(PlayerInstace player)
    {
        var inst = ResourceModel.Create(Amount);
        player.GetResource(inst);
    }
}