using UnityEngine;

[CreateAssetMenu(fileName = "MineTile", menuName = "Tiles/Mine Tile")]
public class ResuorceGeneratorTile : AquirableTile
{
    public ResourceGeneratorModel ResourceGeneratorModel;

    private void OnValidate()
    {
        gameObject = ResourceGeneratorModel?.p_View?.gameObject;
    }

    public override void Aquire(PlayerInstace player)
    {

    }
}
