using System;
using UnityEngine;

public class HeroRecruitBuildingInstance : BuildingInstance
{
    public HeroRecruiterBuildingModel BuildingModel { get; private set; }
    public CastleInstance Holder;
    public override BuildingModel Model => BuildingModel;

    public HeroModel[] AvailableHeroes = new HeroModel[2];

    public void Init(HeroRecruiterBuildingModel model, CastleInstance holder)
    {
        BuildingModel = model;
        Holder = holder;
        AvailableHeroes = GameLogic.GetRandomAvailableHeroes(AvailableHeroes.Length);
    }

    public void ShowUIView(Action onConfirm)
    {
        UIView_HeroRecruitBuilding view = Instantiate(BuildingModel.BuildingView) as UIView_HeroRecruitBuilding;
        view.Init(this, (m) => { ConfirmHeroPurchase(m); onConfirm?.Invoke(); });
    }

    private void ConfirmHeroPurchase(HeroModel model)
    {
        Holder.Holder.Purchase(BuildingModel.Cost);
        var heroInst = GameLogic.CreateHeroInstanceFromCastle(model, Holder.Model, Holder.Position, Holder.Holder);
        heroInst.Show();
        LevelManager.CurrentLevel._gameplayUI.Refresh();
    }
}
