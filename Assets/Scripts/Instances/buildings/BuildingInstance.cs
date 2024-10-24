using UnityEngine;

public abstract class BuildingInstance : ScriptableObject
{
    public abstract BuildingModel Model { get; }
}

public enum BuildingStateView
{
    Built,
    NotEnoughResources,
    CanBuild,
    CantBuild
}