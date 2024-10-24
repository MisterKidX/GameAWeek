using System;
using System.Linq;
using UnityEngine;

public class IncomeGeneratorBuildingInstance : BuildingInstance, ITimeableReactor
{
    public override BuildingModel Model => IModel;
    public IncomeGeneratorBuildingModel IModel { get; private set; }
    public CastleInstance Holder { get; private set; }

    public int ReactionTime => IModel.OverTime;

    public void React(int days)
    {
        Holder.Holder.AddResource(IModel.Resource, IModel.Income);
    }

    internal void Init(IncomeGeneratorBuildingModel incomeGeneratorBuildingModel, CastleInstance holder)
    {
        IModel = incomeGeneratorBuildingModel;
        Holder = holder;
    }

    private void Awake() => (this as ITimeableReactor).EnlistToTimeManager();
}
