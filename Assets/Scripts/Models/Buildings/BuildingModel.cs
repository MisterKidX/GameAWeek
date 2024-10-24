using System;
using UnityEngine;

public abstract class BuildingModel : ScriptableObject
{
    public string Name;
    public int Order;
    public BuildingModel Upgrade;
    public ResourceCost[] Cost;
}

[Serializable]
public struct ResourceCost
{
    public ResourceModel Resource;
    public int Amount;
}