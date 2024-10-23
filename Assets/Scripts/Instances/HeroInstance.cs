using System;
using UnityEngine;

public class HeroInstance : ScriptableObject
{
    public HeroModel Model;
    public Vector3Int Position;
    public PlayerInstace Holder;
    public UnitInstance[] Units;

    public float RemainingMovementPoints { get; set; }
    public float MovementPointsForCurrentTurn { get; private set; }
    public float NormalizedMovementPoints => (float)RemainingMovementPoints / MovementPointsForCurrentTurn;

    public HeroView View { get; private set; }

    internal void Init(HeroModel heroModel, Vector3Int pos, PlayerInstace player, UnitInstance[] units)
    {
        Model = heroModel;
        Position = pos;
        Holder = player;
        RemainingMovementPoints = Model.BaseMovementPoints;
        MovementPointsForCurrentTurn = Model.BaseMovementPoints;

        Units = units;

        if (Units.Length != 7)
            throw new ArgumentException("", nameof(units));
    }

    internal void Show()
    {
        if (View == null)
        {
            View = Instantiate(Model.p_View);
            View.Init(this);
        }
    }
}
