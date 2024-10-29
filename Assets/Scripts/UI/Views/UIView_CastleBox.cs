using System;
using UnityEngine;
using UnityEngine.UI;

public class UIView_CastleBox : MonoBehaviour
{
    [SerializeField]
    Image _portrait;
    [SerializeField]
    Button _click;

    public void Init(Sprite view, Action click)
    {
        _portrait.sprite = view;
        _click.interactable = true;
        _click.onClick.RemoveAllListeners();
        _click.onClick.AddListener(() => click());
    }

    public void Deactivate()
    {
        _click.interactable = false;
        _portrait.sprite = null;
    }
}
