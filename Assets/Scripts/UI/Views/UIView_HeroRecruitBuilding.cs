using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_HeroRecruitBuilding : UIView_Building
{
    [SerializeField]
    TMP_Text _header;
    [SerializeField]
    Button _confirm;
    [SerializeField]
    Button _cancel;
    [SerializeField]
    Toggle _heroA;
    [SerializeField]
    Image _heroAPortrait;
    [SerializeField]
    Toggle _heroB;
    [SerializeField]
    Image _heroBPortrait;
    [SerializeField]
    TMP_Text _description;

    HeroModel _selection;
    HeroRecruitBuildingInstance _instance;
    public void Init(HeroRecruitBuildingInstance instance, Action<HeroModel> confirm)
    {
        _instance = instance;
        _selection = instance.AvailableHeroes[0];

        _header.text = instance.Model.Name;
        _heroAPortrait.sprite = instance.AvailableHeroes[0].Portrait;
        _heroBPortrait.sprite = instance.AvailableHeroes[1].Portrait;

        _heroA.onValueChanged.AddListener(HeroAChange);

        _cancel.onClick.RemoveAllListeners();
        _cancel.onClick.AddListener(() => Destroy(gameObject));

        string message = "";
        if (!instance.Holder.Holder.HasEnoughResources(instance.BuildingModel.HeroCost))
        {
            _confirm.interactable = false;
            message = "You do not have enough resources to buy a new hero!";
            foreach (var cost in instance.BuildingModel.HeroCost)
                message += $"\n{cost.Resource.Name} - {cost.Amount}";
            _description.text = message;
            return;
        }
        else if (_instance.Holder.HasVisitingHero)
        {
            _confirm.interactable = false;
            message = "You cannot purchase a hero when there is a visiting one.";
            _description.text = message;
            return;
        }
        else if (_instance.Holder.Holder.Heroes.Count >= 1)
        {
            _confirm.interactable = false;
            message = "You can only have one hero.";
            _description.text = message;
            return;
        }

        foreach (var cost in instance.BuildingModel.HeroCost)
            message += $"\n{cost.Resource.Name} - {cost.Amount}";
        _description.text = message;

        _confirm.onClick.RemoveAllListeners();
        _confirm.onClick.AddListener(() => confirm?.Invoke(_selection));
        _confirm.onClick.AddListener(() => Destroy(gameObject));
    }

    private void HeroAChange(bool val)
    {
        if (val)
            _selection = _instance.AvailableHeroes[0];
        else
            _selection = _instance.AvailableHeroes[1];
    }
}