using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_Stat : MonoBehaviour
{
    [SerializeField]
    Image _icon;
    [SerializeField]
    TMP_Text _statName;
    [SerializeField]
    TMP_Text _statValue;

    public void Init(Sprite icon, string statName, int statValue)
    {
        _icon.sprite = icon;
        _statName.text = statName;
        _statValue.text = statValue.ToString();
    }

    public void Init(Sprite icon, string statName, Vector2 statValue)
    {
        _icon.sprite = icon;
        _statName.text = statName;
        _statValue.text = $"{statValue.x} - {statValue.y}";
    }
}
