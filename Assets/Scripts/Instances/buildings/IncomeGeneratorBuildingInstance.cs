using System;
using System.Linq;
using UnityEngine;

public class IncomeGeneratorBuildingInstance : ScriptableObject, ITimeableReactor
{
    public IncomeGeneratorBuildingModel Model { get; private set; }
    public PlayerInstace Holder { get; private set; }

    public int ReactionTime => Model.OverTime;

    public void React(int days)
    {
        Holder.AddResource(Model.Resource, Model.Income);
    }

    internal void Init(IncomeGeneratorBuildingModel incomeGeneratorBuildingModel, PlayerInstace holder)
    {
        Model = incomeGeneratorBuildingModel;
        Holder = holder;
    }

    private void Awake() => (this as ITimeableReactor).EnlistToTimeManager();
}
