using System;
using System.Linq;
using UnityEngine;

public static class GameConfig
{
    private static Configuration _config;
    public static Configuration Configuration
    {
        get
        {
            if (_config == null)
                _config = UnityEngine.Resources.LoadAll<Configuration>("")[0];

            return _config;
        }
    }
    static ResourceModel[] _resources;
    public static ResourceModel[] Resources
    {
        get
        {
            if (_resources == null)
                _resources = UnityEngine.Resources.LoadAll<ResourceModel>("");

            return _resources;
        }
    }

    static HeroModel[] _heroes;
    public static HeroModel[] Heroes
    {
        get
        {
            if (_heroes == null)
                _heroes = UnityEngine.Resources.LoadAll<HeroModel>("");

            return _heroes;
        }
    }

    private static UnitModel[] _unitModels;
    public static UnitModel[] Units
    {
        get
        {
            if (_unitModels == null)
                _unitModels = UnityEngine.Resources.LoadAll<UnitModel>("");

            return _unitModels;
        }
    }

    public static ResourceInstance[] GetStartingResouces()
    {
        var startingResources =  Configuration.StartingResources;
        var insts = new ResourceInstance[startingResources.Length];

        for (int i = 0; i < startingResources.Length; i++)
        {
            ResourceCost res = startingResources[i];
            insts[i] = res.Resource.Create(res.Amount);
        }

        return insts;
    }

    internal static Sprite GetIconForStat(Stat stat)
    {
        return _config.StatIcons.First(s => s.Stat == stat).Icon;
    }
}

public enum Stat
{
    Attack,
    Defense,
    Damage,
    Health,
    Speed,
    Growth
}