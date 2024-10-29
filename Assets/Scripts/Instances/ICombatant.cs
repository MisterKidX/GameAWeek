using UnityEngine;

public interface ICombatant
{
    public string Name { get; }
    public Sprite Portrait { get; }
    public UnitInstance[] Units { get; }
    public void Die();
}
