using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_PlayerOutcomeModal : MonoBehaviour
{
    [SerializeField]
    TMP_Text _header;
    [SerializeField]
    Image _flag;
    [SerializeField]
    Button _confirm;

    public void Init(PlayerInstace player, bool won, Action confirm)
    {
        if(won)
            _header.text = $"{player.Name} has won the game!";
        else
            _header.text = $"{player.Name} has been vanquished!";

        _flag.color = player.Color;
        _confirm.onClick.RemoveAllListeners();
        _confirm.onClick.AddListener(() => Destroy(gameObject));
        _confirm.onClick.AddListener(() => confirm?.Invoke());
    }
}
