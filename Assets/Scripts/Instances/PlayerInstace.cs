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
    public List<ResourceInstance> Resources = new();

    public bool HasCastle => Castles.Count > 0;

    internal void Init(PlayerModel model, string name, int order, CastleModel startingCastle, Color color)
    {
        Model = model;
        Name = name;
        Order = order;
        StartingCastle = startingCastle;
        Color = color;

        Resources = GameConfig.GetStartingResouces().ToList();
    }
}
