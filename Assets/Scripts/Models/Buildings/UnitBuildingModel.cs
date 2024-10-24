using UnityEngine;

[CreateAssetMenu(fileName = "Unit Building", menuName = "Models/Buildings/Unit Building")]
public class UnitBuildingModel : BuildingModel
{
    [Space]
    public UnitModel Unit;

    public UnitBuildingInstance Create(CastleInstance holder)
    {
        var instance = ScriptableObject.CreateInstance<UnitBuildingInstance>();
        instance.Init(this, holder);

        return instance;
    }
}

public class UnitBuildingInstance : ScriptableObject, ITimeableReactor
{
    public UnitBuildingModel Model { get; private set; }
    public CastleInstance Holder { get; private set; }

    public UnitInstance AvailableForPurchase;
    public int ReactionTime => 1;
    private void Awake()
    {
        (this as ITimeableReactor).EnlistToTimeManager();
        AvailableForPurchase = Model.Unit.Create(0);
    }

    public void Init(UnitBuildingModel model, CastleInstance holder)
    {
        Model = model;
        Holder = holder;
    }

    public void React(int totalDays)
    {
        var day = totalDays % 7 != 0 ? totalDays % 7 : 7;
        var sunday = day == 1 ? true : false;

        if (sunday)
        {
            AvailableForPurchase.Amount += AvailableForPurchase.Model.Growth;
        }
    }
}