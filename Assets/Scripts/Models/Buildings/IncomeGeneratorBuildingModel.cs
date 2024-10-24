using UnityEngine;

[CreateAssetMenu(fileName = "Income Generator", menuName = "Models/Buildings/Income Generator")]
public class IncomeGeneratorBuildingModel : BuildingModel
{
    [Space]
    public ResourceModel Resource;
    public int Income;
    public int OverTime = 1;

    public IncomeGeneratorBuildingInstance Create(CastleInstance holder)
    {
        IncomeGeneratorBuildingInstance instance = ScriptableObject.CreateInstance<IncomeGeneratorBuildingInstance>();
        instance.Init(this, holder);

        return instance;
    }

    public override BuildingInstance BaseCreate(CastleInstance instance) => Create(instance);
}