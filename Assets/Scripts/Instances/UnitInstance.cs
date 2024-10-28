using System;
using UnityEngine;

public class UnitInstance : ScriptableObject
{
    public Vector3 CombatWorldPosition;
    public Vector3Int CombatCellPosition;
    public UnitModel Model;
    public int Amount;
    public bool Selected { get; set; }

    internal void Init(UnitModel model, int amount)
    {
        Model = model;
        Amount = amount;
    }
}