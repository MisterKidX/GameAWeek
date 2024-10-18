using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_ResourceBox : MonoBehaviour
{
    ResourceInstance _instance;

    public Image Icon;
    public TMP_Text Amount;

    public void Init(ResourceInstance instance)
    {
        _instance = instance;

        Icon.sprite = _instance.Model.Icon;
        Amount.text = _instance.Amount.ToString();
    }
}
