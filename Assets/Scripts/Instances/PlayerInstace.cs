using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInstace : ScriptableObject
{
    public PlayerModel Model { get; private set; }
    public string Name { get; private set; }
    public int Order { get; private set; }
    public CastleModel StartingCastle { get; private set; }
    public Color Color { get; private set; }
    public List<CastleInstance> Castles = new();
    public List<HeroInstance> Heroes = new();
    public HashSet<ResourceInstance> Resources = new();

    public bool HasCastle => Castles.Count > 0;

    //cheat!
    public HeroInstance SelectedHero => HasHeroes ? Heroes[0] : null;
    public bool HasHeroSelected => SelectedHero != null;
    public bool HasHeroes => Heroes != null && Heroes.Count > 0;

    internal void Init(PlayerModel model, string name, int order, CastleModel startingCastle, Color color)
    {
        Model = model;
        Name = name;
        Order = order;
        StartingCastle = startingCastle;
        Color = color;

        Resources = GameConfig.GetStartingResouces().ToHashSet();
    }

    internal void AddResource(ResourceInstance inst)
    {
        var res = Resources.First(r => r.Model == inst.Model);
        res.Amount += inst.Amount;
        Destroy(inst);
    }

    public void AddResource(ResourceModel resource, int amount)
    {
        var res = Resources.First(r => r.Model == resource);
        res.Amount += amount;
    }

    internal void NewTurn()
    {
        foreach (var castle in Castles)
        {
            castle.Reset();
        }
    }

    public int GetMaxRecruitment(UnitInstance unit)
    {
        var cost = unit.Model.Cost;
        var resourceAmount = Resources.First(r => r.Model == cost.Resource).Amount;

        return (int)Mathf.Min(Mathf.Floor((float)resourceAmount / cost.Amount), unit.Amount);
    }

    public bool CanPurchaseUnit(UnitModel unit, int amount, CastleInstance atCastle)
    {
        var unitTotalCost = amount * unit.Cost.Amount;
        var resource = Resources.First(r => r.Model == unit.Cost.Resource);
        var resourceAmount = resource.Amount;

        if (unitTotalCost > resourceAmount)
            return false;
        if (!atCastle.CastledUnits.Any(u => u == null))
            return false;

        return true;
    }

    public void Purchase(UnitModel unit, int amount, CastleInstance atCastle)
    {
        if (!CanPurchaseUnit(unit, amount, atCastle))
            throw new InvalidOperationException("Cannot perform purchase operation for this unit.");

        var unitTotalCost = amount * unit.Cost.Amount;
        var resource = Resources.First(r => r.Model == unit.Cost.Resource);
        var resourceAmount = resource.Amount;

        resource.Amount -= unitTotalCost;

        // check to see if any of the castled untis can be stacked
        var stackableUnit = atCastle.CastledUnits.
            FirstOrDefault(u => u != null && u.Model == unit);

        if (stackableUnit != null)
        {
            stackableUnit.Amount += amount;
        }
        else
        {
            int i = 0;
            for (; i < atCastle.CastledUnits.Length; i++)
            {
                if (atCastle.CastledUnits[i] == null)
                {
                    var inst = unit.Create(amount, true);
                    atCastle.CastledUnits[i] = inst;
                    break;
                }
            }
        }
    }

    internal void Purchase(BuildingModel model)
    {
        foreach (var cost in model.Cost)
            Resources.First(r => r.Model == cost.Resource).Amount -= cost.Amount;
    }

    internal void Purchase(ResourceCost[] costs)
    {
        foreach (ResourceCost cost in costs)
        {
            var resource = Resources.First(r => r.Model == cost.Resource);
            resource.Amount -= cost.Amount;
        }
    }

    public bool HasEnoughResources(ResourceCost[] costs)
    {
        foreach (ResourceCost cost in costs)
        {
            var resource = Resources.First(r => r.Model == cost.Resource);
            if (resource.Amount < cost.Amount)
                return false;
        }

        return true;
    }

    private void OnDestroy()
    {
        if (HasCastle)
        {
            while (Castles.Count != 0)
            {
                Destroy(Castles[0]);
                Castles.RemoveAt(0);
            }
        }
        if (HasHeroes)
        {
            while (Heroes.Count != 0)
            {
                Destroy(Heroes[0]);
                Heroes.RemoveAt(0);
            }
        }
        if (Resources.Count > 0)
        {
            foreach (var r in Resources)
            {
                Destroy(r);
            }
            Resources.Clear();
        }
    }
}
