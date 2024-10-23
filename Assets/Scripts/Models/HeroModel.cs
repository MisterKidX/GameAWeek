using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Models/Hero")]
public class HeroModel : ScriptableObject
{
    public string Name;
    public HeroView p_View;
    public Sprite Portrait;
    public int BaseMovementPoints = 10;

    internal HeroInstance Create(Vector3Int pos, PlayerInstace player, UnitInstance[] units)
    {
        var inst = ScriptableObject.CreateInstance<HeroInstance>();
        inst.Init(this, pos, player, units);

        return inst;
    }
}