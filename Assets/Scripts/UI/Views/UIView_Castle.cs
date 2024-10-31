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
        foreach (System.Collections.Generic.KeyValuePair<BuildingModel, BuildingInstance> building in _instance.BuiltBuildings)
        {
            if (building.Value == null)
                continue;

            var sceneObject = Scene.FirstOrDefault(s => s.BModel == building.Key);

            if (sceneObject == null)
                continue;

            sceneObject.Image.gameObject.SetActive(true);
            sceneObject.Image.sprite = building.Key.View;

            if (building.Key.BuildingView != null && sceneObject.Image.TryGetComponent<Button>(out Button btn))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => InstantiateBuildingView(building.Value));
            }
        }
    }

    private void InstantiateBuildingView(BuildingInstance instance)
    {
        switch (instance)
        {
            case HeroRecruitBuildingInstance heroRecruitBuildingInstance:
                heroRecruitBuildingInstance.ShowUIView(ShowUnits);
                break;
            default:
                break;
        }
    }

    private void ShowUnits()
    {
        var castledUnits = _instance.CastledUnits;
        _castledUnitsStrip.Init(_instance, true);
        _visitingUnitsStrip.Init(_instance, false);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (ImageBuildingModelTuple building in Scene)
        {
            if (building.BModel != null)
            {
                building.Image.sprite = building.BModel.View;
            }
        }
    }
#endif
}

[Serializable]
public record ImageBuildingModelTuple
{
    public BuildingModel BModel;
    public Image Image;
}
