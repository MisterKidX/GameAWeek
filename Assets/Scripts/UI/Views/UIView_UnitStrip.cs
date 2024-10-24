using System;
using System.Linq;
using UnityEngine;

public class UIView_UnitStrip : MonoBehaviour
{
    [SerializeField]
    UIView_UnitBox _heroBox;
    [SerializeField]
    UIView_UnitBox[] _unitBoxes;

    CastleInstance _castle;
    HeroInstance _hero;
    UnitInstance[] _units
    {
        get
        {
            if (_hero != null)
                return _hero.Units;
            else
                return _castle.CastledUnits;
        }
    }

    internal void Init(CastleInstance castle, bool castledUnits)
    {
        _castle = castle;
        _hero = castledUnits ? _castle.CastledHero : _castle.VisitingHero;

        Draw();
    }

    private void Draw()
    {
        _heroBox.Init(_hero == null ? null : _hero.Model.Portrait, 0, this, null);

        if (_units != null)
        {
            for (int i = 0; i < _units.Length; i++)
            {
                if (_units[i] == null)
                {
                    _unitBoxes[i].Init(null, -1, this, null);
                    _unitBoxes[i].Deactivate();
                }
                else
                    _unitBoxes[i].Init(_units[i].Model.Portrait, _units[i].Amount, this, WantToReposition);
            }
        }
        else
        {
            for (int i = 0; i < _unitBoxes.Length; i++)
            {
                _unitBoxes[i].Init(null, -1, this, null);
                _unitBoxes[i].Deactivate();
            }
        }
    }

    private void WantToReposition(UIView_UnitBox from, UIView_UnitBox to)
    {
        var fromIndex = GetIndex(from);
        var toIndex = GetIndex(to);

        // i.e. hero
        if (fromIndex == -1 || toIndex == -1)
            return;

        if (!to.Active)
        {
            to.Manager._units[toIndex] = _units[fromIndex];
            _units[fromIndex] = null;
        }
        else if (_units[fromIndex].Model == to.Manager._units[toIndex].Model)
        {
            to.Manager._units[toIndex].Amount += _units[fromIndex].Amount;
            _units[fromIndex] = null;
        }
        else
        {
            var temp = _units[fromIndex];
            _units[fromIndex] = to.Manager._units[toIndex];
            to.Manager._units[toIndex] = temp;
        }

        Draw();
        to.Manager.Draw();
    }

    private int GetIndex(UIView_UnitBox unitBox)
    {
        for (int i = 0; i < unitBox.Manager._unitBoxes.Length; i++)
        {
            if (unitBox == unitBox.Manager._unitBoxes[i])
                return i;
        }

        return -1;
    }
}
