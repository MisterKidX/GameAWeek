using System;
using UnityEngine;

public class HeroInstance : ScriptableObject, ITimeableReactor
{
    public HeroModel Model;
    public Vector3Int Position;
    public PlayerInstace Holder;
    public UnitInstance[] Units;

    public float RemainingMovementPoints { get; set; }
    public float MovementPointsForCurrentTurn { get; private set; }
    public float NormalizedMovementPoints => (float)RemainingMovementPoints / MovementPointsForCurrentTurn;

    public HeroView View { get; private set; }

    private void Awake() => (this as ITimeableReactor).EnlistToTimeManager();

    public int ReactionTime => 1;

    public void React(int totalDays)
    {
        RemainingMovementPoints = MovementPointsForCurrentTurn;
    }

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
