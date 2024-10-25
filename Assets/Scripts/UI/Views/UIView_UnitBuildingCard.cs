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
    [SerializeField]
    Button _recruit;
    [SerializeField]
    UIView_UnitPurchase p_unitRecruitment;

    UnitBuildingInstance _instance;
    UnitBuildingModel _model;
    Action _recruitAction;

    UnitBuildingModel Model
    {
        get
        {
            return _instance == null ? _model : _instance.UModel;
        }
    }

    public void Init(UnitBuildingInstance buildingInstance, Action recruit)
    {
        _instance = buildingInstance;
        _model = null;
        _recruit.interactable = true;
        _recruitAction = recruit;
        _recruit.onClick.RemoveAllListeners();
        _recruit.onClick.AddListener(() => OpenRecruitmentPanel(buildingInstance));
        Draw();
    }

    public void Init(UnitBuildingModel buildingModel)
    {
        _instance = null;
        _model = buildingModel;
        _recruit.interactable = false;
        _recruitAction = null;

        Draw();
    }

    private void OpenRecruitmentPanel(UnitBuildingInstance buildingInstance)
    {
        var inst = Instantiate(p_unitRecruitment);
        inst.Init(buildingInstance, _recruitAction);
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
