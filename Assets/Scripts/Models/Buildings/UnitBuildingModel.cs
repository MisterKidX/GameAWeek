using UnityEngine;

[CreateAssetMenu(fileName = "Unit Building", menuName = "Models/Buildings/Unit Building")]
public class UnitBuildingModel : BuildingModel
{
    [Space]
    public UnitModel Unit;

    public UnitBuildingInstance Create(CastleInstance holder)
    {
        UnitBuildingInstance instance = ScriptableObject.CreateInstance<UnitBuildingInstance>();
        instance.Init(this, holder);

        return instance;
    }

    public override BuildingInstance BaseCreate(CastleInstance instance) => Create(instance);
}
