using UnityEngine;

[CreateAssetMenu(menuName = "Models/Unit")]
public class UnitModel : ScriptableObject
{
    public string Name;
    public Sprite Portrait;
    [Space]

    public int Attack;
    public int Defense;
    public int AttackRange = 1;
    public bool Flying = false;

    public Vector2 Damage;

    public int Health;
    public int Speed;
    public int Growth;


    public UnitInstance Create(int amount)
    {
        var inst = ScriptableObject.CreateInstance<UnitInstance>();
        inst.Init(this, amount);
        return inst;
    }
}
