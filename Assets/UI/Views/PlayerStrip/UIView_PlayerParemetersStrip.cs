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

    public void Init(string[] castleOptions, bool canRemoveStrip)
    {
        _castle.ClearOptions();
        _castle.AddOptions(castleOptions.ToList());

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
}
