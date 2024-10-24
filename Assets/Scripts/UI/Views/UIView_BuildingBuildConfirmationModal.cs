using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_BuildingBuildConfirmationModal : MonoBehaviour
{
    [SerializeField]
    TMP_Text _name;
    [SerializeField]
    Image _portrait;
    [SerializeField]
    TMP_Text _description;
    [SerializeField]
    RectTransform _resourceCostContent;
    [SerializeField]
    UIView_ResourceBox p_resourceBox;
    [SerializeField]
    Button _cancel;
    [SerializeField]
    Button _confirm;

    BuildingModel _building;

    public void Init(BuildingModel model, bool canBeBuilt, Action<BuildingModel> confirmBuild)
    {
        _building = model;

        _name.text = _building.Name;
        _portrait.sprite = _building.View;
        _description.text = _building.Description;
        FillResourceContent();

        _cancel.onClick.RemoveAllListeners();
        _cancel.onClick.AddListener(() => Destroy(gameObject));

        _confirm.onClick.RemoveAllListeners();
        if (confirmBuild != null)
            _confirm.onClick.AddListener(() => confirmBuild(_building));
        _confirm.onClick.AddListener(() => Destroy(gameObject));

        if (canBeBuilt)
            _confirm.interactable = true;
        else
            _confirm.interactable = false;
    }

    private void FillResourceContent()
    {
        foreach (var cost in _building.Cost)
        {
            var box = Instantiate(p_resourceBox, _resourceCostContent);
            box.Init(cost.Resource.Create(cost.Amount));
        }
    }
}
