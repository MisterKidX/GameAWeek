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
    public HeroInstance SelectedHero => Heroes[0];
    public bool HasHeroSelected => SelectedHero != null;

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
}
