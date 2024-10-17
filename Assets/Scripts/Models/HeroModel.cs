using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Models/Hero")]
public class HeroModel : ScriptableObject
{
    public string Name;
    public HeroView p_View;

    internal HeroInstance Create(Vector3 pos, PlayerInstace player)
    {
        var inst = ScriptableObject.CreateInstance<HeroInstance>();
        inst.Init(this, pos, player);

        return inst;
    }
}