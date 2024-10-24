using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_BuildingCard : MonoBehaviour
{
    [SerializeField]
    TMP_Text _name;
    [SerializeField]
    Image _portrait;
    [SerializeField]
    Image _nameBG;
    [SerializeField]
    Image _statusIcon;
    [SerializeField]
    Sprite _statusIconNotEnoughResources;
    [SerializeField]
    Sprite _cantBuild;
    [SerializeField]
    Button _build;

    BuildingModel _model;

    public void Init(BuildingModel model, Action onClick)
    {
        _model = model;

        _name.text = _model.Name;
        _portrait.sprite = _model.View;

        _build.onClick.RemoveAllListeners();
        _build.onClick.AddListener(() => onClick());
    }

    public void SetState(BuildingStateView built)
    {
        switch (built)
        {
            case BuildingStateView.Built:
                _nameBG.color = Color.yellow;
                _statusIcon.gameObject.SetActive(false);
                break;
            case BuildingStateView.NotEnoughResources:
                _nameBG.color = Color.red;
                _statusIcon.gameObject.SetActive(true);
                _statusIcon.sprite = _statusIconNotEnoughResources;
                break;
            case BuildingStateView.CanBuild:
                _nameBG.color = Color.green;
                _statusIcon.gameObject.SetActive(false);
                break;
            case BuildingStateView.CantBuild:
                _nameBG.color = Color.red;
                _statusIcon.gameObject.SetActive(true);
                _statusIcon.sprite = _cantBuild;
                break;
            default:
                break;
        }
    }
}
