using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIView_ImageText : MonoBehaviour
{
    [SerializeField]
    Image _image;
    [SerializeField]
    TMP_Text _text;

    public void Init(Sprite view, string text)
    {
        _image.sprite = view;
        _text.text = text;
    }
}
