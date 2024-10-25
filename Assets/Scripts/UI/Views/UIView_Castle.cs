using System;
using System.Linq;
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
    UIView_UnitsPurchaseMenu p_unitsPurchaseMenu;
    [SerializeField]
    Button _build;
    [SerializeField]
    Button _units;

    [SerializeField]
    ImageBuildingModelTuple[] Scene;

    CastleInstance _instance;
    public void Init(CastleInstance instance, Action exit)
    {
        _instance = instance;

        _exit.onClick.RemoveAllListeners();
        _exit.onClick.AddListener(() => Destroy(gameObject));
        _exit.onClick.AddListener(() => exit());

        _build.onClick.RemoveAllListeners();
        _build.onClick.AddListener(OpenBuildMenu);

        _units.onClick.RemoveAllListeners();
        _units.onClick.AddListener(OpenUnitsMenu);

        ShowUnits();
        ShowScene();
    }

    private void OpenUnitsMenu()
    {
        var menu = Instantiate(p_unitsPurchaseMenu);
        menu.Init(_instance, ShowUnits);
    }

    private void OpenBuildMenu()
    {
        var castleView = Instantiate(p_castleBuildMenu);
        castleView.Init(_instance, ShowScene);
    }

    private void ShowScene()
    {
        foreach (var item in Scene)
        {
            item.Image.gameObject.SetActive(false);
        }
        foreach (var building in _instance.BuiltBuildings)
        {
            if (building.Value == null)
                continue;

            var sceneObject = Scene.FirstOrDefault(s => s.BModel == building.Key);

            if (sceneObject == null)
                continue;

            sceneObject.Image.gameObject.SetActive(true);
            sceneObject.Image.sprite = building.Key.View;
        }
    }

    private void ShowUnits()
    {
        var castledUnits = _instance.CastledUnits;
        _castledUnitsStrip.Init(_instance, true);
        _visitingUnitsStrip.Init(_instance, false);
    }
}

[Serializable]
public record ImageBuildingModelTuple
{
    public BuildingModel BModel;
    public Image Image;
}
