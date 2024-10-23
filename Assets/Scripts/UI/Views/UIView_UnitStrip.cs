using System;
using UnityEngine;

public class UIView_UnitStrip : MonoBehaviour
{
    [SerializeField]
    UIView_UnitBox[] UnitBoxes;

    internal void Init(HeroInstance hero, UnitInstance[] units)
    {
        UnitBoxes[0].Init(hero == null ? null : hero.Model.Portrait, 0, false);

        for (int i = 0; i < units.Length; i++)
        {
            UnitBoxes[i + 1].Init(units[i] == null ? null : units[i].Model.Portrait,
                units[i] == null ? 0 : units[i].Amount);
        }
    }
}
