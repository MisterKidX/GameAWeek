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
    public void Init(CastleInstance instance, Action exit)
    {
        _instance = instance;

        _exit.onClick.RemoveAllListeners();
        _exit.onClick.AddListener(() => Destroy(gameObject));
        _exit.onClick.AddListener(() => exit());

        ShowUnits();
    }

    private void ShowUnits()
    {
        var castledUnits = _instance.CastledUnits;
        _castledUnitsStrip.Init(_instance, true);
        _visitingUnitsStrip.Init(_instance, false);
    }
}
