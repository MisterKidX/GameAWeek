using System;
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

    CastleView _view;

    public bool HasCastledHero => CastledHero != null;
    public bool HasVisitingHero => VisitingHero != null;

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
