using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_UnitPurchase : MonoBehaviour
{
    [SerializeField]
    TMP_Text _header;
    [SerializeField]
    Image _unitView;
    [SerializeField]
    Image[] _resourceCost;
    [SerializeField]
    TMP_Text _resourceCostPerUnit;
    [SerializeField]
    TMP_Text _totalCost;
    [SerializeField]
    TMP_Text _available;
    [SerializeField]
    TMP_Text _recruited;
    [SerializeField]
    Slider _slider;

    [SerializeField]
    Button _max;
    [SerializeField]
    Button _confirm;
    [SerializeField]
    Button _cancel;

    UnitBuildingInstance _instance;
    public void Init(UnitBuildingInstance instance, Action recruit)
    {
        _instance = instance;
        _slider.onValueChanged.RemoveAllListeners();
        _slider.onValueChanged.AddListener(SliderValueChangedHandler);
        _slider.wholeNumbers = true;
        _slider.minValue = 0;
        _slider.maxValue = _instance.Holder.Holder.GetMaxRecruitment(_instance.AvailableForPurchase);
        _slider.value = 0;
        SliderValueChangedHandler(0);

        _cancel.onClick.RemoveAllListeners();
        _cancel.onClick.AddListener(() => Destroy(gameObject));

        _max.onClick.RemoveAllListeners();
        _max.onClick.AddListener(() => 
        _slider.value = _instance.Holder.Holder.GetMaxRecruitment(_instance.AvailableForPurchase));

        _confirm.onClick.RemoveAllListeners();
        _confirm.onClick.AddListener(ConfirmPurchase);
        _confirm.onClick.AddListener(() => recruit?.Invoke());

        var unitModel = _instance.UModel.Unit;
        _header.text = unitModel.Name;
        _unitView.sprite = unitModel.Portrait;
        foreach (var resource in _resourceCost)
            resource.sprite = unitModel.Cost.Resource.Icon;
        _resourceCostPerUnit.text = unitModel.Cost.Amount.ToString();
    }

    private void ConfirmPurchase()
    {
        _instance.Holder.Holder.Purchase
            (_instance.AvailableForPurchase.Model, (int)_slider.value, _instance.Holder);

        _instance.AvailableForPurchase.Amount -= (int)_slider.value;
        _slider.value = 0;
    }

    private void SliderValueChangedHandler(float value)
    {
        var v = (int)value;

        _available.text = (_instance.AvailableForPurchase.Amount - v).ToString();
        _recruited.text = v.ToString();
        _totalCost.text = (v * _instance.UModel.Unit.Cost.Amount).ToString();

        if (value == 0)
            _confirm.interactable = false;
        else
            _confirm.interactable = true;

        if (value == _slider.maxValue)
            _max.interactable = false;
        else
            _max.interactable = true;
    }
}
