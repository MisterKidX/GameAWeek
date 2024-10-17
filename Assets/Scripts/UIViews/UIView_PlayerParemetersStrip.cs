using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_PlayerParemetersStrip : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_InputField _playerName;
    [SerializeField]
    TMP_Dropdown _castle;
    [SerializeField]
    TMP_Dropdown _color;
    [SerializeField]
    Button _removeStrip;

    public event Action<UIView_PlayerParemetersStrip> Remove;

    public void Init(string[] castleOptions, int color, bool canRemoveStrip)
    {
        _castle.ClearOptions();
        _castle.AddOptions(castleOptions.ToList());

        _color.value = color;

        if (canRemoveStrip)
            _removeStrip.onClick.AddListener(RemoveStrip);
        else
            _removeStrip.interactable = false;
    }

    private void RemoveStrip()
    {
        Remove?.Invoke(this);
        Destroy(gameObject);
    }

    internal (string name, string castle, Color color) GetInfo()
    {
        (string name, string castle, Color color) info;
        info.name = _playerName.text;
        info.castle = _castle.options[_castle.value].text;
        info.color = _color.options[_color.value].color;
        return info;
    }
}
