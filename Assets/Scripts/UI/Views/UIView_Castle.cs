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
    [SerializeField]
    UIView_CastleBuildMenu p_castleBuildMenu;
    [SerializeField]
    Button _build;

    CastleInstance _instance;
    public void Init(CastleInstance instance, Action exit)
    {
        _instance = instance;

        _exit.onClick.RemoveAllListeners();
        _exit.onClick.AddListener(() => Destroy(gameObject));
        _exit.onClick.AddListener(() => exit());

        _build.onClick.RemoveAllListeners();
        _build.onClick.AddListener(OpenBuildMenu);

        ShowUnits();
    }

    private void OpenBuildMenu()
    {
        var castleView = Instantiate(p_castleBuildMenu);
        castleView.Init(_instance);
    }

    private void ShowUnits()
    {
        var castledUnits = _instance.CastledUnits;
        _castledUnitsStrip.Init(_instance, true);
        _visitingUnitsStrip.Init(_instance, false);
    }
}
