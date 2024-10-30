using UnityEngine;

[CreateAssetMenu(menuName = "Models/Buildings/Tavern")]
public class HeroRecruiterBuildingModel : BuildingModel
{
    public ResourceCost[] HeroCost;

    public override BuildingInstance BaseCreate(CastleInstance instance) => Create(instance);

    public HeroRecruitBuildingInstance Create(CastleInstance instance) 
    {
        var inst = ScriptableObject.CreateInstance<HeroRecruitBuildingInstance>();
        inst.Init(this, instance);

        return inst;
    }
}
