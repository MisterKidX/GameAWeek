using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_UnitBox : MonoBehaviour
{
    [SerializeField]
    Image _portrait;
    [SerializeField]
    TMP_Text _amount;

    public void Init(Sprite portrait, int amount, bool showAmount = true)
    {
        _portrait.sprite = portrait;
        if (showAmount && amount > 0)
        {
            _amount.text = amount.ToString();
            _amount.gameObject.SetActive(true);
        }
        else
            _amount.gameObject.SetActive(false);
    }
}