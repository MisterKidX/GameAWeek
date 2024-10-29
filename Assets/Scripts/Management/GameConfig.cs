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
        var models = Resources;
        var insts = new ResourceInstance[models.Length];

        for (int i = 0; i < models.Length; i++)
        {
            ResourceModel rm = models[i];
            switch (rm.Name)
            {
                case "Gold":
                    insts[i] = rm.Create(10_000);
                    break;
                default:
                    insts[i] = rm.Create(10);
                    break;
            }
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