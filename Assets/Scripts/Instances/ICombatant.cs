using UnityEngine;

public interface ICombatant
{
    public string Name { get; }
    public Sprite Portrait { get; }
    public bool Controlled { get; }
    public UnitInstance[] Units { get; }
    public void Die();
}
