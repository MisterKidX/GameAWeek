using UnityEngine;

public class HeroInstance : ScriptableObject
{
    public HeroModel Model;
    public Vector3 Position;
    public PlayerInstace Holder;

    public int RemainingMovementPoints { get; private set; }
    public int MovementPointsForCurrentTurn { get; private set; }
    public float NormalizedMovementPoints => (float)RemainingMovementPoints / MovementPointsForCurrentTurn;

    HeroView _view;

    internal void Init(HeroModel heroModel, Vector3 pos, PlayerInstace player)
    {
        Model = heroModel;
        Position = pos;
        Holder = player;
        RemainingMovementPoints = Model.BaseMovementPoints;
        MovementPointsForCurrentTurn = Model.BaseMovementPoints;
    }

    internal void Show()
    {
        if (_view == null)
        {
            _view = Instantiate(Model.p_View);
            _view.Init(this);
        }
    }
}
