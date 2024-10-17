using UnityEngine;

public class HeroInstance : ScriptableObject
{
    public HeroModel Model;
    public Vector3 Pos;
    public PlayerInstace Holder;

    HeroView _view;

    internal void Init(HeroModel heroModel, Vector3 pos, PlayerInstace player)
    {
        Model = heroModel;
        Pos = pos;
        Holder = player;
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
