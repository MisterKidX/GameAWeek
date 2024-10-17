using System;
using UnityEngine;

public class CastleInstance : ScriptableObject
{
    public Vector3 Position;
    public CastleModel Model;
    public PlayerInstace Holder;
    public string Name;

    CastleView _view;

    public void Init(CastleModel model, Vector3 postion, PlayerInstace holder, string name)
    {
        Model = model;
        Name = name;
        Holder = holder;
        Position = postion;
    }

    internal void Show()
    {
        if (_view ==null)
        {
            _view = Instantiate(Model.p_View);
        }

        _view.Init(this);
    }
}
