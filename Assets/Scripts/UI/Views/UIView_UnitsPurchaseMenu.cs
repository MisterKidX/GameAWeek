using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIView_UnitsPurchaseMenu : MonoBehaviour
{
    [SerializeField]
    Button _done;
    [SerializeField]
    UIView_UnitBuildingCard[] _unitCards;

    public void Init(CastleInstance instance)
    {
        _done.onClick.RemoveAllListeners();
        _done.onClick.AddListener(() => Destroy(gameObject));

        int i = 0;
        foreach (var building in instance.BuiltBuildings)
        {
            if (building.Key is UnitBuildingModel model)
            {
                if (building.Value == null)
                    _unitCards[i].Init(model);
                else
                    _unitCards[i].Init(building.Value as UnitBuildingInstance);

                i++;
            }
        }

        for (; i < _unitCards.Length; i++)
        {
            _unitCards[i].Disable();
        }
    }
}
