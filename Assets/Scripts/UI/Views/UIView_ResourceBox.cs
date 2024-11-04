using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_ResourceBox : MonoBehaviour
{
    ResourceInstance _instance;

    public Image Icon;
    public TMP_Text Amount;

    int _change = 0;

    public void Init(ResourceInstance instance)
    {
        _instance = instance;
        _change = int.MaxValue;
        Icon.sprite = _instance.Model.Icon;
    }

    private void Update()
    {
        if (_instance == null) return;

        if (_change != _instance.Amount)
        {
            Amount.text = _instance.Amount.ToString();
        }
    }
}
