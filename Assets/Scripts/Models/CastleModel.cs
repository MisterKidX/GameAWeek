using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Models/Castle")]
public class CastleModel : ScriptableObject
{
    public string Name;
    public Sprite Portrait;
    public CastleView p_View;
    public UIView_Castle p_UIView;
    public HeroModel[] Heroes;
    public UnitModel[] Units;
    public BuildingModel[] Buildings;

    internal CastleInstance Create(Vector3Int position, PlayerInstace holder = null, string name = null)
    {
        var inst = ScriptableObject.CreateInstance<CastleInstance>();
        name ??= holder.Name + "'s Castle";
        inst.Init(this, position, holder, name);

        return inst;
    }
}