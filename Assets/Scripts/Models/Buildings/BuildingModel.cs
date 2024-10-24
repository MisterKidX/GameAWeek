using System;
using UnityEngine;

public abstract class BuildingModel : ScriptableObject
{
    public string Name;

    public ResourceCost[] Cost;
    public BuildingModel[] Upgrades;
}

[Serializable]
public struct ResourceCost
{
    public ResourceModel Resource;
    public int Amount;
}