using System;
using UnityEngine;

public class UIView_StatsContainer : MonoBehaviour
{
    [SerializeField]
    UIView_Stat[] _stats;

    UnitModel _unitModel;

    public void Init(UnitModel umodel)
    {
        _unitModel = umodel;

        Draw();
    }

    private void Draw()
    {
        _stats[0].Init(GameConfig.GetIconForStat(Stat.Attack), Stat.Attack.ToString(), _unitModel.Attack);
        _stats[1].Init(GameConfig.GetIconForStat(Stat.Defense), Stat.Defense.ToString(), _unitModel.Defense);
        _stats[2].Init(GameConfig.GetIconForStat(Stat.Damage), Stat.Damage.ToString(), _unitModel.Damage);
        _stats[3].Init(GameConfig.GetIconForStat(Stat.Health), Stat.Health.ToString(), _unitModel.Health);
        _stats[4].Init(GameConfig.GetIconForStat(Stat.Speed), Stat.Speed.ToString(), _unitModel.Speed);
        _stats[5].Init(GameConfig.GetIconForStat(Stat.Growth), Stat.Growth.ToString(), _unitModel.Growth);
    }
}
