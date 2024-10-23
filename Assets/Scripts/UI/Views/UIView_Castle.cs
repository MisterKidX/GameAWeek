using System;
using UnityEngine;
using UnityEngine.UI;

public class UIView_Castle : MonoBehaviour
{
    [SerializeField]
    Button _exit;
    [SerializeField]
    UIView_UnitStrip _visitingUnitsStrip;
    [SerializeField]
    UIView_UnitStrip _castledUnitsStrip;

    CastleInstance _instance;
    public void Init(CastleInstance instance)
    {
        _instance = instance;

        _exit.onClick.RemoveAllListeners();
        _exit.onClick.AddListener(() => gameObject.SetActive(false));

        ShowUnits();
    }

    private void ShowUnits()
    {
        var castledUnits = _instance.CastledUnits;
        _castledUnitsStrip.Init(_instance.CastledHero, _instance.CastledUnits);
        _visitingUnitsStrip.Init(_instance.VisitingHero, _instance.VisitingHero?.Units);
    }
}
