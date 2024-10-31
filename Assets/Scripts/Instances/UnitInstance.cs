using System;
using UnityEngine;

public class UnitInstance : ScriptableObject, ICombatant
{
    public Vector3 CombatWorldPosition;
    public Vector3Int CombatCellPosition;
    public Vector3 WorldPosition;
    public Vector3Int CellPosition;
    public UnitModel Model;
    public int Amount;
    public int CurrentHealth;
    public bool HasRetaliation;

    public string Name => Model.Name;
    public Sprite Portrait => Model.Portrait;
    public bool Controlled { get; private set; }

    public bool Selected { get; set; }
    public int CumulativeHP => (Amount - 1) * Model.Health + CurrentHealth;

    public UnitInstance[] _combatUnits;
    public UnitInstance[] Units
    {
        get
        {
            if (_combatUnits != null) return _combatUnits;

            var count = UnityEngine.Random.Range(1, 5);
            _combatUnits = new UnitInstance[7];
            for (int i = 0; i < count; i++)
            {
                _combatUnits[i] = Model.Create(Amount / count, false);
            }
            return _combatUnits;
        }
    }

    public event Action OnAttack;
    public event Action OnDefend;
    public event Action<UnitInstance> OnDie;

    public UnitCombatView RuntimeMapView;

    public void Attack(UnitInstance defender)
    {
        OnAttack?.Invoke();
        defender.Defend(this);
    }

    private void Defend(UnitInstance attacker)
    {
        OnDefend?.Invoke();
        var d = attacker.Model.Attack - Model.Defense;
        var roll = attacker.Model.Damage.Roll();
        float damage =  roll * attacker.Amount;

        if (attacker.Model.IsRanged)
        {
            var delta = CombatCellPosition - attacker.CombatCellPosition;
            if (delta.magnitude > attacker.Model.AttackRange)
                damage /= 2f;
        }

        float multi = 1 + d * 0.05f;

        multi = Mathf.Max(multi, 0.3f);
        multi = Mathf.Min(multi, 3f);

        damage = Mathf.Ceil(damage * multi);

        Debug.Log($"{attacker.Model.Name} attacks {Model.Name} for {damage} damage " +
            $"[ATD: {d}, Roll: {Model.Damage.x}-{Model.Damage.y} ({roll}), Amount: {attacker.Amount}]. " +
            $"{(float)damage / Model.Health} perish.");

        if (damage > CumulativeHP)
        {
            Die();
            return;
        }
        else
        {
            var health = Model.Health;
            var relative = (float)damage / health;
            var amountReduction = Mathf.FloorToInt(relative);
            Amount -= amountReduction;
            CurrentHealth -= (int)((relative - amountReduction) * health);
            if (CurrentHealth <= 0)
            {
                Amount -= 1;
                CurrentHealth = Model.Health;
            }
        }
        if (Amount == 0)
        {
            Die();
            return;
        }

        if (HasRetaliation && !attacker.Model.IsRanged)
        {
            HasRetaliation = false;
            attacker.Defend(this);
        }
    }

    public void Die()
    {
        OnDie?.Invoke(this);
        if (RuntimeMapView != null)
            Destroy(RuntimeMapView.gameObject);
        if (_combatUnits != null)
        {
            for (int i = 0; i < _combatUnits.Length; i++)
            {
                if (_combatUnits[i] != null)
                    Destroy(_combatUnits[i]);
            }
        }

        Destroy(this);
    }

    internal void Init(UnitModel model, int amount, bool controlled)
    {
        Model = model;
        Amount = amount;
        CurrentHealth = model.Health;
        Controlled = controlled;
    }

    internal void ShowMap()
    {
        if (RuntimeMapView == null)
        {
            RuntimeMapView = Instantiate(Model.p_combatView);
            RuntimeMapView.MapInit(WorldPosition);
        }
    }
}