using UnityEngine;

[CreateAssetMenu(fileName = "Income Generator", menuName = "Models/Buildings/Income Generator")]
public class IncomeGeneratorBuildingModel : BuildingModel
{
    [Space]
    public ResourceModel Resource;
    public int Income;
    public int OverTime = 1;

    public IncomeGeneratorBuildingInstance Create(PlayerInstace holder)
    {
        IncomeGeneratorBuildingInstance instance = ScriptableObject.CreateInstance<IncomeGeneratorBuildingInstance>();
        instance.Init(this, holder);

        return instance;
    }
}