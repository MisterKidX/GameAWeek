using UnityEngine;

[CreateAssetMenu(menuName = "Models/Unit")]
public class UnitModel : ScriptableObject
{
    public string Name;
    public Sprite Portrait;
    public int Tier;
    [Space]

    [Header("Combat")]
    public int Attack;
    public int Defense;
    public int AttackRange = 1;
    public bool Flying = false;

    public Vector2 Damage;

    public int Health;
    public int Speed;
    public int Growth;

    // This will be an array in the future
    public ResourceCost Cost;

    [Header("View")]
    public UnitCombatView p_combatView;
    public float HexAnimationSpeed = 0.2f;

    public bool IsRanged => AttackRange > 1;

    public UnitInstance Create(int amount, bool controlled)
    {
        var inst = ScriptableObject.CreateInstance<UnitInstance>();
        inst.Init(this, amount, controlled);
        return inst;
    }
}