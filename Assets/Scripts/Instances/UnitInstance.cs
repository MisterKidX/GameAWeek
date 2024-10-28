using System;
using UnityEngine;

public class UnitInstance : ScriptableObject
{
    public Vector3 CombatWorldPosition;
    public Vector3Int CombatCellPosition;
    public UnitModel Model;
    public int Amount;
    public int CurrentHealth;

    public bool Selected { get; set; }
    public int CumulativeHP => (Amount - 1) * Model.Health + CurrentHealth;

    public event Action OnAttack;
    public event Action OnDefend;
    public event Action<UnitInstance> OnDie;

    public void Attack(UnitInstance defender)
    {
        OnAttack?.Invoke();
        defender.Defend(this);
    }

    private void Defend(UnitInstance attacker)
    {
        OnDefend?.Invoke();
        var d = attacker.Model.Attack - Model.Defense;
        var damage = attacker.Model.Damage.Roll();

        // i.e. attacker has bigger attack
        float multi = damage * (1 + d * 0.05f);

        multi = Mathf.Max(multi, 0.3f);
        multi = Mathf.Min(multi, 3f);

        damage = (int)(damage * multi);

        if (damage > CumulativeHP)
            Die();
        else
        {
            var health = Model.Health;
            var relative = (float)damage / health;
            var amountReduction = Mathf.FloorToInt(relative);
            Amount -= amountReduction;
            CurrentHealth -= (int)((relative - amountReduction) * health);
        }
    }

    private void Die()
    {
        OnDie?.Invoke(this);
        Destroy(this);
    }

    internal void Init(UnitModel model, int amount)
    {
        Model = model;
        Amount = amount;
        CurrentHealth = model.Health;
    }
}