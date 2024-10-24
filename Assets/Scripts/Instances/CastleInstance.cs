using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CastleInstance : ScriptableObject
{
    public Vector3 Position;
    public CastleModel Model;
    public PlayerInstace Holder;
    public string Name;

    private UnitInstance[] _castledUnits;
    public UnitInstance[] CastledUnits
    {
        get
        {
            return CastledHero == null ? _castledUnits : CastledHero.Units;
        }
        set
        {
            if (CastledHero != null)
                throw new InvalidOperationException("Cannot set castled units when a hero is castled");

            _castledUnits = value;
        }
    }

    public HeroInstance CastledHero;
    public HeroInstance VisitingHero
    {
        get
        {
            foreach (var hero in Holder.Heroes)
                if (hero.Position == Position)
                    return hero;

            return null;
        }
    }

    public Dictionary<BuildingModel, BuildingInstance> BuiltBuildings = new();

    CastleView _view;

    public bool HasCastledHero => CastledHero != null;
    public bool HasVisitingHero => VisitingHero != null;

    public bool BuiltThisTurn { get; private set; }

    public void Init(CastleModel model, Vector3 postion, PlayerInstace holder, string name, UnitInstance[] castledUnits = null)
    {
        Model = model;
        Name = name;
        Holder = holder;
        Position = postion;
        _castledUnits = castledUnits;

        if (CastledUnits == null)
            _castledUnits = new UnitInstance[7];
        else if (CastledUnits.Length != 7)
            throw new ArgumentException("", nameof(castledUnits));

        foreach(var buildingModel in Model.Buildings)
        {
            // the special building which must always be built
            if (buildingModel.Order == 0)
            {
                BuiltBuildings.Add(buildingModel, (buildingModel as IncomeGeneratorBuildingModel).Create(this));
            }
            else
                BuiltBuildings.Add(buildingModel, null);
        }
    }

    internal void Show()
    {
        if (_view ==null)
        {
            _view = Instantiate(Model.p_View);
        }

        _view.Init(this);
    }

    public bool HasEnoughResourcesForBuilding(BuildingModel bmodel)
    {
        var resourcesNeeded = bmodel.Cost;
        var playerResources = Holder.Resources;

        return playerResources.Join(
            resourcesNeeded,
            resourceInstance => resourceInstance.Model,
            resourceCost => resourceCost.Resource,
            (ri,rc) => new { playerHas = ri.Amount, buildingDemands = rc.Amount })
            .All(r => r.playerHas > r.buildingDemands);
    }

    public void Reset()
    {
        BuiltThisTurn = false;
    }
}
