using UnityEngine;

public class UnitBuildingInstance : BuildingInstance, ITimeableReactor
{
    public override BuildingModel Model => Model;
    public UnitBuildingModel UModel { get; private set; }
    public CastleInstance Holder { get; private set; }

    public UnitInstance AvailableForPurchase;
    public int ReactionTime => 1;
    private void Awake()
    {
        (this as ITimeableReactor).EnlistToTimeManager();
    }

    public void Init(UnitBuildingModel model, CastleInstance holder)
    {
        UModel = model;
        Holder = holder;
        AvailableForPurchase = UModel.Unit.Create(UModel.Unit.Growth / 2);
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