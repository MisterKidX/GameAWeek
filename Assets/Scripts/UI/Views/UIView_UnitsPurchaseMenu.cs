using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIView_UnitsPurchaseMenu : MonoBehaviour
{
    [SerializeField]
    Button _done;
    [SerializeField]
    UIView_UnitBuildingCard[] _unitCards;

    CastleInstance _instance;
    public void Init(CastleInstance instance, Action recruit)
    {
        _instance = instance;

        _done.onClick.RemoveAllListeners();
        _done.onClick.AddListener(() => Destroy(gameObject));
        _done.onClick.AddListener(() => recruit?.Invoke());

        Draw();
    }

    private void Draw()
    {
        int i = 0;
        foreach (var building in _instance.BuiltBuildings)
        {
            if (building.Key is UnitBuildingModel model)
            {
                var unitCard = _unitCards[i];
                if (building.Value == null)
                    _unitCards[i].Init(model);
                else
                    _unitCards[i].Init(building.Value as UnitBuildingInstance, Draw);

                i++;
            }
        }

        for (; i < _unitCards.Length; i++)
        {
            _unitCards[i].Disable();
        }
    }
}
