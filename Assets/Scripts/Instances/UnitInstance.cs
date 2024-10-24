﻿using UnityEngine;

public class UnitInstance :ScriptableObject
{
    public UnitModel Model;
    public int Amount;

    internal void Init(UnitModel model, int amount)
    {
        Model = model;
        Amount = amount;
    }
}