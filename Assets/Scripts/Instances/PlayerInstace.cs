using UnityEngine;

public class PlayerInstace : ScriptableObject
{
    public PlayerModel Model { get; private set; }
    public string Name { get; private set; }
    public int Order { get; private set; }
    public CastleModel StartingCastle { get; private set; }
    public Color Color { get; private set; }

    internal void Init(PlayerModel model, string name, int order, CastleModel startingCastle, Color color)
    {
        Model = model;
        Name = name;
        Order = order;
        StartingCastle = startingCastle;
        Color = color;
    }
}
