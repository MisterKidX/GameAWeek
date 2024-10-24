using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_UnitBuildingCard : MonoBehaviour
{
    [SerializeField]
    TMP_Text _unitName;
    [SerializeField]
    Image _buildingPortrait;
    [SerializeField]
    TMP_Text _buildingName;
    [SerializeField]
    TMP_Text _available;
    [SerializeField]
    Image _unitPortrait;
    [SerializeField]
    UIView_StatsContainer _unitStatsContainer;

    UnitBuildingInstance _instance;
    UnitBuildingModel _model;

    UnitBuildingModel Model
    {
        get
        {
            return _instance == null ? _model : _instance.UModel;
        }
    }

    public void Init(UnitBuildingInstance buildingInstance)
    {
        _instance = buildingInstance;
        _model = null;

        Draw();
    }

    public void Init(UnitBuildingModel buildingModel)
    {
        _instance = null;
        _model = buildingModel;

        Draw();
    }

    internal void Disable()
    {
        gameObject.SetActive(false);
    }

    private void Draw()
    {
        gameObject.SetActive(true);

        _unitName.text = Model.Unit.Name;
        _buildingPortrait.sprite = Model.View;
        _buildingName.text = Model.Name;
        _unitPortrait.sprite = Model.Unit.Portrait;
        _available.text = _instance != null ? "Available: " + _instance.AvailableForPurchase.Amount.ToString() : null;
        _unitStatsContainer.Init(Model.Unit);
    }
}
