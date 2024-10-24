using System;
using UnityEngine;

public abstract class BuildingModel : ScriptableObject
{
    public string Name;
    public Sprite View;
    public int Order;
    public BuildingModel Upgrade;
    public ResourceCost[] Cost;

    public bool Upgradable => Upgrade != null;
}

[Serializable]
public struct ResourceCost
{
    public ResourceModel Resource;
    public int Amount;
}